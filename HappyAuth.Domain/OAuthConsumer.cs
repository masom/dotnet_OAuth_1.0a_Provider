using System;
using System.Security.Cryptography.X509Certificates;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace HappyAuth.Domain
{
    /// <summary>
    /// Implements a happy OAuth Consumer representation.
    /// </summary>
    public class OAuthConsumer : IConsumerDescription
    {
        public long Id;
        public string Name;
        public bool PreAuthorized;

        public Uri Callback;

        Uri IConsumerDescription.Callback
        {
            get { return Callback; }
        }

        public X509Certificate2 Certificate;

        X509Certificate2 IConsumerDescription.Certificate
        {
            get { return Certificate; }
        }

        public String Key;
        String IConsumerDescription.Key
        {
            get { return Key; }
        }

        public String Secret;
        String IConsumerDescription.Secret
        {
            get { return Secret; }
        }

        public VerificationCodeFormat VerificationCodeFormat;
        VerificationCodeFormat IConsumerDescription.VerificationCodeFormat
        {
            get { return VerificationCodeFormat; }
        }

        public int VerificationCodeLength;
        int IConsumerDescription.VerificationCodeLength
        {
            get { return VerificationCodeLength; }
        }

    }
}