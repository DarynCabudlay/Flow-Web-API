using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class AuditTrail
    {
        public AuditTrail()
        {
            AuditTrailDetails = new HashSet<AuditTrailDetail>();
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public string TableName { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }
        public string Transaction { get; set; }
        public string IP { get; set; }
        public string Device { get; set; }
        public string UsedApp { get; set; }
        public string Action { get; set; }
        public string ReasonOfDeletion { get; set; }
        public string DeletionRemarks { get; set; }
        public virtual ICollection<AuditTrailDetail> AuditTrailDetails { get; set; }
    }
}
