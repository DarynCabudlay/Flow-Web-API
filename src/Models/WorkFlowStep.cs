using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class WorkFlowStep
    {
        public WorkFlowStep()
        {
            RequestSteps = new HashSet<RequestStep>();
        }

        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StepTypeId { get; set; }
        public virtual WorkFlowStepType StepType { get; set; }
        public virtual ICollection<RequestStep> RequestSteps { get; set; }
    }
}
