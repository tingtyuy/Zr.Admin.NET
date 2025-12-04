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
    public class WeiXinGroupStatisticsDto
    {
        public string GroupName { get; set; }
        public int FNumber { get; set; }
    }
}
