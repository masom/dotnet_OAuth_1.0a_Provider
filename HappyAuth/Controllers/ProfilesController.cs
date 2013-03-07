using System.Linq;
using System.Web.Mvc;
using HappyAuth.Controllers.Attributes;
using HappyAuth.Controllers.Components;
using HappyAuth.Domain;

namespace HappyAuth.Controllers
{
    /// <summary>
    /// This controller requires the user scope when using OAuth.
    /// If the request is not made with OAuth, the authentication is skipped.
    /// </summary>
    [OAuthAuthorizationManager(OAuthScopes.User, false)]
    public class ProfilesController : Controller
    {
        private readonly OAuthComponent oAuthComponent;
        public ProfilesController()
            : base()
        {
            oAuthComponent = new OAuthComponent();
        }
        //
        // GET: /Profiles/
        public JsonResult Index()
        {
            //WARNING user could be null if the request is not made with OAuth.
            var userId = oAuthComponent.GetUser(RouteData);
            var user = MvcApplication.Collections.Users.FirstOrDefault(u => u.Id == long.Parse(userId));
            return Json(user, JsonRequestBehavior.AllowGet);
        }

    }
}
