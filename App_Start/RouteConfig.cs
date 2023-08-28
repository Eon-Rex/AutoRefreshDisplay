using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AmberProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


             routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "StoreDisplay",
                url: "StoreDisplay",
                defaults: new { controller = "AmberFetchData", action = "ViewData" }
            );

            routes.MapRoute(
                name: "GateDisplay",
                url: "GateDislpay",
                defaults: new { controller = "GetDisplayData", action = "GetDisplayView" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );



        }
    }
}
