
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class TbResultQueryDto : PagerInfo 
    {
    }

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class TbResultDto
    {
        public string 问题件类型 { get; set; }

        public string 单号 { get; set; }

        public string 商家名称 { get; set; }

        public string 收件人信息 { get; set; }

        public string 结果 { get; set; }

        public string 执行机器人 { get; set; }

        public string 操作时间 { get; set; }

        public string CompanyId { get; set; }



    }
}