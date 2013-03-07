using System;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;

namespace HappyAuth.Domain
{
    /// <summary>
    /// Custom OAuth request message used to pass a few custom attributes.
    /// </summary>
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
        /// Allows a client to directly submit a user's password.
        /// This kindof defeat the point of OAuth but it is quite useful for your own clients.
        /// </summary>
        [MessagePart("password", IsRequired = false)]
        public string Password { get; set; }

        /// <summary>
        /// Requested scope.
        /// </summary>
        [MessagePart("scope", IsRequired = false)]
        public string Scope { get; set; }
    }
}