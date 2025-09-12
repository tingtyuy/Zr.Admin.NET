
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class TbContactQueryDto : PagerInfo 
    {

        public string 客户 { get; set; }

        public string 客户商家名称 { get; set; }

        public string 对接方式 { get; set; }

        public string 群名称 { get; set; }

        public string 联系人 { get; set; }

        public string 是否直接退回 { get; set; }

        public string CompanyId { get; set; }

        public bool IsEnable { get; set; }

        public string MatchParam { get; set; }

        public bool? IsMatch { get; set; }



        [ExcelColumn(Name = "启用状态：0启用，1禁用")]
        public string IsEnableLabel { get; set; }

    }


   

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class TbContactDto
    {
        public int Id { get; set; }

        public string 客户 { get; set; }

        public string 客户商家名称 { get; set; }

        public string 对接方式 { get; set; }

        public string 群名称 { get; set; }

        public string 联系人 { get; set; }

        public string 是否直接退回 { get; set; }

        public string CompanyId { get; set; }

        public bool IsEnable { get; set; }

        public string MatchParam { get; set; }

        public bool IsMatch { get; set; }

        public string MatchParamDes { get; set; } 
            

        [ExcelColumn(Name = "启用状态：0启用，1禁用")]
        public string IsEnableLabel { get; set; }

        public List<TbWxGroupMember>? TbWxGroupMembers { get; set; } 
    }

    /// <summary>
    /// 设置匹配规则传入对象
    /// </summary>
    public class TbContactMatchDto
    {
        public long Id { get; set; }
              
        public bool IsEnable { get; set; }

        public string[] MatchParam { get; set; }

        public List<int>? MIds { get; set; }
    }
}