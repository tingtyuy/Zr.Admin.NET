using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// FDataCount的输出对象
    /// </summary>
    public class FDataCountDto
    {
        /// <summary>
        /// 网点管家中全部的问题件数量
        /// </summary>
        public int allcount { get; set; }

        /// <summary>
        /// 网点管家中仅查询的问题件数量
        /// </summary>
        public int checkcount { get; set; }

        /// <summary>
        /// 网点管家中仅通知的问题件数量
        /// </summary>
        public int noticecount { get; set; }
        public string companyid { get; set; }
        
    }
}
