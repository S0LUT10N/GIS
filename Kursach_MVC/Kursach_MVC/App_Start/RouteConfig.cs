using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kursach_MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Урок 23
            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                new[] { "Kursach_MVC.Controllers" });
            // *********************

            // Урок 20
            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                new[] { "Kursach_MVC.Controllers" });
            // *********************


            routes.MapRoute("GetMap", "Pages/GetMap", new { controller = "Pages", action = "GetMap" },
             new[] { "Kursach_MVC.Controllers" });

            routes.MapRoute("SidebarPartial", "Pages/SidebarPartial", new { controller = "Pages", action = "SidebarPartial" },
                new[] { "Kursach_MVC.Controllers" });

            routes.MapRoute("Shop", "Shop/{action}/{name}", new { controller = "Shop", action = "Index", name = UrlParameter.Optional },
                new[] { "Kursach_MVC.Controllers" });

            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" },
             new[] { "Kursach_MVC.Controllers" });

            routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" },
               new[] { "Kursach_MVC.Controllers" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" },
                new[] { "Kursach_MVC.Controllers" });

          //  routes.MapRoute(
          //      name: "Default",
          //      url: "{controller}/{action}/{id}",
         //       defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
         //   );
        }
    }
}
