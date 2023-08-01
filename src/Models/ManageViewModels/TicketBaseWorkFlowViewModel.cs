using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class TicketBaseWorkFlowViewModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int StepId { get; set; }
        public string StepName { get; set; }
        public int StepTypeId { get; set; }
        public string StepTypeName { get; set; }
        public int AssigneeId { get; set; }
        public int AssigneeType { get; set; }
        public string AssigneeTypeDesc { get; set; }
        public int RequiredToExecute { get; set; }
        public int TAT { get; set; }
        public string ApplicableButtons { get; set; }
        public TicketViewModel Ticket { get; set; }
        public List<TicketBaseWorkFlowAssigneeViewModel> TicketBaseWorkFlowAssignees { get; set; }
    }
}
