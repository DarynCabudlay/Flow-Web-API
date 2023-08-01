using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class WorkFlowValidationViewModel
    {
        public string ReturnMessage { get; set; }
        public List<string> DetailedMessages { get; set; }
    }
}
