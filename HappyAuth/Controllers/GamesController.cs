using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HappyAuth.Libs;
using HappyAuth.Libs.Attributes;

namespace HappyAuth.Controllers
{
    /// <summary>
    /// Sample controlle that is accessible by any authenticated OAuth client, regardless of their requested scope.
    /// </summary>
    [OAuthAuthorizationManager]
    public class GamesController : Controller
    {
        //
        // GET: /Games/
        public JsonResult Index()
        {
            var games = new List<String>
                {
                    "Reddit",
                    "StackOverflow",
                    "Slashdot"
                };
            return Json(games, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Really, this is rather pointless to only allow 2-legged to access this.
        /// but it shows how the requested scoped impacts resource access.
        /// </summary>
        /// <returns></returns>
        [OAuthScope(scope: OAuthScopes.Consumer)]
        public JsonResult Nope()
        {
            var games = new List<String>
                {
                    "User's can't see this.",
                    "hahahahaha."
                };
            return Json(games, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This action is only accessible if the OAuth consumer is associated with a user (3-legged).
        /// </summary>
        /// <returns></returns>
        [OAuthScope(OAuthScopes.User)]
        public JsonResult My()
        {
            var my = new List<String>();
            return Json(my, JsonRequestBehavior.AllowGet);
        }
    }
}
