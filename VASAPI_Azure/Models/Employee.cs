using System;
using System.Collections.Generic;

#nullable disable

namespace VASAPI_Azure.Models
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? OnBoardDate { get; set; }
        public DateTime? OffBoardDate { get; set; }
        public int? Status { get; set; }
        public int? Category { get; set; }
        public int? Position { get; set; }
        public string Gender { get; set; }
    }
}
