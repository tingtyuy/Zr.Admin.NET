using Microsoft.AspNetCore.Mvc;
using ZR.Model.System;
using ZR.Model.System.Dto;
using ZR.ServiceCore.Services;

namespace ZR.Admin.WebApi.Controllers
{
    /// <summary>
    /// ComfyUI管理
    /// </summary>
    [Route("comfyui")]
    [ApiExplorerSettings(GroupName = "ai")]
    public class ComfyuiController : BaseController
    {
        private readonly IComfyuiService _comfyuiService;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ComfyuiController(IComfyuiService comfyuiService)
        {
            _comfyuiService = comfyuiService;
        }

        /// <summary>
        /// 获取ComfyUI服务端配置（域名）
        /// </summary>
        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return SUCCESS(new { serverUrl = _comfyuiService.GetServerUrl() });
        }

        /// <summary>
        /// 保存ComfyUI服务端地址（域名）
        /// </summary>
        [HttpPost("config")]
        [Log(Title = "ComfyUI服务端配置", BusinessType = BusinessType.UPDATE)]
        public IActionResult SaveConfig([FromBody] ComfyuiConfigSaveDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ServerUrl))
                return ToResponse(ResultCode.PARAM_ERROR, "ComfyUI服务地址不能为空");
            try
            {
                var result = _comfyuiService.SaveServerUrl(dto.ServerUrl);
                return SUCCESS(new { message = result ? "保存成功" : "保存失败", result, serverUrl = _comfyuiService.GetServerUrl() });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        /// <summary>
        /// 测试ComfyUI服务连通性
        /// </summary>
        [HttpPost("config/test")]
        public IActionResult TestConfig([FromBody] ComfyuiConfigSaveDto dto)
        {
            bool ok = _comfyuiService.TestConnection(dto?.ServerUrl, out string message);
            return SUCCESS(new { ok, message });
        }

        #region 工作流
        [HttpPost("workflow/import")]
        [Log(Title = "ComfyUI工作流导入", BusinessType = BusinessType.INSERT)]
        public IActionResult ImportWorkflows([FromBody] ComfyuiWorkflowBatchImportDto dto)
        {
            if (dto?.Workflows == null || dto.Workflows.Count == 0)
            {
                return ToResponse(ResultCode.PARAM_ERROR, "工作流列表不能为空");
            }
            try
            {
                var userId = HttpContext.GetUId();
                var count = _comfyuiService.BatchImportWorkflows(dto, userId);
                return SUCCESS(new { message = $"成功导入 {count} 个工作流", count });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpGet("workflow/list")]
        public IActionResult GetWorkflowList([FromQuery] ComfyuiWorkflowListDto parm)
        {
            var userId = HttpContext.GetUId();
            return SUCCESS(_comfyuiService.GetWorkflowList(parm, userId));
        }

        [HttpGet("workflow/detail/{id}")]
        public IActionResult GetWorkflowDetail(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            var userId = HttpContext.GetUId();
            var detail = _comfyuiService.GetWorkflowDetail(idLong, userId);
            if (detail == null)
                return ToResponse(ResultCode.CUSTOM_ERROR, "工作流不存在");
            return SUCCESS(detail);
        }

        [HttpGet("workflow/variables/{id}")]
        public IActionResult GetWorkflowVariables(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            var userId = HttpContext.GetUId();
            return SUCCESS(_comfyuiService.GetWorkflowVariableNodes(idLong, userId));
        }

        /// <summary>
        /// 筛查工作流可编辑节点（带描述，供选择可变节点）
        /// </summary>
        [HttpGet("workflow/{id}/editable-nodes")]
        public IActionResult GetEditableNodes(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            var userId = HttpContext.GetUId();
            return SUCCESS(_comfyuiService.GetEditableNodes(idLong, userId));
        }

        [HttpPost("workflow/category")]
        [Log(Title = "ComfyUI工作流分类", BusinessType = BusinessType.UPDATE)]
        public IActionResult SetCategory([FromBody] ComfyuiWorkflowCategoryDto dto)
        {
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.SetWorkflowCategory(dto, userId);
                return SUCCESS(new { message = result ? "设置成功" : "设置失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("workflow/delete/{id}")]
        [Log(Title = "ComfyUI工作流删除", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteWorkflow(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            var userId = HttpContext.GetUId();
            var result = _comfyuiService.DeleteWorkflow(idLong, userId);
            return SUCCESS(new { message = result ? "删除成功" : "删除失败", result });
        }

        [HttpPost("workflow/update/{id}")]
        [Log(Title = "ComfyUI工作流编辑", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateWorkflow(string id, [FromBody] ComfyuiWorkflowImportDto dto)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.UpdateWorkflow(idLong, dto, userId);
                return SUCCESS(new { message = result ? "保存成功" : "保存失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("workflow/variables/{id}")]
        [Log(Title = "ComfyUI工作流可变节点配置", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateVariables(string id, [FromBody] ComfyuiWorkflowVariablesDto dto)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.UpdateWorkflowVariables(idLong, dto?.VariableNodes, userId);
                return SUCCESS(new { message = result ? "配置成功" : "配置失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }
        #endregion

        #region 任务
        [HttpPost("task/create")]
        [RequestSizeLimit(500 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 500 * 1024 * 1024)]
        [Log(Title = "ComfyUI任务创建", BusinessType = BusinessType.INSERT)]
        public async Task<IActionResult> CreateTask()
        {
            string workflowId = null, funcType = null, variableValues = null, seedMode = null;
            int taskCount = 1;
            var refs = new Dictionary<string, IFormFile>();

            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                workflowId = form["workflowId"].FirstOrDefault();
                funcType = form["funcType"].FirstOrDefault();
                variableValues = form["variableValues"].FirstOrDefault();
                seedMode = form["seedMode"].FirstOrDefault();
                if (form.ContainsKey("taskCount") && int.TryParse(form["taskCount"].FirstOrDefault(), out int tc))
                    taskCount = tc;
                foreach (var f in form.Files)
                {
                    string key = f.Name;
                    if (key.StartsWith("ref_")) key = key.Substring(4);
                    if (!refs.ContainsKey(key)) refs[key] = f;
                }
            }
            else
            {
                return ToResponse(ResultCode.PARAM_ERROR, "请以 multipart/form-data 方式提交");
            }

            if (!long.TryParse(workflowId, out long wid) || wid <= 0)
                return ToResponse(ResultCode.PARAM_ERROR, "请选择工作流");
            if (string.IsNullOrEmpty(funcType))
                return ToResponse(ResultCode.PARAM_ERROR, "功能类型不能为空");

            var userId = HttpContext.GetUId();
            var dto = new ComfyuiTaskCreateDto
            {
                WorkflowId = wid,
                FuncType = funcType,
                VariableValues = variableValues,
                TaskCount = taskCount,
                SeedMode = seedMode
            };
            try
            {
                var (taskNos, validationError) = _comfyuiService.CreateTask(dto, refs, userId);
                string msg = validationError == null
                    ? $"成功创建 {taskNos.Count} 个任务（草稿，可入队执行）"
                    : $"已保存为草稿（{taskNos.Count} 个），校验提示：{validationError}";
                return SUCCESS(new { message = msg, taskNos, validationError });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("task/update/{id}")]
        [RequestSizeLimit(500 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 500 * 1024 * 1024)]
        [Log(Title = "ComfyUI任务更新", BusinessType = BusinessType.UPDATE)]
        public async Task<IActionResult> UpdateTask(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            string funcType = null, variableValues = null, seedMode = null;
            var refs = new Dictionary<string, IFormFile>();

            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                funcType = form["funcType"].FirstOrDefault();
                variableValues = form["variableValues"].FirstOrDefault();
                seedMode = form["seedMode"].FirstOrDefault();
                foreach (var f in form.Files)
                {
                    string key = f.Name;
                    if (key.StartsWith("ref_")) key = key.Substring(4);
                    if (!refs.ContainsKey(key)) refs[key] = f;
                }
            }
            else
            {
                return ToResponse(ResultCode.PARAM_ERROR, "请以 multipart/form-data 方式提交");
            }

            var userId = HttpContext.GetUId();
            var dto = new ComfyuiTaskCreateDto
            {
                WorkflowId = 0,
                FuncType = funcType,
                VariableValues = variableValues,
                TaskCount = 1,
                SeedMode = seedMode
            };
            try
            {
                var (taskNos, validationError) = _comfyuiService.UpdateTask(idLong, dto, refs, userId);
                string msg = validationError == null
                    ? $"任务已更新并入队执行"
                    : $"已保存，校验提示：{validationError}";
                return SUCCESS(new { message = msg, taskNos, validationError });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpGet("task/list")]
        public IActionResult GetTaskList([FromQuery] ComfyuiTaskListDto parm)
        {
            var userId = HttpContext.GetUId();
            return SUCCESS(_comfyuiService.GetTaskList(parm, userId));
        }

        [HttpGet("task/detail/{id}")]
        public IActionResult GetTaskDetail(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            var userId = HttpContext.GetUId();
            var detail = _comfyuiService.GetTaskDetail(idLong, userId);
            if (detail == null)
                return ToResponse(ResultCode.CUSTOM_ERROR, "任务不存在");
            return SUCCESS(detail);
        }

        [HttpPost("task/delete/{id}")]
        [Log(Title = "ComfyUI任务删除", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteTask(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.DeleteTask(idLong, userId);
                return SUCCESS(new { message = result ? "删除成功" : "删除失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("task/batch-delete")]
        [Log(Title = "ComfyUI任务批量删除", BusinessType = BusinessType.DELETE)]
        public IActionResult BatchDeleteTask([FromBody] ComfyuiQueueEnqueueDto dto)
        {
            var userId = HttpContext.GetUId();
            var count = _comfyuiService.BatchDeleteTask(dto?.TaskIds, userId);
            return SUCCESS(new { message = $"已删除 {count} 个任务", count });
        }

        [HttpPost("task/publish-status/{id}")]
        [Log(Title = "ComfyUI任务发布状态", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdatePublishStatus(string id, [FromBody] ComfyuiPublishStatusDto dto)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.UpdatePublishStatus(idLong, dto?.PublishStatus, userId);
                return SUCCESS(new { message = result ? "更新成功" : "更新失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("task/publish-status/batch")]
        [Log(Title = "ComfyUI任务批量发布状态", BusinessType = BusinessType.UPDATE)]
        public IActionResult BatchUpdatePublishStatus([FromBody] ComfyuiPublishStatusBatchDto dto)
        {
            try
            {
                var userId = HttpContext.GetUId();
                var count = _comfyuiService.BatchUpdatePublishStatus(dto?.TaskIds, dto?.PublishStatus, userId);
                return SUCCESS(new { message = $"已更新 {count} 个任务", count });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("task/enqueue")]
        [Log(Title = "ComfyUI任务入队", BusinessType = BusinessType.UPDATE)]
        public IActionResult Enqueue([FromBody] ComfyuiQueueEnqueueDto dto)
        {
            if (dto?.TaskIds == null || dto.TaskIds.Count == 0)
                return ToResponse(ResultCode.PARAM_ERROR, "请选择任务");
            try
            {
                var userId = HttpContext.GetUId();
                var count = _comfyuiService.EnqueueTasks(dto.TaskIds, userId);
                return SUCCESS(new { message = $"已入队 {count} 个任务", count });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }
        #endregion

        #region 执行队列
        [HttpGet("queue/list")]
        public IActionResult GetQueueList([FromQuery] ComfyuiQueueListDto parm)
        {
            var userId = HttpContext.GetUId();
            return SUCCESS(_comfyuiService.GetQueueList(parm, userId));
        }

        [HttpPost("queue/cancel/{id}")]
        [Log(Title = "ComfyUI队列取消", BusinessType = BusinessType.UPDATE)]
        public IActionResult CancelQueue(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.CancelQueue(idLong, userId);
                return SUCCESS(new { message = result ? "已取消" : "取消失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("queue/dequeue/{id}")]
        [Log(Title = "ComfyUI队列出队", BusinessType = BusinessType.UPDATE)]
        public IActionResult Dequeue(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.Dequeue(idLong, userId);
                return SUCCESS(new { message = result ? "已出队" : "出队失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("queue/retry/{id}")]
        [Log(Title = "ComfyUI队列重试", BusinessType = BusinessType.UPDATE)]
        public IActionResult RetryQueue(string id)
        {
            if (!long.TryParse(id, out long idLong))
                return ToResponse(ResultCode.PARAM_ERROR, "ID格式错误");
            try
            {
                var userId = HttpContext.GetUId();
                var result = _comfyuiService.RetryQueue(idLong, userId);
                return SUCCESS(new { message = result ? "已重新入队" : "重试失败", result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }

        [HttpPost("queue/batch-retry")]
        [Log(Title = "ComfyUI队列批量重试", BusinessType = BusinessType.UPDATE)]
        public IActionResult BatchRetryQueue([FromBody] ComfyuiQueueEnqueueDto dto)
        {
            if (dto?.TaskIds == null || dto.TaskIds.Count == 0)
                return ToResponse(ResultCode.PARAM_ERROR, "请选择任务");
            try
            {
                var userId = HttpContext.GetUId();
                var count = _comfyuiService.BatchRetryQueue(dto.TaskIds, userId);
                return SUCCESS(new { message = $"已重试 {count} 个任务", count });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }
        #endregion

        #region 工具
        /// <summary>
        /// 文本翻译（联网翻译，目标语言 zh-CN / en）
        /// </summary>
        [HttpPost("translate")]
        [Log(Title = "ComfyUI文本翻译", BusinessType = BusinessType.OTHER)]
        public async Task<IActionResult> Translate([FromBody] ComfyuiTranslateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Text))
            {
                return ToResponse(ResultCode.PARAM_ERROR, "请输入要翻译的内容");
            }
            try
            {
                var result = await _comfyuiService.TranslateAsync(dto.Text.Trim(), dto.Target);
                return SUCCESS(new { translated = result });
            }
            catch (Exception ex)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, ex.Message);
            }
        }
        #endregion
    }
}
