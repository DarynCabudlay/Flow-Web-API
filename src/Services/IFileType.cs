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
    public interface IFileType : IGenericMethod<FileTypeViewModel>
    {
        IQueryable<FileTypeViewModel> GetFileTypes(PagingRequest paging = null, bool? published = null);

        PagingResponse<FileTypeViewModel> PagingFeature(IQueryable<FileTypeViewModel> query, PagingRequest paging);
    }
}
