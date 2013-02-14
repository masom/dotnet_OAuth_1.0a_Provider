using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HappyAuth.Controllers
{
    public class ErrorsController : Controller
    {
        //
        // GET: /Errors/
        public JsonResult Index()
        {
            if (!RouteData.Values.ContainsKey("error"))
            {
                return Json("Internal Server Error", JsonRequestBehavior.AllowGet);
            }

            var error = RouteData.Values["error"] as Exception;
            if (error == null)
            {
                return Json("Internal Server Error", JsonRequestBehavior.AllowGet);
            }

            string msg = error.Message;

            var exception = error as HttpException;
            if (exception != null)
            {
                if (exception.GetHttpCode() == (int)HttpStatusCode.NotFound)
                {
                    msg = "404 Not Found";
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

    }
}
