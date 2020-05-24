
/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/9/13 4:40:55
 * *******************************************************************/
using System;

namespace Fisk.EnterpriseManageUtilities.WebPage
{
    /// <summary>
    /// Web string 帮助
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// 将字符串按指定长度截取,后用...号代替
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetFixedEllipsesStr(string sourceStr, int len)
        {
            string sReturn;
            int sLen = sourceStr.Trim().Length;
            if (sLen <= len || sLen == len + 2 || sLen == len + 1)
            {
                sReturn = sourceStr.Trim();
            }
            else
            {
                sReturn = sourceStr.Trim().Substring(0, len) + "...";
            }
            return sReturn;

        }

        /// <summary>
        /// 将包含HTML标记的字符串去掉HTML标记之后按指定长度截取，后用..代替
        /// </summary>
        /// <param name="original">字符串</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string RegexHTMLEllipsesStr(string original, int length)
        {
            string str = string.Empty;
            string tempStr = original;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("(<\\s*[a-zA-Z][^>]*>)|(</\\s*[a-zA-Z][^>]*>)|(\\s)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (original.Length > 0)
            {
                tempStr = reg.Replace(original, "");
            }
            if (tempStr.Length > length)
            {
                str = tempStr.Substring(0, length - 3) + "...";
            }
            else
            {
                str = tempStr;
            }
            return str;
        }


        /// <summary>
        /// 判断一个字符串是不是数字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumber(string s)
        {
            int flag = 0;
            char[] str = s.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsNumber(str[i]))
                {
                    flag++;
                }
                else
                {
                    flag = -1;
                    break;
                }
            }
            if (flag > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
