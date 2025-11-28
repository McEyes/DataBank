using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Encrypt
{
    public class MD5Util
    {
        // 向量（必须为8字节）
        private readonly byte[] DESIV = new byte[] { 0x22, 0x54, 0x36, 110, 0x40, 0xAC, 0xAD, 0xDF };
        // 自定义密钥
        private readonly string deSkey = "cloudcloud";
        // 加密算法的参数接口
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;
        private Encoding charset = Encoding.UTF8;

        private static MD5Util instance;
        private static readonly object lockObj = new object();

        private MD5Util()
        {
            // 设置密钥参数
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
            desProvider.Key = Encoding.UTF8.GetBytes(deSkey)[0..8];
            desProvider.IV = DESIV;
            // 创建加密器和解密器
            encryptor = desProvider.CreateEncryptor();
            decryptor = desProvider.CreateDecryptor();
        }

        public static MD5Util GetInstance()
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new MD5Util();
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">待加密的字符串</param>
        /// <returns>加密后的Base64字符串</returns>
        public string Encode(string data)
        {
            byte[] inputBytes = charset.GetBytes(data);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">待解密的Base64字符串</param>
        /// <returns>解密后的字符串</returns>
        public string Decode(string data)
        {
            byte[] inputBytes = Convert.FromBase64String(data);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return charset.GetString(decryptedBytes);
        }

        /// <summary>
        /// 获取MD5的值，可用于对比校验
        /// </summary>
        /// <param name="sourceStr">输入的字符串</param>
        /// <returns>MD5哈希值的字符串表示</returns>
        public static string GetMD5Value(string sourceStr)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(sourceStr);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // 将每个字节转换为两位的十六进制字符串
                }

                return sb.ToString();
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                string value = "1234567890dataXaccess";
                MD5Util mt = MD5Util.GetInstance();
                Console.WriteLine("加密前的字符：" + value);
                string encodedValue = mt.Encode(value);
                Console.WriteLine("加密后的字符：" + encodedValue);
                Console.WriteLine("解密后的字符：" + mt.Decode(encodedValue));
                Console.WriteLine("字符串的MD5值：" + GetMD5Value(value));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
