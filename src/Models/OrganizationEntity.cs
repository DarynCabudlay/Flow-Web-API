using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class OrganizationEntity
    {
        public OrganizationEntity()
        {
            OrganizationalStructures = new HashSet<OrganizationalStructure>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Hierarchy { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string WebClass { get; set; }
        public string Color { get; set; }
        public virtual ICollection<OrganizationalStructure> OrganizationalStructures { get; set; }
        public virtual ApprovalLevel ApprovalLevel { get; set; }

    }
}
