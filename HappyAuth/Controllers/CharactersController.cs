using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HappyAuth.Controllers
{
    [Libs.OAuthAuthorizationManager]
    [Libs.Attributes.OAuthScope]
    public class CharactersController : Controller
    {
        //
        // GET: /Characters/
        public JsonResult Index()
        {
            var characters = MvcApplication.Collections.Characters;
            return Json(characters, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Details(long id)
        {
            var character = MvcApplication.Collections.Characters.FirstOrDefault(c => c.Id.Equals(id));
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
