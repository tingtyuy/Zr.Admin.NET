
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class TbResultQueryDto : PagerInfo 
    {
        public string 问题件类型 { get; set; }
        public string 问题件类别 { get; set; }
        public string 处理状态 { get; set; }
        public string 单号 { get; set; }

        public string 商家名称 { get; set; }

        /// <summary>
        /// 操作开始时间 
        /// </summary>
        public DateTime? 操作开始时间 { get; set; }

        /// <summary>
        /// 操作结束时间 
        /// </summary>
        public DateTime? 操作结束时间 { get; set; }
    }

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class TbResultDto
    {
        public string 问题件类型 { get; set; }
        public string 问题件类别 { get; set; }
        public string 处理状态 { get; set; }

        public string 单号 { get; set; }

        public string 商家名称 { get; set; }

        public string 收件人信息 { get; set; }

        public string 结果 { get; set; }

        public string 执行机器人 { get; set; }

        public string 操作时间 { get; set; }

        public string CompanyId { get; set; }



    }
}