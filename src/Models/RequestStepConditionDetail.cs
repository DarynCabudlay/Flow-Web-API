using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestStepConditionDetail
    {
        public int Id { get; set; }
        public int RequestStepConditionId { get; set; }
        public int Source { get; set; }
        public int ReferenceDetail { get; set; }
        public int ComparisonOperatorId { get; set; }
        public string LogicalOperator { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public int Sequence { get; set; }
        public bool Published { get; set; }
        public virtual RequestStepCondition ConditionHeader { get; set; }
        public virtual ComparisonOperator ComparisonOperator { get; set; }
        public virtual Options SourceInfo { get; set; }
    }
}
