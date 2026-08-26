namespace ZR.Model.System
{
    /// <summary>
    /// AI任务表
    /// </summary>
    [SugarTable("ai_task", "AI任务表")]
    [Tenant("0")]
    public class AiTask : SysBase
    {
        /// <summary>
        /// 主键ID（雪花ID，也是任务号）
        /// </summary>
        [JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 提交用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 任务名称（如：首页、二图、三图、四图或自定义）
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string TaskName { get; set; }

        /// <summary>
        /// 功能类型: img2img
        /// </summary>
        [SugarColumn(Length = 50, DefaultValue = "img2img")]
        public string FuncType { get; set; } = "img2img";

        /// <summary>
        /// 原图路径
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string InputImagePath { get; set; }

        /// <summary>
        /// 提示词
        /// </summary>
        [SugarColumn(Length = 500)]
        public string Prompt { get; set; }

        /// <summary>
        /// 状态: pending/processing/done/failed
        /// </summary>
        [SugarColumn(Length = 20, DefaultValue = "pending")]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// 结果图路径
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string OutputImagePath { get; set; }

        /// <summary>
        /// 开始处理时间
        /// </summary>
        public DateTime? ProcessStartTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int RetryCount { get; set; }

        /// <summary>
        /// 处理尝试次数（每次fetch递增，callback时校验）
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int AttemptCount { get; set; }

        /// <summary>
        /// 输入图MD5哈希（提交时计算，fetch时返回给N8N验证）
        /// </summary>
        [SugarColumn(Length = 32, IsNullable = true)]
        public string InputImageHash { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 标签(逗号分隔，如"2026-07-02,批次A")
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Tags { get; set; }

        /// <summary>
        /// 扩展参数(JSON)
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string ExtParams { get; set; }

        /// <summary>
        /// 发布状态: 0=未发布, 1=已发布
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int PublishStatus { get; set; }
    }
}
