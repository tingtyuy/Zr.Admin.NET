namespace ZR.Model.System
{
    /// <summary>
    /// ComfyUI运行配置表（key-value，如ServerUrl）
    /// </summary>
    [SugarTable("comfyui_config", "ComfyUI配置表")]
    [Tenant("0")]
    public class ComfyuiConfig : SysBase
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 配置键
        /// </summary>
        [SugarColumn(Length = 50)]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string ConfigValue { get; set; }
    }
}
