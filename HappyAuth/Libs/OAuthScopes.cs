using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyAuth.Libs
{
    /// <summary>
    /// Defines different possible OAuth scopes.
    /// 
    /// Scopes other than Consumer requires the consumer to have a user's authorization.
    /// </summary>
    public static class OAuthScopes
    {
        /// <summary>
        /// The `User` scope gives access to user's data
        /// </summary>
        public const string User = "user";
        /// <summary>
        /// The `UserEdit` scope gives access to editing the associated user.
        /// </summary>
        public const string UserEdit = "user_edit";

        /// <summary>
        /// The `Character` scopes gives access to creating and editing characters.
        /// </summary>
        public const string Character = "character";
        /// <summary>
        /// The `CharacterDelete` scope gives access to deleting characters.
        /// </summary>
        public const string CharacterDelete = "character_delete";

        /// <summary>
        /// The `Consumer` scope is rather pointless.
        /// </summary>
        public const string Consumer = "consumer";
    }
}