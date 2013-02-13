using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HappyAuth.Libs;

namespace HappyAuth
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Keeps track of the logs.
        /// </summary>
        public static List<String> Logs = new List<String>();

        public static Collections Collections = new Collections();

        /// <summary>
        /// Ghetto log4net
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            Logs.Add(msg);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "OAuthRequest",
                "oauth/request_token",
                new { controller = "OAuth", action="RequestToken" }
            );
            routes.MapRoute(
                "OAuthAuthorize",
                "oauth/authorize",
                new { controller = "OAuth", action="Authorize" }
            );
            routes.MapRoute(
                "OAuthAccess",
                "oauth/access_token",
                new { controller = "OAuth", action="AccessToken" }
            );

            routes.MapRoute(
                "OAuthDescription",
                "oauth/description",
                new { controller = "OAuth", action = "Description" }
            );
            routes.MapRoute(
                "Characters",
                "characters/{action}/{id}",
                new { controller = "Characters", action = "Index", id = ""}
            );
        }
        protected void Application_BeginRequest(object sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(Request.RawUrl);
        }

        protected void Application_Start()
        {
            System.Diagnostics.Debug.WriteLine("Started");
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();


            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var user = new Models.User
            {
                Id = 1,
                Username = "Derp"
            };

            var character = new Models.Character
            {
                Id = 1,
                Owner = user,
                Name = "McDerp",
                Age = 15,
                Locale = "en-US"
            };

            var consumer = new Libs.OAuthConsumer
            {
                Id = 432456,
                Name = "HappyStudioWeb",
                PreAuthorized = true,
                Key = "derp",
                Secret = "mc"
            };
            Collections.Characters.Add(character);
            Collections.Consumers.Add(consumer);
            Collections.Characters.Add(character);
            Collections.Users.Add(user);
        }
    }
}