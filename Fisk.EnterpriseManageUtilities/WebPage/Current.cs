using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fisk.EnterpriseManageUtilities.WebPage
{
    /// <summary>
    /// 当前类
    /// </summary>
    public class Current
    {
        //用户
        public static string  UserAccount
        {
            get
            {
                return CurrentUser.UserAccount;
            }

        }
        //当前单元
        public static int UUnit
        {
            get
            {
                DateTime dt = DateTime.Now;
                int month = dt.Month;
                return month / 2 + 1;
            }
        }
    }
}
