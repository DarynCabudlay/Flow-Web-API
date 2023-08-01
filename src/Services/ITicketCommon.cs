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
    public interface ITicketCommon
    {
        IQueryable<TicketRequestDetailViewModel> GetTicketRequestDetails();
        IQueryable<TicketRequestDetailViewModel> GetTicketRequestDetail(int id);
        IQueryable<TicketRequestViewModel> GetTicketRequests();
        IQueryable<TicketViewModel> GetTickets(PagingRequest paging = null, int status = 0);
        PagingResponse<TicketViewModel> TicketsPagingFeature(IQueryable<TicketViewModel> query, PagingRequest paging);
        IQueryable<TicketViewModel> GetTicket(int id);
        IQueryable<TicketAttachmentViewModel> GetTicketAttachments(PagingRequest paging = null, int category = 0);
        IQueryable<TicketAttachmentViewModel> GetTicketAttachment(int id);
        PagingResponse<TicketAttachmentViewModel> TicketAttachmentsPagingFeature(IQueryable<TicketAttachmentViewModel> query, PagingRequest paging);
    }
}
