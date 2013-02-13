using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HappyAuth.Controllers.Components;
using HappyAuth.Libs;

namespace HappyAuth.Controllers
{
    /// <summary>
    /// This controller requires the User scope.
    /// Some specific actions will required other scopes such has `character`.
    /// </summary>
    [OAuthAuthorizationManager]
    [Libs.Attributes.OAuthScope(OAuthScopes.User)]
    public class CharactersController : Controller
    {
        private readonly OAuthComponent oAuthComponent;
        public CharactersController()
            : base()
        {
            oAuthComponent = new OAuthComponent();
        }
        //
        // GET: /Characters/
        public JsonResult Index()
        {
            var user = oAuthComponent.GetUser(RouteData);
            var characters = MvcApplication.Collections.Characters;
            var myCharacters = characters.Where(c => c.Owner.Equals(user));

            return Json(myCharacters, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Details(long id)
        {
            var user = oAuthComponent.GetUser(RouteData);
            var character = MvcApplication.Collections.Characters.FirstOrDefault(c => c.Id.Equals(id) && c.Owner.Equals(user));
            if (character == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "Character not found.");
            }

            return Json(character, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add()
        {
            return Json("ok");
        }

        
        [HttpPut]
        public JsonResult Update()
        {
            return Json("ok");
        }
    }
}
