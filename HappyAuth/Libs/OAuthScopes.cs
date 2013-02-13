using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyAuth.Libs
{
    public static class OAuthScopes
    {
        public const string User = "user";
        public const string UserEdit = "user_edit";
        public const string UserDelete = "user_delete";
        public const string UserCreate = "user_create";

        public const string Consumer = "consumer";
    }
}