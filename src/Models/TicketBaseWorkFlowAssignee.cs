using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class TicketBaseWorkFlowAssignee
    {
        public int Id { get; set; }
        public int TicketBaseWorkFlowId { get; set; }
        public int? OrganizationalEntityId { get; set; }
        public string OrganizationalEntity { get; set; }
        public int? OrganizationalEntityHierarchy { get; set; }
        public int? OrganizationalStructureId { get; set; }
        public string OrganizationalStructure { get; set; }
        public int? ApprovalLevelDetailId { get; set; }
        public string ApprovalLevelDetail { get; set; }
        public int? ApprovalLevelId { get; set; }
        public string ApprovalLevel { get; set; }
        public string UserId { get; set; }
        public int? SortOrder { get; set; }
        public virtual TicketBaseWorkFlow TicketBaseWorkFlow { get; set; }
    }
}
