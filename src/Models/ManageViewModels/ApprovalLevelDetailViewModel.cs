using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class ApprovalLevelDetailViewModel
    {
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public int ApprovalLevelId { get; set; }
        public string ApprovalLevel { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntityName { get; set; }
        public int OrganizationEntityHierarchy { get; set; }
        public bool OrganizationEntityPublished { get; set; }
        public int Sequence { get; set; }
        public string ApprovalLevelAndSequence { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public OrganizationEntityViewModel OrganizationEntity { get; set; }
    }

    public class ApprovalLevelDetailWithUserViewModel
    {
        public int Id { get; set; }
        public int ApprovalLevelId { get; set; }
        public string ApprovalLevel { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntityName { get; set; }
        public int OrganizationEntityHierarchy { get; set; }
        public int Sequence { get; set; }
        public string ApprovalLevelAndSequence { get; set; }
        public bool Published { get; set; }
        public UserBasicInfoModel User { get; set; }
    }
}
