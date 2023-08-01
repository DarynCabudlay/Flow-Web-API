using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestTypesGroupingDetail
    {
        public int Id { get; set; }
        public int RequestTypesGroupingId { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual RequestTypesGrouping RequestTypesGrouping { get; set; }
        public virtual RequestType RequestTypes { get; set; }
    }
}
