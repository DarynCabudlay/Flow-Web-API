using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IUser
    {
        IQueryable<UserRegisterModel> Find(object obj = null);
        IQueryable<UserRegisterModel> FindAll(object obj = null);
        IQueryable<UserRegisterModel> GetUserList(PagingRequest paging = null);
        Task<IEnumerable<UserListViewModel>> GetUsers();
        Task<UserRegisterModel> Add(SaveUserModel model);
        Task Update(SaveUserModel model);
        Task Delete(object obj);
        Task UpdateProfile(ProfileViewModel data);
        Task<IdentityResult> ChangePassword(ChangePasswordViewModel model);
        IQueryable<PageVisitedViewModel> GetPageVisited(string id = null, object obj = null);
        PagingResponse<PageVisitedViewModel> PagingPageVisited(IQueryable<PageVisitedViewModel> query, PagingRequest paging, object id = null);
        IQueryable<LoginHistoryViewModel> GetLoginHistory(string id = null, object obj = null);
        PagingResponse<LoginHistoryViewModel> PagingLogHistory(IQueryable<LoginHistoryViewModel> query, PagingRequest paging, object id = null);
        Task<ObjectReturnModel> GetProfileData();
        IQueryable<UserRoleViewModel> GetUserRole(object obj);
        Task<UserPreferencesViewModel> GetUserPreferences();
        Task SavePreferences(UserPreferencesViewModel model);
        Task SendInitialPassword(string userid);
        Task ResendInitialPassword(string userid);
        Task SaveUserOrganizationalStructures(SaveUserOrganizationalStructureWithDBValidationViewModel model);
        Task SaveUserRoles(List<UserRoleViewModel> model);
        IQueryable<UserOrganizationalStructureViewModel> GetUserOrganizationalStructuresPerUser(string id, PagingRequest paging = null, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null);

        IQueryable<UserOrganizationalStructureViewModel> GetUserOrganizationalStructures(bool? published = null, bool? organizationalStructurePublished = null, bool? organizationalEntityPublished = null);

        PagingResponse<UserOrganizationalStructureViewModel> UserOrganizationalStructuresPagingFeature(IQueryable<UserOrganizationalStructureViewModel> query, PagingRequest paging);

        PagingResponse<UserRegisterModel> PagingFeature(IQueryable<UserRegisterModel> query, PagingRequest paging);

        Task DeleteUserOrganizationalStructure(int id);

        Task<IEnumerable<RequestTypeBaseWorkAssigneeFlowViewModel>> GetUserOrganizationalStructurePath(int userOrganizationalStructureId, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationalEntityPublished = null, bool? userPublished = null, bool? approvalLevelPublished = null);
    }
}
