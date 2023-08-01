using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; } // Ticket Id or Ticket Request Id, Ticket WorkFlow Id
        public int Category { get; set; } // Ticket or Ticket Request or WorkFlows
        public int FileTypeId { get; set; }
        public string Notes { get; set; }
        public string OriginalFileName { get; set; }
        public string MaskedFileName { get; set; }
        public string FilePath { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual FileType FileType { get; set; }
    }
}
