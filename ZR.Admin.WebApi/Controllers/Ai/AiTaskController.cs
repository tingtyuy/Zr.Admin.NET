using Microsoft.AspNetCore.Mvc;
using ZR.Model.System;
using ZR.Model.System.Dto;
using ZR.ServiceCore.Services;

namespace ZR.Admin.WebApi.Controllers
{
    /// <summary>
    /// AI任务管理
    /// </summary>
    [Route("ai/task")]
    [ApiExplorerSettings(GroupName = "ai")]
    public class AiTaskController : BaseController
    {
        private readonly IAiTaskService _aiTaskService;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public AiTaskController(IAiTaskService aiTaskService)
        {
            _aiTaskService = aiTaskService;
        }

        [HttpPost("submit")]
        [Log(Title = "AI任务提交", BusinessType = BusinessType.INSERT)]
        public IActionResult Submit([FromForm] string prompt, IFormFile file, [FromForm] string tags = null, [FromForm] string taskName = null, [FromForm] int taskCount = 1)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "提示词不能为空");
            }
            if (file == null || file.Length == 0)
            {
                return ToResponse(ResultCode.PARAM_ERROR, "上传图片不能为空");
            }
            if (taskCount < 1) taskCount = 1;
            if (taskCount > 20) taskCount = 20;

            var userId = HttpContext.GetUId();
            var dto = new AiTaskSubmitDto { Prompt = prompt, Tags = tags, TaskName = taskName };

            var taskNos = new List<string>();
            for (int i = 0; i < taskCount; i++)
            {
                string name = taskName;
                if (taskCount > 1 && !string.IsNullOrEmpty(taskName))
                {
                    name = $"{taskName}{i + 1}";
                }
                dto.TaskName = name;
                long taskNo = _aiTaskService.SubmitTask(dto, file, userId);
                taskNos.Add(taskNo.ToString());
            }

            return SUCCESS(new { taskNos });
        }

        [HttpGet("status/{taskNo}")]
        public IActionResult GetStatus(string taskNo)
        {
            if (!long.TryParse(taskNo, out long taskNoLong))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "任务号格式错误");
            }
            var userId = HttpContext.GetUId();
            var task = _aiTaskService.GetTaskStatus(taskNoLong, userId);
            if (task == null)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "任务不存在");
            }

            return SUCCESS(new
            {
                taskNo = task.Id.ToString(),
                status = task.Status,
                inputImageUrl = task.InputImagePath,
                outputImageUrl = task.OutputImagePath,
                prompt = task.Prompt,
                tags = task.Tags,
                createTime = task.Create_time,
                completeTime = task.CompleteTime,
                errorMessage = task.ErrorMessage
            });
        }

        [HttpGet("list")]
        public IActionResult GetList([FromQuery] AiTaskListDto parm)
        {
            var userId = HttpContext.GetUId();
            var response = _aiTaskService.GetMyTaskList(parm, userId);
            return SUCCESS(response);
        }

        [HttpGet("fetch")]
        [AllowAnonymous]
        public IActionResult Fetch()
        {
            var task = _aiTaskService.FetchPendingTask();
            if (task == null)
            {
                return SUCCESS(new { message = "no task available" });
            }
            var imageUrl = BuildImageUrl(task.InputImageUrl);
            return SUCCESS(new
            {
                taskNo = task.TaskNo.ToString(),
                inputImageUrl = imageUrl,
                prompt = task.Prompt,
                extParams = task.ExtParams,
                inputImageHash = task.InputImageHash,
                attemptCount = task.AttemptCount
            });
        }

        private string BuildImageUrl(string storedUrl)
        {
            if (string.IsNullOrEmpty(storedUrl)) return storedUrl;
            var uri = new Uri(storedUrl);
            var request = HttpContext.Request;
            var basePath = request.Scheme + "://" + request.Host;
            return basePath + uri.PathAndQuery;
        }

        [HttpPost("upload")]
        [AllowAnonymous]
        public IActionResult UploadResult([FromForm] string taskNo, [FromForm] string image, [FromForm] string attemptCount = null)
        {
            if (string.IsNullOrEmpty(taskNo))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "任务号不能为空");
            }
            if (string.IsNullOrEmpty(image))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "image参数不能为空");
            }
            if (!long.TryParse(taskNo, out long taskNoLong))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "任务号格式错误");
            }

            int? fetchAttempt = null;
            if (!string.IsNullOrEmpty(attemptCount) && int.TryParse(attemptCount, out int ac))
                fetchAttempt = ac;

            try
            {
                var outputImageUrl = _aiTaskService.UploadBase64Image(taskNoLong, image, fetchAttempt);
                return SUCCESS(new { outputImageUrl });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("update/{taskNo}")]
        public IActionResult UpdateTask(string taskNo, [FromBody] AiTaskUpdateDto dto)
        {
            if (!long.TryParse(taskNo, out long taskNoLong))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "任务号格式错误");
            }
            if (dto == null)
            {
                return ToResponse(ResultCode.PARAM_ERROR, "参数错误");
            }

            var userId = HttpContext.GetUId();
            var result = _aiTaskService.UpdateTask(taskNoLong, dto.Prompt, userId, dto.Tags, dto.TaskName);
            if (result) return SUCCESS(new { message = "ok" });
            return ToResponse(ResultCode.CUSTOM_ERROR, "更新失败");
        }

        [HttpPost("callback/success")]
        [AllowAnonymous]
        public IActionResult CallbackSuccess([FromBody] N8nCallbackSuccessDto dto)
        {
            if (dto == null || dto.TaskNo <= 0)
            {
                return ToResponse(ResultCode.PARAM_ERROR, "参数错误");
            }
            var result = _aiTaskService.CallbackSuccess(dto.TaskNo, dto.OutputImageUrl, dto.FetchAttemptCount);
            if (result) return SUCCESS(new { message = "ok" });
            return ToResponse(ResultCode.CUSTOM_ERROR, "回调处理失败");
        }

        [HttpPost("callback/failed")]
        [AllowAnonymous]
        public IActionResult CallbackFailed([FromBody] N8nCallbackFailedDto dto)
        {
            if (dto == null || dto.TaskNo <= 0)
            {
                return ToResponse(ResultCode.PARAM_ERROR, "参数错误");
            }
            var result = _aiTaskService.CallbackFailed(dto.TaskNo, dto.ErrorMessage ?? "unknown error");
            if (result) return SUCCESS(new { message = "ok" });
            return ToResponse(ResultCode.CUSTOM_ERROR, "回调处理失败");
        }

        [HttpPost("retry/{taskNo}")]
        public IActionResult Retry(string taskNo)
        {
            if (!long.TryParse(taskNo, out long taskNoLong))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "任务号格式错误");
            }
            try
            {
                var result = _aiTaskService.RetryTask(taskNoLong);
                if (result) return SUCCESS(new { message = "ok" });
                return ToResponse(ResultCode.CUSTOM_ERROR, "重试失败");
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("batch-retry")]
        public IActionResult BatchRetry()
        {
            try
            {
                var userId = HttpContext.GetUId();
                var count = _aiTaskService.BatchRetryFailed(userId);
                return SUCCESS(new { message = $"已重试{count}个任务", count });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("delete/{taskNo}")]
        public IActionResult Delete(string taskNo)
        {
            if (!long.TryParse(taskNo, out long taskNoLong))
                return ToResponse(ResultCode.PARAM_ERROR, "任务号格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _aiTaskService.DeleteTask(taskNoLong, userId);
                if (result) return SUCCESS(new { message = "ok" });
                return ToResponse(ResultCode.CUSTOM_ERROR, "删除失败");
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpGet("template/list")]
        public IActionResult GetTemplateList()
        {
            var userId = HttpContext.GetUId();
            return SUCCESS(_aiTaskService.GetMyTemplates(userId));
        }

        [HttpPost("template/save")]
        [Log(Title = "保存提示词模板", BusinessType = BusinessType.INSERT)]
        public IActionResult SaveTemplate([FromBody] AiPromptTemplateDto dto)
        {
            var userId = HttpContext.GetUId();
            var result = _aiTaskService.SaveTemplate(dto, userId);
            return SUCCESS(result);
        }

        [HttpPost("template/delete/{id}")]
        [Log(Title = "删除提示词模板", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteTemplate(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            var userId = HttpContext.GetUId();
            var result = _aiTaskService.DeleteTemplate(idLong, userId);
            return SUCCESS(result);
        }

        /// <summary>
        /// 批量管理标签
        /// </summary>
        [HttpPost("batch-tags")]
        [Log(Title = "批量管理标签", BusinessType = BusinessType.UPDATE)]
        public IActionResult BatchTags([FromBody] AiBatchTagsDto dto)
        {
            if (dto == null || dto.TaskNos == null || dto.TaskNos.Count == 0)
                return ToResponse(ResultCode.PARAM_ERROR, "请选择任务");
            if (string.IsNullOrEmpty(dto.Tags) && string.IsNullOrEmpty(dto.RemoveTags))
                return ToResponse(ResultCode.PARAM_ERROR, "请至少添加或删除一个标签");

            var userId = HttpContext.GetUId();
            try
            {
                var count = _aiTaskService.BatchAddTags(dto.TaskNos, dto.Tags, dto.RemoveTags, userId);
                return SUCCESS(new { message = $"已为{count}个任务更新标签", count });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        /// <summary>
        /// 批量下载结果图（ZIP）
        /// </summary>
        [HttpPost("batch-download")]
        public IActionResult BatchDownload([FromBody] AiBatchDownloadDto dto)
        {
            if (dto == null || dto.TaskNos == null || dto.TaskNos.Count == 0)
                return ToResponse(ResultCode.PARAM_ERROR, "请选择任务");

            var userId = HttpContext.GetUId();
            try
            {
                var stream = _aiTaskService.BatchDownloadResult(dto.TaskNos, userId);
                return File(stream, "application/zip", "ai_results.zip");
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        /// <summary>
        /// 获取结果图存储路径
        /// </summary>
        [HttpGet("storage-path")]
        public IActionResult GetStoragePath()
        {
            var path = _aiTaskService.GetResultStoragePath();
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", path.Replace('/', Path.DirectorySeparatorChar));
            return SUCCESS(new { path = fullPath });
        }

        /// <summary>
        /// 获取AI任务结果图列表（供QianFan Sync导入，匿名可访问，只返回未发布的）
        /// </summary>
        [HttpGet("result-images")]
        [AllowAnonymous]
        public IActionResult GetResultImages([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 20, [FromQuery] string keyword = null)
        {
            var query = new AiTaskListDto { PageNum = pageNum, PageSize = pageSize, Status = "done" };
            if (!string.IsNullOrEmpty(keyword))
                query.Prompt = keyword;

            long userId = 0;
            try { userId = HttpContext.GetUId(); } catch { }

            if (userId > 0)
            {
                var response = _aiTaskService.GetMyTaskList(query, userId);
                var result = response.Result?
                    .Where(x => !string.IsNullOrEmpty(x.OutputImagePath) && (x.PublishStatus == 0 || x.PublishStatus == null))
                    .Select(x => new { id = x.Id, prompt = x.Prompt, outputImagePath = x.OutputImagePath, tags = x.Tags, createTime = x.Create_time })
                    .ToList();
                return SUCCESS(new { result, totalNum = result.Count });
            }
            else
            {
                var response = _aiTaskService.GetResultImageList(query);
                return SUCCESS(response);
            }
        }

        /// <summary>
        /// 批量标记任务为已发布
        /// </summary>
        [HttpPost("batch-publish")]
        public IActionResult BatchMarkPublished([FromBody] BatchPublishDto dto)
        {
            if (dto?.TaskNos == null || dto.TaskNos.Count == 0)
                return ToResponse(ResultCode.PARAM_ERROR, "任务号不能为空");

            var userId = HttpContext.GetUId();
            var count = _aiTaskService.BatchMarkPublished(dto.TaskNos, userId);
            return SUCCESS(new { message = $"已标记 {count} 个任务为已发布" });
        }
    }
}

/// <summary>
/// 批量标记已发布DTO
/// </summary>
public class BatchPublishDto
{
    public List<long> TaskNos { get; set; }
}
