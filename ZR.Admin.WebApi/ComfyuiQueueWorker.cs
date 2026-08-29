using Infrastructure.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.IOC;
using ZR.Model.System;
using ZR.ServiceCore.Services;

namespace ZR.Admin.WebApi
{
    /// <summary>
    /// ComfyUI执行队列后台Worker
    /// 负责将pending队列任务提交到ComfyUI服务、轮询执行进度、获取输出文件、处理超时
    /// </summary>
    public class ComfyuiQueueWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ComfyuiQueueWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Info("ComfyUI队列后台Worker启动");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPending(stoppingToken);
                    await ProcessRunning(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "ComfyUI队列Worker处理异常");
                }
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            logger.Info("ComfyUI队列后台Worker停止");
        }

        /// <summary>
        /// 处理pending队列任务：上传参考文件、构建prompt、提交到ComfyUI
        /// </summary>
        private async Task ProcessPending(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var comfyService = scope.ServiceProvider.GetRequiredService<IComfyuiService>();
            var db = DbScoped.SugarScope.GetConnectionScope(0);

            var pends = db.Queryable<ComfyuiQueue>()
                .Where(x => x.Status == "pending")
                .OrderBy(x => x.QueuedTime)
                .Take(3)
                .ToList();

            foreach (var queue in pends)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var task = db.Queryable<ComfyuiTask>().First(x => x.Id == queue.TaskId);
                    if (task == null)
                    {
                        db.Updateable<ComfyuiQueue>()
                            .SetColumns(x => x.Status == "failed")
                            .SetColumns(x => x.ErrorMessage == "任务不存在")
                            .SetColumns(x => x.CompleteTime == DateTime.Now)
                            .Where(x => x.Id == queue.Id)
                            .ExecuteCommand();
                        continue;
                    }
                    var workflow = db.Queryable<ComfyuiWorkflow>().First(x => x.Id == task.WorkflowId);
                    if (workflow == null)
                    {
                        UpdateQueueFailed(db, queue.Id, "工作流不存在", task.Id);
                        continue;
                    }

                    // 步骤1：上传参考文件到ComfyUI input目录（回填各节点ComfyName）
                    comfyService.UploadReferenceToComfy(task);
                    if (!string.IsNullOrEmpty(task.RefFiles))
                    {
                        db.Updateable<ComfyuiTask>()
                            .SetColumns(x => x.RefFiles == task.RefFiles)
                            .Where(x => x.Id == task.Id)
                            .ExecuteCommand();
                    }

                    // 步骤2：构建prompt请求体
                    string promptJson = comfyService.BuildPromptJson(task, workflow);

                    // 步骤3：提交到ComfyUI
                    string promptId = comfyService.SubmitToComfy(promptJson);

                    db.Updateable<ComfyuiQueue>()
                        .SetColumns(x => x.PromptId == promptId)
                        .SetColumns(x => x.PromptJson == promptJson)
                        .SetColumns(x => x.Status == "processing")
                        .SetColumns(x => x.ProcessStartTime == DateTime.Now)
                        .Where(x => x.Id == queue.Id)
                        .ExecuteCommand();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"ComfyUI提交队列任务失败: queueId={queue.Id}");
                    UpdateQueueFailed(db, queue.Id, ex.Message, queue.TaskId);
                }
            }
        }

        /// <summary>
        /// 处理processing队列任务：轮询ComfyUI历史，获取输出
        /// </summary>
        private async Task ProcessRunning(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var comfyService = scope.ServiceProvider.GetRequiredService<IComfyuiService>();
            var db = DbScoped.SugarScope.GetConnectionScope(0);

            var running = db.Queryable<ComfyuiQueue>()
                .Where(x => x.Status == "processing")
                .OrderBy(x => x.ProcessStartTime)
                .Take(5)
                .ToList();

            foreach (var queue in running)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var outputs = comfyService.QueryHistory(queue.PromptId, queue.TaskId);
                    if (outputs.Count > 0)
                    {
                        // 完成
                        string urlsJson = JsonConvert.SerializeObject(outputs);
                        db.Updateable<ComfyuiQueue>()
                            .SetColumns(x => x.Status == "done")
                            .SetColumns(x => x.OutputUrls == urlsJson)
                            .SetColumns(x => x.Progress == 100)
                            .SetColumns(x => x.CompleteTime == DateTime.Now)
                            .Where(x => x.Id == queue.Id)
                            .ExecuteCommand();

                        // 同步任务状态
                        db.Updateable<ComfyuiTask>()
                            .SetColumns(x => x.Status == "done")
                            .SetColumns(x => x.CompleteTime == DateTime.Now)
                            .Where(x => x.Id == queue.TaskId)
                            .ExecuteCommand();
                        logger.Info($"ComfyUI任务完成: taskId={queue.TaskId}, 输出数={outputs.Count}");
                    }
                    else
                    {
                        // 检查超时
                        var timeout = comfyService.GetTimeoutSeconds();
                        if (queue.ProcessStartTime.HasValue &&
                            (DateTime.Now - queue.ProcessStartTime.Value).TotalSeconds > timeout)
                        {
                            UpdateQueueFailed(db, queue.Id, "执行超时", queue.TaskId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"ComfyUI轮询队列任务失败: queueId={queue.Id}");
                }
            }
        }

        private void UpdateQueueFailed(ISqlSugarClient db, long queueId, string error, long? taskId = null)
        {
            try
            {
                db.Updateable<ComfyuiQueue>()
                    .SetColumns(x => x.Status == "failed")
                    .SetColumns(x => x.ErrorMessage == error)
                    .SetColumns(x => x.CompleteTime == DateTime.Now)
                    .Where(x => x.Id == queueId)
                    .ExecuteCommand();
                if (taskId.HasValue)
                {
                    db.Updateable<ComfyuiTask>()
                        .SetColumns(x => x.Status == "failed")
                        .SetColumns(x => x.ErrorMessage == error)
                        .SetColumns(x => x.CompleteTime == DateTime.Now)
                        .Where(x => x.Id == taskId.Value)
                        .ExecuteCommand();
                }
            }
            catch { }
        }
    }
}
