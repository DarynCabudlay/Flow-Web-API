using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class LockedRequestType
    {
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public string User { get; set; }
        public DateTime DateLocked { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual AspNetUsers UserInfo { get; set; }
    }
}
