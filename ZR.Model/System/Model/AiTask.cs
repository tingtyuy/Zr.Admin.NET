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
        /// 错误信息
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 扩展参数(JSON)
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string ExtParams { get; set; }
    }
}
