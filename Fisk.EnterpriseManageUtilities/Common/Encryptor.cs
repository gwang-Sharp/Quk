using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Fisk.EnterpriseManageUtilities.Common
{
    /// <summary>
    /// 加解密算法
    /// </summary>
    public class Encryptor
    {
        # region AES 加密
        static string AESPwd = "1#2$3%4(5)6@7!8$9&pass$3%4(5)1qazbut2wsxnnn";  //加密秘钥
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AESEncrypt(string content)
        {
            return AESEncrypt(content, AESPwd);
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AESDecrypt(string content)
        {
            return AESDecrypt(content, AESPwd);
        }

        private static string AESEncrypt(string content, string password)
        {
            try
            {
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                byte[] plainBytes = encoding.GetBytes(content);
                byte[] keyBytes = ShortMD5(password);
                Aes kgen = Aes.Create("AES");
                kgen.Mode = CipherMode.ECB;
                kgen.Padding = PaddingMode.PKCS7;
                kgen.Key = keyBytes;
                ICryptoTransform cTransform = kgen.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                // return byte2hex(resultArray).ToLower();
                return ByteToHex(resultArray);
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                return string.Empty;
            }
        }
        private static string AESDecrypt(string toDecrypt, string key)
        {
            try
            {
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                byte[] keyArray = ShortMD5(key);
                //  byte[] toEncryptArray = hex2byte(toDecrypt);
                byte[] toEncryptArray = HexToByte(toDecrypt);
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                return string.Empty;
            }
        }
        private static byte[] ShortMD5(string password)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            byte[] b = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return b;
        }
        private static byte[] HexToByte(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                hexString = "00";
            }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        public static string ByteToHex(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        #endregion
    }
}
