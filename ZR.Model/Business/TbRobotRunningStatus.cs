using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business
{
    [SugarTable("tb_robot_running_status")]
    public class TbRobotRunningStatus
    {
        /// <summary>
        /// Id 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// CompanyId 
        /// </summary>
        public string companyId { get; set; }

        /// <summary>
        /// 读取机器人 
        /// </summary>
        public string robotId { get; set; }

        /// <summary>
        /// 客户端的ip地址 
        /// </summary>
        public string ip_address { get; set; }

        /// <summary>
        /// 客户端的主机名 
        /// </summary>
        public string server_name { get; set; }

        /// <summary>
        /// 检测日期 
        /// </summary>
        public string inspect_date { get; set; }

        /// <summary>
        /// 检查结果
        /// </summary>
        public string inspect_result { get; set; }
    }
}
