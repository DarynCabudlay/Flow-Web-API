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
    public interface ILinkType : IGenericMethod<LinkTypeViewModel>
    {
        IQueryable<LinkTypeViewModel> GetLinkTypes(PagingRequest paging = null, bool? published = null);

        PagingResponse<LinkTypeViewModel> PagingFeature(IQueryable<LinkTypeViewModel> query, PagingRequest paging);
    }
}
