using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyAuth.Models
{
    /// <summary>
    /// A really happy User...
    /// </summary>
    public class User
    {
        public long Id { get; set; }
        public String Username { get; set;}
        public String Password { get; set; }
    }
}