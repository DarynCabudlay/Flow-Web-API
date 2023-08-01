using System.Linq;
using System.Threading.Tasks;
using workflow.Models;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IMenu : IGenericMethod<MenuViewModel>
    {
        // for Paging response
        PagingResponse<MenuViewModel> PagingFeature(IQueryable<MenuViewModel> query, PagingRequest paging, object Id = null);

        IQueryable<AspNetUsersMenu> GetMenuByUrl(string url);

        Task<ObjectReturnModel> GetSideMenu();

        ObjectReturnModel GetMenuList(object obj = null);
        ObjectReturnModel GetMenuPerRole(string roleId);
    }
}
