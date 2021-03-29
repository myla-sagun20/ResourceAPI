using System;
using System.Collections.Generic;

#nullable disable

namespace VASAPI_Azure.Models
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public int LoginuserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual LoginUser Loginuser { get; set; }
    }
}
