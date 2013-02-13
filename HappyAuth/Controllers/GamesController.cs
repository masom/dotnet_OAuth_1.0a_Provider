using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HappyAuth.Libs;

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

    }
}
