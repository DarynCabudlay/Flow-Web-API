using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class Ticket
    {
        public Ticket()
        {
            TicketRequests = new HashSet<TicketRequest>();
            TicketBaseWorkFlows = new HashSet<TicketBaseWorkFlow>();
        }
        public int Id { get; set; }
        public string TicketNo { get; set; }
        public string RushTicketNo { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int Status { get; set; }
        public DateTime StatusDate { get; set; }
        public int? DetailedStatus { get; set; } //Last Activity
        public int? CancellationReason { get; set; } 
        public string CancellationRemarks { get; set; }
        public int UserOrganizationalStructureId { get; set; }
        public string UserOrganizationalStructureName { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> DateSubmitted { get; set; }
        public virtual ICollection<TicketRequest> TicketRequests { get; set; }
        public virtual ICollection<TicketBaseWorkFlow> TicketBaseWorkFlows { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual UserOrganizationalStructure UserOrganizationalStructure { get; set; }
    }
}
