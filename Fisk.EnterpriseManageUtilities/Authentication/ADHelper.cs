using Fisk.EnterpriseManageUtilities.Common;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Text;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;

namespace Fisk.EnterpriseManageUtilities.Authentication
{

    /* webconfig中要引入以下内容才可使用
    <add key="LDAPPath" value="LDAP://test1/DC=contoso,DC=com" />  
    <add key="LDAPDomain" value="contoso/bot" />
    <add key="LDAPUser" value="contoso/bot" />  
    <add key="LDAPPassword" value="123456" />  
     */

    public class ADHelper
    {
       private static string ADPassword = ConfigurationManager.AppSettings["LDAPPassword"];
       private static string ADPath = ConfigurationManager.AppSettings["LDAPPath"];
       private static string ADUser = ConfigurationManager.AppSettings["LDAPUser"];
       private static string DomainName = ConfigurationManager.AppSettings["LDAPDomain"];

       private static AuthenticationTypes DefaultAuthenticationType = AuthenticationTypes.Secure;

        //得到根目录下的OU
       public static SearchResultCollection  GetRootOus()
       {
           try
           {
               DirectoryEntry entry = new DirectoryEntry(ADPath, ADUser, ADPassword, DefaultAuthenticationType);

               DirectorySearcher directorySearch = new DirectorySearcher(entry);
              directorySearch.Filter = "(&(objectClass=organizationalUnit))";
               directorySearch.SearchScope = SearchScope.OneLevel;
               SearchResultCollection results = directorySearch.FindAll();

               if (results != null)
               {
                   return results;
               }
               else
               {
                   return null;
               }
           }
           catch (Exception ex)
           {
               Log.CreateLogManager().Error("方法：ADHelper.GetRootOus方法执行失败",ex);
               return null;
           }

       }
        //得到Ad中所有用户
       public static SearchResultCollection GetAllUsers()
       {
           try
           {
               DirectoryEntry entry = new DirectoryEntry(ADPath, ADUser, ADPassword, DefaultAuthenticationType);

               DirectorySearcher directorySearch = new DirectorySearcher(entry);
               directorySearch.Filter = "(&(objectClass=user))";
               directorySearch.Sort.PropertyName = "cn";
               directorySearch.SearchScope = SearchScope.Subtree;
               SearchResultCollection results = directorySearch.FindAll();

               if (results != null)
               {
                   return results;
               }
               else
               {
                   return null;
               }
           }
           catch (Exception ex)
           {
               Log.CreateLogManager().Error("方法：ADHelper.GetRootUsers方法执行失败", ex);
               return null;
           }

       }
        //根据Ou的GUID得到OU下的用户和子OU
       public static SearchResultCollection GetOuItemByOUGUID(string guid)
       {
           try
           {
               DirectoryEntry entry = GetDirectoryEntryByGuid(guid);

               DirectorySearcher directorySearch = new DirectorySearcher(entry);
              // directorySearch.Filter = "(&(objectClass=organizationalUnit))";
               directorySearch.SearchScope = SearchScope.OneLevel;
               SearchResultCollection results = directorySearch.FindAll();

               if (results != null)
               {
                   return results;
               }
               else
               {
                   return null;
               }
           }
           catch (Exception ex)
           {
               Log.CreateLogManager().Error("方法：ADHelper.GetRootOus方法执行失败", ex);
               return null;
           }
       }
        //根据信息查找用户
       public static SearchResultCollection FindUser(string searchText)
       {
           try
           {
               DirectoryEntry entry = new DirectoryEntry(ADPath, ADUser, ADPassword, DefaultAuthenticationType);

               DirectorySearcher directorySearch = new DirectorySearcher(entry);
               directorySearch.Filter = string.Format("(&(objectCategory=user)(|(displayName=*{0}*)(sAMAccountName=*{0}*)))", searchText); 
               directorySearch.SearchScope = SearchScope.Subtree;
               directorySearch.Sort.PropertyName = "cn";
               SearchResultCollection results = directorySearch.FindAll();

               if (results != null)
               {
                   return results;
               }
               else
               {
                   return null;
               }
           }
           catch (Exception ex)
           {
               Log.CreateLogManager().Error("方法：ADHelper.FindUser方法执行失败", ex);
               return null;
           }
       }



