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
    public interface IUserCommon
    {
        IQueryable<UserRequestTypesGroupingViewModel> GetUserRequestTypesGroupings(PagingRequest paging = null, bool? published = null, bool? requestTypesGroupingPublished = null);

        PagingResponse<UserRequestTypesGroupingViewModel> UserRequestTypeGroupingsPagingFeature(IQueryable<UserRequestTypesGroupingViewModel> query, PagingRequest paging);

        IQueryable<UserNotAllowedLinkTypeViewModel> GetUserNotAllowedLinkTypes(PagingRequest paging = null, bool? published = null, bool? linkTypePublished = null);

        PagingResponse<UserNotAllowedLinkTypeViewModel> UserNotAllowedLinkTypePagingFeature(IQueryable<UserNotAllowedLinkTypeViewModel> query, PagingRequest paging);
    }
}
