using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class WorkFlowStepTypeButton
    {
        public int Id { get; set; }
        public int StepTypeId { get; set; }
        public int ActionButtonId { get; set; }
        public bool IsActionToNextStep { get; set; }
        public virtual WorkFlowStepType StepType { get; set; }
        public virtual Options ActionButton { get; set; }
    }
}
