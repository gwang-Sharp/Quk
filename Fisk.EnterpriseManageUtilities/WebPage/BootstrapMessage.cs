using System;
using System.Web.UI;

namespace Fisk.EnterpriseManageUtilities.WebPage
{
    public class BootstrapMessage
    {
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="page"></param>
        /// <param name="containerID"></param>
        public static void AlertClose(Page page, string containerID)
        {
            page.ClientScript.RegisterStartupScript(
                          page.GetType(),
                          Guid.NewGuid().ToString(),
                          string.Format(@"<script language=javascript> $('#{0}').empty();</script>", containerID));
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="page"></param>
        /// <param name="containerID"></param>
        /// <param name="title"></param>
        /// <param name="alertText"></param>
        public static void AlertSuccess(Page page, string containerID, string title, string alertText)
        {
            page.ClientScript.RegisterStartupScript(
                          page.GetType(),
                          Guid.NewGuid().ToString(),
                          string.Format(@"<script language=javascript>BootstrapAlert.PrivateAlert(""{0}"",""{1}"", ""{2}"", ""{3}"");</script>", containerID, title, alertText, "alert-success"));


        }
        /// <summary>
        ///一般信息提示
        /// </summary>
        /// <param name="page"></param>
        /// <param name="containerID"></param>
        /// <param name="title"></param>
        /// <param name="alertText"></param>
        public static void AlertInfo(Page page, string containerID, string title, string alertText)
        {
            page.ClientScript.RegisterStartupScript(
                          page.GetType(),
                          Guid.NewGuid().ToString(),
                          string.Format(@"<script language=javascript>BootstrapAlert.PrivateAlert(""{0}"",""{1}"", ""{2}"", ""{3}"");</script>", containerID, title, alertText, "alert-info"));


        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="page"></param>
        /// <param name="containerID"></param>
        /// <param name="title"></param>
        /// <param name="alertText"></param>
        public static void AlertWarning(Page page, string containerID, string title, string alertText)
        {
            page.ClientScript.RegisterStartupScript(
                          page.GetType(),
                          Guid.NewGuid().ToString(),
                          string.Format(@"<script language=javascript>BootstrapAlert.PrivateAlert(""{0}"",""{1}"", ""{2}"", ""{3}"");</script>", containerID, title, alertText, "alert-warning"));


        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="containerID"></param>
        /// <param name="title"></param>
        /// <param name="alertText"></param>
        public static void AlertError(Page page, string containerID, string title, string alertText)
        {
            page.ClientScript.RegisterStartupScript(
                          page.GetType(),
                          Guid.NewGuid().ToString(),
                          string.Format(@"<script language=javascript>BootstrapAlert.PrivateAlert(""{0}"",""{1}"", ""{2}"", ""{3}"");</script>", containerID, title, alertText, "alert-danger"));


        }



    }
}
