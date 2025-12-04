using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 微信群数量统计，用于输出
    /// </summary>
    public class WeiXinGroupStatisticsDto_2
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime fdate { get; set; }
        public string companyId { get; set; }

        /// <summary>
        /// 本月商户微信群总数
        /// </summary>
        public int totalNumber { get; set; }

        /// <summary>
        /// 上个月商户微信群总数
        /// </summary>
        public int lastMonthNumber { get; set; }

        /// <summary>
        /// 本月已匹配的商户群
        /// </summary>
        public int matchedNumber { get; set; }

        /// <summary>
        /// 本月待匹配的商户群
        /// </summary>
        public int unMatchedNumber { get; set; }
    }
}
