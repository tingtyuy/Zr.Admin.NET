using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Infrastructure.ICompare
{
    /// <summary>
    /// 自定义排序顺序
    /// </summary>
    public class DCompare : IComparer<string>
    {

        public int Compare(string x, string y)
        {
            string[] arr = { "软件", "硬件", "网络", "其他" };
            var xValue = Array.IndexOf(arr, x);
            var yValue = Array.IndexOf(arr, y);
            return xValue.CompareTo(yValue);
        }

    }
}
