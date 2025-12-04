using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business.Dto
{
    /// <summary>
    /// 用于数据查询传参数
    /// </summary>
    public class TbResultQueryDto_3
    {
        /// <summary>
        /// tb_result表的Id 列表
        /// </summary>
        public List<long> ids { get; set; }

        //Liangzw   2025-11-08
        /// <summary>
        /// 账户的昵称，昵称不能重复
        /// </summary>
        public string strAccount { get; set; }
    }
}
