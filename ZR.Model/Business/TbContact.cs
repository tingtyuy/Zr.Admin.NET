
namespace ZR.Model.Business
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("tb_contact")]
    public class TbContact
    {
        /// <summary>
        /// ID 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        /// <summary>
        /// 客户 
        /// </summary>
        public string 客户 { get; set; }

        /// <summary>
        /// 客户商家名称 
        /// </summary>
        public string 客户商家名称 { get; set; }

        /// <summary>
        /// 对接方式 
        /// </summary>
        public string 对接方式 { get; set; }

        /// <summary>
        /// 群名称 
        /// </summary>
        public string 群名称 { get; set; }

        /// <summary>
        /// @联系人 
        /// </summary>
        public string @联系人 { get; set; }

        /// <summary>
        /// 是否直接退回 
        /// </summary>
        public string 是否直接退回 { get; set; }

        /// <summary>
        /// CompanyId 
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 启用状态：0启用，1禁用 
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 匹配参数 
        /// </summary>
        public int? MatchParam { get; set; }

        /// <summary>
        /// 是否匹配：0启用，1禁用 
        /// </summary>
        public bool IsMatch { get; set; }

    }
}