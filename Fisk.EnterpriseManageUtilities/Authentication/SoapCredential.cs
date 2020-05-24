using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Fisk.EnterpriseManageUtilities.Authentication
{
    /// <summary>
    /// 身份验证
    /// </summary>
    public class SoapCredential : ICredentials
    {
        private string _username;
        private string _password;

        public SoapCredential(string username, string password)
        {
            _username = username;
            _password = password;
        }

        NetworkCredential ICredentials.GetCredential(Uri uri, string authType)
        {
            NetworkCredential obj = new NetworkCredential();
            obj.UserName = _username;
            obj.Password = _password;
            return obj;
            //throw new NotImplementedException();
        }


    }
}
