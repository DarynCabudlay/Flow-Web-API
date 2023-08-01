using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class ComparisonOperator
    {
        public ComparisonOperator()
        {
            ConditionDetails = new HashSet<RequestStepConditionDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ApplicableDataTypes { get; set; }
        public string ApplicableFieldTypes { get; set; }
        public string TechnicalEquivalent { get; set; }
        public string TechnicalEquivalent2 { get; set; }
        public bool UseInMultipleTypes { get; set; }
        public virtual ICollection<RequestStepConditionDetail> ConditionDetails { get; set; }
    }
}
