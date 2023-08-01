using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class WorkFlowStepTypeButtonViewModel
    {
        public int Id { get; set; }
        public int StepTypeId { get; set; }
        public int ActionButtonId { get; set; }
        public bool IsActionToNextStep { get; set; }
        public string ActionButton { get; set; }
        public string ActionButtonDescription { get; set; }
        public bool WithActionInWorkflow{ get; set; }

    }
}
