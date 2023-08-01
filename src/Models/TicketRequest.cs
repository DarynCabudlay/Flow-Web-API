using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class TicketRequest
    {
        public TicketRequest()
        {
            TicketRequestDetails = new HashSet<TicketRequestDetail>();
        }

        public int Id { get; set; }
        public int TicketId { get; set; }
        public int Sequence { get; set; }
        public string Remarks { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ICollection<TicketRequestDetail> TicketRequestDetails { get; set; }
    }
}
