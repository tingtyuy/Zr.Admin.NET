using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// 过滤掉空和特殊符号字符
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int,string> FilterSpecial(this Dictionary<int, string> keyValuePairs)
        {
          //return Regex.Replace(str, @"[^\u4e00-\u9fa5a-zA-Z0-9_]", "");

            return keyValuePairs;
        }
       
    }
}
