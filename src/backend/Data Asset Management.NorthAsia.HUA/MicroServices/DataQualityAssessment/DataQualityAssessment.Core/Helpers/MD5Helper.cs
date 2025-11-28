using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Helpers
{
    public class MD5Helper
    {
        public static string Encrypt(string plaintext)
        {
            using (MD5 md5 = MD5.Create())
            {
                // 将字符串转换为字节数组
                byte[] inputBytes = Encoding.UTF8.GetBytes(plaintext);
                // 计算哈希值
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" 表示两位十六进制
                }
                return sb.ToString();
            }
        }
    }
}
