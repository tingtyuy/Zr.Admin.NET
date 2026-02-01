namespace ZR.Mall.Model
{
    /// <summary>
    /// 商品品牌
    /// </summary>
    [SugarTable("dr_key_pair_data", "质控日报键值数据")]
    [Tenant("1")]
    public class DRKeyPairData
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public long Id { get; set; }

        public DateTime 业务日期 { get; set; }
        public string 网点名称 { get; set; }
        public string 未及时出仓量 { get; set; }
        public string 出仓率及时率 { get; set; }
        public string 预估考核金额元 { get; set; }
        public string 滞留量
        { get; set; }
        public string 滞留率
        { get; set; }
        public string 投诉量_进港
        { get; set; }
        public string 投诉率百万分_进港
        { get; set; }
        public string 投诉量_虚假
        { get; set; }

        public string 投诉率百万分_虚假
        { get; set; }
        public string 未签收量_送货上门
        { get; set; }
        public string 签收率_送货上门
        { get; set; }
        public string 订单总量
        { get; set; }
        public string 及时揽收率
        { get; set; }
        public string 当天考核量
        { get; set; }
        public string 当天签收率
        { get; set; }
        public string 预估总罚款元 { get; set; }
        public string 未按时签收量_48小时
        { get; set; }
        public string 签收率_48小时
        { get; set; }
        public string 未履约量
        { get; set; }
        public string 履约率
        { get; set; }
        public string 揽收上传不及时
        { get; set; }
        public string 派件上传不及时
        { get; set; }
        public string 签收上传不及时
        { get; set; }
        public string 未揽收
        { get; set; }
        public string 未派件
        { get; set; }
        public string 未到件
        { get; set; }
        public string 揽收晚于派件
        { get; set; }
        public string 揽收晚于签收
        { get; set; }
        public string 派件晚于签收
        { get; set; }
        public string 到件晚于签收
        { get; set; }



    }
}
