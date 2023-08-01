using System;
using System.Collections.Generic;

namespace workflow.Models
{
    public partial class Options
    {
        public Options()
        {
            AspNetUsersMenuControl = new HashSet<AspNetUsersMenuControl>();
            Reasons = new HashSet<Reason>();
            ActionButtons = new HashSet<WorkFlowStepTypeButton>();
            ConditionSource = new HashSet<RequestStepConditionDetail>();
            HolidayAffectedOffices = new HashSet<HolidayAffectedOffice>();
            UserOffices = new HashSet<AspNetUsersProfile>();
            HolidayDates = new HashSet<HolidayDate>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OptionGroup { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public virtual ICollection<AspNetUsersMenuControl> AspNetUsersMenuControl { get; set; }
        public virtual ICollection<Reason> Reasons { get; set; }
        public virtual ICollection<WorkFlowStepTypeButton> ActionButtons { get; set; }
        public virtual ICollection<RequestStepConditionDetail> ConditionSource { get; set; }
        public virtual ICollection<HolidayAffectedOffice> HolidayAffectedOffices { get; set; }
        public virtual ICollection<AspNetUsersProfile> UserOffices { get; set; }
        public virtual ICollection<HolidayDate> HolidayDates { get; set; }
    }
}
