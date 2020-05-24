namespace Fisk.EnterpriseManageUtilities.WebPage
{
    public class CurrentUser
    {
        private static string _userAccount;
        /// <summary>
        /// 得到当前登录用户的域帐号
        /// </summary>
        public static string UserAccount
        {
            get
            {
                string AuthenticationType = ConfigHelper.GetConfigStr("AuthenticationType");
                switch (AuthenticationType)
                {
                    case "form":
                        if (SessionHelper.Get("UserName") != null)
                        {
                            _userAccount = SessionHelper.Get("UserName").ToString();
                        }
                        break;
                    case "windows":
                        {
                            _userAccount = System.Web.HttpContext.Current.User.Identity.Name;
                            if (string.IsNullOrEmpty(_userAccount))
                            {
                                _userAccount = ConfigHelper.GetConfigStr("DevelopUser");
                            }
                            break;
                        }
                }
                return _userAccount;
            }
        }
    }
}