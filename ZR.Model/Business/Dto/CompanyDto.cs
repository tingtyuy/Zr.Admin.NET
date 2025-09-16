
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class CompanyQueryDto : PagerInfo 
    {
    }

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class CompanyDto
    {
        [Required(ErrorMessage = "主键，自增1不能为空")]
        public int Id { get; set; }

        [Required(ErrorMessage = "CompanyId不能为空")]
        public string CompanyId { get; set; }

        [Required(ErrorMessage = "CompanyName不能为空")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Createtime不能为空")]
        public DateTime? Createtime { get; set; }

        [Required(ErrorMessage = "1：使用中2：禁用3：测试中不能为空")]
        public int State { get; set; }

        [Required(ErrorMessage = "1：是 2：否不能为空")]
        public int IsfixedStaff { get; set; }

        public string StaffName { get; set; }

        [Required(ErrorMessage = "异常邮件通知不能为空")]
        public string EmailTo { get; set; }

        public string EmailCC { get; set; }



        [ExcelColumn(Name = "1：使用中2：禁用3：测试中")]
        public string StateLabel { get; set; }
    }
}