using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyNet.Common
{
    public class Crypter
    {

        public static string MD5Bit16(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(input)), 4, 8).ToLower();
            t2 = t2.Replace("-", "");
            return t2;
        }
        public static string MD5(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }



        public static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(key);
            provider.Mode = CipherMode.ECB;
            return provider.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// 对称解密
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string encrypted, string key)
        {
            byte[] buffer = Convert.FromBase64String(encrypted);
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            return Encoding.UTF8.GetString(Decrypt(buffer, bytes));
        }

        public static byte[] Encrypt(byte[] original, byte[] key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(key);
            provider.Mode = CipherMode.ECB;
            return provider.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }
        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string original, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(original);
            byte[] buffer2 = Encoding.UTF8.GetBytes(key);
            return Convert.ToBase64String(Encrypt(bytes, buffer2));
        }
        
    }
}
