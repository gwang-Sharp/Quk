using System;
using System.Web;

namespace Fisk.EnterpriseManageUtilities.Common
{
    public class CommonMethod
    {
        #region cookie操作
        /// <summary>
        /// Cookies赋值
        /// </summary>
        /// <param name="strName">主键</param>
        /// <param name="strValue">键值</param>
        /// <param name="Domain">跨域指定域名</param>
        /// <param name="strDay">有效天数</param>
        /// <returns></returns>
        public static bool setCookie(string strName, string strValue, string Domain, int strDay, bool typehttp)
        {
            try
            {


                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com DateTime.Now.AddDays();
                Cookie.Domain = Domain;
                Cookie.Path = "/";
                Cookie.HttpOnly = typehttp;
                Cookie.Expires = DateTime.Now.Add(new TimeSpan(strDay, 2, 0));
                //Cookie.Expires = DateTime.Now.Add(new TimeSpan(strDay, 2, 0));
                Cookie.Value = strValue;
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cookies赋值
        /// </summary>
        /// <param name="strName">主键</param>
        /// <param name="strValue">键值</param>
        /// <param name="Domain">跨域指定域名</param>
        /// <param name="strDay">有效天数</param>
        /// <returns></returns>
        public static bool NewsetCookiepath(string strName, string strValue, string Domain, int strDay, string Path)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Domain = Domain;
                Cookie.Path = Path;
                //Cookie.Expires = DateTime.Now.AddDays(strDay);
                Cookie.Expires = DateTime.Now.Add(new TimeSpan(strDay, 2, 0));
                Cookie.Value = strValue;
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cookies赋值 小时  服务器时间差5天
        /// </summary>
        /// <param name="strName">主键</param>
        /// <param name="strValue">键值</param>
        /// <param name="Domain">跨域指定域名</param>
        /// <param name="strDay">有效小时数</param>
        /// <returns></returns>
        public static bool NewsetCookie(string strName, string strValue, string Domain, int strDay)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Domain = Domain;
                Cookie.Path = "/";
                //Cookie.Expires = DateTime.Now.AddDays(strDay);
                Cookie.Expires = DateTime.Now.Add(new TimeSpan(5,0, strDay, 0));
                Cookie.Value = strValue;
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 读取Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>

        public static string getCookie(string strName)
        {


            HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[strName];
            if (Cookie != null)
            {
                return Cookie.Value.ToString();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 读取Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>

        public static string getCookieDomain(string strName, string DomainName)
        {
            System.Web.HttpCookie Cookie = new System.Web.HttpCookie(DomainName);

            // HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[strName];
            if (Cookie != null)
            {
                return Cookie.Value.ToString();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 删除Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>
        public static bool delCookie(string strName, string Domain)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                Cookie.Domain = Domain;//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Path = "/";
                Cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="key">键</param>
        public static void DelCookie(string key)
        {
            HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[key];
            if (Cookie != null)
            {
                Cookie.Value = "";
                Cookie.Path = "/";
                Cookie.Expires = DateTime.Now.AddDays(-1);
            }
            else {
                Cookie = new HttpCookie(key);
                Cookie.Value = "";
                Cookie.Path = "/";
                Cookie.Expires = DateTime.Now.AddDays(-1);
            }
            HttpContext.Current.Response.Cookies.Add(Cookie);
        }

        /// <summary>
        /// 在*huazhu.com二级域名下生成cookie
        /// </summary>
        /// <param name="token"></param>
        /// <param name="flag"></param>
        public static void SetCookie(string token, bool flag, DateTime? flagExpirationDate = null)
        {
            HttpContext context = HttpContext.Current;

            System.Web.HttpCookie cookie = new System.Web.HttpCookie("SS0^HUAZHU", token);
            if (flagExpirationDate != null)
            {
                cookie.Expires = flagExpirationDate.Value;
            }
            //设置true不能通过JS 获取cookie
            cookie.HttpOnly = false;
            cookie.Path = "/";
            cookie.Domain = "huazhu.com";
            if (flag)
            {
                context.Response.Cookies.Add(cookie);
            }
            else
            {
                context.Response.Cookies.Set(cookie);
            }
        }
        /// <summary>
        /// 在*huazhu.com二级域名下生成cookie
        /// </summary>
        /// <param name="token"></param>
        /// <param name="flag"></param>
        public static void SetCookiewks(string token, bool flag, DateTime? flagExpirationDate = null)
        {
            HttpContext context = HttpContext.Current;

            System.Web.HttpCookie cookie = new System.Web.HttpCookie("SS0^HUAZHU", token);
            if (flagExpirationDate != null)
            {
                cookie.Expires = flagExpirationDate.Value;
            }
            //设置true不能通过JS 获取cookie
            cookie.HttpOnly = false;
            cookie.Path = "/";
            cookie.Domain = "huazhu.com";
            if (flag)
            {
                context.Response.Cookies.Add(cookie);
            }
            else
            {
                context.Response.Cookies.Set(cookie);
            }
        }
        #endregion
    }
}
