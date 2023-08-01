using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{

    public enum TicketLinkCategory
    {
        Ticket = 1,
        TicketRequest = 2,
        WorkFlow = 3
    }

    public class TicketLinkViewModel
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; } // Ticket Id or Ticket Request Id, Ticket WorkFlow Id
        public TicketLinkCategory Category { get; set; } // Ticket or Ticket Request or WorkFlows
        public int LinkTypeId { get; set; }
        public string Link { get; set; }
        public string LinkText { get; set; }
        public string Notes { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public LinkTypeViewModel LinkType { get; set; }
    }
}
