using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IRequestTypesGroupingCommon
    {
        IQueryable<RequestTypesGroupingViewModel> GetRequestTypesGroupings(PagingRequest paging = null, bool? published = null, bool? detailPublished = null);

        PagingResponse<RequestTypesGroupingViewModel> RequestTypeGroupingsPagingFeature(IQueryable<RequestTypesGroupingViewModel> query, PagingRequest paging);

        IQueryable<RequestTypesGroupingDetailViewModel> GetRequestTypesGroupingDetails(PagingRequest paging = null, bool? published = null, bool? detailPublished = null);

        PagingResponse<RequestTypesGroupingDetailViewModel> RequestTypeGroupingDetailsPagingFeature(IQueryable<RequestTypesGroupingDetailViewModel> query, PagingRequest paging);
    }
}
