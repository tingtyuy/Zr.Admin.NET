using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Common
{
    public static class MD5Utility
    {
        public static string GetMD5UnicodeEncode(string value)
        {
            return GetMD5Encode(Encoding.Unicode, value);
        }

        public static string GetMD5UTF8Encode(string value)
        {
            return GetMD5Encode(Encoding.UTF8, value, "lower");
        }

        public static string GetMD5EaspEncode(string value)
        {
            return GetMD5Encode(Encoding.Unicode, value, "lower");
        }

        public static string GetMD5Encode(Encoding encoding, string value, string caseinsertive = "upper")
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(encoding.GetBytes(value));
                var strResult = BitConverter.ToString(result);
                string md5result = strResult.Replace("-", "");
                if (caseinsertive == "lower")
                {
                    md5result = md5result.ToLower();
                }
                return md5result;
            }
        }
    }
}
