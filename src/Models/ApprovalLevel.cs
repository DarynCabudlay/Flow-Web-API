using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class ApprovalLevel
    {
        public ApprovalLevel()
        {
            ApprovalLevelDetails = new HashSet<ApprovalLevelDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationEntityId { get; set; }
        public virtual OrganizationEntity OrganizationEntity { get; set; }
        public virtual ICollection<ApprovalLevelDetail> ApprovalLevelDetails { get; set; }
    }
}
