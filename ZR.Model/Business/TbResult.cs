
namespace ZR.Model.Business
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("tb_result")]
    public class TbResult
    {
        /// <summary>
        /// ID 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        /// <summary>
        /// 问题件类型 
        /// </summary>
        public string 问题件类型 { get; set; }
        /// <summary>
        /// 问题件类别 
        /// </summary>
        public string 问题件类别 { get; set; }
        /// <summary>
        /// 反馈信息 
        /// </summary>
        public string 反馈信息 { get; set; }

        /// <summary>
        /// 处理状态 
        /// </summary>
        public string 处理状态 { get; set; }

        /// <summary>
        /// 单号 
        /// </summary>
        public string 单号 { get; set; }

        /// <summary>
        /// 商家名称 
        /// </summary>
        public string 商家名称 { get; set; }

        /// <summary>
        /// 收件人信息 
        /// </summary>
        public string 收件人信息 { get; set; }

        /// <summary>
        /// 结果 
        /// </summary>
        public string 结果 { get; set; }

        /// <summary>
        /// 执行机器人 
        /// </summary>
        public string 执行机器人 { get; set; }

        /// <summary>
        /// 操作时间 
        /// </summary>
        public string 操作时间 { get; set; }

        /// <summary>
        /// CompanyId 
        /// </summary>
        public string CompanyId { get; set; }

        public string account { get; set; }

        public DateTime  匹配时间{ get; set; }

        /// <summary>
        /// 使用时间，这是DateTime类型
        /// </summary>
        public DateTime operateTime { get; set; }

    }
}