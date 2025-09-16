
namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class UserWxacountQueryDto : PagerInfo 
    {
    }

    /// <summary>
    /// 输入输出对象
    /// </summary>
    public class UserWxacountDto
    {
        [Required(ErrorMessage = "Id不能为空")]
        public int Id { get; set; }

        public string Wxaccount { get; set; }

        public int? UserId { get; set; }



    }
}