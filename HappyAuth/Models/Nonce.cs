using System;
using System.Security.Cryptography;

namespace HappyAuth.Models
{
    /// <summary>
    /// A somewhat happy Nonce
    /// 
    /// TODO: Figure out what Nonce stands for...
    /// </summary>
    public class Nonce
    {
        public Nonce(String context, String nonce, DateTime timestamp)
        {
            var key = String.Format("{0}{1}{2}", context, nonce, timestamp);
            var cryptoProvider = new SHA1CryptoServiceProvider();
            var encoder = new System.Text.UnicodeEncoding();
            Hash = cryptoProvider.ComputeHash(encoder.GetBytes(key));
        }

        /// <summary>
        /// Hash of the context + nonce + timestamp.
        /// </summary>
        public byte[] Hash;

        public String Context;
        public String Code;
        public DateTime Timestamp;
    }
}