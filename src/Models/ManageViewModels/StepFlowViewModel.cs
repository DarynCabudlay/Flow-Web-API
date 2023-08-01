using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class StepFlowViewModel
    {
        public List<StepViewModel> StepNodes { get; set; }
        public List<LinkStepViewModel> LinkNodes { get; set; }
        public List<OtherLinkStepViewModel> OtherLinkNodes { get; set; }
    }

    public class StepViewModel
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
    }

    public class LinkStepViewModel
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Label { get; set; }
    }


    public class OtherLinkStepViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BackgroundColor { get; set; }
        public string UpperManagerId { get; set; }
    }
}
