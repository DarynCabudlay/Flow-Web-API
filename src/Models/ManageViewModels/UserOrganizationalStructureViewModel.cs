using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class UserOrganizationalStructureViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int OrganizationalStructureId { get; set; }
        public string OrganizationalStructure { get; set; }
        public bool OrganizationalStructurePublished { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntityName { get; set; }
        public bool OrganizationEntityPublished { get; set; }
        public int ApprovalLevelDetailId { get; set; }
        public ApprovalLevelDetailViewModel ApprovalLevel { get; set; }
        public int ReportingTo { get; set; }
        public ApprovalLevelDetailViewModel ReportingToApprovalLevel { get; set; }
        public UserOrganizationalStructureViewModel ReportingToUser { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public UserRegisterModel User { get; set; }
    }

    public class SaveUserOrganizationalStructureViewModel
    {
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
    }

    public class UserOrganizationalStructureValidationViewModel
    {
        public string ReturnMessage { get; set; }
        public int ReturnCode { get; set; }
    }

    public class SaveUserOrganizationalStructureWithDBValidationViewModel
    {
        public bool WithCheckingOfEntryInDb { get; set; }
        public List<SaveUserOrganizationalStructureViewModel> UserOrganizationalStructures { get; set; }
    }

    public class UserWithReportingToOrganizationalStructureViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int UserOrganizationalStructureId { get; set; }
        public string UserOrganizationalStructure { get; set; }
        public int UserOrganizationEntityId { get; set; }
        public string UserOrganizationEntityName { get; set; }
        public int UserApprovalLevelDetailId { get; set; }
        public ApprovalLevelDetailViewModel UserApprovalLevel { get; set; }
        public int ReportingToId { get; set; }
        public string ReportingToUserId { get; set; }
        public string ReportingToFirstName { get; set; }
        public string ReportingToLastName { get; set; }
        public string ReportingToFullName { get; set; }
        public int ReportingToOrganizationalStructureId { get; set; }
        public string ReportingToOrganizationalStructure { get; set; }
        public int ReportingToOrganizationEntityId { get; set; }
        public string ReportingToOrganizationEntityName { get; set; }
        public int ReportingToApprovalLevelDetailId { get; set; }
        public ApprovalLevelDetailViewModel ReportingToApprovalLevel { get; set; }
        public bool Published { get; set; }
    }
}
