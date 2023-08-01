using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Models
{
    public partial class AspNetUsersMenuControl
    {
        public AspNetUsersMenuControl()
        {
            AspNetUsersMenuPermissionControl = new HashSet<AspNetUsersMenuPermissionControl>();
        }

        public string MenuControlId { get; set; }
        public string VMenuId { get; set; }
        public int OptionId { get; set; }

        public virtual AspNetUsersMenu VMenu { get; set; }
        public virtual Options Option { get; set; }
        public virtual ICollection<AspNetUsersMenuPermissionControl> AspNetUsersMenuPermissionControl { get; set; }
    }
}
