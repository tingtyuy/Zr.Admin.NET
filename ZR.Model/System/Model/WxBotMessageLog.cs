using ZR.Model.System;

namespace ZR.Model
{
    /// <summary>
    /// 用户系统消息
    /// </summary>
    [SugarTable("WxBotMessageLog")]
    [Tenant(0)]
    public class WxBotMessageLog
    {
        public Int64 id { get; set; }
        public int ts { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(3000)")]
        public string sign { get; set; }
        public int type { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(3000)")]
        public string xml { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(3000)")]
        public string sender { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(3000)")]
        public string roomid { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(3000)")]
        public string content { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(3000)")]
        public string thumb { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(3000)")]
        public string extra { get; set; }
        public bool is_at { get; set; }
        public bool is_self { get; set; }
        public bool is_group { get; set; }
    }
}