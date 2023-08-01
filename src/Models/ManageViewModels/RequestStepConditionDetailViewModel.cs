using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RequestStepConditionDetailViewModel
    {
        public int Id { get; set; }
        public int RequestStepConditionId { get; set; }
        public int Source { get; set; }
        public string SourceName { get; set; }
        public int ReferenceDetail { get; set; }
        public string ReferenceDetailDescription { get; set; }
        public int ComparisonOperatorId { get; set; }
        public string ComparisonOperatorName { get; set; }
        public string LogicalOperator { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public int Sequence { get; set; }
        public bool Published { get; set; }
        public RequestStepConditionViewModel ConditionHeader { get; set; }
        public ComparisonOperatorViewModel ComparisonOperator { get; set; }
    }

    public class ConditionSourceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
    }

}
