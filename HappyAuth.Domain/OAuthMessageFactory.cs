using System.Collections.Generic;
using System.Linq;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;

namespace HappyAuth.Domain
{
    /// <summary>
    /// A custom class that will cause the OAuth library to use our custom message types
    /// where we have them.
    /// </summary>
    public class OAuthMessageFactory : OAuthServiceProviderMessageFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthMessageFactory"/> class.
        /// </summary>
        /// <param name="tokenManager">The token manager instance to use.</param>
        public OAuthMessageFactory(IServiceProviderTokenManager tokenManager)
            : base(tokenManager)
        {
        }

        /// <summary>
        /// Called by DotNetOpenAuth when parsing new incoming messages.
        /// Overloads the base OAuthServiceProviderMessageFactory function.
        /// 
        /// If the field count is 0, null is returned to handle cases of not sending any OAuth data.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public override IDirectedProtocolMessage GetNewRequestMessage(MessageReceivingEndpoint recipient, IDictionary<string, string> fields)
        {
            //This might be wrong but otherwise the whole api fails to return a null value if there is no oauth signature.
            if (!fields.Any())
            {
                return null;
            }

            var message = base.GetNewRequestMessage(recipient, fields);

            // inject our own type here to replace the standard one
            if (message is UnauthorizedTokenRequest)
            {
                message = new RequestScopedTokenMessage(recipient, message.Version);
            }
            return message;
        }
    }
}