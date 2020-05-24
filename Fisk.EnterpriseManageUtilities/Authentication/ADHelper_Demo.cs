using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.DirectoryServices;
using System.Data;
using System.Collections;


namespace Fisk.EnterpriseManageUtilities.Authentication
{
    /// <summary>
    /// AD帮助类
    /// AppSettings:ADPassword,ADPath,ADUser,DomainName,LDAPDomain,sPrincpleNameTail
    /// </summary>
    public class ADHelper_Demo
    {
        private static string ADPassword =ConfigurationManager.AppSettings["ADPassword"];
        private static string ADPath = ConfigurationManager.AppSettings["ADPath"];
        private static string ADUser = ConfigurationManager.AppSettings["ADUser"];
        private static AuthenticationTypes DefaultAuthenticationType = AuthenticationTypes.Secure;
        private static string DomainName = ConfigurationManager.AppSettings["DomainName"];
        private static string homeMDB = "";
        private static string LDAPDomain = ConfigurationManager.AppSettings["LDAPDomain"];
        private static string sPrincpleNameTail = ConfigurationManager.AppSettings["sPrincpleNameTail"];

        /// <summary>
        /// 
        /// </summary>
        public ADHelper_Demo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sADPath"></param>
        /// <param name="sDomainName"></param>
        /// <param name="sADUser"></param>
        /// <param name="sADUserPWD"></param>
        public ADHelper_Demo(string sADPath, string sDomainName, string sADUser, string sADUserPWD)
        {
            ADPath = sADPath;
            DomainName = sDomainName;
            ADUser = sADUser;
            ADPassword = sADUserPWD;
        }

        /// <summary>
        /// 添加用户到指定安全组
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool AddUserToGroup(string userPrincipalName, string groupName)
        {
            return AddUserToGroup(ADUser, ADPassword, userPrincipalName, groupName);
        }

        /// <summary>
        /// 添加用户到指定安全组
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="adminPassword"></param>
        /// <param name="userPrincipalName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool AddUserToGroup(string adminName, string adminPassword, string userPrincipalName, string groupName)
        {
            return AddUserToGroupInOU(adminName, adminPassword, userPrincipalName, groupName, null);
        }

        /// <summary>
        /// 添加用户到指定OU的安全组
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="groupName"></param>
        /// <param name="organizeUnit"></param>
        /// <returns></returns>
        public static bool AddUserToGroupInOU(string userPrincipalName, string groupName, string organizeUnit)
        {
            return AddUserToGroupInOU(ADUser, ADPassword, userPrincipalName, groupName, organizeUnit);
        }

        /// <summary>
        /// 添加用户到指定OU的安全组
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="adminPassword"></param>
        /// <param name="userPrincipalName"></param>
        /// <param name="groupName"></param>
        /// <param name="organizeUnit"></param>
        /// <returns></returns>
        public static bool AddUserToGroupInOU(string adminName, string adminPassword, string userPrincipalName, string groupName, string organizeUnit)
        {
            Exception exception;
            bool flag = true;
            DirectoryEntry entry = new DirectoryEntry(GetOrganizeNamePath(organizeUnit), adminName, adminPassword, DefaultAuthenticationType);
            DirectoryEntry entry2 = null;
            DirectoryEntry entry3 = null;
            try
            {
                entry2 = entry.Children.Find("CN=" + groupName);
            }
            catch (Exception exception1)
            {
                exception = exception1;
                return false;
            }
            entry3 = GetADAccountByUPN(adminName, adminPassword, userPrincipalName);
            if (entry3 == null)
            {
                return false;
            }
            if ((entry2 != null) && !entry2.Properties["member"].Contains(entry3.Properties["distinguishedName"].Value))
            {
                entry2.Properties["member"].Add(entry3.Properties["distinguishedName"].Value);
                try
                {
                    entry2.CommitChanges();
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    flag = false;
                }
            }
            return flag;
        }

