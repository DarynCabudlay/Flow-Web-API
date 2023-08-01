using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class FileType
    {
        public FileType()
        {
            TicketAttachments = new HashSet<TicketAttachment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
    }
}
