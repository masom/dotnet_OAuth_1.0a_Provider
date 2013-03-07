using System;
using HappyAuth.Domain.Interfaces;

namespace HappyAuth.Models
{
    /// <summary>
    /// A really happy User...
    /// </summary>
    [Serializable]
    public class User : IOAuthUser
    {
        public long Id { get; set; }
        public String Username { get; set;}
        public String Password { get; set; }
    }
}