using System.Web;

/************************************************************************************
 *Copyright (c) 2014 All Rights Reserved.
 *CLR版本： 4.0.30319.18444
 *公司名称：容盟软件（上海）有限公司
 *机器名称：DEV
 *命名空间：Fisk.EnterpriseManageUtilities.WebPage
 *文件名：  SessionHelper
 *版本号：  V1.0.0.0
 *唯一标识：197d785a-202f-4e8e-a4b5-a62d292ed0d6
 *当前的用户域：LXY
 *创建人：  jerryli
 *创建时间：2014/3/12 13:21:39
 *描述：Session操作类
 *
 *=====================================================================
 *修改时间：2014/3/12 13:21:39
 *修改人： jerryli
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/



namespace Fisk.EnterpriseManageUtilities.WebPage
{
    public class SessionHelper
    {
        /// <summary>
        /// 添加Session，调动有效期为120分钟
        /// </summary>
        /// <param name="strSessionName">Session对象名称</param>
        /// <param name="strValue">Session值</param>
        public static void Add(string strSessionName, object strValue)
        {
            HttpContext.Current.Session[strSessionName] = strValue;
            HttpContext.Current.Session.Timeout = 120;
        }

        /// <summary>
        /// 添加Session
        /// </summary>
        /// <param name="strSessionName">Session对象名称</param>
        /// <param name="strValue">Session值</param>
        /// <param name="iExpires">调动有效期（分钟）</param>
        public static void Add(string strSessionName, string strValue, int iExpires)
        {
            HttpContext.Current.Session[strSessionName] = strValue;
            HttpContext.Current.Session.Timeout = iExpires;
        }
        /// <summary>
        /// 读取某个Session对象值
        /// </summary>
        /// <param name="strSessionName">Session对象名称</param>
        /// <returns>Session对象值</returns>
        public static object Get(string strSessionName)
        {
            if (HttpContext.Current.Session == null
                || HttpContext.Current.Session[strSessionName] == null)
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Session[strSessionName];
            }
        }
        /// <summary>
        /// 修改Session
        /// </summary>
        /// <param name="strSessionName"></param>
        /// <param name="objVal"></param>
        public static void Update(string strSessionName, object objVal)
        {
            object obj = Get(strSessionName);
            if (obj != null)
            {
                Del(strSessionName);
            }
            Add(strSessionName, objVal);
        }


        /// <summary>
        /// 删除某个Session对象
        /// </summary>
        /// <param name="strSessionName">Session对象名称</param>
        public static void Del(string strSessionName)
        {
            HttpContext.Current.Session[strSessionName] = null;
            HttpContext.Current.Session.Remove(strSessionName);
        }

    }
}
