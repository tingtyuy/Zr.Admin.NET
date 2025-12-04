using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business
{
    //自动化处理的问题件统计
    public class ProcessOrder
    {
        /// <summary>
        /// 处理件总数
        /// </summary>
        public int TotalNumber { get; set; }

        /// <summary>
        /// 拒收件
        /// </summary>
        public int FReject { get; set; }

        /// <summary>
        /// 破损件
        /// </summary>
        public int FDamage { get; set; }

        /// <summary>
        /// 信息不详
        /// </summary>
        public int Funknown { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public int FOther { get; set; }

        /// <summary>
        /// 总耗时(秒),该字段仅用于输出
        /// </summary>
        public double TotalUseTime { get; set; }
    }
}
