using Infrastructure;
using Infrastructure.Attribute;
using Infrastructure.Enums;
using Infrastructure.Extensions;
using Infrastructure.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SqlSugar;
using ZR.Common;
using ZR.Model;
using ZR.Model.System;
using ZR.Model.System.Dto;

namespace ZR.ServiceCore.Services
{
    /// <summary>
    /// AI任务服务
    /// </summary>
    [AppService(ServiceType = typeof(IAiTaskService), ServiceLifetime = LifeTime.Transient)]
    public class AiTaskService : BaseService<AiTask>, IAiTaskService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly OptionsSetting _optionsSetting;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public AiTaskService(IWebHostEnvironment webHostEnvironment, IOptions<OptionsSetting> options)
        {
            _webHostEnvironment = webHostEnvironment;
            _optionsSetting = options.Value;
        }

        /// <summary>
        /// 提交AI任务
        /// </summary>
        public long SubmitTask(AiTaskSubmitDto dto, IFormFile formFile, long userId)
        {
            if (formFile == null || formFile.Length == 0)
            {
                throw new CustomException("上传图片不能为空");
            }

            // 生成任务号（雪花ID）
            long taskNo = SnowFlakeSingle.Instance.NextId();

            // 保存原图
            string storageRoot = GetStorageRoot();
            string inputDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "input", taskNo.ToString());
            Directory.CreateDirectory(inputDir);

            string ext = Path.GetExtension(formFile.FileName);
            string fileName = "original" + ext;
            string filePath = Path.Combine(inputDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            // 构建访问URL
            string accessUrl = string.Concat(
                _optionsSetting.Upload.UploadUrl.TrimEnd('/'), "/",
                storageRoot.Replace("\\", "/"), "/input/",
                taskNo, "/", fileName);

            // 创建任务记录
            var task = new AiTask
            {
                Id = taskNo,
                UserId = userId,
                FuncType = "img2img",
                InputImagePath = accessUrl,
                Prompt = dto.Prompt,
                Status = "pending",
                RetryCount = 0,
                Create_time = DateTime.Now,
                Create_by = userId.ToString()
            };

            Insertable(task).ExecuteCommand();

            logger.Info($"新任务提交: taskNo={taskNo}, userId={userId}");

            return taskNo;
        }

        /// <summary>
        /// 获取任务状态
        /// </summary>
        public AiTask GetTaskStatus(long taskNo, long userId)
        {
            return Queryable()
                .Where(x => x.Id == taskNo && x.UserId == userId)
                .First();
        }

