using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class UserOrganizationalStructure
    {
        public UserOrganizationalStructure()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public int OrganizationalStructureId { get; set; }
        public int ApprovalLevelDetailId { get; set; }
        public int ReportingTo { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
