using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class UserNotAllowedLinkType
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int LinkTypeId { get; set; }
        public int Category { get; set; } // Request or WorkFlows
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual LinkType LinkType { get; set; }
    }
}
