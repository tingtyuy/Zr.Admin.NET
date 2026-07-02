namespace ZR.Model.System
{
    /// <summary>
    /// AI提示词模板
    /// </summary>
    [SugarTable("ai_prompt_template", "AI提示词模板")]
    [Tenant("0")]
    public class AiPromptTemplate : SysBase
    {
        [JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        public long UserId { get; set; }

        [SugarColumn(Length = 100, ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string Name { get; set; }

        [SugarColumn(Length = 500, ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string Prompt { get; set; }

        [SugarColumn(Length = 50, DefaultValue = "img2img")]
        public string FuncType { get; set; } = "img2img";
    }
}
