using System;
using System.Collections.Generic;

#nullable disable

namespace VASAPI_Azure.Models
{
    public partial class Wotype
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? WopriorityId { get; set; }
    }
}
