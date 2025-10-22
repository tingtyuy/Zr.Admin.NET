using JinianNet.JNTemplate.Dynamic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Extensions
{
    public static class ArrayExtension
    {
        /// <summary>
        /// 过滤掉空和特殊符号字符
        /// </summary>
        /// <returns></returns>
        public static string[] FilterSpecial(this string[] array)
        {
  
            // 找到第一个null或空字符串的索引
            int firstNullOrEmptyIndex = Array.FindIndex(array, s => s == null || s == "");

            // 如果存在null或空字符串，则只取前面的部分
            if (firstNullOrEmptyIndex != -1)
            {
                array = array.Take(firstNullOrEmptyIndex).ToArray();
            }

            // 过滤特殊符号
            array = array
                .Select(s => s.FilterSpecial())
                .Where(s => s.Length > 0) // 可选：如果过滤后变成空字符串，也可以移除
                .ToArray();

            return array;
        }
       
    }
}
