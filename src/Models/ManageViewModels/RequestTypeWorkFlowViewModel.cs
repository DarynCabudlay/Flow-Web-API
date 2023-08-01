using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{

    public enum TypeWhenRejected
    {
        ForClosing = 1,
        ReturnToRequestor = 2,
        ReturnToPrevious = 3
    }
    public class RequestTypeWorkFlowViewModel
    {
        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public string StepName { get; set; }
        public int? NextStepRequestTypeId { get; set; }
        public int? NextStepVersion { get; set; }
        public int? NextStepId { get; set; }
        public string NextStepName { get; set; }
        public string Name { get; set; } // this is only temporary
        public int Action { get; set; }
        public string ActionName { get; set; }
        public TypeWhenRejected? TypeWhenRejected { get; set; }
        public RequestStepViewModel RequestStep { get; set; }
        public RequestStepViewModel NextRequestStep { get; set; }
    }

    public class SaveRequestTypeWorkFlowViewModel
    {
        public bool IsForNextStep { get; set; }
        public List<RequestTypeWorkFlowViewModel> Workflows { get; set; }
    }
}