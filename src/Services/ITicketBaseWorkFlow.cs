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
    public interface ITicketBaseWorkFlow : IGenericMethod<TicketBaseWorkFlowViewModel>
    {
        Task<IEnumerable<RequestStepViewModel>> GetTicketBaseWorkFlowSteps(int ticketId);
        Task<StepFlowViewModel> GetTicketBaseStepsAndWorkFlows(int ticketId);
    }
}
