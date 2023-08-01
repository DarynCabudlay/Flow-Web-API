using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class SaveRequestStepViewModel
    {
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public int OldStepId { get; set; }
        public string StepName { get; set; }
        public string StepDescription { get; set; }
        public int StepTypeId { get; set; }
        public bool IsConditional { get; set; }
        public bool IsScheduled { get; set; }
        public int TAT { get; set; }
        public bool EnableEmail { get; set; }
        public bool EnableSMS { get; set; }
        public RequestStepConditionViewModel Condition { get; set; }
        public List<RequestStepAssigneeViewModel> Assignees { get; set; }
        public List<RequestTypeWorkFlowViewModel> WorkFlows { get; set; }
    }
}
