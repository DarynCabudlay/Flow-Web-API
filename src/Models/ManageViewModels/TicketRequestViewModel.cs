using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class TicketRequestViewModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int Sequence { get; set; }
        public string Remarks { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public bool WithCheckingOfDetailEntryInDb { get; set; }
        public TicketViewModel Ticket { get; set; }
        public List<TicketRequestDetailViewModel> TicketRequestDetails { get; set; }
        public List<TicketLinkViewModel> TicketLinks { get; set; }
        public List<TicketLinkViewModel> TicketAttachments { get; set; }
    }
}
