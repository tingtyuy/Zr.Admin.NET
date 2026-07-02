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
        public IActionResult Submit([FromForm] string prompt, IFormFile file)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "提示词不能为空");
            }
            if (file == null || file.Length == 0)
            {
                return ToResponse(ResultCode.PARAM_ERROR, "上传图片不能为空");
            }

            var userId = HttpContext.GetUId();
            var dto = new AiTaskSubmitDto { Prompt = prompt };
            long taskNo = _aiTaskService.SubmitTask(dto, file, userId);

            return SUCCESS(new { taskNo = taskNo.ToString() });
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
                extParams = task.ExtParams
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
        public IActionResult UploadResult([FromForm] string taskNo, [FromForm] string image)
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

            try
            {
                var outputImageUrl = _aiTaskService.UploadBase64Image(taskNoLong, image);
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
            if (dto == null || string.IsNullOrEmpty(dto.Prompt))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "提示词不能为空");
            }

            var userId = HttpContext.GetUId();
            var result = _aiTaskService.UpdateTask(taskNoLong, dto.Prompt, userId);
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
            var result = _aiTaskService.CallbackSuccess(dto.TaskNo, dto.OutputImageUrl);
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
    }
}
