using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestStepAssigneeDetail
    {
        public int Id { get; set; }
        public int RequestStepAssigneeId { get; set; }
        public string Assignee { get; set; }
        public virtual RequestStepAssignee RequestStepAssignee { get; set; }
    }
}
