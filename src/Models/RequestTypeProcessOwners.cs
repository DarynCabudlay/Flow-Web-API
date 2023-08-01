using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestTypeProcessOwner
    {
        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public string Owner { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual AspNetUsers User { get; set; }

    }
}
