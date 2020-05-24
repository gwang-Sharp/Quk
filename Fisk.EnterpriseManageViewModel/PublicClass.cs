using log4net;
using System.Configuration;
namespace Fisk.EnterpriseManageSolution.Models
{
    public class PublicClass
    {
        public static readonly string corpid = ConfigurationManager.AppSettings["Corpid"];
        public static readonly string secret = ConfigurationManager.AppSettings["Secret"];
        public static readonly string agentid = ConfigurationManager.AppSettings["AgentId"];
        public static readonly string tokenurl = ConfigurationManager.AppSettings["TokenUrl"];
        public static readonly string authorizeurl = ConfigurationManager.AppSettings["AuthorizeUrl"];
        public static readonly string useridurl = ConfigurationManager.AppSettings["UserIdUrl"];
        public static readonly string userinfourl = ConfigurationManager.AppSettings["UserInfoUrl"];
        public static readonly string PmPosition = ConfigurationManager.AppSettings["TeamsPM"];
        public static readonly string EngineerPosition = ConfigurationManager.AppSettings["TeamsMember"];
        public static readonly string PM_URL = ConfigurationManager.AppSettings["TeamsPMUrl"];
        public static readonly string Engineer_URL = ConfigurationManager.AppSettings["TeamsMemberUrl"];
        public static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
    }
}