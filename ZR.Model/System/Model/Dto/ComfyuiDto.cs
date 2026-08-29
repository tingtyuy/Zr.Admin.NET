namespace ZR.Model.System.Dto
{
    /// <summary>
    /// ComfyUI服务端配置保存DTO
    /// </summary>
    public class ComfyuiConfigSaveDto
    {
        public string ServerUrl { get; set; }
    }

    /// <summary>
    /// 工作流可编辑节点DTO（供前端选择可变节点时筛选+展示描述）
    /// </summary>
    public class ComfyuiEditableNodeDto
    {
        /// <summary>
        /// 节点key
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 节点类型（如 CLIPTextEncode/LoadImage/KSampler）
        /// </summary>
        public string ClassType { get; set; }

        /// <summary>
        /// 节点标题（ComfyUI _meta.title，或中文映射）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 推荐可变类型: prompt/value/image/video
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 该节点可修改的字段列表
        /// </summary>
        public List<ComfyuiEditableFieldDto> Fields { get; set; }

        /// <summary>
        /// 节点用途描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 可编辑字段DTO
    /// </summary>
    public class ComfyuiEditableFieldDto
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 字段类型: text/number
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// 推荐可变类型: prompt/value
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 工作流可变节点配置
    /// </summary>
    public class ComfyuiVariableNodeDto
    {        /// <summary>
        /// ComfyUI API中节点key（nodeId）
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 要替换的字段名（如 text/image）
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 类型: prompt/negative_prompt/image/video/value
        /// </summary>
        public string Type { get; set; } = "value";

        /// <summary>
        /// 前端表单显示标签
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// 工作流导入DTO
    /// </summary>
    public class ComfyuiWorkflowImportDto
    {
        [Display(Name = "工作流名称")]
        [Required(ErrorMessage = "工作流名称不能为空")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; } = "default";

        [Display(Name = "工作流JSON")]
        [Required(ErrorMessage = "工作流JSON不能为空")]
        public string WorkflowJson { get; set; }

        /// <summary>
        /// 可变节点配置JSON字符串（数组）
        /// </summary>
        public string VariableNodes { get; set; }

        public string Tags { get; set; }
    }

    /// <summary>
    /// 工作流批量导入DTO
    /// </summary>
    public class ComfyuiWorkflowBatchImportDto
    {
        [Display(Name = "工作流列表")]
        [Required(ErrorMessage = "工作流列表不能为空")]
        public List<ComfyuiWorkflowImportDto> Workflows { get; set; }
    }

    /// <summary>
    /// 工作流列表查询DTO
    /// </summary>
    public class ComfyuiWorkflowListDto : PagerInfo
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Tag { get; set; }
    }

    /// <summary>
    /// 工作流分类设置DTO
    /// </summary>
    public class ComfyuiWorkflowCategoryDto
    {
        [Display(Name = "工作流ID列表")]
        [Required(ErrorMessage = "请选择工作流")]
        public List<long> Ids { get; set; }

        [Display(Name = "分类")]
        [Required(ErrorMessage = "请选择分类")]
        public string Category { get; set; }
    }

    /// <summary>
    /// 工作流可变节点配置更新DTO
    /// </summary>
    public class ComfyuiWorkflowVariablesDto
    {
        public string VariableNodes { get; set; }
    }

    /// <summary>
    /// ComfyUI任务创建DTO（表单创建任务，未入队）
    /// </summary>
    public class ComfyuiTaskCreateDto
    {
        [Display(Name = "工作流")]
        [Required(ErrorMessage = "请选择工作流")]
        public long WorkflowId { get; set; }

        [Display(Name = "功能类型")]
        [Required(ErrorMessage = "功能类型不能为空")]
        public string FuncType { get; set; }

        /// <summary>
        /// 任务数量
        /// </summary>
        public int TaskCount { get; set; } = 1;

        /// <summary>
        /// 可变节点最终值JSON（nodeId->value）
        /// </summary>
        public string VariableValues { get; set; }
    }

    /// <summary>
    /// 参考文件（nodeId + 文件）
    /// </summary>
    public class ComfyuiRefFile
    {
        public string NodeId { get; set; }
        public string LocalPath { get; set; }
        public string OriginalName { get; set; }
        public string ComfyName { get; set; }
        public string Subfolder { get; set; }
        public string ComfyType { get; set; }
    }

    /// <summary>
    /// ComfyUI任务列表查询DTO
    /// </summary>
    public class ComfyuiTaskListDto : PagerInfo
    {
        public string Prompt { get; set; }
        public string Status { get; set; }
        public string FuncType { get; set; }
        public int? Queued { get; set; }
    }

    /// <summary>
    /// ComfyUI执行队列入队DTO
    /// </summary>
    public class ComfyuiQueueEnqueueDto
    {
        [Display(Name = "任务ID列表")]
        [Required(ErrorMessage = "请选择任务")]
        public List<long> TaskIds { get; set; }
    }

    /// <summary>
    /// ComfyUI执行队列查询DTO
    /// </summary>
    public class ComfyuiQueueListDto : PagerInfo
    {
        public string Status { get; set; }
        public string FuncType { get; set; }
    }

    /// <summary>
    /// ComfyUI任务视图DTO（含执行队列信息与输出）
    /// </summary>
    public class ComfyuiTaskView
    {
        [JsonConverter(typeof(ValueToStringConverter))]
        public long Id { get; set; }
        public string TaskName { get; set; }
        public string FuncType { get; set; }
        [JsonConverter(typeof(ValueToStringConverter))]
        public long WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public int Queued { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? QueuedTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public string Create_time { get; set; }
        // 执行队列信息
        [JsonConverter(typeof(ValueToStringConverter))]
        public long? QueueId { get; set; }
        public string QueueStatus { get; set; }
        public int Progress { get; set; }
        public string OutputUrls { get; set; }
        public string QueueErrorMessage { get; set; }
    }

    /// <summary>
    /// ComfyUI文本翻译DTO
    /// </summary>
    public class ComfyuiTranslateDto
    {
        /// <summary>
        /// 待翻译文本
        /// </summary>
        [Display(Name = "翻译文本")]
        [Required(ErrorMessage = "请输入要翻译的内容")]
        public string Text { get; set; }

        /// <summary>
        /// 目标语言：zh-CN / en（默认 zh-CN）
        /// </summary>
        public string Target { get; set; }
    }
}
