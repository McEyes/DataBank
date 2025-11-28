using Furion.DataEncryption;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core
{
    public class DESEncryption
    {
        // 向量（必须为8字节）
        private readonly byte[] DESIV = new byte[] { 0x22, 0x54, 0x36, 110, 0x40, 0xAC, 0xAD, 0xDF };
        // 自定义密钥
        private const string deSkey = "cloudcloud";
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">加密文本</param>
        /// <param name="skey">密钥</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns></returns>
        public static string Encrypt(string text, string skey = deSkey, bool uppercase = false)
        {
            using var des = DES.Create();
            var inputByteArray = Encoding.UTF8.GetBytes(text);

            var md5KeyBytes = Encoding.UTF8.GetBytes(MD5Encryption.Encrypt(skey, uppercase)[..8]);
            var md5Bytes = Encoding.UTF8.GetBytes(MD5Encryption.Encrypt(skey, uppercase)[..8]);
            des.Key = md5KeyBytes;
            des.IV = md5Bytes;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (var b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            Convert.FromBase64String(ret.ToString()).ToString();
            return ret.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="hash">加密后字符串</param>
        /// <param name="skey">密钥</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns></returns>
        public static string Decrypt(string hash, string skey = deSkey, bool uppercase = false)
        {
            using var des = DES.Create();
            var len = hash.Length / 2;
            var inputByteArray = new byte[len];
            int x, i;

            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(hash.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            var md5KeyBytes = Encoding.UTF8.GetBytes(MD5Encryption.Encrypt(skey, uppercase)[..8]);
            var md5Bytes = Encoding.UTF8.GetBytes(MD5Encryption.Encrypt(skey, uppercase)[..8]);
            des.Key = md5KeyBytes;
            des.IV = md5Bytes;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
