using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Business_Tier
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional } 
                // By default (so, if you just go to https://my.web.site/ ) the website will go to controller Home, function Index.

                defaults: new { controller = "Root", action = "Index", id = UrlParameter.Optional }
                // Thus will go to controller Root, function Index. 
            );
        }
    }
}
