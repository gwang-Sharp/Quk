using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fisk.EnterpriseManageUtilities.Common
{
    public class TokenEncrypt
    {
        //创建Token值
        public static string GenerateHMACMD5(string text)
        {
            HMACMD5 myMd5 = new HMACMD5(Encoding.UTF8.GetBytes("shini"));
            byte[] byteText = myMd5.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(byteText);
        }
        //创建明文密文
        public static string GenerateHMACSHA1(string strText, string strKey)
        {
            HMACSHA1 myHMACSHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(strKey));
            byte[] byteText = myHMACSHA1.ComputeHash(Encoding.UTF8.GetBytes(strText));
            return System.Convert.ToBase64String(byteText);
        }
        //创建密文
        public static string EncryptData(string plainText, string key)
        {
            TripleDESCryptoServiceProvider Tripledes = new TripleDESCryptoServiceProvider();

            byte[] b_input = Encoding.UTF8.GetBytes(plainText);
            byte[] b_key = Encoding.UTF8.GetBytes(key);
            MemoryStream tempStream = new MemoryStream();
            CryptoStream encStream = new CryptoStream(tempStream, Tripledes.CreateEncryptor(b_key, b_key), CryptoStreamMode.Write);
            encStream.Write(b_input, 0, b_input.Length);
            encStream.Close();
            return Convert.ToBase64String(tempStream.ToArray());
        }

        //解密明文
        public static string DecryptData(string input, string key)
        {
            TripleDESCryptoServiceProvider Tripledes = new TripleDESCryptoServiceProvider();
            byte[] b_input = Convert.FromBase64String(input);
            byte[] b_key = Encoding.UTF8.GetBytes(key);
            MemoryStream tempStream = new MemoryStream();
            CryptoStream encStream = new CryptoStream(tempStream, Tripledes.CreateDecryptor(b_key, b_key), CryptoStreamMode.Write);
            encStream.Write(b_input, 0, b_input.Length);
            encStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(tempStream.ToArray());
        }

        public static string GetMd5(string msg)
        {
            string cryptStr = "";
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            byte[] cryptBytes = md5.ComputeHash(bytes);
            for (int i = 0; i < cryptBytes.Length; i++)
            {
                cryptStr += cryptBytes[i].ToString("X2");
            }
            return cryptStr;
        }
    }
}
