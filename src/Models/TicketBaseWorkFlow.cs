using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class TicketBaseWorkFlow
    {
        public TicketBaseWorkFlow()
        {
            TicketBaseWorkFlowAssignees = new HashSet<TicketBaseWorkFlowAssignee>();
        }

        public int Id { get; set; }
        public int TicketId { get; set; }
        public int StepId { get; set; }
        public int StepTypeId { get; set; }
        public int AssigneeId { get; set; }
        public int AssigneeType { get; set; }
        public int RequiredToExecute { get; set; }
        public int TAT { get; set; }
        public string ApplicableButtons { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ICollection<TicketBaseWorkFlowAssignee> TicketBaseWorkFlowAssignees { get; set; }
    }
}
