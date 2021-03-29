using System;
using System.Collections.Generic;

#nullable disable

namespace VASAPI_Azure.Models
{
    public partial class WorkOrder
    {
        public int WorkOrderNo { get; set; }
        public int? WopriorityId { get; set; }
        public int? WotypeId { get; set; }
        public int? WostatusId { get; set; }
        public int? Requestor { get; set; }
        public int? Location { get; set; }
        public DateTime? DateAssigned { get; set; }
        public DateTime? DateDue { get; set; }
        public int? Technician { get; set; }
        public decimal? EstimatedHours { get; set; }
        public int? AuthorizedBy { get; set; }
        public string Description { get; set; }
    }
}
