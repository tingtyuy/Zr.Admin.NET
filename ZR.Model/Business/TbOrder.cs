
namespace ZR.Model.Business
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("tb_order")]
    public class TbOrder
    {
        /// <summary>
        /// Id 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 订单号 
        /// </summary>
        public string 订单号 { get; set; }

        /// <summary>
        /// 问题件类型 
        /// </summary>
        public string 问题件类型 { get; set; }

        /// <summary>
        /// 写入时间 
        /// </summary>
        public string 写入时间 { get; set; }

        /// <summary>
        /// 使用时间 
        /// </summary>
        public string 使用时间 { get; set; }

        /// <summary>
        /// 读取机器人 
        /// </summary>
        public string 读取机器人 { get; set; }

        /// <summary>
        /// 状态 
        /// </summary>
        public string 状态 { get; set; }

        /// <summary>
        /// 问题件类别 
        /// </summary>
        public string 问题件类别 { get; set; }

        /// <summary>
        /// CompanyId 
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 写入时间，该字段类型是DateTime
        /// </summary>
        public DateTime writeTime { get; set; }

        /// <summary>
        /// 使用时间，该字段类型是DateTime
        /// </summary>
        public DateTime useTime { get; set; }

    }
}