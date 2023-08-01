using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{

    public enum AssigneeType
    {
        Name = 1,
        Position = 2
    }

    public class RequestStepAssigneeViewModel
    {
        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public bool IsConditional { get; set; }
        public AssigneeType AssigneeType { get; set; }
        public int RequiredAssigneeToExecute { get; set; }
        public int? Field1 { get; set; }
        public string FieldName1 { get; set; }
        public string Value1 { get; set; }
        public int? Field2 { get; set; }
        public string FieldName2 { get; set; }
        public string Value2 { get; set; }
        public int? Field3 { get; set; }
        public string FieldName3 { get; set; }
        public string Value3 { get; set; }
        public List<RequestStepAssigneeDetailViewModel> RequestStepAssigneeDetails { get; set; }
        public RequestStepViewModel RequestStep { get; set; }
    }
}
