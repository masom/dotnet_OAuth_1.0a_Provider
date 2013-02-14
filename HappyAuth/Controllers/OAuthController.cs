using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.Messages;

namespace HappyAuth.Controllers
{
    /// <summary>
    /// Handles OAuth requests.
    /// 
    /// Implements both OAuth 1.0a 2-legged and 3-legged flows.
    /// 
    /// To enable the 2-legged flow, a cusom request parameter must be provided: `authmode`.
    /// If `authmode` is set to `consumer`, the OAuth 2-legged flow is enabled.
    /// </summary>
    public class OAuthController : Controller
    {
        /// <summary>
        /// The OAuth service provider.
        /// </summary>
        private readonly ServiceProvider serviceProvider;

        /// <summary>
        /// Helper class handling all the dirty OAuth stuff.
        /// </summary>
        private readonly Components.OAuthComponent oAuthComponent;

        public OAuthController()
            : base()
        {
            serviceProvider = Libs.OAuthServiceProvider.Create();
            oAuthComponent = new Components.OAuthComponent(serviceProvider);            
        }

        
        //
        // POST: /oauth/access_token
        /// <summary>
        /// Delivers authorized access tokens.
        /// </summary>
        public void AccessToken()
        {
            var request = serviceProvider.ReadAccessTokenRequest();
            var response = serviceProvider.PrepareAccessTokenMessage(request);
            serviceProvider.Channel.Respond(response);
        }

        //
        // POST: /oauth/authorize_consumer
        /// <summary>
        /// Form callback used when a user authorizes a consumer.
        /// 
        /// Some data is stored in session using the `consumer_id` as key.
        /// </summary>
        /// <param name="consumer_id"></param>
        /// <param name="authorization_secret"></param>
        public void AuthorizeConsumer(long consumer_id, string authorization_secret)
        {
            var request = oAuthComponent.ParseSession(Session, consumer_id, authorization_secret);
            var msg = (ITokenContainingMessage)request;
            var token = MvcApplication.Collections.GetTokenFromToken(msg.Token);
            try
            {
                oAuthComponent.HandleConsumerAuthorization(request, token);
            }
            catch (InvalidOperationException ex)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, ex.Message);
            }
        }

        //
        // POST: /oauth/authorize
        /// <summary>
        /// Authorization endpoint for 3-legged requests.
        /// If the consumer is pre-authorized, the user is promtply redirected.
        /// </summary>
        /// <returns></returns>
        public ActionResult Authorize()
        {
            UserAuthorizationRequest request = null;
            var token = oAuthComponent.ParseAuthorizationRequest(ref request);

            if (token.Consumer.PreAuthorized)
            {
                try
                {
                    oAuthComponent.HandleConsumerAuthorization(request, token);
                }    
                catch (InvalidOperationException ex)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, ex.Message);
                }
                return null;
            }

            var authorization_secret = oAuthComponent.GenerateAuthorizationSecret();
            var secret_key = String.Format("{0}.AuthorizationSecret", token.Consumer.Id);
            var request_key = String.Format("{0}.request", token.Consumer.Id);

            Session[secret_key] = authorization_secret;
            Session[request_key] = request;
            ViewBag.ConsumerId = token.Consumer.Id;
            ViewBag.AuthorizationSecret = authorization_secret;
                
            return View();
            
        }

        //
        // GET: /oauth/request_token
        /// <summary>
        /// Handles unauthenticated token requests.
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestToken()
        {
            return oAuthComponent.ProcessTokenRequest();
        }

        /// <summary>
        /// Auto-describe the oauth service endpoints.
        /// </summary>
        /// <returns></returns>
        public JsonResult Description()
        {
            var description = serviceProvider.ServiceDescription;
            return Json(description, JsonRequestBehavior.AllowGet);
        }
    }
}
