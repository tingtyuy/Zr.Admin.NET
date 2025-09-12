
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class TbWxGroupMemberQueryDto : PagerInfo 
    {

        public string GroupName { get; set; }
        public int? ContactId { get; set; }
        public bool? IsInternal { get; set; }
    }

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class TbWxGroupMemberDto
    {
        [Required(ErrorMessage = "自增主键不能为空")]
        public int Id { get; set; }

        public string Name { get; set; }

        public string NickName { get; set; }

        public string HeadPhoto { get; set; }

        public string GroupName { get; set; }

        public int? ContactId { get; set; }

        public bool IsInternal { get; set; }

        [ExcelColumn(Name = "是否是内部人员")]
        public string IsInternalLabel { get; set; }
    }
}