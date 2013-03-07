using System;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace HappyAuth.Domain
{
    public static class OAuthServiceProvider
    {
        private static Uri _webroot;

        /// <summary>
        /// Provide the webroot of the application for the current http context.
        /// </summary>
        public static Uri Webroot
        {
            get
            {
                if (_webroot == null)
                {
                    var appPath = HttpContext.Current.Request.ApplicationPath;
                    if (!appPath.EndsWith("/"))
                    {
                        appPath += '/';
                    }
                    _webroot = new Uri(HttpContext.Current.Request.Url, appPath);
                }
                return _webroot;
            }
        }

        /// <summary>
        /// Provide an OAuth service description for the current http context.
        /// </summary>
        public static ServiceProviderDescription SelfDescription
        {
            get
            {
                var accessMessageEndpoint = new Uri(Webroot, "/oauth/access_token");
                var requestMessageEndpoint = new Uri(Webroot, "/oauth/authorize");
                var userMessageEndpoint = new Uri(Webroot, "/oauth/request_token");
                var tamperProtection = new ITamperProtectionChannelBindingElement[]
                {
                    new HmacSha1SigningBindingElement()
                };

                var description = new ServiceProviderDescription
                {
                    ProtocolVersion = ProtocolVersion.V10a,
                    AccessTokenEndpoint = new MessageReceivingEndpoint(accessMessageEndpoint, HttpDeliveryMethods.PostRequest),
                    RequestTokenEndpoint = new MessageReceivingEndpoint(requestMessageEndpoint, HttpDeliveryMethods.PostRequest),
                    UserAuthorizationEndpoint = new MessageReceivingEndpoint(userMessageEndpoint, HttpDeliveryMethods.GetRequest),
                    TamperProtectionElements = tamperProtection
                };
                return description;
            }
        }

        /// <summary>
        /// Creates an instance of a ServiceProvider.
        /// </summary>
        /// <returns></returns>
        public static ServiceProvider Create(INonceStore nonceStore, IServiceProviderTokenManager tokenManager)
        {
            /**
             * The second parameter is the TokenManager
             */
            return new ServiceProvider(SelfDescription,  tokenManager, nonceStore, new OAuthMessageFactory(tokenManager));
        }
    }
}