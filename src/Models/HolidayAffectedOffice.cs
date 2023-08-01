using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class HolidayAffectedOffice
    {
        public int Id { get; set; }
        public int HolidayDateId { get; set; }
        public int OfficeId { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual HolidayDate HolidayDate { get; set; }
        public virtual Options Office { get; set; }
    }
}
