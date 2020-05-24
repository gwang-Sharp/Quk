
/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/9/13 4:40:26
 * *******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Fisk.EnterpriseManageUtilities.WebPage
{
    /// <summary>
    /// 附件帮助
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 验证上传附件的大小
        /// </summary>
        /// <param name="file"></param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static bool CheckInput(HttpPostedFile file ,int length)
        {
            //上传文档大小限制
            if (file.ContentLength != 0)
            {
                if (file.ContentLength > length)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
