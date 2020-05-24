using Fisk.EnterpriseManageUtilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace Fisk.EnterpriseManageUtilities.Common
{
   public   class EncryptAndDecrypt
    {
       //2017年5月9日16:49:29 xyh
        public static string Encryption(string express)
        {
            //加密
            CspParameters param = new CspParameters();
            param.KeyContainerName = "fisksoft_work";//密匙容器的名称，保持加密解密一致才能解密成功
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] plaindata = Encoding.Default.GetBytes(express);//将要加密的字符串转换为字节数组
                byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
                return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            }
        }

        //解密
        public static string Decrypt(string ciphertext)
        {
            try
            {
            //    Log.CreateLogManager().Error("解密");
                CspParameters param = new CspParameters();
                param.KeyContainerName = "fisksoft_work";
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
                {
                    byte[] encryptdata = Convert.FromBase64String(ciphertext);
                    byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                    return Encoding.Default.GetString(decryptdata);
                }
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch(Exception ex){
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
                //Log.CreateLogManager().Error("解密失败！");
              //  Log.CreateLogManager().Error("失败信息："+ex);
                return "";
            }
        }
    }
}
