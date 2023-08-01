using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IReason : IGenericMethod<ReasonsViewModel>
    {
        IQueryable<ReasonsViewModel> GetReasons(PagingRequest paging);
        Task<IEnumerable<ReasonBasicData>> GetReasonByReasonType(string reasontype, bool published = true);
        Task<IEnumerable<ReasonTypesViewModel>> GetReasonTypes(bool published = true);
        PagingResponse<ReasonsViewModel> PagingFeature(IQueryable<ReasonsViewModel> query, PagingRequest paging, object Id = null);
    }
}
