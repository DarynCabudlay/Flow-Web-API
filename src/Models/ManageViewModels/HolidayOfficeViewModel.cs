using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class HolidayOfficeViewModel
    {
        public int HolidayOfficeId { get; set; }
        public int HolidayDateId { get; set; }
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public string Office { get; set; }
        public string Name { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
