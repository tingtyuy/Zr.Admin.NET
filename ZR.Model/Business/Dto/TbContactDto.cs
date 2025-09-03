
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class TbContactQueryDto : PagerInfo 
    {
    }

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class TbContactDto
    {
        public string 客户 { get; set; }

        public string 客户商家名称 { get; set; }

        public string 对接方式 { get; set; }

        public string 群名称 { get; set; }

        public string @联系人 { get; set; }

        public string 是否直接退回 { get; set; }

        public string CompanyId { get; set; }

        public bool IsEnable { get; set; }

        public int? MatchParam { get; set; }

        public bool IsMatch { get; set; }



        [ExcelColumn(Name = "启用状态：0启用，1禁用")]
        public string IsEnableLabel { get; set; }
    }
}