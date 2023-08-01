using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class GeneralField
    {
        public GeneralField()
        {
            RequestForms = new HashSet<RequestForm>();
        }

        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public int FieldType { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public int? DecimalDigit { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? MinNumber { get; set; }
        public int? MaxNumber { get; set; }
        public string CurrencySymbol { get; set; }
        public string LOVs { get; set; }
        public virtual ICollection<RequestForm> RequestForms { get; set; }
        public virtual ICollection<TicketRequestDetail> TicketRequestDetails { get; set; }

    }
}
