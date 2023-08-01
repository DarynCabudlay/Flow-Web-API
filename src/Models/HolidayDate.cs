using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class HolidayDate
    {
        public HolidayDate()
        {
            HolidayAffectedOffices = new HashSet<HolidayAffectedOffice>();
        }

        public int Id { get; set; }
        public int HolidayId { get; set; }
        public int HolidayTypeId { get; set; }
        public DateTime Date { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual Holiday Holiday { get; set; }
        public virtual ICollection<HolidayAffectedOffice> HolidayAffectedOffices { get; set; }
        public virtual Options HolidayType { get; set; }
    }
}
