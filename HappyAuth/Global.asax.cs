using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HappyAuth.Controllers;
using HappyAuth.Libs;

namespace HappyAuth
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
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

            routes.MapRoute(
                "Profiles",
                "profiles/{action}/{id}",
                new { controller = "Profiles", action = "Index", id = ""}
            );
            routes.MapRoute(
                "Games",
                "games/{action}/{id}",
                new { controller = "Games", action = "Index", id = "" }
            );
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication) sender).Context;
            var exception = Server.GetLastError();
            var isHttpException = (exception is HttpException);
            var controller = new ErrorsController();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Errors";
            routeData.Values["action"] = "Index";

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = isHttpException ? ((HttpException)exception).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            var context = new RequestContext(new HttpContextWrapper(httpContext), routeData);
            context.RouteData.Values["error"] = exception;
            ((IController)controller).Execute(context);
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

            var consumer = new Domain.OAuthConsumer
            {
                Id = 432456,
                Name = "HappyStudioWeb",
                PreAuthorized = true,
                Key = "derp",
                Secret = "mc"
            };
            Collections.Characters.Add(character);
            Collections.Consumers.Add(consumer);
            Collections.Users.Add(user);
        }
    }
}