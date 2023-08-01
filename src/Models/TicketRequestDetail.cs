using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class TicketRequestDetail
    {
        public int Id { get; set; }
        public int TicketRequestId { get; set; }
        public int FieldId { get; set; }
        public bool IsLov { get; set; }
        public string ValueCode { get; set; }
        public string Value { get; set; }
        public string ValueCode2 { get; set; }
        public string Value2 { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual TicketRequest TicketRequest { get; set; }
        public virtual GeneralField GeneralField { get; set; }
    }
}
