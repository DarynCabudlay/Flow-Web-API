using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestStepAssignee
    {
        public RequestStepAssignee()
        {
            RequestStepAssigneeDetails = new HashSet<RequestStepAssigneeDetail>();
        }

        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public bool IsConditional { get; set; }
        public int AssigneeType { get; set; }
        public int RequiredAssigneeToExecute { get; set; }
        public int? Field1 { get; set; }
        public string Value1 { get; set; }
        public int? Field2 { get; set; }
        public string Value2 { get; set; }
        public int? Field3 { get; set; }
        public string Value3 { get; set; }
        public virtual ICollection<RequestStepAssigneeDetail> RequestStepAssigneeDetails { get; set; }
        public virtual RequestStep RequestStep { get; set; }
    }
}
