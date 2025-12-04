using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business
{
    [SugarTable("robot")]
    public class Robot
    {
        /// <summary>
        /// ID 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public string robotId { get; set; }
        /// <summary>
        /// 客户 
        /// </summary>
        public string robotName { get; set; }

        /// <summary>
        /// 客户商家名称 
        /// </summary>
        public string companyId { get; set; }

        /// <summary>
        /// 对接方式 
        /// </summary>
        public Int32 dingding { get; set; }

        /// <summary>
        /// WeChat 
        /// </summary>
        public Int32 WeChat { get; set; }

        /// <summary>
        /// QQ 
        /// </summary>
        public Int32 QQ { get; set; }

        /// <summary>
        /// wangguan 
        /// </summary>
        public string wangguan { get; set; }

        /// <summary>
        /// stopTime
        /// </summary>
        public DateTime stopTime { get; set; }

        /// <summary>
        /// expirationTime
        /// </summary>
        public DateTime expirationTime { get; set; }

        /// <summary>
        /// startTime 
        /// </summary>
        public DateTime startTime { get; set; }

        
    }
}
