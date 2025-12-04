
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class TbOrderQueryDto : PagerInfo 
    {
    }

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class TbOrderDto
    {
        [Required(ErrorMessage = "Id不能为空")]
        public long Id { get; set; }

        public string 订单号 { get; set; }

        public string 问题件类型 { get; set; }

        public string 写入时间 { get; set; }

        public string 使用时间 { get; set; }

        public string 读取机器人 { get; set; }

        public string 状态 { get; set; }

        public string 问题件类别 { get; set; }

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