        /// <summary>
        /// 获取我的任务列表
        /// </summary>
        public PagedInfo<AiTask> GetMyTaskList(AiTaskListDto parm, long userId)
        {
            var predicate = Expressionable.Create<AiTask>();
            predicate = predicate.And(x => x.UserId == userId);
            predicate = predicate.AndIF(parm.Status.IsNotEmpty(), x => x.Status == parm.Status);
            predicate = predicate.AndIF(parm.FuncType.IsNotEmpty(), x => x.FuncType == parm.FuncType);
            predicate = predicate.AndIF(parm.Prompt.IsNotEmpty(), x => x.Prompt.Contains(parm.Prompt));

            return GetPages(predicate.ToExpression(), parm, x => x.Id, OrderByType.Desc);
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        public bool UpdateTask(long taskNo, string prompt, long userId)
        {
            var task = Queryable().Where(x => x.Id == taskNo && x.UserId == userId).First();
            if (task == null)
            {
                throw new CustomException("任务不存在");
            }
            if (task.Status == "done")
            {
                throw new CustomException("已完成的任务不能修改");
            }

            task.Prompt = prompt;
            task.Update_time = DateTime.Now;

            var result = Update(task);
            logger.Info($"任务更新: taskNo={taskNo}");
            return result > 0;
        }

        /// <summary>
        /// N8N取一个待处理任务
        /// </summary>
        public N8nFetchTaskDto FetchPendingTask()
        {
            // 取最早的pending任务
            var pendingTask = Queryable()
                .Where(x => x.Status == "pending")
                .OrderBy(x => x.Create_time)
                .First();

            if (pendingTask == null)
            {
                return null;
            }

            // 更新状态为processing
            pendingTask.Status = "processing";
            pendingTask.ProcessStartTime = DateTime.Now;
            Update(pendingTask);

            logger.Info($"任务分配给N8N: taskNo={pendingTask.Id}");

            return new N8nFetchTaskDto
            {
                TaskNo = pendingTask.Id,
                InputImageUrl = pendingTask.InputImagePath,
                Prompt = pendingTask.Prompt,
                ExtParams = pendingTask.ExtParams
            };
        }

        /// <summary>
        /// N8N上传结果图
        /// </summary>
        public string UploadResultImage(long taskNo, IFormFile file)

        {
            if (file == null || file.Length == 0)
            {
                throw new CustomException("上传文件不能为空");
            }

            var task = Queryable().Where(x => x.Id == taskNo).First();
            if (task == null)
            {
                throw new CustomException("任务不存在");
            }

            string storageRoot = GetStorageRoot();
            string outputDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "output", taskNo.ToString());
            Directory.CreateDirectory(outputDir);

            string ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext)) ext = ".png";
            string fileName = "result" + ext;
            string filePath = Path.Combine(outputDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string accessUrl = string.Concat(
                _optionsSetting.Upload.UploadUrl.TrimEnd('/'), "/",
                storageRoot.Replace("\\", "/"), "/output/",
                taskNo, "/", fileName);

            logger.Info($"结果图上传: taskNo={taskNo}, url={accessUrl}");

            return accessUrl;
        }

        /// <summary>
        /// 上传Base64图片
        /// </summary>
        public string UploadBase64Image(long taskNo, string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                throw new CustomException("Base64图片数据不能为空");
            }

            var task = Queryable().Where(x => x.Id == taskNo).First();
            if (task == null)
            {
                throw new CustomException("任务不存在");
            }

            string base64Data = base64Image;
            string ext = ".png";

            if (base64Data.Contains(","))
            {
                var header = base64Data.Split(',')[0];
                base64Data = base64Data.Split(',')[1];
                if (header.Contains("jpeg") || header.Contains("jpg")) ext = ".jpg";
                else if (header.Contains("webp")) ext = ".webp";
            }

            byte[] imageBytes = Convert.FromBase64String(base64Data);

            string storageRoot = GetStorageRoot();
            string outputDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "output", taskNo.ToString());
            Directory.CreateDirectory(outputDir);

            string fileName = "result" + ext;
            string filePath = Path.Combine(outputDir, fileName);
            File.WriteAllBytes(filePath, imageBytes);

            string accessUrl = string.Concat(
                _optionsSetting.Upload.UploadUrl.TrimEnd('/'), "/",
                storageRoot.Replace("\\", "/"), "/output/",
                taskNo, "/", fileName);

            task.Status = "done";
            task.OutputImagePath = accessUrl;
            task.CompleteTime = DateTime.Now;
            Update(task);

            logger.Info($"Base64结果图上传: taskNo={taskNo}, url={accessUrl}");

            return accessUrl;
        }

        /// <summary>
        /// N8N成功回调
        /// </summary>
        public bool CallbackSuccess(long taskNo, string outputImageUrl)
        {
            var task = Queryable().Where(x => x.Id == taskNo).First();
            if (task == null)
            {
                logger.Warn($"回调失败: 任务不存在 taskNo={taskNo}");
                return false;
            }

            if (task.Status != "processing")
            {
                logger.Warn($"回调失败: 任务状态不是processing taskNo={taskNo}, status={task.Status}");
                return false;
            }

            task.Status = "done";
            task.OutputImagePath = outputImageUrl;
            task.CompleteTime = DateTime.Now;

            var result = Update(task);
            logger.Info($"任务完成: taskNo={taskNo}");

            return result > 0;
        }

        /// <summary>
        /// N8N失败回调
        /// </summary>
        public bool CallbackFailed(long taskNo, string errorMsg)
        {
            var task = Queryable().Where(x => x.Id == taskNo).First();
            if (task == null)
            {
                logger.Warn($"回调失败: 任务不存在 taskNo={taskNo}");
                return false;
            }

            task.Status = "failed";
            task.ErrorMessage = errorMsg;
            task.CompleteTime = DateTime.Now;

            var result = Update(task);
            logger.Info($"任务失败: taskNo={taskNo}, error={errorMsg}");

            return result > 0;
        }

        /// <summary>
        /// 重试任务
        /// </summary>
        public bool RetryTask(long taskNo)
        {
            var task = Queryable().Where(x => x.Id == taskNo).First();
            if (task == null) return false;

            if (task.Status == "pending")
            {
                throw new CustomException("排队中的任务无需重试");
            }

            task.Status = "pending";
            task.RetryCount += 1;
            task.ProcessStartTime = null;
            task.ErrorMessage = null;

            var result = Update(task);
            logger.Info($"任务重试: taskNo={taskNo}, retryCount={task.RetryCount}");

            return result > 0;
        }

        /// <summary>
        /// 批量重试失败任务
        /// </summary>
        public int BatchRetryFailed(long userId)
        {
            var failedTasks = Queryable()
                .Where(x => x.UserId == userId && (x.Status == "failed" || x.Status == "processing"))
                .ToList();

            if (failedTasks.Count == 0) return 0;

            foreach (var task in failedTasks)
            {
                task.Status = "pending";
                task.RetryCount += 1;
                task.ProcessStartTime = null;
                task.ErrorMessage = null;
            }

            var result = Context.Updateable(failedTasks).ExecuteCommand();
            logger.Info($"批量重试: userId={userId}, count={failedTasks.Count}");

            return result;
        }

        /// <summary>
        /// 检测超时任务
        /// </summary>
        public int CheckTimeout(int timeoutMinutes)
        {
            if (timeoutMinutes <= 0) timeoutMinutes = 5;

            var cutoffTime = DateTime.Now.AddMinutes(-timeoutMinutes);

            // 先检查是否有processing状态的任务
            var hasProcessing = Queryable().Any(x => x.Status == "processing" && x.ProcessStartTime < cutoffTime);
            if (!hasProcessing) return 0;

            // 将超时的processing任务标记为failed
            var result = Update(
                x => x.Status == "processing" && x.ProcessStartTime < cutoffTime,
                x => new AiTask
                {
                    Status = "failed",
                    ErrorMessage = "处理超时",
                    CompleteTime = DateTime.Now
                });

            if (result > 0)
            {
                logger.Warn($"检测到{result}个超时任务，已标记为failed");
            }

            return result;
        }

        /// <summary>
        /// 获取存储根目录
        /// </summary>
        private string GetStorageRoot()
        {
            var root = AppSettings.GetConfig("AiTask:StorageRoot");
            return string.IsNullOrEmpty(root) ? "storage/ai" : root;
        }

        public List<AiPromptTemplate> GetMyTemplates(long userId)
        {
            return Context.Queryable<AiPromptTemplate>()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Create_time)
                .ToList();
        }

        public AiPromptTemplate SaveTemplate(AiPromptTemplateDto dto, long userId)
        {
            if (dto.Id > 0)
            {
                var existing = Context.Queryable<AiPromptTemplate>()
                    .Where(x => x.Id == dto.Id && x.UserId == userId).First();
                if (existing == null) throw new CustomException("模板不存在");
                existing.Name = dto.Name;
                existing.Prompt = dto.Prompt;
                existing.FuncType = dto.FuncType ?? "img2img";
                existing.Update_time = DateTime.Now;
                Context.Updateable(existing).ExecuteCommand();
                return existing;
            }
            else
            {
                var template = new AiPromptTemplate
                {
                    Id = SnowFlakeSingle.Instance.NextId(),
                    UserId = userId,
                    Name = dto.Name,
                    Prompt = dto.Prompt,
                    FuncType = dto.FuncType ?? "img2img",
                    Create_time = DateTime.Now,
                    Create_by = userId.ToString()
                };
                Context.Insertable(template).ExecuteCommand();
                return template;
            }
        }

        public bool DeleteTemplate(long id, long userId)
        {
            return Context.Deleteable<AiPromptTemplate>()
                .Where(x => x.Id == id && x.UserId == userId)
                .ExecuteCommand() > 0;
        }

        public bool DeleteTask(long taskNo, long userId)
        {
            var task = Queryable().Where(x => x.Id == taskNo && x.UserId == userId).First();
            if (task == null) throw new CustomException("任务不存在");
            if (task.Status == "processing") throw new CustomException("处理中的任务不能删除");

            // 删除关联的文件
            if (!string.IsNullOrEmpty(task.InputImagePath))
            {
                var uri = new Uri(task.InputImagePath);
                var localPath = Path.Combine(_webHostEnvironment.WebRootPath,
                    uri.PathAndQuery.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                var dir = Path.GetDirectoryName(localPath);
                if (dir != null && Directory.Exists(dir)) Directory.Delete(dir, true);
            }

            return Context.Deleteable<AiTask>().Where(x => x.Id == taskNo && x.UserId == userId).ExecuteCommand() > 0;
        }
    }
}
