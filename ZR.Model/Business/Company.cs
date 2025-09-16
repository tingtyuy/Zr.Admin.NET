
namespace ZR.Model.Business
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("company")]
    public class Company
    {
        /// <summary>
        /// 主键，自增1 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public int Id { get; set; }

        /// <summary>
        /// CompanyId 
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// CompanyName 
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Createtime 
        /// </summary>
        public DateTime? Createtime { get; set; }

        /// <summary>
        /// 1：使用中
///2：禁用
///3：测试中 
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 1：是 2：否 
        /// </summary>
        public int IsfixedStaff { get; set; }

        /// <summary>
        /// 如果不填，就不筛选 
        /// </summary>
        public string StaffName { get; set; }

        /// <summary>
        /// 异常邮件通知 
        /// </summary>
        public string EmailTo { get; set; }

        /// <summary>
        /// EmailCC 
        /// </summary>
        public string EmailCC { get; set; }

    }
}