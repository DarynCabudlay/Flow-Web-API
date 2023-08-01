using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestTypesGrouping
    {
        public RequestTypesGrouping()
        {
            RequestTypesGroupingDetails = new HashSet<RequestTypesGroupingDetail>();
            UserRequestTypesGroupings = new HashSet<UserRequestTypesGrouping>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual ICollection<RequestTypesGroupingDetail> RequestTypesGroupingDetails { get; set; }
        public virtual ICollection<UserRequestTypesGrouping> UserRequestTypesGroupings { get; set; }
    }
}
