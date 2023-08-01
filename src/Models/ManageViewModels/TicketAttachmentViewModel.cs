using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public enum TicketAttachmentCategory
    {
        Ticket = 1,
        TicketRequest = 2,
        WorkFlow = 3
    }

    public class TicketAttachmentViewModel
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; } // Ticket Id or Ticket Request Id, Ticket WorkFlow Id
        public TicketAttachmentCategory Category { get; set; } // Ticket or Ticket Request or WorkFlows
        public string CategoryDesc { get; set; }
        public int FileTypeId { get; set; }
        public string Notes { get; set; }
        public string OriginalFileName { get; set; }
        public string MaskedFileName { get; set; }
        public string FilePath { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public FileTypeViewModel FileType { get; set; }
    }
}
