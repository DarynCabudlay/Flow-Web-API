using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IOption : IGenericMethod<OptionViewModel>
    {
        // for Paging response
        PagingResponse<OptionViewModel> PagingFeature(IQueryable<OptionViewModel> query, PagingRequest paging, object Id = null);
        // get distinct option group
        IEnumerable<object> GetDistinctOptionGroup();
        // get distinct option group
        IQueryable<OptionViewModel> GetOptionsByGroup(object obj = null);

        IQueryable<OptionViewModel> GetOptions(PagingRequest paging);
    }
}
