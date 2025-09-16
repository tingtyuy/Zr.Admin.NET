
namespace ZR.Model.Business
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("sys_user_wxacount")]
    public class UserWxacount
    {
        /// <summary>
        /// Id 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public int Id { get; set; }

        /// <summary>
        /// 微信账号 
        /// </summary>
        public string Wxaccount { get; set; }

        /// <summary>
        /// 用户id 
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        public int? UserId { get; set; }

    }
}