       //根据GUID获取DirectoryEntry
       private static DirectoryEntry GetDirectoryEntryByGuid(string nativeGuid)
       {
           string pathurl = string.Empty;
         int i=  ADPath.IndexOf("DC");
         if (i > 0)
         {
             pathurl = ADPath.Substring(0, i);
         }
         string rootPath = pathurl + "<GUID=" + nativeGuid + ">";
           try
           {
               //string rootPath = "LDAP://172.24.63.106/<GUID=CEE168ACD3EF8B41B97881FA9F32EF67>";
               DirectoryEntry de = new DirectoryEntry(rootPath, ADUser, ADPassword);
               string s = de.NativeGuid;
               return de;
           }
           catch (Exception exception)
           {

               throw new Exception("根据guid获得对象失败(没有此对象)", exception);
           }
       }

        /// <summary>
        /// 验证登录账号
        /// </summary>
        /// <param name="aduser">账号</param>
        /// <param name="adpassword">密码</param>
        /// <returns></returns>
       public static bool ValidateUser(string aduser,string adpassword)
       {
           try
           {
                var obj = ValidateDomainUser(aduser, adpassword);

                if (!string.IsNullOrEmpty(obj))
                {
                    return true;
                }
                else
                {
                    return false;
                }
              
           }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
           catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
           {
               return false;
           }
       }

        public static string ValidateDomainUser( string UserName, string Password)
        {
            string LDAPPath = ConfigHelper.GetConfigStr("LDAPPath");
            

            DirectoryEntry entry = new DirectoryEntry(LDAPPath, UserName, Password);
            try
            {
                string objectSid = (new SecurityIdentifier((byte[])entry.Properties["objectSid"].Value, 0).Value);//如果账号不对则会抛出异常
                return objectSid;
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch(Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
          
                return null;
            }
            finally
            {
                entry.Dispose();
            }

        }



       private static DirectoryEntry GetDirectoryObject(string adUser, string adPassword)
       {
           string LDAPPath=ConfigHelper.GetConfigStr("LDAPPath");
       

           DirectoryEntry entry = new DirectoryEntry(LDAPPath, adUser, adPassword, AuthenticationTypes.Secure);
           return entry;
       }


                // Nested Types
        public enum ADS_USER_FLAG_ENUM
        {
            ADS_UF_ACCOUNTDISABLE = 2,
            ADS_UF_DONT_EXPIRE_PASSWD = 0x10000,
            ADS_UF_DONT_REQUIRE_PREAUTH = 0x4000000,
            ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x80,
            ADS_UF_HOMEDIR_REQUIRED = 8,
            ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x800,
            ADS_UF_LOCKOUT = 0x10,
            ADS_UF_MNS_LOGON_ACCOUNT = 0x20000,
            ADS_UF_NORMAL_ACCOUNT = 0x200,
            ADS_UF_NOT_DELEGATED = 0x100000,
            ADS_UF_PASSWD_CANT_CHANGE = 0x40,
            ADS_UF_PASSWD_NOTREQD = 0x20,
            ADS_UF_PASSWORD_EXPIRED = 0x800000,
            ADS_UF_SCRIPT = 1,
            ADS_UF_SERVER_TRUST_ACCOUNT = 0x2000,
            ADS_UF_SMARTCARD_REQUIRED = 0x40000,
            ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x100,
            ADS_UF_TRUSTED_FOR_DELEGATION = 0x80000,
            ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000,
            ADS_UF_USE_DES_KEY_ONLY = 0x200000,
            ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000
        }


    }
}
