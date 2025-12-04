using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business
{
    [SugarTable("tb_daily_match")]
    public class TbDailyMatch
    {
        /// <summary>
        /// ID 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        /// 匹配日期 
        /// </summary>
        public DateTime matchDate { get; set; }

        /// <summary>
        /// 登录用户的id
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 公司id
        /// </summary>
        public string companyId { get; set; }

        /// <summary>
        /// 每天的匹配次数
        /// </summary>
        public Int32 matchTimes { get; set; }
    }
}
