using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class HolidayViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool  Published { get; set; }
        public bool WithFixedSchedule { get; set; }
        public string WithFixedScheduleText { get; set; }
        public int? FixedScheduleMonth { get; set; }
        public string FixedScheduleMonthText 
        {
           get
           {
                string monthInText = "";
                if (FixedScheduleMonth == 1)
                    monthInText = "January";
                else if (FixedScheduleMonth == 2)
                    monthInText = "February";
                else if (FixedScheduleMonth == 3)
                    monthInText = "March";
                else if (FixedScheduleMonth == 4)
                    monthInText = "April";
                else if (FixedScheduleMonth == 5)
                    monthInText = "May";
                else if (FixedScheduleMonth == 6)
                    monthInText = "June";
                else if (FixedScheduleMonth == 7)
                    monthInText = "July";
                else if (FixedScheduleMonth == 8)
                    monthInText = "August";
                else if (FixedScheduleMonth == 9)
                    monthInText = "September";
                else if (FixedScheduleMonth == 10)
                    monthInText = "October";
                else if (FixedScheduleMonth == 11)
                    monthInText = "November";
                else if (FixedScheduleMonth == 12)
                    monthInText = "December";

                return monthInText;
           }
        }

        public int? FixedScheduleDay { get; set; }
        public string FixedScheduleInText { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
