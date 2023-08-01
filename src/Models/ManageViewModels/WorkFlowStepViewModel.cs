using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class WorkFlowStepViewModel
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StepTypeId { get; set; }
        public WorkFlowStepTypeViewModel StepType { get; set; }
        public List<RequestStepViewModel> RequestSteps { get; set; }
    }
}
