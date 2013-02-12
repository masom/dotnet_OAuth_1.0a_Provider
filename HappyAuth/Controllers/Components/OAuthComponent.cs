using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;
using DotNetOpenAuth.OAuth;
using HappyAuth.Libs;
using System.Security.Cryptography;

namespace HappyAuth.Controllers.Components
{
    public class OAuthComponent
    {
        private static readonly RandomNumberGenerator CryptoRandomDataGenerator = new RNGCryptoServiceProvider();
        private readonly ServiceProvider serviceProvider;

        public OAuthComponent(ServiceProvider provider)
        {
            serviceProvider = provider;
        }

        /// <summary>
        /// Authorizes a consumer.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        public void HandleConsumerAuthorization(UserAuthorizationRequest request, OAuthToken token)
        {
            var user = MvcApplication.Collections.Users.First();
            MvcApplication.Collections.AuthorizeRequestToken(token, user);
            var response = serviceProvider.PrepareAuthorizationResponse(request);
            if (response == null)
            {
                //Something weird happened...
                throw new HttpException((int)HttpStatusCode.BadRequest, "Something weird happened");
            }

            serviceProvider.Channel.Respond(response);
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
            byte[] randomData = new byte[8];
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
        /// <returns></returns>
        public OAuthToken ParseAuthorizationRequest(ref UserAuthorizationRequest request)
        {
            request = serviceProvider.ReadAuthorizationRequest();
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
        public void ProcessTokenRequest()
        {
            UnauthorizedTokenRequest request;
            try
            {
                request = serviceProvider.ReadTokenRequest();
            }
            catch (ProtocolException ex)
            {
                throw;
            }
            var response = serviceProvider.PrepareUnauthorizedTokenMessage(request);
            serviceProvider.Channel.Respond(response);
        }
    }
}