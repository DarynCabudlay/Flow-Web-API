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
    public interface IUserNotAllowedLinkType : IGenericMethod<UserNotAllowedLinkTypeViewModel>
    {
        Task<UserNotAllowedLinkTypeViewModel> AddInUser(UserNotAllowedLinkTypeViewModel data);

        Task<UserNotAllowedLinkTypeValidationViewModel> ValidateDetails(List<UserNotAllowedLinkTypeViewModel> model);

        Task SaveUserNotAllowedLinkType(SaveUserNotAllowedLinkTypeWithCheckingDetailEntryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null);
    }
}
