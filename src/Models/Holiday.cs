using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class Holiday
    {
        public Holiday()
        {
            HolidayDates = new HashSet<HolidayDate>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public bool WithFixedSchedule { get; set; }
        public int? FixedScheduleMonth { get; set; }
        public int? FixedScheduleDay { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual ICollection<HolidayDate> HolidayDates { get; set; }
    }
}
