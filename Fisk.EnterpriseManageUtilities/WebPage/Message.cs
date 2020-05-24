
/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/9/13 4:34:08
 * *******************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web;
using System.Web.UI.HtmlControls;


namespace Fisk.EnterpriseManageUtilities.WebPage
{
    /// <summary>
    /// Page页面弹出信息框
    /// </summary>
    public class Message
    {
       
        /// <summary>
        /// 消息对话框 alert
        /// </summary>
        /// <param name="page"></param>
        /// <param name="message"></param>
        public static void ShowAlert(Page page, string message)
        {
            page.ClientScript.RegisterStartupScript(
                page.GetType(),
                Guid.NewGuid().ToString(),
                string.Format(@"<script language=javascript>alert(""{0}"");</script>",
                              message.Replace(@"""", "'").Replace("\r\n", @"\n").Replace("\n", @"\n").Replace("\\", "\\\\").Replace(@"\\n", @"\n")));
        }


        /// <summary>
        /// 消息对话框 alert
        /// </summary>
        /// <param name="page"></param>
        /// <param name="message"></param>
        /// <param name="target"></param>
        public static void ShowAlert(Page page, string message, string target)
        {
            page.ClientScript.RegisterStartupScript(
                page.GetType(),
                Guid.NewGuid().ToString(),
                string.Format(@"<script language=javascript>alert(""{0}"");window.location.href=""{1}"";</script>",
                              message.Replace(@"""", "'").Replace("\r\n", @"\n").Replace("\n", @"\n").Replace("\\", "\\\\").Replace(@"\\n", @"\n"), target));
        }

        

       
       
     

       
    }
}
