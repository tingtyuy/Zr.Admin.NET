using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Model.Business
{
    /// <summary>
    /// 统计微信群的数量
    /// </summary>
    public class WeiXinGroupCount
    {
        public int TotalNumber { get; set; }
        public int MatchedNumber { get; set; }

        public int UNMatchedNumber { get; set; }
    }
}
