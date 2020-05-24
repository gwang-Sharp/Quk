using Fisk.EnterpriseManageDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Fisk.EnterpriseManageSolution
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //配置Log4Net读取config路径 
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            EFPreheat();
        }
        protected void Session_End(object sender, EventArgs e)
        {
            Server.ClearError();
            Response.WriteFile("~/HttpError/ServerError.html");
        }
        /// <summary>
        /// ef预热
        /// </summary>
        public void EFPreheat()
        {
            using (var dbcontext = new EnterpriseManageDBEntities())
            {
                var objectContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
                var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                mappingCollection.GenerateViews(new List<EdmSchemaError>());
            }
        }
    }
}
