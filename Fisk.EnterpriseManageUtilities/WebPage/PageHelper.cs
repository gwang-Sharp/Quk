
/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/9/13 4:37:36
 * *******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Fisk.EnterpriseManageUtilities.WebPage
{
    /// <summary>
    /// 页面帮助类
    /// </summary>
    public class PageHelper
    {
        /// <summary>
        /// 返回前一个页面
        /// </summary>
        /// <param name="page"></param>
        public static void HistoryToBack(Page page)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), @"<script>window.history.go(-1);</script>");
        }

      
    }
}
