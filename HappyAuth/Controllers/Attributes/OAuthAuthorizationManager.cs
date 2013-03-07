using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.Messages;
using HappyAuth.Domain;
using HappyAuth.Domain.Attributes;
using HappyAuth.Domain.Interfaces;

namespace HappyAuth.Controllers.Attributes
{
    /// <summary>
    /// Handles OAuth authentication against a resource. Can be a Controller or an Action.
    /// </summary>
    public class OAuthAuthorizationManager : ActionFilterAttribute, IOAuthScope
    {
        /// <summary>
        /// The RouteData key the current <see cref="OAuthToken"/> will be saved to.
        /// TODO There should be a better place to store this.
        /// </summary>
        public static readonly string TokenRouteKey = "oauth_access_token";

        /// <summary>
        /// Determine if the OAuth authentication is enforced.
        /// </summary>
        private readonly bool _enforce;

        /// <summary>
        /// Defines a required scope to access a resource.
        /// </summary>
        private readonly string _scope;

        public String Scope
        {
            get { return _scope; }
        }

        /// <param name="enforce">
        ///     Determine if the OAuth authentication is enforced.
        ///     If set to false and the OAuth data is not present in a request, the access to the resource will be allowed.
        /// </param>
        /// <param name="scope">A scope needed to access the protected resource.</param>
        public OAuthAuthorizationManager(string scope = "", bool enforce = true)
        {
            _enforce = enforce;
            _scope = String.IsNullOrEmpty(scope) ? null : scope;
        }

        /// <summary>
        /// If OAuth authentication data is provided, perform access scope validation on the resource being accessed.
        /// </summary>
        /// <param name="context"></param>
        public override void  OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var serviceProvider = OAuthServiceProvider.Create(MvcApplication.Collections, MvcApplication.Collections);

            Authenticate(context, serviceProvider, request);
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Validates a consumer is authorized to access a resource based off it's allowed scope.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <param name="clientScopes"></param>
        /// <exception cref="HttpException"></exception>
        private void VerifyScopeAccess(Object user, ActionExecutingContext context, String[] clientScopes)
        {
            var controllerScope = GetScopeFromSubject(context.ActionDescriptor.ControllerDescriptor);
            var actionScope = GetScopeFromSubject(context.ActionDescriptor);

            var evaluatedScopes = new List<bool>
                {
                    // Evaluates the manager's assigned scope.
                    EvaluateScope(user, this, clientScopes),
                    EvaluateScope(user, controllerScope, clientScopes),
                    EvaluateScope(user, actionScope, clientScopes)
                };

            // All scope evaluations must succeed.
            var clientScopesValid = evaluatedScopes.All(b => b);

            if (!clientScopesValid)
            {
                //TODO Log
                throw new HttpException((int)HttpStatusCode.Unauthorized, "Not Authorized");
            }
        }

        /// <summary>
        /// Evaluates a provided <see cref="IOAuthScope"/> and determine if it is available to the provided scopes.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scope"><see cref="IOAuthScope"/> instance being evaluated</param>
        /// <param name="consumerScopes">List of scopes the consumer has access to.</param>
        /// <returns>True if the consumer has access to the scope.</returns>
        private bool EvaluateScope(Object user, IOAuthScope scope, IEnumerable<string> consumerScopes)
        {
            if (scope == null)
            {
                return true;
            }

            if (scope.Scope == null)
            {
                return true;
            }

            /**
             * If the requested scope is not destined to a consumer (no user has authorized access)
             * but the requested resources is scoped differently (requires a user authorization)
             * and there is no user associated with the access token, deny access.
             */
            if (scope.Scope != OAuthScopes.Consumer && user == null)
            {
                //TODO Log
                return false;
            }

            return consumerScopes.Contains(scope.Scope);
        }

        /// <summary>
        /// Introspect a given type and return an <see cref="OAuthScope"/> if any.
        /// </summary>
        /// <param name="subject">An object to be inspected for a scope.</param>
        /// <returns>An object implementing <see cref="IOAuthScope"/> or null</returns>
        private IOAuthScope GetScopeFromSubject(ICustomAttributeProvider subject)
        {
            var scope = subject.GetCustomAttributes(typeof(OAuthScope), true);
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
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="httpRequest"></param>
        /// <exception cref="HttpException"></exception>
        /// <returns></returns>
        private void Authenticate(ActionExecutingContext context, ServiceProvider provider, HttpRequestBase httpRequest)
        {
            AccessProtectedResourceRequest auth = null;
            try
            {
                auth = provider.ReadProtectedResourceAuthorization(httpRequest);
            }
            catch (ProtocolException ex)
            {
                //TODO Log error
                throw new HttpException((int) HttpStatusCode.Unauthorized, ex.Message);
            }

            if (auth == null)
            {
                if (_enforce)
                {
                    //TODO Log
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "Requires OAuth 1.0a");
                }
                return;
            }

            var accessToken = MvcApplication.Collections.GetTokenFromToken(auth.AccessToken);
            var user = MvcApplication.Collections.Users.FirstOrDefault(u => u.Id == long.Parse(accessToken.UserId));
            VerifyScopeAccess(user, context, accessToken.ScopeAsList);
            context.RouteData.Values.Add(TokenRouteKey, accessToken);
        }
    }
}