using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IRequestCategory : IGenericMethod<RequestCategoryViewModel>
    {
        PagingResponse<RequestCategoryViewModel> PagingFeature(IQueryable<RequestCategoryViewModel> query, PagingRequest paging, object Id = null);

        IQueryable<RequestCategoryViewModel> GetRequestCategories(PagingRequest paging);

        Task<IEnumerable<RequestCategoryViewModel>> GetRequestCategoriesForTicketCreation(string userId);
    }
}
