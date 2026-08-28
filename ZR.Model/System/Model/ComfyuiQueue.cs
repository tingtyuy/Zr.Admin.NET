namespace ZR.Model.System
{
    /// <summary>
    /// ComfyUI执行队列表（入队执行后的任务，记录ComfyUI运行状态与输出）
    /// </summary>
    [SugarTable("comfyui_queue", "ComfyUI执行队列表")]
    [Tenant("0")]
    public class ComfyuiQueue : SysBase
    {
        /// <summary>
        /// 主键ID（雪花ID）
        /// </summary>
        [JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string TaskName { get; set; }

        /// <summary>
        /// 功能类型
        /// </summary>
        [SugarColumn(Length = 50)]
        public string FuncType { get; set; }

        /// <summary>
        /// 工作流ID
        /// </summary>
        public long WorkflowId { get; set; }

        /// <summary>
        /// 提交给ComfyUI的完整prompt请求JSON
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string PromptJson { get; set; }

        /// <summary>
        /// ComfyUI返回的prompt_id
        /// </summary>
        [SugarColumn(Length = 64, IsNullable = true)]
        public string PromptId { get; set; }

        /// <summary>
        /// 入队用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 状态: pending/processing/done/failed/cancelled
        /// </summary>
        [SugarColumn(Length = 20, DefaultValue = "pending")]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// 执行进度(0-100)
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int Progress { get; set; }

        /// <summary>
        /// 输出文件URL列表(JSON数组)
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string OutputUrls { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 入队时间
        /// </summary>
        public DateTime? QueuedTime { get; set; }

        /// <summary>
        /// 开始处理时间
        /// </summary>
        public DateTime? ProcessStartTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }
    }
}
