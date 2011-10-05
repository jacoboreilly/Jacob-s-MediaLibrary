using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;

namespace MediaLib
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static LibraryProvider provider;
        public static LibraryProvider LibraryProvider
        {
            get
            {
                if (provider == null)
                {
                    provider = new LibraryProvider(ConfigurationManager.AppSettings["mediaPath"], HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["mediaPath"]));
                }
                return provider;
            }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "LibraryPages", // Route name
                "Library/{*path}", // URL with parameters
                new { controller = "Library", action = "Index", path = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}