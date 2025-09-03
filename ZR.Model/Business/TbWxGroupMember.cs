
namespace ZR.Model.Business
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("tb_wx_group_member")]
    public class TbWxGroupMember
    {
        /// <summary>
        /// 自增主键 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 姓名 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// NickName 
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像base64 
        /// </summary>
        public string HeadPhoto { get; set; }

        /// <summary>
        /// 群名称 
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 公司ID 
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 是否是内部人员 
        /// </summary>
        public bool IsInternal { get; set; }

    }
}