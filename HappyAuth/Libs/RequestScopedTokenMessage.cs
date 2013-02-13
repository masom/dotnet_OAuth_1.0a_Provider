using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;

namespace HappyAuth.Libs
{
    public class RequestScopedTokenMessage : UnauthorizedTokenRequest
    {
        public RequestScopedTokenMessage(MessageReceivingEndpoint endpoint, Version version)
            : base(endpoint, version)
        {}

        /// <summary>
        /// OAuth authentication mode: Consumer | User
        /// Consumer is "2-legged" aka Client Grants in OAuth 2.0
        /// User is the standard 3-legged OAuth 1.0a
        /// </summary>
        [MessagePart("authmode", IsRequired = false)]
        public string AuthMode { get; set; }

        /// <summary>
        /// Requested scope.
        /// </summary>
        [MessagePart("scope", IsRequired = false)]
        public string Scope { get; set; }
    }
}