        public static bool ChangeAccPassword(string sAMacc, string oldPassword, string newPassword)
        {
            try
            {
                if (IsAccExists(sAMacc))
                {
                    DirectoryEntry directoryEntryByAccount = GetDirectoryEntryByAccount(sAMacc);
                    directoryEntryByAccount.Invoke("SetPassword", new object[] { newPassword });
                    directoryEntryByAccount.CommitChanges();
                    directoryEntryByAccount.Close();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void ChangeUserPassword(string commonName, string oldPassword, string newPassword)
        {
            DirectoryEntry directoryEntry = GetDirectoryEntry(commonName);
            directoryEntry.Invoke("ChangePassword", new object[] { oldPassword, newPassword });
            directoryEntry.Close();
        }

        /// <summary>
        /// 检查OU是否存在
        /// </summary>
        /// <param name="sOU"></param>
        /// <returns></returns>
        public static bool CheckOU(string sOU)
        {
            DirectoryEntry entry = null;
            try
            {
                entry = new DirectoryEntry(GetOrganizeNamePath(sOU), ADUser, ADPassword, DefaultAuthenticationType);
                string name = entry.Name;
                entry.Close();
                entry = null;
                return true;
            }
            catch
            {
                entry.Close();
                entry = null;
                return false;
            }
        }

        /// <summary>
        /// 创建AD帐号
        /// </summary>
        /// <param name="CommonName"></param>
        /// <param name="Account"></param>
        /// <param name="organizeName"></param>
        /// <param name="password"></param>
        /// <param name="CreateMail"></param>
        /// <returns></returns>
        public static string CreateADAccount(string CommonName, string Account, string organizeName, string password, bool CreateMail)
        {
            if (IsAccExists(Account))
            {
                return "";
            }
            DirectoryEntry entry = null;
            DirectoryEntry entry2 = null;
            string str = Account;
            try
            {
                entry = new DirectoryEntry(GetOrganizeNamePath(organizeName), ADUser, ADPassword, DefaultAuthenticationType);
                entry2 = entry.Children.Add("CN=" + CommonName, "user");
                entry2.Properties["userPrincipalName"].Value = str + sPrincpleNameTail;
                entry2.Properties["sAMAccountName"].Add(str);
                entry2.Properties["displayName"].Add(CommonName);
                entry2.CommitChanges();
                entry2.Invoke("SetPassword", new object[] { password });
                entry2.Properties["userAccountControl"].Value = ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD | ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT;
                entry2.CommitChanges();
                if (CreateMail)
                {
                    entry2.Invoke("CreateMailbox", new object[] { homeMDB });
                    entry2.CommitChanges();
                }
            }
            catch (Exception exception)
            {
                try
                {
                    entry2.DeleteTree();
                }
                catch
                {
                }
                throw exception;
            }
            return entry2.Path;
        }

        /// <summary>
        /// 创建OU
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentOrganizeUnit"></param>
        /// <returns></returns>
        public static DirectoryEntry CreateOrganizeUnit(string name, string parentOrganizeUnit)
        {
            return CreateOrganizeUnit(null, null, name, parentOrganizeUnit);
        }

        /// <summary>
        /// 创建OU
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="adminPassword"></param>
        /// <param name="name"></param>
        /// <param name="parentOrganizeUnit"></param>
        /// <returns></returns>
        public static DirectoryEntry CreateOrganizeUnit(string adminName, string adminPassword, string name, string parentOrganizeUnit)
        {
            adminName = ADUser;
            adminPassword = ADPassword;
            DirectoryEntry entry = null;
            if ((adminName == null) || (adminPassword == null))
            {
                entry = new DirectoryEntry(GetOrganizeNamePath(parentOrganizeUnit));
            }
            else if (parentOrganizeUnit != "")
            {
                entry = new DirectoryEntry(GetOrganizeNamePath(parentOrganizeUnit), adminName, adminPassword, DefaultAuthenticationType);
            }
            else
            {
                entry = new DirectoryEntry(GetOrganizeNamePath(parentOrganizeUnit), adminName, adminPassword, DefaultAuthenticationType);
            }
            DirectoryEntry entry2 = entry.Children.Add("OU=" + name, "organizationalUnit");
            entry2.CommitChanges();
            return entry2;
        }

        /// <summary>
        /// 删除AD帐号
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public static bool DeleteADAccount(string Account)
        {
            if (IsAccExists(Account))
            {
                DetTree(GetAccPath(Account));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除AD帐号
        /// </summary>
        /// <param name="adminUser"></param>
        /// <param name="adminPassword"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool DeleteADAccount(string adminUser, string adminPassword, string userName)
        {
            DirectoryEntry entry = null;
            DirectoryEntry parent = null;
            try
            {
                adminUser = ADUser;
                adminPassword = ADPassword;
                entry = FindObject(adminUser, adminPassword, "user", userName);
                parent = entry.Parent;
                parent.Children.Remove(entry);
                parent.CommitChanges();
                parent.Dispose();
                entry.Dispose();
                return true;
            }
            catch (Exception)
            {
                parent.Dispose();
                entry.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 删除OU
        /// </summary>
        /// <param name="OUName"></param>
        /// <returns></returns>
        public static string DeleteOU(string OUName)
        {
            try
            {
                if (CheckOU(OUName))
                {
                    DirectoryEntry entry = new DirectoryEntry(GetOrganizeNamePath(OUName));
                    DirectoryEntry parent = entry.Parent;
                    parent.Children.Remove(entry);
                    parent.CommitChanges();
                    return "成功";
                }
                return "不存在";
            }
            catch (Exception)
            {
                return "非空";
            }
        }

        /// <summary>
        /// 删除OU
        /// </summary>
        /// <param name="OUName"></param>
        /// <param name="boolComplete"></param>
        /// <returns></returns>
        public static bool DeleteOU(string OUName, bool boolComplete)
        {
            bool flag;
            try
            {
                if (!boolComplete)
                {
                    return false;
                }
                if (CheckOU(OUName))
                {
                    string organizeNamePath = GetOrganizeNamePath(OUName);
                    if (organizeNamePath == "")
                    {
                        return false;
                    }
                    DetTree(organizeNamePath);
                    return true;
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        /// <summary>
        /// 删除指定路径的目录树
        /// </summary>
        /// <param name="path"></param>
        public static void DetTree(string path)
        {
            try
            {
                new DirectoryEntry(path, ADUser, ADPassword, DefaultAuthenticationType).DeleteTree();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="de"></param>
        public static void DisableUser(DirectoryEntry de)
        {
            de.Properties["userAccountControl"][0] = ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD | ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT | ADS_USER_FLAG_ENUM.ADS_UF_ACCOUNTDISABLE;
            de.CommitChanges();
            de.Close();
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="commonName"></param>
        public static void DisableUser(string commonName)
        {
            DisableUser(GetDirectoryEntry(commonName));
        }

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="de"></param>
        public static void EnableUser(DirectoryEntry de)
        {
            de.Properties["userAccountControl"][0] = ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD | ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT;
            de.CommitChanges();
            de.Close();
        }

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="commonName"></param>
        public static void EnableUser(string commonName)
        {
            EnableUser(GetDirectoryEntry(commonName));
        }

        /// <summary>
        /// 查找目录实体
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DirectoryEntry FindObject(string category, string name)
        {
            return FindObject(ADUser, ADPassword, category, name);
        }

        /// <summary>
        /// 查找目录实体
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="adminPassword"></param>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DirectoryEntry FindObject(string adminName, string adminPassword, string category, string name)
        {
            DirectoryEntry searchRoot = null;
            searchRoot = new DirectoryEntry(ADPath, adminName, adminPassword, DefaultAuthenticationType);
            DirectorySearcher searcher = new DirectorySearcher(searchRoot);
            string str = string.Format("(&(objectCategory=" + category + ")(sAMAccountName={0}))", name);
            searcher.Filter = str;
            searcher.Sort.PropertyName = "cn";
            DirectoryEntry directoryEntry = null;
            try
            {
                directoryEntry = searcher.FindOne().GetDirectoryEntry();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (searchRoot != null)
                {
                    searchRoot.Dispose();
                }
                if (searcher != null)
                {
                    searcher.Dispose();
                }
            }
            return directoryEntry;
        }

        /// <summary>
        /// 按用户sAMAccountName属性值查找Path
        /// </summary>
        /// <param name="sAcc">sAMAccountName</param>
        /// <returns></returns>
        public static string GetAccPath(string sAcc)
        {
            DirectoryEntry directoryObject = GetDirectoryObject();
            DirectorySearcher searcher = new DirectorySearcher(directoryObject);
            searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + sAcc + "))";
            searcher.SearchScope = SearchScope.Subtree;
            try
            {
                string path = searcher.FindOne().Path;
                searcher.Dispose();
                directoryObject.Dispose();
                return path;
            }
            catch (Exception)
            {
                searcher.Dispose();
                directoryObject.Dispose();
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取AD帐号目录实体
        /// </summary>
        /// <param name="userPath"></param>
        /// <returns></returns>
        public static DirectoryEntry GetADAccount(string userPath)
        {
            if (!string.IsNullOrEmpty(userPath))
            {
                string str = "";
                string str2 = "";
                if (userPath.Split(new char[] { '\\' }).Length > 1)
                {
                    str = SplitContainerNameToDN(userPath.Substring(0, userPath.LastIndexOf('\\')));
                    str2 = userPath.Substring(userPath.LastIndexOf('\\') + 1, (userPath.Length - userPath.LastIndexOf('\\')) - 1);
                }
                else
                {
                    str = SplitContainerNameToDN("");
                    str2 = userPath;
                }
                DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject("/" + str));
                searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + str2.Replace(@"\", "") + "))";
                searcher.SearchScope = SearchScope.Subtree;
                try
                {
                    DirectoryEntry entry;
                    SearchResult result = searcher.FindOne();
                    if (result != null)
                    {
                        entry = new DirectoryEntry(result.Path, ADUser, ADPassword);
                    }
                    else
                    {
                        return null;
                    }
                    return entry;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public static DirectoryEntry GetADAccountByUPN(string userPrincipalName)
        {
            return GetADAccountByUPN(ADUser, ADPassword, userPrincipalName);
        }

        /// <summary>
        /// 按sAMAccountName获取AD帐号
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="adminPassword"></param>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public static DirectoryEntry GetADAccountByUPN(string adminName, string adminPassword, string userPrincipalName)
        {
            if (!string.IsNullOrEmpty(userPrincipalName))
            {
                try
                {
                    DirectoryEntry entry;
                    DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject(adminName, adminPassword));
                    searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + userPrincipalName + "))";
                    searcher.SearchScope = SearchScope.Subtree;
                    SearchResult result = searcher.FindOne();
                    if (result != null)
                    {
                        entry = new DirectoryEntry(result.Path, adminName, adminPassword, DefaultAuthenticationType);
                    }
                    else
                    {
                        return null;
                    }
                    return entry;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="organizeUnit"></param>
        /// <returns></returns>
        public static DirectoryEntry GetADGroupInOU(string groupName, string organizeUnit)
        {
            return GetADGroupInOU(ADUser, ADPassword, groupName, organizeUnit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="adminPassword"></param>
        /// <param name="groupName"></param>
        /// <param name="organizeUnit"></param>
        /// <returns></returns>
        public static DirectoryEntry GetADGroupInOU(string adminName, string adminPassword, string groupName, string organizeUnit)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                DirectoryEntry searchRoot = new DirectoryEntry(GetOrganizeNamePath(organizeUnit), adminName, adminPassword, DefaultAuthenticationType);
                DirectorySearcher searcher = new DirectorySearcher(searchRoot);
                searcher.Filter = "(&(objectClass=group)(cn=" + groupName.Replace(@"\", "") + "))";
                searcher.SearchScope = SearchScope.Subtree;
                try
                {
                    SearchResult result = searcher.FindOne();
                    if (result != null)
                    {
                        searchRoot = new DirectoryEntry(result.Path, adminName, adminPassword);
                    }
                    else
                    {
                        return null;
                    }
                    return searchRoot;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ouPath"></param>
        /// <returns></returns>
        public static DirectoryEntry GetADOrganizationUnit(string ouPath)
        {
            if (!string.IsNullOrEmpty(ouPath))
            {
                string str = "";
                string str2 = "";
                if (ouPath.Split(new char[] { '\\' }).Length > 1)
                {
                    str = SplitOrganizeNameToDN(ouPath.Substring(0, ouPath.LastIndexOf('\\')));
                    str2 = ouPath.Substring(ouPath.LastIndexOf('\\') + 1, (ouPath.Length - ouPath.LastIndexOf('\\')) - 1);
                }
                else
                {
                    str = SplitOrganizeNameToDN("");
                    str2 = ouPath;
                }
                DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject("/" + str));
                searcher.Filter = "(&(objectClass=organizationalUnit)(ou=" + str2 + "))";
                searcher.SearchScope = SearchScope.Subtree;
                try
                {
                    DirectoryEntry entry;
                    SearchResult result = searcher.FindOne();
                    if (result != null)
                    {
                        entry = new DirectoryEntry(result.Path, ADUser, ADPassword, DefaultAuthenticationType);
                    }
                    else
                    {
                        return null;
                    }
                    return entry;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static string GetContainerNamePath(string containerName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ADPath);
            builder.Append("/");
            return builder.Append(SplitContainerNameToDN(containerName)).ToString();
        }

        public static DirectoryEntry GetDirectoryEntry(string commonName)
        {
            DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject());
            searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName.Replace(@"\", "") + "))";
            searcher.SearchScope = SearchScope.Subtree;
            try
            {
                return new DirectoryEntry(searcher.FindOne().Path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commonName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static DirectoryEntry GetDirectoryEntry(string commonName, string password)
        {
            DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject(commonName, password));
            searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName.Replace(@"\", "") + "))";
            searcher.SearchScope = SearchScope.Subtree;
            try
            {
                return new DirectoryEntry(searcher.FindOne().Path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DirectoryEntry GetDirectoryEntryByAccount(string sAMAccountName)
        {
            DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject(ADUser, ADPassword));
            searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + sAMAccountName + "))";
            searcher.SearchScope = SearchScope.Subtree;
            try
            {
                return new DirectoryEntry(searcher.FindOne().Path, ADUser, ADPassword);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DirectoryEntry GetDirectoryEntryByAccount(string sAMAccountName, string password)
        {
            DirectoryEntry directoryEntryByAccount = GetDirectoryEntryByAccount(sAMAccountName);
            if (directoryEntryByAccount != null)
            {
                string commonName = directoryEntryByAccount.Properties["cn"][0].ToString();
                if (GetDirectoryEntry(commonName, password) != null)
                {
                    return GetDirectoryEntry(commonName, password);
                }
                return null;
            }
            return null;
        }

        public static DirectoryEntry GetDirectoryEntryOfGroup(string groupName)
        {
            DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject());
            searcher.Filter = "(&(objectClass=group)(cn=" + groupName.Replace(@"\", "") + "))";
            searcher.SearchScope = SearchScope.Subtree;
            try
            {
                return new DirectoryEntry(searcher.FindOne().Path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DirectoryEntry GetDirectoryObject()
        {
            DirectoryEntry entry = null;
            try
            {
                entry = new DirectoryEntry(ADPath, ADUser, ADPassword, DefaultAuthenticationType);
            }
            catch (Exception)
            {
            }
            return entry;
        }

        public static DirectoryEntry GetDirectoryObject(string domainReference)
        {
            DirectoryEntry entry = null;
            try
            {
                entry = new DirectoryEntry(ADPath + domainReference, ADUser, ADPassword, DefaultAuthenticationType);
            }
            catch (Exception)
            {
            }
            return entry;
        }

        public static DirectoryEntry GetDirectoryObject(string userName, string password)
        {
            DirectoryEntry entry = null;
            try
            {
                entry = new DirectoryEntry(ADPath, userName, password, DefaultAuthenticationType);
            }
            catch (Exception)
            {
            }
            return entry;
        }

        private static DirectoryEntry GetDirectoryObject(string domainReference, string userName, string password)
        {
            DirectoryEntry entry = null;
            try
            {
                entry = new DirectoryEntry(ADPath + domainReference, userName, password, DefaultAuthenticationType);
            }
            catch (Exception)
            {
            }
            return entry;
        }

        public static string GetDomainDN()
        {
            return LDAPDomain;
        }

        public static string GetOrganizeNamePath(string organizeUnit)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ADPath);
            builder.Append("/");
            return builder.Append(SplitOrganizeNameToDN(organizeUnit)).ToString();
        }

        public static string GetProperty(DirectoryEntry de, string propertyName)
        {
            if (de.Properties.Contains(propertyName))
            {
                return de.Properties[propertyName][0].ToString();
            }
            return string.Empty;
        }

        public static string GetProperty(SearchResult searchResult, string propertyName)
        {
            if (searchResult.Properties.Contains(propertyName))
            {
                return searchResult.Properties[propertyName][0].ToString();
            }
            return string.Empty;
        }

        private static string GetUserPath()
        {
            return GetUserPath(null);
        }

        private static string GetUserPath(string userName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ADPath);
            builder.Append("/");
            if ((userName != null) && (userName.Length > 0))
            {
                builder.Append("CN=").Append(userName).Append(",");
            }
            builder.Append("CN=Users,").Append(GetDomainDN());
            return builder.ToString();
        }

        public static string GetUserPath(string userName, string organzieName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ADPath);
            builder.Append("/");
            builder.Append("CN=").Append(userName).Append(",").Append(SplitOrganizeNameToDN(organzieName));
            return builder.ToString();
        }

        public static DataTable GetUsersFromFile(string FileName)
        {
            try
            {
                DataTable table = new DataTable("users");
                table.Columns.Add("UserID", Type.GetType("System.String"));
                table.Columns.Add("UserName", Type.GetType("System.String"));
                table.Columns.Add("UserAccount", Type.GetType("System.String"));
                DirectorySearcher searcher = new DirectorySearcher("(&(objectCategory=Person)(objectClass=user))");
                DirectoryEntry entry = new DirectoryEntry(ADPath + "/CN=" + FileName + "," + LDAPDomain);
                searcher.SearchRoot = entry;
                searcher.SearchScope = SearchScope.Subtree;
                foreach (SearchResult result in searcher.FindAll())
                {
                    IDictionaryEnumerator enumerator = result.Properties.GetEnumerator();
                    DataRow row = table.NewRow();
                    row["UserID"] = GetProperty(result, "Name");
                    row["UserName"] = GetProperty(result, "displayname");
                    if (IsAccountActive(Convert.ToInt32(GetProperty(result, "userAccountControl"))))
                    {
                        row["UserAccount"] = "激活";
                    }
                    else
                    {
                        row["UserAccount"] = "禁用";
                    }
                    table.Rows.Add(row);
                }
                return table;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetUsersFromOU(string OUName)
        {
            try
            {
                if (CheckOU(OUName))
                {
                    DirectoryEntry entry = new DirectoryEntry(GetOrganizeNamePath(OUName), ADUser, ADPassword, DefaultAuthenticationType);
                    DataTable table = new DataTable("users");
                    table.Columns.Add("UserID", Type.GetType("System.String"));
                    table.Columns.Add("UserName", Type.GetType("System.String"));
                    table.Columns.Add("UserAccount", Type.GetType("System.String"));
                    DirectorySearcher searcher = new DirectorySearcher("(&(objectCategory=Person)(objectClass=user))");
                    DirectoryEntry entry2 = new DirectoryEntry(ADPath + "/OU=" + OUName + "," + LDAPDomain);
                    searcher.SearchRoot = entry2;
                    searcher.SearchScope = SearchScope.Subtree;
                    foreach (SearchResult result in searcher.FindAll())
                    {
                        IDictionaryEnumerator enumerator = result.Properties.GetEnumerator();
                        DataRow row = table.NewRow();
                        row["UserID"] = GetProperty(result, "Name");
                        row["UserName"] = GetProperty(result, "displayname");
                        if (IsAccountActive(Convert.ToInt32(GetProperty(result, "userAccountControl"))))
                        {
                            row["UserAccount"] = "激活";
                        }
                        else
                        {
                            row["UserAccount"] = "禁用";
                        }
                        table.Rows.Add(row);
                    }
                    return table;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsAccExists(string sAMAccountName)
        {
            DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject());
            searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + sAMAccountName + "))";
            if (searcher.FindAll().Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsAccountActive(int userAccountControl)
        {
            int num = Convert.ToInt32(ADS_USER_FLAG_ENUM.ADS_UF_ACCOUNTDISABLE);
            int num2 = userAccountControl & num;
            if (num2 > 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsUserExists(string commonName)
        {
            DirectorySearcher searcher = new DirectorySearcher(GetDirectoryObject());
            searcher.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName.Replace(@"\", "") + "))";
            if (searcher.FindAll().Count == 0)
            {
                return false;
            }
            return true;
        }

        public static LoginResult Login(string commonName, string password)
        {
            DirectoryEntry directoryEntry = GetDirectoryEntry(commonName);
            if (directoryEntry != null)
            {
                int userAccountControl = Convert.ToInt32(directoryEntry.Properties["userAccountControl"][0]);
                directoryEntry.Close();
                if (!IsAccountActive(userAccountControl))
                {
                    return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;
                }
                if (GetDirectoryEntry(commonName, password) != null)
                {
                    return LoginResult.LOGIN_USER_OK;
                }
                return LoginResult.LOGIN_USER_PASSWORD_INCORRECT;
            }
            return LoginResult.LOGIN_USER_DOESNT_EXIST;
        }

        public static LoginResult LoginByAccount(string sAMAccountName, string password)
        {
            DirectoryEntry directoryEntryByAccount = GetDirectoryEntryByAccount(sAMAccountName);
            if (directoryEntryByAccount != null)
            {
                int userAccountControl = Convert.ToInt32(directoryEntryByAccount.Properties["userAccountControl"][0]);
                directoryEntryByAccount.Close();
                if (!IsAccountActive(userAccountControl))
                {
                    return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;
                }
                if (GetDirectoryEntryByAccount(sAMAccountName, password) != null)
                {
                    return LoginResult.LOGIN_USER_OK;
                }
                return LoginResult.LOGIN_USER_PASSWORD_INCORRECT;
            }
            return LoginResult.LOGIN_USER_DOESNT_EXIST;
        }

        public static string MoveUser(string user_path, string target_path)
        {
            DirectoryEntry entry = new DirectoryEntry(user_path, ADUser, ADPassword, DefaultAuthenticationType);
            DirectoryEntry newParent = new DirectoryEntry(target_path, ADUser, ADPassword, DefaultAuthenticationType);
            entry.MoveTo(newParent);
            return entry.Path;
        }

        public static bool RemoveUserFromGroup(string userPrincipalName, string groupName)
        {
            return RemoveUserFromGroupInOU(ADUser, ADPassword, userPrincipalName, groupName, null);
        }

        public static bool RemoveUserFromGroupInOU(string userPrincipalName, string groupName, string organizeUnit)
        {
            return RemoveUserFromGroupInOU(ADUser, ADPassword, userPrincipalName, groupName, organizeUnit);
        }

        public static bool RemoveUserFromGroupInOU(string adminName, string adminPassword, string userPrincipalName, string groupName, string organizeUnit)
        {
            Exception exception;
            bool flag = true;
            DirectoryEntry entry = new DirectoryEntry(GetOrganizeNamePath(organizeUnit), adminName, adminPassword, DefaultAuthenticationType);
            DirectoryEntry entry2 = null;
            DirectoryEntry entry3 = null;
            try
            {
                entry2 = entry.Children.Find("CN=" + groupName);
            }
            catch (Exception exception1)
            {
                exception = exception1;
                return false;
            }
            entry3 = GetADAccountByUPN(adminName, adminPassword, userPrincipalName);
            if (entry3 == null)
            {
                return false;
            }
            if ((entry2 != null) && entry2.Properties["member"].Contains(entry3.Properties["distinguishedName"].Value))
            {
                entry2.Properties["member"].Remove(entry3.Properties["distinguishedName"].Value);
                try
                {
                    entry2.CommitChanges();
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    flag = false;
                }
            }
            return flag;
        }

        public static bool RenameAcc(string oldAcc, string newAcc)
        {
            try
            {
                if (IsAccExists(oldAcc))
                {
                    if (IsAccExists(newAcc))
                    {
                        return false;
                    }
                    DirectoryEntry directoryEntryByAccount = GetDirectoryEntryByAccount(oldAcc);
                    directoryEntryByAccount.Properties["sAMAccountName"][0] = newAcc;
                    directoryEntryByAccount.CommitChanges();
                    directoryEntryByAccount.Dispose();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RenameOU(string oldOUName, string newOUName)
        {
            try
            {
                if (CheckOU(oldOUName))
                {
                    DirectoryEntry entry = new DirectoryEntry(GetOrganizeNamePath(oldOUName), ADUser, ADPassword, DefaultAuthenticationType);
                    entry.Rename("OU=" + newOUName);
                    entry.CommitChanges();
                    entry.Dispose();
                    return CheckOU(newOUName);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RenameUser(string sAcc, string newDisplayName)
        {
            try
            {
                if (IsAccExists(sAcc))
                {
                    DirectoryEntry directoryEntryByAccount = GetDirectoryEntryByAccount(sAcc);
                    directoryEntryByAccount.Properties["displayName"][0] = newDisplayName;
                    directoryEntryByAccount.Rename("CN=" + newDisplayName);
                    directoryEntryByAccount.CommitChanges();
                    directoryEntryByAccount.Dispose();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ReplaceGroupListFromGroupInOU(string[] subGroupList, string groupName, string organizeUnit)
        {
            return ReplaceGroupListFromGroupInOU(ADUser, ADPassword, subGroupList, groupName, organizeUnit);
        }

        public static bool ReplaceGroupListFromGroupInOU(string adminName, string adminPassword, string[] subGroupList, string groupName, string organizeUnit)
        {
            bool flag = true;
            if ((subGroupList != null) && (subGroupList.Length > 0))
            {
                Exception exception;
                DirectoryEntry entry = new DirectoryEntry(GetOrganizeNamePath(organizeUnit), adminName, adminPassword, DefaultAuthenticationType);
                DirectoryEntry entry2 = null;
                try
                {
                    entry2 = entry.Children.Find("CN=" + groupName);
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    flag = false;
                }
                if (entry2 == null)
                {
                    return flag;
                }
                for (int i = 0; i < entry2.Properties["member"].Count; i++)
                {
                    string[] strArray = entry2.Properties["member"][i].ToString().Split(new char[] { ',' })[0].Split(new char[] { '=' });
                    if ((strArray.Length == 2) && strArray[0].Equals("CN", StringComparison.OrdinalIgnoreCase))
                    {
                        DirectoryEntry entry3 = GetADGroupInOU(adminName, adminPassword, strArray[1], organizeUnit);
                        if ((entry3 != null) && (entry3.SchemaClassName == "group"))
                        {
                            entry2.Properties["member"].RemoveAt(i);
                            i--;
                        }
                    }
                }
                foreach (string str in subGroupList)
                {
                    DirectoryEntry entry4 = GetADGroupInOU(adminName, adminPassword, str, organizeUnit);
                    if (entry4 == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        entry2.Properties["member"].Add(entry4.Properties["distinguishedName"].Value);
                    }
                }
                try
                {
                    entry2.CommitChanges();
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    flag = false;
                }
            }
            return flag;
        }

        public static bool ReplaceUserListFromGroup(string[] userPrincipalNameList, string groupName)
        {
            return ReplaceUserListFromGroupInOU(ADUser, ADPassword, userPrincipalNameList, groupName, null);
        }

        public static bool ReplaceUserListFromGroupInOU(string[] userPrincipalNameList, string groupName, string organizeUnit)
        {
            return ReplaceUserListFromGroupInOU(ADUser, ADPassword, userPrincipalNameList, groupName, organizeUnit);
        }

        public static bool ReplaceUserListFromGroupInOU(string adminName, string adminPassword, string[] userPrincipalNameList, string groupName, string organizeUnit)
        {
            bool flag = true;
            if ((userPrincipalNameList != null) && (userPrincipalNameList.Length > 0))
            {
                Exception exception;
                DirectoryEntry entry = new DirectoryEntry(GetOrganizeNamePath(organizeUnit), adminName, adminPassword, DefaultAuthenticationType);
                DirectoryEntry entry2 = null;
                try
                {
                    entry2 = entry.Children.Find("CN=" + groupName);
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    flag = false;
                }
                if (entry2 == null)
                {
                    return flag;
                }
                for (int i = 0; i < entry2.Properties["member"].Count; i++)
                {
                    string[] strArray = entry2.Properties["member"][i].ToString().Split(new char[] { ',' })[0].Split(new char[] { '=' });
                    if ((strArray.Length == 2) && strArray[0].Equals("CN", StringComparison.OrdinalIgnoreCase))
                    {
                        DirectoryEntry aDAccount = GetADAccount(strArray[1]);
                        if ((aDAccount != null) && (aDAccount.SchemaClassName == "user"))
                        {
                            entry2.Properties["member"].RemoveAt(i);
                            i--;
                        }
                    }
                }
                foreach (string str in userPrincipalNameList)
                {
                    DirectoryEntry entry4 = GetADAccountByUPN(adminName, adminPassword, str);
                    if (entry4 == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        entry2.Properties["member"].Add(entry4.Properties["distinguishedName"].Value);
                    }
                }
                try
                {
                    entry2.CommitChanges();
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    flag = false;
                }
            }
            return flag;
        }

        public static void SetProperty(DirectoryEntry de, string propertyName, string propertyValue)
        {
            if (de.Properties.Contains(propertyName))
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    de.Properties[propertyName].RemoveAt(0);
                }
                else
                {
                    de.Properties[propertyName][0] = propertyValue;
                }
            }
            else if (!string.IsNullOrEmpty(propertyValue))
            {
                de.Properties[propertyName].Add(propertyValue);
            }
        }

        public static void SetUserPassword(string userName, string password)
        {
            SetUserPassword(null, null, userName, password);
        }

        public static void SetUserPassword(string adminName, string adminPassword, string userName, string password)
        {
            adminName = ADUser;
            adminPassword = ADPassword;
            DirectoryEntry entry = FindObject(adminName, adminPassword, "user", userName);
            entry.Invoke("SetPassword", new object[] { password });
            entry.CommitChanges();
        }

        public static string SplitContainerNameToDN(string containerName)
        {
            StringBuilder builder = new StringBuilder();
            if ((containerName != null) && (containerName.Length > 0))
            {
                string[] strArray = containerName.Split(new char[] { '\\' });
                for (int i = strArray.Length - 1; i >= 0; i--)
                {
                    string str = strArray[i];
                    if (builder.Length > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append("CN=").Append(str);
                }
            }
            if (builder.Length > 0)
            {
                builder.Append(",");
            }
            builder.Append(GetDomainDN());
            return builder.ToString();
        }

        public static string SplitOrganizeNameToDN(string organizeName)
        {
            StringBuilder builder = new StringBuilder();
            if ((organizeName != null) && (organizeName.Length > 0))
            {
                string[] strArray = organizeName.Split(new char[] { '/', '\\' });
                for (int i = strArray.Length - 1; i >= 0; i--)
                {
                    string str = strArray[i];
                    if (builder.Length > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append("OU=").Append(str);
                }
            }
            if (builder.Length > 0)
            {
                builder.Append(",");
            }
            builder.Append(GetDomainDN());
            return builder.ToString();
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

        public enum LoginResult
        {
            LOGIN_USER_OK,
            LOGIN_USER_DOESNT_EXIST,
            LOGIN_USER_ACCOUNT_INACTIVE,
            LOGIN_USER_PASSWORD_INCORRECT
        }
    }
}