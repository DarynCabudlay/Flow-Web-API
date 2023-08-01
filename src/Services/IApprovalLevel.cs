using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IApprovalLevel : IGenericMethod<ApprovalLevelDetailViewModel>
    {
        IQueryable<ApprovalLevelDetailViewModel> GetApprovalLevelDetails(PagingRequest paging = null, bool? published = null, bool? organizationEntityPublished = null);

        IQueryable<ApprovalLevelDetailViewModel> GetApprovalLevelDetail(int id, bool? organizationEntityPublished = null);

        PagingResponse<ApprovalLevelDetailViewModel> PagingFeature(IQueryable<ApprovalLevelDetailViewModel> query, PagingRequest paging);

        IQueryable<ApprovalLevelDetailViewModel> GetDetailsPerApprovalLevel(int id, bool? published = null, bool? organizationEntityPublished = null);
        Task<IEnumerable<ApprovalLevelDetailWithUserViewModel>> GetDetailsWithUsersPerApprovalLevel(int id, int organizationalStructureId, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null);
        IQueryable<ApprovalLevelViewModel> GetApprovalLevels(bool? organizationEntityPublished = null);

        Task<IEnumerable<ApprovalLevelViewModel>> GetApprovalLevels(int organizationalStructureId, bool? organizationalStruturePublished = null, bool? organizationalEntityPublished = null);

        Task<IEnumerable<ApprovalLevelReportingToViewModel>> GetApprovalLevelReportingTo(int approvalLevelDetailId, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null);

        Task<UserOrganizationalStructureViewModel> GetSpecificApprovalLevelReportingToAsync(int approvalLevelDetailId, int reportingTo, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null);

        UserOrganizationalStructureViewModel GetSpecificApprovalLevelReportingTo(int approvalLevelDetailId, int reportingTo, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null);

        Task<List<ApprovalLevelDetailViewModel>> GetApprovalLevelDetailsInSequentialMannerAsync(bool? published = null, bool? organizationEntityPublished = null);

        List<ApprovalLevelDetailViewModel> GetApprovalLevelDetailsInSequentialManner(bool? published = null, bool? organizationEntityPublished = null);

        Task<IEnumerable<ApprovalLevelStructureViewModel>> GetApprovalLevelStructures(int organizationalStructureId = 0, int userOrganizationalStructureId = 0, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null);
        Task<IEnumerable<OrganizationalStructureWithUserViewModel>> GetOrganizationalStructurePathWithUser(int organizationalStructureId, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null, bool? approvalLevelPublished = null);

        int GetApprovalDetailRowNumber(int id, List<ApprovalLevelDetailViewModel> details);
    }
}
