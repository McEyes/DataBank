using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Encrypt
{

    public class DESUtil
    {
        private static readonly string ALGORITHM = "DES/ECB/PKCS5Padding";

        public static string Encrypt(string data, string key)
        {
            using DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] array = new byte[8];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(array.Length, '#')), array, array.Length);
            dESCryptoServiceProvider.Mode = CipherMode.ECB;
            dESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            dESCryptoServiceProvider.Key = array;
            dESCryptoServiceProvider.IV = array;
            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string encryptedData, string key)
        {
            using DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] array = new byte[8];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(array.Length, '#')), array, array.Length);
            dESCryptoServiceProvider.Mode = CipherMode.ECB;
            dESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            dESCryptoServiceProvider.Key = array;
            dESCryptoServiceProvider.IV = array;
            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
            byte[] array2 = Convert.FromBase64String(encryptedData);
            cryptoStream.Write(array2, 0, array2.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
