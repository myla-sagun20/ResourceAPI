using System;
using System.Collections.Generic;

#nullable disable

namespace VASAPI_Azure.Models
{
    public partial class LoginUser
    {
        public LoginUser()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
