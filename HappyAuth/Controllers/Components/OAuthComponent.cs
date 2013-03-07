using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;
using DotNetOpenAuth.OAuth;
using HappyAuth.Controllers.Attributes;
using HappyAuth.Domain;
using System.Security.Cryptography;

namespace HappyAuth.Controllers.Components
{
    public class OAuthComponent
    {
        private static readonly RandomNumberGenerator CryptoRandomDataGenerator = new RNGCryptoServiceProvider();
        private readonly ServiceProvider _serviceProvider;

        public OAuthComponent()
        {}

        public OAuthComponent(ServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        /// <summary>
        /// Authorizes a consumer.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <exception cref="HttpException"></exception>
        public void HandleConsumerAuthorization(UserAuthorizationRequest request, OAuthToken token)
        {
            var user = MvcApplication.Collections.Users.First();


            MvcApplication.Collections.AuthorizeRequestToken(token, user);


            var response = _serviceProvider.PrepareAuthorizationResponse(request);
            if (response == null)
            {
                //Something weird happened...
                throw new HttpException((int)HttpStatusCode.BadRequest, "Something weird happened");
            }

            _serviceProvider.Channel.Respond(response);
        }

        /// <summary>
        /// Generates an authorization secret to be used with the "Authorize this application" page.
        /// </summary>
        /// <returns></returns>
        public string GenerateAuthorizationSecret()
        {
            // Generate an unpredictable secret that goes to the user agent and must come back
            // with authorization to guarantee the user interacted with this page rather than
            // being scripted by an evil Consumer.
            var randomData = new byte[8];
            CryptoRandomDataGenerator.GetBytes(randomData);
            var authorization_secret = Convert.ToBase64String(randomData);
            return authorization_secret;
        }

        /// <summary>
        /// Parse the current user's session and verifies a the request is valid (not spoofed)
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="consumer_id"></param>
        /// <param name="authorization_secret"></param>
        /// <exception cref="HttpException"></exception>
        /// <returns></returns>
        public UserAuthorizationRequest ParseSession(HttpSessionStateBase Session, long consumer_id, string authorization_secret)
        {
            var secret_key = String.Format("{0}.AuthorizationSecret", consumer_id);
            var request_key = String.Format("{0}.request", consumer_id);
            if (Session[secret_key] == null || Session[request_key] == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Nice try.");
            }

            var secret = (string)Session[secret_key];
            var request = (UserAuthorizationRequest)Session[request_key];
            if (!secret.Equals(authorization_secret))
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid secret");
            }

            return request;
        }

        /// <summary>
        /// Parse an authorization request, modifying the provided <see cref="UserAuthorizationRequest"/> object.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="HttpException"></exception>
        /// <returns></returns>
        public OAuthToken ParseAuthorizationRequest(ref UserAuthorizationRequest request)
        {
            request = _serviceProvider.ReadAuthorizationRequest();
            if (request == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Missing authorization request");
            }

            var pending = (ITokenContainingMessage)request;
            if (pending.Token == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Missing authorization request");
            }

            var token = MvcApplication.Collections.GetTokenFromToken(pending.Token);
            return token;
        }

        /// <summary>
        /// Process a token request and generates a response.
        /// </summary>
        /// <exception cref="HttpException"></exception>
        public ActionResult ProcessTokenRequest()
        {
            UnauthorizedTokenRequest tokenRequest;
            try
            {
                tokenRequest = _serviceProvider.ReadTokenRequest();
            }
            catch (ProtocolException ex)
            {
                const string template = "Invalid authentication data. Reason: {0}";
                throw new HttpException((int) HttpStatusCode.Unauthorized, String.Format(template, ex.Message));
            }

            var tokenResponse = _serviceProvider.PrepareUnauthorizedTokenMessage(tokenRequest);
            var response = _serviceProvider.Channel.PrepareResponse(tokenResponse);

            if (((RequestScopedTokenMessage) tokenRequest).AuthMode == "consumer")
            {
                var issuedToken = ((ITokenContainingMessage)tokenResponse).Token;
                var token = MvcApplication.Collections.GetTokenFromToken(issuedToken);

                //Authorize the reques token but do not associate any users with it.
                MvcApplication.Collections.AuthorizeRequestToken(token, null);
            }

            //TODO If the client is pre-authorized, should we just authorize here or redirect?
            //Pre-authorizing here kinda breaks the oauth flow but pre-authorized clients is not normal to begin with.

            return response.AsActionResult();
        }

        /// <summary>
        /// Extract the user an OAuth request is made on the behalf of.
        /// 
        /// TODO This should probably be moved to a extension method OR some way to dynamically assign.
        /// </summary>
        /// <param name="routeData">Controller's <see cref="RouteData"/> possibly containing an <see cref="OAuthToken"/>.</param>
        /// <returns>The <see cref="Models.User"/> this request is being made on the behalf of.</returns>
        public string GetUser(RouteData routeData)
        {
            if (!routeData.Values.Keys.Contains(OAuthAuthorizationManager.TokenRouteKey))
            {
                return null;
            }

            var accessToken = routeData.Values[OAuthAuthorizationManager.TokenRouteKey] as OAuthToken;
            if (accessToken == null)
            {
                return null;
            }

            //AccessToken should really only contain a user id instead of a pure object reference.
            return accessToken.UserId;
        }
    }
}