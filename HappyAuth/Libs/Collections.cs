using System;
using System.Collections.Generic;
using System.Linq;
using DotNetOpenAuth.OAuth.Messages;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth.ChannelElements;
using HappyAuth.Models;

namespace HappyAuth.Libs
{
    public class Collections : INonceStore, IServiceProviderTokenManager
    {

        public List<Character> Characters { get; set; }
        public List<OAuthConsumer> Consumers { get; set; }
        public List<OAuthToken> Tokens { get; set; }
        public List<Nonce> Nonces { get; set; }
        public List<User> Users { get; set; }

        public Collections()
        {
            Characters = new List<Character>();
            Consumers = new List<OAuthConsumer>();
            Nonces = new List<Nonce>();
            Tokens = new List<OAuthToken>();
            Users = new List<User>();
        }

        public OAuthToken GetTokenFromToken(string access_token)
        {
            if (String.IsNullOrEmpty(access_token))
            {
                throw new ArgumentException("access_token cannot be empty");
            }

            return Tokens.FirstOrDefault(t => t.Token.Equals(access_token));
        }

        public void AuthorizeRequestToken(OAuthToken token, User user)
        {
            if (!token.State.Equals(TokenAuthorizationState.UnauthorizedRequestToken))
            {
                throw new Exception("The token state does not match unauthorized request.");
            }

            token.Authorize(user);
        }

        #region INonceStore
        bool INonceStore.StoreNonce(string context, string nonce, DateTime timestamp)
        {
            var curNonce = new Nonce(context, nonce, timestamp);
            var exists = Nonces.FirstOrDefault(n => n.Hash.Equals(curNonce.Hash));
            if (exists != null)
            {
                return false;
            }
            Nonces.Add(curNonce);
            return true;
        }
        #endregion

        #region IServiceProviderTokenManager
        IServiceProviderAccessToken IServiceProviderTokenManager.GetAccessToken(string token)
        {
            var entity = Tokens.FirstOrDefault(t => t.Token.Equals(token) && t.State.Equals(TokenAuthorizationState.AccessToken));
            if (entity == null)
            {
                throw new KeyNotFoundException();
            }
            return entity;
        }

        bool IServiceProviderTokenManager.IsRequestTokenAuthorized(string token)
        {
            var entity = Tokens.SingleOrDefault(t => t.Token.Equals(token) && t.State.Equals(TokenAuthorizationState.AuthorizedRequestToken));
            return entity != null;
        }

        IConsumerDescription IServiceProviderTokenManager.GetConsumer(string key)
        {
            var consumer = Consumers.FirstOrDefault(c => c.Key.Equals(key));
            if (consumer == null)
            {
                throw new KeyNotFoundException();
            }

            return consumer;
        }

        IServiceProviderRequestToken IServiceProviderTokenManager.GetRequestToken(string request_token)
        {
            var token = Tokens.FirstOrDefault(t => t.Token == request_token && t.State != TokenAuthorizationState.AccessToken);
            if (token == null)
            {
                throw new KeyNotFoundException();
            }
            return token;
        }

        void IServiceProviderTokenManager.UpdateToken(IServiceProviderRequestToken token)
        {
            var entity = Tokens.SingleOrDefault(t => t.Token.Equals(token.Token));
            if (entity == null)
            {
                throw new KeyNotFoundException();
            }

            entity.VerificationCode = token.VerificationCode;
            entity.Callback = token.Callback;
            entity.Version = token.ConsumerVersion;
            entity.Token = token.Token;
            entity.VerificationCode = token.VerificationCode;
        }

        #endregion

        #region ITokenManager
        TokenType ITokenManager.GetTokenType(string token)
        {
            var entity = Tokens.SingleOrDefault(t => t.Token.Equals(token));
            if (entity == null)
            {
                return TokenType.InvalidToken;
            }

            if (entity.State == TokenAuthorizationState.AccessToken)
            {
                return TokenType.AccessToken;
            }

            return TokenType.RequestToken;
        }

        string ITokenManager.GetTokenSecret(string token)
        {
            var entity = Tokens.SingleOrDefault(t => t.Token.Equals(token));
            if (entity == null)
            {
                throw new KeyNotFoundException();
            }
            return entity.TokenSecret;
        }

        void ITokenManager.StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            var scopedRequest = (RequestScopedTokenMessage) request;
            var consumer = Consumers.Single(c => c.Key.Equals(request.ConsumerKey));
            var token = new OAuthToken
            {
                Consumer = consumer,
                Token = response.Token,
                TokenSecret = response.TokenSecret,
                IssueDate = DateTime.UtcNow,
                Scope = scopedRequest.Scope
            };

            Tokens.Add(token);
        }

        void ITokenManager.ExpireRequestTokenAndStoreNewAccessToken(string consumer_key, string request_token, string access_token, string access_token_secret)
        {
            var consumer = Consumers.Single(c => c.Key.Equals(consumer_key));
            var token = Tokens.Single(t => t.Token.Equals(request_token) && t.Consumer.Equals(consumer));
            
            if (token.State != TokenAuthorizationState.AuthorizedRequestToken)
            {
                throw new InvalidOperationException("The token should already be authorized.");
            }

            token.IssueDate = DateTime.Now;
            token.State = TokenAuthorizationState.AccessToken;
            token.Token = access_token;
            token.TokenSecret = access_token_secret;
        }
        #endregion


    }
}