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
        {
        }

        [MessagePart("scope", IsRequired = true)]
        public string Scope { get; set; }
    }
}