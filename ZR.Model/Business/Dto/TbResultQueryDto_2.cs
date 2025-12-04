using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 用于查询
    /// </summary>
    public class TbResultQueryDto_2
    {       
        public string 问题件类型 { get; set; }
        
        public string 单号 { get; set; }

        public string CompanyId { get; set; }
        public string 商家名称 { get; set; }

        

        public string 收件人信息 { get; set; }

        public string 群名称 { get; set; }

        /// <summary>
        /// 使用时间，这是DateTime类型
        /// </summary>
        public DateTime operateTime { get; set; }
    }
}
