namespace ZR.Model.System
{
    /// <summary>
    /// ComfyUI工作流表
    /// </summary>
    [SugarTable("comfyui_workflow", "ComfyUI工作流表")]
    [Tenant("0")]
    public class ComfyuiWorkflow : SysBase
    {
        /// <summary>
        /// 主键ID（雪花ID）
        /// </summary>
        [JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 工作流名称
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// 工作流描述
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 分类: default/txt2img/img2img/txt2video/img2video
        /// </summary>
        [SugarColumn(Length = 50, DefaultValue = "default")]
        public string Category { get; set; } = "default";

        /// <summary>
        /// ComfyUI API工作流JSON（/prompt请求格式，nodeId->{class_type,inputs}）
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string WorkflowJson { get; set; }

        /// <summary>
        /// 可变节点配置JSON（数组：nodeId/field/type/label）
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string VariableNodes { get; set; }

        /// <summary>
        /// 节点数量
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int NodeCount { get; set; }

        /// <summary>
        /// 标签(逗号分隔)
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Tags { get; set; }

        /// <summary>
        /// 状态: 0=正常, 1=停用
        /// </summary>
        [SugarColumn(Length = 10, DefaultValue = "0")]
        public string Status { get; set; } = "0";
    }
}
