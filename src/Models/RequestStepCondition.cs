using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestStepCondition
    {
        public RequestStepCondition()
        {
            ConditionDetails = new HashSet<RequestStepConditionDetail>();
        }

        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public string Condition { get; set; }
        public string ConditionInText { get; set; }
        public virtual RequestStep RequestStep { get; set; }
        public virtual ICollection<RequestStepConditionDetail> ConditionDetails { get; set; }
    }
}
