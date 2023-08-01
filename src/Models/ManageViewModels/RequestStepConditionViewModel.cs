using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RequestStepConditionViewModel
    {
        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public string Condition { get; set; }
        public string ConditionInText { get; set; }
        public RequestStepViewModel RequestStep { get; set; }
        public List<RequestStepConditionDetailViewModel> ConditionDetails { get; set; }
    }
}
