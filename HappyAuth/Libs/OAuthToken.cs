using System;
using System.Linq;
using DotNetOpenAuth.OAuth.ChannelElements;
using HappyAuth.Models;

namespace HappyAuth.Libs
{
    /// <summary>
    /// A Happy OAuth Token
    /// </summary>
    public class OAuthToken : IServiceProviderRequestToken, IServiceProviderAccessToken
    {
        
        public Uri Callback;
        public OAuthConsumer Consumer;
        public DateTime? ExpirationDate;
        public DateTime IssueDate;
        public String[] Roles;
        public String Scope;
        public String ServiceProviderRequestToken;
        public String ServiceProviderAccessToken;
        public TokenAuthorizationState State;

        public String TokenSecret;
        public User User;
        public String VerificationCode;
        public Version Version;

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
        /// <param name="user"></param>
        public void Authorize(User user)
        {
            Authorize();
            User = user;
        }

        protected void Authorize()
        {
            if (!State.Equals(TokenAuthorizationState.UnauthorizedRequestToken))
            {
                throw new Exception("The token state does not match unauthorized request.");
            }
            State = TokenAuthorizationState.AuthorizedRequestToken;
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

        String[] IServiceProviderAccessToken.Roles
        {
            get { return Roles; }
        }

        String IServiceProviderAccessToken.Username
        {
            get { return User.Username; }
        }

        String IServiceProviderAccessToken.Token
        {
            get { return ServiceProviderAccessToken; }
        }

        #endregion


    }
}