using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public partial class RequestCategory
    {
        public RequestCategory()
        {
            RequestTypes = new HashSet<RequestType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public virtual ICollection<RequestType> RequestTypes { get; set; }
    }
}
