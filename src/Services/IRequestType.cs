using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IRequestType : IGenericMethod<RequestTypeViewModel>
    {
        Task<IEnumerable<RequestFormViewModel>> GetFields(int requestTypeId = 0, int version = 0, bool? isLov = null, bool? isActiveLovOnly = null);
        IEnumerable<RequestTypeStates> GetRequestTypeStatuses();
        IQueryable<RequestTypeViewModel> GetRequestTypes(PagingRequest paging = null, int status = 0, bool? requestCategoryPublished = null);
        Task<IEnumerable<RequestFormControlsViewModel>> GetRequestForms(int requestTypeId, int version, bool? published = null, bool? activeLOVs = null);
        Task<IEnumerable<LockedRequestTypesViewModel>> GetLockedRequestTypes();
        Task<LockedRequestTypesViewModel> GetLockedRequestType(int requestTypeId, int version);
        IQueryable<RequestTypeProcessOwnersViewModel> GetRequestProcessOwners(PagingRequest paging = null, int requestTypeId = 0, int version = 0);
        Task<IEnumerable<RequestStepViewModel>> GetRequestSteps(int requestTypeId, int version, bool withDetails = true);
        Task<IEnumerable<WorkFlowStepTypeViewModel>> GetStepTypes();
        Task<IEnumerable<ComparisonOperatorViewModel>> GetApplicableComparisonOperators(int fieldId);
        Task<IEnumerable<RequestStepViewModel>> GetApplicableWorkFlowNextStep(int requestTypeId, int version, int stepId);
        Task<IEnumerable<ConditionSourceViewModel>> GetStepsConditionSources();
        Task SaveRequestForm(RequestFormViewModel data);
        Task SaveRequestTypeProcessOwners(RequestTypeProcessOwnersViewModel data);
        Task SaveRequestStep(SaveRequestStepViewModel data);
        Task SaveRequestTypeWorkflow(SaveRequestTypeWorkFlowViewModel data);
        Task LockRequestType(LockedRequestTypesViewModel data);
        Task DeleteRequestForm(int requestTypeId, int version, int fieldId = 0);
        Task DeleteRequestTypeProcessOwner(int id);
        Task DeleteLockedRequestType(int requestTypeId, int version);
        Task DeleteRequestTypeWorkFlow(int id = 0, int requesttypeid = 0, int version = 0);
        Task DeleteRequestStepAssignees(int id, int detailId = 0);
        Task DeleteRequestStepConditions(int id);
        Task DeleteRequestStep(int requestTypeId, int version, int stepId = 0);
        PagingResponse<RequestTypeViewModel> PagingFeature(IQueryable<RequestTypeViewModel> query, PagingRequest paging, object id = null);
        PagingResponse<RequestTypeProcessOwnersViewModel> ProcessOwnersPagingFeatureProcessOwners(IQueryable<RequestTypeProcessOwnersViewModel> query, PagingRequest paging);
        Task SwithFieldSequence(FieldSequenceSwitchViewModel data);
        Task<StepFlowViewModel> GetRequestTypeProcessWorkFlows(int requestTypeId, int version);
        bool CheckFieldIsInvolvedInProcess(int requestTypeId, int version, int fieldId, bool? published = null);
        Task<bool> CheckFieldIsInvolvedInProcessAsync(int requestTypeId, int version, int fieldId, bool? published = null);
        Task<RequestFormViewModel> GetField(int requestTypeId, int version, int fieldId);
        bool CheckLOVIsInvolvedInProcess(int requestTypeId, int version, int fieldId, string LovValue, bool? published = null);
        Task<bool> CheckLOVIsInvolvedInProcessAsync(int requestTypeId, int version, int fieldId, string LovValue, bool? published = null);

        Task<IEnumerable<RequestTypeDisplayViewModel>> GetRequestTypesForGrouping(int status = 0, bool? requestCategoryPublished = null);
        Task<IEnumerable<RequestTypeViewModel>> GetRequestTypesForTicketCreation(string userId, int requestCategoryId = 0);
        Task<IEnumerable<RequestTypeWorkFlowViewModel>> GetRequestTypeWorkFlows(int requestTypeId, int version);
        Task<IEnumerable<RequestTypeBaseWorkFlowViewModel>> GetRequestTypeBaseWorkFlows(int requestTypeId, int version, List<TicketRequestDetailViewModel> requestForms, int userOrganizationalStructureId);

        Task<string> ValidateFieldForCondition(int fieldId, string value, string value2);
    }
}
