using System;
using System.Globalization;
using System.Linq;
using DotNetOpenAuth.OAuth.ChannelElements;
using HappyAuth.Domain.Interfaces;

namespace HappyAuth.Domain
{
    /// <summary>
    /// A Happy OAuth Token
    /// </summary>
    public class OAuthToken : IServiceProviderRequestToken, IServiceProviderAccessToken
    {
        /// <summary>
        /// The URL the provider will redirect the user after they (dis)approved the request.
        /// </summary>
        public Uri Callback;

        /// <summary>
        /// The consumer linked with this token.
        /// </summary>
        public OAuthConsumer Consumer;

        /// <summary>
        /// The token's expiration date/time.
        /// </summary>
        public DateTime? ExpirationDate;

        /// <summary>
        /// The token's issue date/time.
        /// </summary>
        public DateTime IssueDate;

        /// <summary>
        /// A list of roles associated with this token.
        /// </summary>
        public String[] Roles;

        /// <summary>
        /// The requested token scopes.
        /// </summary>
        public String Scope;

        public String ServiceProviderRequestToken;
        public String ServiceProviderAccessToken;

        /// <summary>
        /// The state of the token.
        /// </summary>
        public TokenAuthorizationState State;

        public String TokenSecret;

        /// <summary>
        /// The user linked with this token.
        /// </summary>
        public string UserId;

        /// <summary>
        /// The verification code for this token.
        /// </summary>
        public String VerificationCode;
        public Version Version;

        /// <summary>
        /// Generates a list of scopes associated with this token.
        /// </summary>
        public String[] ScopeAsList
        {
            get
            {
                return Scope.Split(',').Select(s => s.Trim()).ToArray();
            }
        }

        public String Token
        {
            get { return ServiceProviderAccessToken; }
            set
            {
                ServiceProviderAccessToken = value;
                ServiceProviderRequestToken = value;
            }
        }

        public TokenType GetTokenType()
        {
            if (State == TokenAuthorizationState.AccessToken)
            {
                return TokenType.AccessToken;
            }
            return TokenType.RequestToken;
        }

        #region IServiceProviderRequestToken        

        /// <summary>
        /// Authorize the token for a given user.
        /// If the user is null, the token will be authorized but not associated with a user.
        /// </summary>
        /// <param name="userId"></param>
        public void Authorize(string userId)
        {
            if (!State.Equals(TokenAuthorizationState.UnauthorizedRequestToken))
            {
                throw new InvalidOperationException("The token state does not match unauthorized request.");
            }
            State = TokenAuthorizationState.AuthorizedRequestToken;
            UserId = userId;
        }

        String IServiceProviderRequestToken.Token
        {
            get { return ServiceProviderRequestToken; }
        }

        
        Uri IServiceProviderRequestToken.Callback
        {
            get { return Callback; }
            set { Callback = value; }
        }

        String IServiceProviderRequestToken.ConsumerKey
        {
            get { return Consumer.Key; }
        }

        
        Version IServiceProviderRequestToken.ConsumerVersion
        {
            get { return Version; }
            set { Version = value; }
        }

        DateTime IServiceProviderRequestToken.CreatedOn
        {
            get { return IssueDate; }
        }

        
        String IServiceProviderRequestToken.VerificationCode
        {
            get { return VerificationCode; }
            set { VerificationCode = value; }
        }
        #endregion

        #region IServiceProviderAccessToken
        DateTime? IServiceProviderAccessToken.ExpirationDate
        {
            get { return ExpirationDate; }
        }


        /// <summary>
        /// Used to provided Identity roles.
        /// </summary>
        String[] IServiceProviderAccessToken.Roles
        {
            get { return Roles; }
        }

        String IServiceProviderAccessToken.Username
        {
            get { return UserId.ToString(CultureInfo.InvariantCulture); }
        }

        String IServiceProviderAccessToken.Token
        {
            get { return ServiceProviderAccessToken; }
        }

        #endregion


    }
}