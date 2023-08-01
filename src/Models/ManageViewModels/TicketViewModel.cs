using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public enum TicketStatus
    {
        Draft = 1,
        Inprocess = 2,
        Rejected = 3,
        Closed = 4,
        Cancelled = 5
    }

    public class TicketViewModel
    {
        public int Id { get; set; }
        public string TicketNo { get; set; }
        public string RushTicketNo { get; set; }
        public int RequestCategoryId { get; set; }
        public string RequestCategoryName { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public int Version { get; set; }
        public TicketStatus Status { get; set; }
        public string StatusDescription { get; set; }
        public DateTime StatusDate { get; set; }
        public int? DetailedStatus { get; set; } //Last Activity
        public int? CancellationReason { get; set; }
        public string CancellationReasonName { get; set; }
        public string CancellationRemarks { get; set; }
        public int UserOrganizationalStructureId { get; set; }
        public string UserOrganizationalStructureName { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> DateSubmitted { get; set; }
        public int AgingFromDateSubmittedUptoDate 
        {
            get 
            {
                if (DateSubmitted != null)
                {
                    if (Status == TicketStatus.Inprocess)
                        return DateTime.Now.Date.Subtract(Convert.ToDateTime(DateSubmitted).Date).Days;
                    else
                        return StatusDate.Date.Subtract(Convert.ToDateTime(DateSubmitted).Date).Days;
                }
                else
                    return 0;
            } 
        }
        public bool WithCheckingOfDetailEntryInDb { get; set; }
        public List<TicketRequestViewModel> TicketRequests { get; set; }
        public List<TicketBaseWorkFlowViewModel> TicketBaseWorkFlows { get; set; }
        public RequestTypeViewModel RequestType { get; set; }
        public UserOrganizationalStructureViewModel UserOrganizationalStructure { get; set; }
        public List<TicketLinkViewModel> TicketLinks { get; set; }
        public List<TicketAttachmentViewModel> TicketAttachments { get; set; }
    }

    public class CancelTicketViewModel
    {
        public int Id { get; set; }
        public int ReasonId { get; set; }
        public string Remarks { get; set; }
    }
}
