using System;
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
                return Scope.Split('|');
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

        public void Authorize(User user)
        {
            State = TokenAuthorizationState.AuthorizedRequestToken;
            User = user;
        }

        #region IServiceProviderRequestToken        

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