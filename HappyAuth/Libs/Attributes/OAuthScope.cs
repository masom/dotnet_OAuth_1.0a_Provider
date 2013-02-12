using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyAuth.Libs.Attributes
{
    public class OAuthScope : Attribute
    {
        private string _scope;
        public string Scope
        {
            get { return _scope; }
        }

        public OAuthScope()
        {
            _scope = null;
        }

        public OAuthScope(string scope)
        {
            _scope = scope;
        }
    }
}