using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class WorkFlowStepType
    {
        public WorkFlowStepType()
        {
            Steps = new HashSet<WorkFlowStep>();
            StepsButtons = new HashSet<WorkFlowStepTypeButton>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Published { get; set; }
        public bool ApplyApprovalPolicyForAssignees { get; set; }
        public virtual ICollection<WorkFlowStep> Steps { get; set; }
        public virtual ICollection<WorkFlowStepTypeButton> StepsButtons { get; set; }
        
    }
}
