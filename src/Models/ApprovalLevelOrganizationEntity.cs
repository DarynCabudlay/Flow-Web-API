using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class ApprovalLevelOrganizationEntity
    {
        public int Id { get; set; }
        public int ApprovalLevelId { get; set; }
        public int OrganizationEntityId { get; set; }
        public virtual ICollection<ApprovalLevel> ApprovalLevels { get; set; }
        public virtual ICollection<OrganizationEntity> OrganizationEntities { get; set; }
    }
}
