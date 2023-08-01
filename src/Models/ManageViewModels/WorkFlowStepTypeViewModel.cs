using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class WorkFlowStepTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Published { get; set; }
        public bool ApplyApprovalPolicyForAssignees { get; set; }
        public List<WorkFlowStepViewModel> Steps { get; set; }
        public List<WorkFlowStepTypeButtonViewModel> StepsButtons { get; set; }
    }
}
