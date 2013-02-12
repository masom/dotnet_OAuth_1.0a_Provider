using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.Messages;
using HappyAuth.Libs.Attributes;

namespace HappyAuth.Libs
{
    /// <summary>
    /// Handles OAuth authentication against a resource. Can be a Controller or an Action.
    /// </summary>
    public class OAuthAuthorizationManager : ActionFilterAttribute
    {
        /// <summary>
        /// Determine if the OAuth authentication is enforced.
        /// </summary>
        private readonly bool _enforce;

        /// <param name="enforce">
        ///     Determine if the OAuth authentication is enforced.
        ///     If set to false and the OAuth data is not present in a request, the access to the resource will be allowed.
        /// </param>
        public OAuthAuthorizationManager(bool enforce = true)
        {
            _enforce = enforce;
        }

        /// <summary>
        /// If OAuth authentication data is provided, perform access scope validation on the resource being accessed.
        /// </summary>
        /// <param name="context"></param>
        public override void  OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var serviceProvider = OAuthServiceProvider.Create();

            var auth = Authenticate(serviceProvider, request);
            if (auth == null)
            {
                // No authentication provided.

                if (_enforce)
                {
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "Requires OAuth 1.0a");
                }
                
                return;
            }

            var accessToken = MvcApplication.Collections.GetTokenFromToken(auth.AccessToken);
            var scopeIsAuthorized = AuthorizedToAccessScope(context, accessToken.ScopeAsList);

            if (!scopeIsAuthorized)
            {
                throw new HttpException((int) HttpStatusCode.Unauthorized, "Not Authorized");
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Validates a consumer is authorized to access a resource based off it's allowed scope.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clientScopes"></param>
        /// <returns></returns>
        private bool AuthorizedToAccessScope(ActionExecutingContext context, String[] clientScopes)
        {
            var controllerScope = GetScopeFromType(context.Controller.GetType());
            var actionScope = GetScopeFromType(context.ActionDescriptor.GetType());

            return EvaluateScope(controllerScope, clientScopes) && EvaluateScope(actionScope, clientScopes);
        }

        /// <summary>
        /// Evaluates a provided <see cref="OAuthScope"/> and determine if it is available to the provided scopes.
        /// </summary>
        /// <param name="scope"><see cref="OAuthScope"/> instance being evaluated</param>
        /// <param name="consumerScopes">List of scopes the consumer has access to.</param>
        /// <returns>True if the consumer has access to the scope.</returns>
        private bool EvaluateScope(OAuthScope scope, IEnumerable<string> consumerScopes)
        {
            if (scope == null)
            {
                return true;
            }

            if (scope.Scope == null)
            {
                return true;
            }
            return consumerScopes.Contains(scope.Scope);
        }

        /// <summary>
        /// Introspect a given type and return an <see cref="OAuthScope"/> if any.
        /// </summary>
        /// <param name="type"><see cref="System.Type"/></param>
        /// <returns><see cref="OAuthScope"/> or null</returns>
        private OAuthScope GetScopeFromType(Type type)
        {
            var scope = type.GetCustomAttributes(typeof(OAuthScope), true);
            if (!scope.Any())
            {
                return null;
            }

            // This could easily return a list of OAuthScope.
            return (OAuthScope)scope.First();
        }

        /// <summary>
        /// Authenticate a request against a service provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        private AccessProtectedResourceRequest Authenticate(ServiceProvider provider, HttpRequestBase httpRequest)
        {
            AccessProtectedResourceRequest auth = null;
            try
            {
                auth = provider.ReadProtectedResourceAuthorization(httpRequest);
            }
            catch (ProtocolException ex)
            {
                //TODO Handle this
                throw;
            }

            return auth;
        }
    }
}