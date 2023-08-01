using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{

    public enum RequestTypeStatus
    {
        Draft = 1,
        Finalized = 2,
        Active = 3,
        Inactive = 4
    }

    public class RequestTypeStates
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RequestTypeBasicData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class SearchRequestType
    {
        public int Id { get; set; }
        public int Version { get; set; }
    }

    public class RequestTypeDeletionParam
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public int ReasonId { get; set; }
        public string Remarks { get; set; }
    }

    public class SearchRequestForm
    {
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int FieldId { get; set; }
    }

    public class SearchRequestTypeProcessOwners
    {
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
    }
    public class FieldSequenceSwitchViewModel
    {
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int SequenceFrom { get; set; }
        public int SequenceTo { get; set; }
    }

    public class RequestTypeViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int RequestCategoryId { get; set; }
        public string RequestCategory { get; set; }
        public bool RequestCategoryPublished { get; set; }
        [Required]
        public int Version { get; set; }
        public int Version2 { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RequestTypeStatus  Status { get; set; }
        public string StatusDesc 
        {
            get
            {
                if (Status == RequestTypeStatus.Active)
                    return "Active";
                else if (Status == RequestTypeStatus.Draft)
                    return "Draft";
                else if (Status == RequestTypeStatus.Finalized)
                    return "Finalized";
                else
                    return "Inactive";
            }
        }
        public string StatusDescForSorting { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public List<RequestFormViewModel> RequestForms { get; set; }
        public List<RequestTypeProcessOwnersViewModel> ProcessOwners { get; set; }
        public List<LockedRequestTypesViewModel> Locked { get; set; }
        public List<ListData> DsList { get; set; }
        public bool WithCreatedProcessOrStep { get; set; }
        public List<RequestStep> RequestSteps { get; set; }
    }

    public class RequestTypeDisplayViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public int Version { get; set; }
        public List<RequestTypeDisplayViewModel> Items { get; set; }
    }

    public class RequestTypeBaseWorkFlowViewModel
    {
        public int StepId { get; set; }
        public int StepTypeId { get; set; }
        public int AssigneeId { get; set; }
        public int AssigneeType { get; set; }
        public int RequiredToExecute { get; set; }
        public int TAT { get; set; }
        public string ApplicableButtons { get; set; }
        public List<RequestTypeBaseWorkAssigneeFlowViewModel> BaseWorkAssignees { get; set; }

    }

    public class RequestTypeBaseWorkAssigneeFlowViewModel
    {
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
    }
}
