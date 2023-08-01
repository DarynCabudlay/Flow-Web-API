using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class WorkFlowApprovalSetup
    {
        public int Id { get; set; }
        public int OrganizationalStructureId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public int UpToApprovalLevelId { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }
        public virtual ApprovalLevel UpToApprovalLevel { get; set; }
    }
}
