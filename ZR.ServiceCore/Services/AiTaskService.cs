using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
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

            // 计算输入图MD5哈希
            string imageHash = null;
            using (var md5 = MD5.Create())
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    imageHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
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
                InputImageHash = imageHash,
                Prompt = dto.Prompt,
                TaskName = dto.TaskName,
                Tags = dto.Tags.IsNotEmpty() ? dto.Tags : DateTime.Now.ToString("yyyy-MM-dd"),
                Status = "pending",
                RetryCount = 0,
                AttemptCount = 0,
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
            predicate = predicate.AndIF(parm.Tag.IsNotEmpty(), x => x.Tags != null && x.Tags.Contains(parm.Tag));

            return GetPages(predicate.ToExpression(), parm, x => x.Id, OrderByType.Desc);
        }

        /// <summary>
        /// 更新任务（任务名称可随时修改，提示词和标签只能在未完成时修改）
        /// </summary>
        public bool UpdateTask(long taskNo, string prompt, long userId, string tags = null, string taskName = null)
        {
            var task = Queryable().Where(x => x.Id == taskNo && x.UserId == userId).First();
            if (task == null)
            {
                throw new CustomException("任务不存在");
            }

            // 任务名称可以随时修改
            if (taskName != null) task.TaskName = taskName;

            // 提示词和标签只能在未完成时修改
            if (task.Status != "done")
            {
                if (prompt != null) task.Prompt = prompt;
                if (tags != null) task.Tags = tags;
            }

            task.Update_time = DateTime.Now;

            var result = Update(task);
            logger.Info($"任务更新: taskNo={taskNo}, taskName={taskName}");
            return result > 0;
        }

        /// <summary>
        /// N8N取一个待处理任务（原子操作，防止并发拉取同一任务）
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

            // 原子更新：WHERE条件包含status=pending，确保只有一个worker能成功更新
            // 同时递增AttemptCount，用于callback校验
            var affected = Context.Updateable<AiTask>()
                .SetColumns(x => new AiTask {
                    Status = "processing",
                    ProcessStartTime = DateTime.Now,
                    AttemptCount = pendingTask.AttemptCount + 1
                })
                .Where(x => x.Id == pendingTask.Id && x.Status == "pending")
                .ExecuteCommand();

            // 如果更新失败，说明其他worker已抢到该任务
            if (affected == 0)
            {
                logger.Warn($"任务已被其他worker抢走: taskNo={pendingTask.Id}");
                return null;
            }

            int newAttemptCount = pendingTask.AttemptCount + 1;
            // 详细日志：记录每次fetch返回的完整信息，用于排查图片错乱
            logger.Info($"[FETCH] taskNo={pendingTask.Id}, inputImageUrl={pendingTask.InputImagePath}, prompt={pendingTask.Prompt?.Substring(0, Math.Min(50, pendingTask.Prompt?.Length ?? 0))}, attempt={newAttemptCount}, hash={pendingTask.InputImageHash}");

            return new N8nFetchTaskDto
            {
                TaskNo = pendingTask.Id,
                InputImageUrl = pendingTask.InputImagePath,
                Prompt = pendingTask.Prompt,
                ExtParams = pendingTask.ExtParams,
                InputImageHash = pendingTask.InputImageHash,
                AttemptCount = newAttemptCount
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
            if (task.Status != "processing")
            {
                throw new CustomException($"任务状态不允许上传结果（当前状态: {task.Status}）");
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
        public string UploadBase64Image(long taskNo, string base64Image, int? fetchAttemptCount = null)
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
            if (task.Status != "processing")
            {
                throw new CustomException($"任务状态不允许上传结果（当前状态: {task.Status}）");
            }
            // 校验AttemptCount：防止旧worker的stale上传覆盖新结果
            if (fetchAttemptCount.HasValue && fetchAttemptCount.Value != task.AttemptCount)
            {
                throw new CustomException($"任务已重新处理（attemptCount不匹配: 期望{task.AttemptCount}, 收到{fetchAttemptCount.Value}）");
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

            // 原子UPDATE：只有Status仍为processing时才成功
            var affected = Context.Updateable<AiTask>()
                .SetColumns(x => new AiTask {
                    Status = "done",
                    OutputImagePath = accessUrl,
                    CompleteTime = DateTime.Now
                })
                .Where(x => x.Id == taskNo && x.Status == "processing")
                .ExecuteCommand();

            if (affected == 0)
            {
                logger.Warn($"Base64上传被拒绝（状态已变更）: taskNo={taskNo}, 当前status={task.Status}");
                throw new CustomException($"任务状态不允许上传结果（当前状态: {task.Status}）");
            }

            logger.Info($"Base64结果图上传: taskNo={taskNo}, url={accessUrl}");

            return accessUrl;
        }

        /// <summary>
        /// N8N成功回调（下载图片保存到本地）
        /// 允许processing和failed(超时)状态的callback成功，防止超时后N8N完成但任务卡在failed
        /// </summary>
        public bool CallbackSuccess(long taskNo, string outputImageUrl, int? fetchAttemptCount = null)
        {
            var task = Queryable().Where(x => x.Id == taskNo).First();
            if (task == null)
            {
                logger.Warn($"回调失败: 任务不存在 taskNo={taskNo}");
                return false;
            }

            // 允许processing和failed(超时)两种状态的callback成功
            // 这样即使超时服务把任务标记为failed，N8N最终完成时也能正确更新状态
            if (task.Status != "processing" && task.Status != "failed")
            {
                logger.Warn($"回调拒绝: 任务状态不接受callback taskNo={taskNo}, status={task.Status}");
                return false;
            }

            // 如果是failed状态但错误不是"处理超时"，拒绝
            if (task.Status == "failed" && task.ErrorMessage != "处理超时")
            {
                logger.Warn($"回调拒绝: 任务已失败且非超时 taskNo={taskNo}, error={task.ErrorMessage}");
                return false;
            }

            // 校验AttemptCount
            if (fetchAttemptCount.HasValue && fetchAttemptCount.Value != task.AttemptCount)
            {
                logger.Warn($"回调拒绝（attemptCount不匹配）: taskNo={taskNo}, callbackAttempt={fetchAttemptCount.Value}, currentAttempt={task.AttemptCount}");
                return false;
            }

            // 详细日志
            logger.Info($"[CALLBACK] taskNo={taskNo}, inputImageUrl={task.InputImagePath}, outputImageUrl={outputImageUrl}, currentStatus={task.Status}, attempt={task.AttemptCount}");

            // 下载图片并保存到本地存储
            string localUrl = DownloadAndSaveImage(taskNo, outputImageUrl);

            // 原子UPDATE：允许processing和failed(超时)状态更新为done
            var affected = Context.Updateable<AiTask>()
                .SetColumns(x => new AiTask {
                    Status = "done",
                    OutputImagePath = localUrl,
                    CompleteTime = DateTime.Now,
                    ErrorMessage = null
                })
                .Where(x => x.Id == taskNo && (x.Status == "processing" || (x.Status == "failed" && x.ErrorMessage == "处理超时")))
                .ExecuteCommand();

            if (affected == 0)
            {
                logger.Warn($"回调被拒绝（状态已变更）: taskNo={taskNo}, 当前status={task.Status}");
                return false;
            }

            logger.Info($"[DONE] taskNo={taskNo}, inputImageUrl={task.InputImagePath}, outputSavedTo={localUrl}");

            return true;
        }

        /// <summary>
        /// 从URL下载图片并保存到本地存储目录
        /// </summary>
        private string DownloadAndSaveImage(long taskNo, string imageUrl)
        {
            string storageRoot = GetStorageRoot();
            string outputDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "output", taskNo.ToString());
            Directory.CreateDirectory(outputDir);

            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var imageBytes = httpClient.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();

            // 根据Content-Type或URL判断扩展名
            string ext = ".png";
            if (imageUrl.Contains(".jpg") || imageUrl.Contains(".jpeg")) ext = ".jpg";
            else if (imageUrl.Contains(".webp")) ext = ".webp";
            else if (imageUrl.Contains(".gif")) ext = ".gif";

            string fileName = "result" + ext;
            string filePath = Path.Combine(outputDir, fileName);
            File.WriteAllBytes(filePath, imageBytes);

            string accessUrl = string.Concat(
                _optionsSetting.Upload.UploadUrl.TrimEnd('/'), "/",
                storageRoot.Replace("\\", "/"), "/output/",
                taskNo, "/", fileName);

            // 详细日志：记录文件保存
            logger.Info($"[SAVE] taskNo={taskNo}, savedTo={filePath}, size={imageBytes.Length}bytes, outputUrl={accessUrl}");

            return accessUrl;
        }

        /// <summary>
        /// N8N失败回调
        /// 使用原子UPDATE，防止已成功的任务被误标为failed
        /// </summary>
        public bool CallbackFailed(long taskNo, string errorMsg)
        {
            var task = Queryable().Where(x => x.Id == taskNo).First();
            if (task == null)
            {
                logger.Warn($"回调失败: 任务不存在 taskNo={taskNo}");
                return false;
            }

            if (task.Status == "done")
            {
                logger.Warn($"回调忽略: 任务已完成 taskNo={taskNo}, attempt={task.AttemptCount}");
                return false;
            }

            // 原子UPDATE：只有Status不是done时才标记为failed
            var affected = Context.Updateable<AiTask>()
                .SetColumns(x => new AiTask {
                    Status = "failed",
                    ErrorMessage = errorMsg,
                    CompleteTime = DateTime.Now
                })
                .Where(x => x.Id == taskNo && x.Status != "done")
                .ExecuteCommand();

            if (affected == 0)
            {
                logger.Warn($"失败回调被拒绝（任务已完成）: taskNo={taskNo}, attempt={task.AttemptCount}");
                return false;
            }

            logger.Info($"任务失败: taskNo={taskNo}, error={errorMsg}, attempt={task.AttemptCount}");

            return true;
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
            if (task.Status == "processing")
            {
                throw new CustomException("任务正在处理中，请等待完成后再重试");
            }

            task.Status = "pending";
            task.RetryCount += 1;
            task.ProcessStartTime = null;
            task.ErrorMessage = null;
            task.CompleteTime = null;

            // 删除旧的结果图文件，清空结果图路径
            DeleteOutputFile(taskNo);
            task.OutputImagePath = null;

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
                .Where(x => x.UserId == userId && x.Status == "failed")
                .ToList();

            if (failedTasks.Count == 0) return 0;

            foreach (var task in failedTasks)
            {
                task.Status = "pending";
                task.RetryCount += 1;
                task.ProcessStartTime = null;
                task.ErrorMessage = null;
                task.CompleteTime = null;
                DeleteOutputFile(task.Id);
                task.OutputImagePath = null;
            }

            var result = Context.Updateable(failedTasks).ExecuteCommand();
            logger.Info($"批量重试: userId={userId}, count={failedTasks.Count}");

            return result;
        }

        /// <summary>
        /// 检测超时任务（大幅增加超时时间，防止N8N处理中被误标为failed）
        /// </summary>
        public int CheckTimeout(int timeoutMinutes)
        {
            // 最少30分钟超时，AI图片生成可能耗时较长
            if (timeoutMinutes <= 0 || timeoutMinutes < 30) timeoutMinutes = 30;

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
                logger.Warn($"检测到{result}个超时任务，已标记为failed（超时阈值={timeoutMinutes}分钟）");
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

        /// <summary>
        /// 删除任务的结果图文件
        /// </summary>
        private void DeleteOutputFile(long taskNo)
        {
            try
            {
                string storageRoot = GetStorageRoot();
                string outputDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "output", taskNo.ToString());
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                    logger.Info($"已删除结果图目录: taskNo={taskNo}, dir={outputDir}");
                }
            }
            catch (Exception ex)
            {
                logger.Warn($"删除结果图失败: taskNo={taskNo}, error={ex.Message}");
            }
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

        /// <summary>
        /// 批量管理标签
        /// </summary>
        public int BatchAddTags(List<long> taskNos, string tags, string removeTags, long userId)
        {
            if (taskNos == null || taskNos.Count == 0)
                throw new CustomException("请选择要操作的任务");

            var addList = string.IsNullOrEmpty(tags)
                ? new List<string>()
                : tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Distinct()
                    .ToList();

            var removeList = string.IsNullOrEmpty(removeTags)
                ? new List<string>()
                : removeTags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Distinct()
                    .ToList();

            if (addList.Count == 0 && removeList.Count == 0)
                throw new CustomException("请至少添加或删除一个标签");

            var tasks = Queryable()
                .Where(x => taskNos.Contains(x.Id) && x.UserId == userId)
                .ToList();

            if (tasks.Count == 0)
                throw new CustomException("未找到可操作的任务");

            foreach (var task in tasks)
            {
                var existingTags = string.IsNullOrEmpty(task.Tags)
                    ? new List<string>()
                    : task.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .ToList();

                // 移除标签
                foreach (var tag in removeList)
                {
                    existingTags.Remove(tag);
                }

                // 添加标签
                foreach (var tag in addList)
                {
                    if (!existingTags.Contains(tag))
                        existingTags.Add(tag);
                }

                task.Tags = existingTags.Count > 0 ? string.Join(",", existingTags) : null;
                task.Update_time = DateTime.Now;
            }

            var result = Context.Updateable(tasks).ExecuteCommand();
            logger.Info($"批量管理标签: userId={userId}, count={tasks.Count}, add={tags}, remove={removeTags}");
            return result;
        }

        /// <summary>
        /// 批量下载结果图（打包ZIP）
        /// </summary>
        public MemoryStream BatchDownloadResult(List<long> taskNos, long userId)
        {
            var tasks = Queryable()
                .Where(x => taskNos.Contains(x.Id) && x.UserId == userId && x.Status == "done")
                .ToList();

            if (tasks.Count == 0)
                throw new CustomException("没有可下载的已完成任务");

            var stream = new MemoryStream();
            using (var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                foreach (var task in tasks)
                {
                    if (string.IsNullOrEmpty(task.OutputImagePath)) continue;

                    // 解析本地文件路径
                    var uri = new Uri(task.OutputImagePath);
                    var localPath = Path.Combine(_webHostEnvironment.WebRootPath,
                        uri.PathAndQuery.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (!File.Exists(localPath)) continue;

                    // 确定文件夹名（标签或"未分类"）
                    var folderName = "未分类";
                    if (!string.IsNullOrEmpty(task.Tags))
                    {
                        var firstTag = task.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
                        if (!string.IsNullOrEmpty(firstTag))
                            folderName = firstTag;
                    }

                    var entryName = $"{folderName}/{task.Id}{Path.GetExtension(localPath)}";
                    archive.CreateEntryFromFile(localPath, entryName, System.IO.Compression.CompressionLevel.Fastest);
                }
            }

            stream.Position = 0;
            logger.Info($"批量下载结果图: userId={userId}, count={tasks.Count}");
            return stream;
        }

        /// <summary>
        /// 获取结果图存储路径
        /// </summary>
        public string GetResultStoragePath()
        {
            var root = AppSettings.GetConfig("AiTask:StorageRoot");
            return string.IsNullOrEmpty(root) ? "storage/ai" : root;
        }

        /// <summary>
        /// 获取结果图列表（匿名可访问，只返回未发布的，按任务名称模糊搜索）
        /// </summary>
        public object GetResultImageList(AiTaskListDto parm)
        {
            var query = Queryable()
                .Where(x => x.Status == "done" && x.OutputImagePath != null && x.OutputImagePath != "" && (x.PublishStatus == 0 || x.PublishStatus == null));

            // 按任务名称模糊搜索
            if (!string.IsNullOrEmpty(parm.Prompt))
                query = query.Where(x => x.TaskName != null && x.TaskName.Contains(parm.Prompt));

            var totalNum = query.Count();

            // 先取全部，再内存排序
            var allList = query.OrderByDescending(x => x.Create_time).ToList();

            // 自定义排序：首页*在前、二图*其次、三图*、四图*、其他在后
            Func<string, int> getOrder = name => {
                if (string.IsNullOrEmpty(name)) return 5;
                if (name.StartsWith("首页")) return 1;
                if (name.StartsWith("二图")) return 2;
                if (name.StartsWith("三图")) return 3;
                if (name.StartsWith("四图")) return 4;
                return 5;
            };
            allList = allList.OrderBy(x => getOrder(x.TaskName)).ThenBy(x => x.TaskName).ToList();

            var result = allList
                .Skip((parm.PageNum - 1) * parm.PageSize)
                .Take(parm.PageSize)
                .Select(x => new
                {
                    id = x.Id,
                    taskName = x.TaskName,
                    prompt = x.Prompt,
                    outputImagePath = x.OutputImagePath,
                    tags = x.Tags,
                    createTime = x.Create_time
                })
                .ToList();

            return new { result, totalNum };
        }

        /// <summary>
        /// 批量标记任务为已发布
        /// </summary>
        public int BatchMarkPublished(List<long> taskNos, long userId)
        {
            return Context.Updateable<AiTask>()
                .SetColumns(x => new AiTask { PublishStatus = 1 })
                .Where(x => taskNos.Contains(x.Id) && x.UserId == userId && x.Status == "done")
                .ExecuteCommand();
        }
    }
}
