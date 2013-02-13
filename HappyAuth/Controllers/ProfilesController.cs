using System.Linq;
using System.Web.Mvc;
using HappyAuth.Controllers.Components;
using HappyAuth.Libs;

namespace HappyAuth.Controllers
{
    [OAuthAuthorizationManager(false)]
    [Libs.Attributes.OAuthScope(scope: OAuthScopes.User)]
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
            Models.User user = oAuthComponent.GetUser(RouteData);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

    }
}
