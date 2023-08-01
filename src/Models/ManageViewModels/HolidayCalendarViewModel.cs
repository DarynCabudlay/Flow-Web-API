using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class HolidayCalendarViewModel
    {
        public int Id { get; set; }
        public int HolidayId { get; set; }
        public string Name { get; set; }
        public int HolidayTypeId { get; set; }
        public string HolidayType { get; set; }
        public bool HolidayPublished { get; set; }
        public DateTime Date { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public List<HolidayOfficeViewModel> HolidayOffices { get; set; }
    }

    public class CopyHolidayCalendarParam
    {
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
    }
}
