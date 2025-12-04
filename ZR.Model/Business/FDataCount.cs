using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business
{
    [SugarTable("data_count")]
    public class FDataCount
    {
        public long id { get; set; }

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
        public DateTime fdate { get; set; }
        public DateTime createtime { get; set; }

    }
}
