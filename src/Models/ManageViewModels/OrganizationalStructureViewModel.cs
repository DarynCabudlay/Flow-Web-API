using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class OrganizationalStructureViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Parent { get; set; }
        public string ParentOrganizationEntityName { get; set; }
        public int ParentOrganizationEntityHierarchy { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntityName { get; set; }
        public int OrganizationEntityHierarchy { get; set; }
        public bool OrganizationEntityPublished { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public OrganizationEntityViewModel OrganizationEntity { get; set; }
        public List<UserOrganizationalStructureViewModel> UserOrganizationalStructures { get; set; }
    }

    public class OrganizationalStructureTabularViewModel
    {
        public List<string> ColumnHeaders { get; set; }
        public DataTable Details { get; set; }
    }


    public class OrganizationalStructureWithUserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntityName { get; set; }
        public int OrganizationEntityHierarchy { get; set; }
        public int UserOrgStructureId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int ApprovalLevelDetailId { get; set; }
        public ApprovalLevelDetailViewModel ApprovalLevel { get; set; }
        public int ApprovalLevelDetailRowNumber { get; set; }
        public int ReportingTo { get; set; }
        public ApprovalLevelDetailViewModel ReportingToApprovalLevel { get; set; }
        public int ReportingToRowNumber { get; set; }
        public bool Published { get; set; }
    }
}
