using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class ApprovalLevelDetail
    {
        public int Id { get; set; }
        public int ApprovalLevelId { get; set; }
        public int Sequence { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual ApprovalLevel ApprovelLevel { get; set; }
    }
}
