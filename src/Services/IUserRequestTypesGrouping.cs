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
    public interface IUserRequestTypesGrouping : IGenericMethod<UserRequestTypesGroupingViewModel>
    {
        Task<UserRequestTypesGroupingViewModel> AddInUser(UserRequestTypesGroupingViewModel data);

        Task<UserRequestTypesGroupingValidationViewModel> ValidateDetails(List<UserRequestTypesGroupingViewModel> model);

        Task SaveUserRequestTypesGrouping(SaveUserRequestTypesGroupingWithCheckingDetailEntryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null);
    }
}
