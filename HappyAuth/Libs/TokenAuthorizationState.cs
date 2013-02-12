using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyAuth.Libs
{
    public enum TokenAuthorizationState : int
    {
        UnauthorizedRequestToken = 0,
        AuthorizedRequestToken = 1,
        AccessToken = 2
    }
}