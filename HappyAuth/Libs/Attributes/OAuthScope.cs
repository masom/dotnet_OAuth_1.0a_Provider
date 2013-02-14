using System;

namespace HappyAuth.Libs.Attributes
{
    /// <summary>
    /// Define a required scope to access a resource.
    /// </summary>
    public class OAuthScope : Attribute
    {
        /// <summary>
        /// Scope required to access a resource.
        /// </summary>
        private readonly string _scope;

        public string Scope
        {
            get { return _scope; }
        }

        /// <summary>
        /// Defines a required scope to access a resource.
        /// The `scope` parameter should come from the <see cref="OAuthScopes" /> class constants.
        /// </summary>
        /// <param name="scope">The required scope.</param>
        public OAuthScope(string scope)
        {
            _scope = scope;
        }

    }
}