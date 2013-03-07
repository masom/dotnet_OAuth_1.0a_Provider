using System;
using HappyAuth.Domain.Interfaces;

namespace HappyAuth.Domain.Attributes
{
    /// <summary>
    /// Define a required scope to access a resource.
    /// </summary>
    public class OAuthScope : Attribute, IOAuthScope
    {
        /// <summary>
        /// Scope required to access a resource.
        /// </summary>
        private readonly string _scope;

        /// <summary>
        /// The required scope to access a resource.
        /// </summary>
        public string Scope
        {
            get { return _scope; }
        }

        /// <summary>
        /// Defines a required scope to access a resource.
        /// The `scope` parameter should come from the <see cref="OAuthScopes" /> class constants.
        /// </summary>
        /// <param name="scope">The required scope to access a resource.</param>
        public OAuthScope(string scope)
        {
            _scope = scope;
        }

    }
}