using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class WorkFlowApprovalSetupPerStep
    {
        public int Id { get; set; }
        public int WorkFlowApprovalSetupId { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public virtual WorkFlowApprovalSetup WorkFlowApprovalSetup { get; set; }
        public virtual RequestStep RequestStep { get; set; }
    }
}
