namespace ZR.Model.System
{
    /// <summary>
    /// ComfyUI任务表（用户创建，可入队执行）
    /// </summary>
    [SugarTable("comfyui_task", "ComfyUI任务表")]
    [Tenant("0")]
    public class ComfyuiTask : SysBase
    {
        /// <summary>
        /// 主键ID（雪花ID，也是任务号）
        /// </summary>
        [JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string TaskName { get; set; }

        /// <summary>
        /// 功能类型: txt2img/img2img/txt2video/img2video
        /// </summary>
        [SugarColumn(Length = 50)]
        public string FuncType { get; set; }

        /// <summary>
        /// 工作流ID
        /// </summary>
        public long WorkflowId { get; set; }

        /// <summary>
        /// 工作流名称（冗余，便于列表展示）
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string WorkflowName { get; set; }

        /// <summary>
        /// 可变节点最终值JSON（nodeId->value）
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string VariableValues { get; set; }

        /// <summary>
        /// 种子模式：random=每次随机，fixed=沿用工作流固定种子
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true, DefaultValue = "random")]
        public string SeedMode { get; set; } = "random";

        /// <summary>
        /// 参考文件路径集合JSON（数组：nodeId/localPath/originalName/comfyName）
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string RefFiles { get; set; }

        /// <summary>
        /// 是否已入队: 0=未入队(草稿), 1=已入队
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int Queued { get; set; }

        /// <summary>
        /// 任务状态: draft/pending/processing/done/failed
        /// </summary>
        [SugarColumn(Length = 20, DefaultValue = "draft")]
        public string Status { get; set; } = "draft";

        /// <summary>
        /// 错误信息
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 入队时间
        /// </summary>
        public DateTime? QueuedTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }
    }
}
