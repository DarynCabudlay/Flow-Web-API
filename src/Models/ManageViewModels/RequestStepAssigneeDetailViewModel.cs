using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RequestStepAssigneeDetailViewModel
    {
        public int RequestStepAssigneeDetailId { get; set; }
        public int Id { get; set; }
        public int RequestStepAssigneeId { get; set; }
        public string Assignee { get; set; }
        public string AssigneeFirstName { get; set; }
        public string AssigneeLastName { get; set; }
        public RequestStepAssigneeViewModel RequestStepAssignee { get; set; }
    }
}
