using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestTypeWorkFlowParallel
    {
        public int Id { get; set; }
        public int ParallelNo { get; set; }
        public int RequestTypeWorkFlowId { get; set; }
        public virtual RequestTypeWorkFlow WorkFlow { get; set; }
    }
}
