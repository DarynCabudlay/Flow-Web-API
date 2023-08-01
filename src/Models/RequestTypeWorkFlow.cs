using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestTypeWorkFlow
    {
        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public int? NextStepRequestTypeId { get; set; }
        public int? NextStepVersion { get; set; }
        public int? NextStepId { get; set; }
        public int Action { get; set; }
        public int? TypeWhenRejected { get; set; }
        public virtual RequestStep RequestStep { get; set; }
        public virtual RequestStep NextRequestStep { get; set; }
        public virtual RequestTypeWorkFlowParallel WorkFlowParallel { get; set; }
    }
}
