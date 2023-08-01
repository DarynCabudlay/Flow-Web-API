using System.Linq;
using workflow.Models;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IRole : IGenericMethod<RoleViewModel>
    {
        PagingResponse<RoleViewModel> PagingFeature(IQueryable<RoleViewModel> query, PagingRequest paging, object Id = null);

        IQueryable<AspNetUsersMenuPermission> GetMenuPer(object obj = null);

        IQueryable<RoleViewModel> GetRole(bool filterAdminRole = false);
    }
}
