using System.Web.Mvc;
using System.Web.Routing;

namespace Fisk.EnterpriseManageSolution
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
            defaults: new { controller = "main", action = "index", id = UrlParameter.Optional }
            //defaults: new { controller = "admin", action = "AdminManage", id = UrlParameter.Optional }
            );
        }
    }
}
