using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class AuditTrailDetail
    {
        public int DetailId { get; set; }
        public int Id { get; set; }
        public string Key { get; set; }
        public string TableName { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public virtual AuditTrail AuditTrail { get; set; }
    }
}
