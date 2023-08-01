using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestStep
    {
        public RequestStep()
        {
            Assignees = new HashSet<RequestStepAssignee>();
            Conditions = new HashSet<RequestStepCondition>();
            WorkFlows = new HashSet<RequestTypeWorkFlow>();
            NextWorkFlows = new HashSet<RequestTypeWorkFlow>();
        }

        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public bool IsConditional { get; set; }
        public bool IsScheduled { get; set; }
        public int TAT { get; set; }
        public bool EnableEmail { get; set; }
        public bool EnableSMS { get; set; }
        public int Sequence { get; set; }
        public virtual WorkFlowStep WorkFlowStep { get; set; }
        public virtual ICollection<RequestStepAssignee> Assignees { get; set; }
        public virtual ICollection<RequestStepCondition> Conditions { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual ICollection<RequestTypeWorkFlow> WorkFlows { get; set; }
        public virtual ICollection<RequestTypeWorkFlow> NextWorkFlows { get; set; }
    }
}
