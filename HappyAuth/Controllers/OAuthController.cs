using System;
using System.Web.Mvc;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.Messages;

namespace HappyAuth.Controllers
{
    public class OAuthController : Controller
    {
        private readonly ServiceProvider serviceProvider;
        private readonly Components.OAuthComponent oAuthComponent;

        public OAuthController()
            : base()
        {
            serviceProvider = Libs.OAuthServiceProvider.Create();
            oAuthComponent = new Components.OAuthComponent(serviceProvider);            
        }

        //
        // POST: /oauth/access_token
        public void AccessToken()
        {
            var request = serviceProvider.ReadAccessTokenRequest();
            var response = serviceProvider.PrepareAccessTokenMessage(request);
            serviceProvider.Channel.Respond(response);
        }

        //
        // POST: /oauth/authorize_consumer
        public void AuthorizeConsumer(long consumer_id, string authorization_secret)
        {
            var request = oAuthComponent.ParseSession(Session, consumer_id, authorization_secret);
            var msg = (ITokenContainingMessage)request;
            var token = MvcApplication.Collections.GetTokenFromToken(msg.Token);
            oAuthComponent.HandleConsumerAuthorization(request, token);
        }

        //
        // POST: /oauth/authorize
        public ActionResult Authorize()
        {
            UserAuthorizationRequest request = null;
            var token = oAuthComponent.ParseAuthorizationRequest(ref request);

            if (token.Consumer.PreAuthorized)
            {
                oAuthComponent.HandleConsumerAuthorization(request, token);
                return null;
            }
            else
            {
                var authorization_secret = oAuthComponent.GenerateAuthorizationSecret();
                var secret_key = String.Format("{0}.AuthorizationSecret", token.Consumer.Id);
                var request_key = String.Format("{0}.request", token.Consumer.Id);

                Session[secret_key] = authorization_secret;
                Session[request_key] = request;
                ViewBag.ConsumerId = token.Consumer.Id;
                ViewBag.AuthorizationSecret = authorization_secret;
                
                return View();
            }
        }

        //
        // GET: /oauth/request_token
        public void RequestToken()
        {
            oAuthComponent.ProcessTokenRequest();
        }

        public JsonResult Description()
        {
            var description = serviceProvider.ServiceDescription;
            return Json(description, JsonRequestBehavior.AllowGet);
        }

    }
}
