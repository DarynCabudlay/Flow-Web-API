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
    public interface IRequestTypesGroupingDetail : IGenericMethod<RequestTypesGroupingDetailViewModel>
    {
        Task SaveRequestTypesGroupingDetails(SaveRequestTypesGroupingDetailWithCheckingDetailEntryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null);
        Task<RequestTypesGroupingDetailValidationViewModel> ValidateDetails(List<RequestTypesGroupingDetailViewModel> model);
    }
}
