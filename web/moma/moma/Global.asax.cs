using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace moma
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Summary",                                                // Route name
                "summary/{action}-{type}/{page}",				// URL with parameters
                new { controller = "summary", action = "Index", page = 1 }  // Parameter defaults
            );

            routes.MapRoute(
                "Apis",                                                // Route name
                "apis/{apiname}",				// URL with parameters
                new { controller = "Apis", action = "Index" }  // Parameter defaults
            );

            routes.MapRoute(
                "Single Report",                                                // Route name
                "report/{guid}",				// URL with parameters
                new { controller = "Reports", action = "SingleReport" }  // Parameter defaults
            );

            routes.MapRoute(
                "Reports",                                                // Route name
                "reports/{page}",				// URL with parameters
                new { controller = "Reports", action = "Index", page = 1}  // Parameter defaults
            );

            routes.MapRoute(
                "Tree",                                                // Route name
                "tree",				// URL with parameters
                new { controller = "Tree", action = "Index" }  // Parameter defaults
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}",                           // URL with parameters
                new { controller = "Home", action = "Index"}  // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}