using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class WorkFlowStepTypeToSeedViewModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Published {get;set;}
        public bool ApplyApprovalPolicyForAssignees { get; set; }
        public List<WorkFlowStepTypeButtonToSeedViewModel> ActionButtons { get; set; }
    }
}
