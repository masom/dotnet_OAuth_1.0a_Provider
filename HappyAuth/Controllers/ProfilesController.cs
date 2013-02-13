using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HappyAuth.Libs;

namespace HappyAuth.Controllers
{
    [OAuthAuthorizationManager]
    [Libs.Attributes.OAuthScope(scope: OAuthScopes.User)]
    public class ProfilesController : Controller
    {
        //
        // GET: /Profiles/

        public JsonResult Index()
        {
            Models.User user = null;
            if (RouteData.Values.Keys.Contains("oauth_access_token"))
            {
                var accessToken = (OAuthToken)RouteData.Values["oauth_access_token"];
                user = accessToken.User;
            }

            return Json(user);
        }

    }
}
