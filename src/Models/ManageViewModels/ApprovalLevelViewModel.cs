using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class ApprovalLevelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntityName { get; set; }
        public int OrganizationEntityHierarchy { get; set; }
        public bool OrganizationEntityPublished { get; set; }
        public OrganizationEntityViewModel OrganizationEntity { get; set; }
        public List<ApprovalLevelDetailViewModel> ApprovalLevelDetails { get; set; }
    }

    public class ApprovalLevelReportingToViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntityName { get; set; }
        public int OrganizationEntityHierarchy { get; set; }
        public UserBasicInfoModel User { get; set; }
        public ApprovalLevelDetailViewModel UserApprovalLevelDetail { get; set; }
    }
}
