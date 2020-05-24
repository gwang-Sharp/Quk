using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 正则表达式Helper
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/9/13 4:29:13
 * *******************************************************************/

namespace Fisk.EnterpriseManageUtilities.Common
{
    /// <summary>
    /// 正则表达式Helper
    /// </summary>
    public  class RegexHelper
    {
        /// <summary>
        /// 判断是否为正确格式的电子邮箱
        /// </summary>
        /// <param name="Str">需要判断的电子邮箱</param>
        /// <returns></returns>
        public static bool IsEmail(string Str)
        {
            string strRegex = @"^[_\.0-9a-z-]+@([0-9a-z][0-9a-z-]+\.){1,4}[a-z]{2,3}$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(Str))
                return true;
            else
                return false;
        }
    }
}
