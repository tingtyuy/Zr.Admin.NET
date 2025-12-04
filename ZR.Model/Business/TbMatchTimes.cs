using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business
{
    //Liangzw  2025-11-08
    /// <summary>
    /// 记录用户待转发信息手动匹配的次数。用户点击按钮"匹配"或"复制并转发商户"，记录一次。
    /// </summary>
    [SugarTable("tb_match_times")]
    public class TbMatchTimes
    {
        public int fid { get; set; }
        public DateTime fdate { get; set; }


        public string foperator { get; set; }
        public DateTime matchTime { get; set; }

        /// <summary>
        /// 待转发信息
        /// </summary>
        public string fmessage { get; set; }

        /// <summary>
        /// 待转发信息前面的问题件数量
        /// </summary>
        public int messageNumber { get; set; }

    }
}
