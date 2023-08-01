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
    public interface ITicketRequestDetail : IGenericMethod<TicketRequestDetailViewModel>
    {
        Task<TicketRequestDetailValidationViewModel> ValidateDetails(List<TicketRequestDetailViewModel> model);
    }
}
