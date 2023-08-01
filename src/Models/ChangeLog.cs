using System;
using System.Collections.Generic;

namespace workflow.Models
{
    public partial class ChangeLog
    {
        public int Id { get; set; }
        public string VMenuId { get; set; }
        public string EventType { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public string ObjectType { get; set; }
        public string OldContentDetail { get; set; }
        public string ContentDetail { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
    }
}
