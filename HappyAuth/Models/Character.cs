using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyAuth.Models
{
    /// <summary>
    /// A angry character. It is angry because it cannot be publicly accessed...
    /// </summary>
    [Serializable]
    public class Character
    {
        public long Id;
        public User Owner;
        public String Name;
        public String Locale;
        public Int16 Age;
    }
}