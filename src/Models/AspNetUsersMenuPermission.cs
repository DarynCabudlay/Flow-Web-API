using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Models
{
    public partial class AspNetUsersMenuPermission
    {
        public AspNetUsersMenuPermission()
        {
            AspNetUsersMenuPermissionControl = new HashSet<AspNetUsersMenuPermissionControl>();
        }

        public string VMenuPermissionId { get; set; }
        public string RoleId { get; set; }
        public string VMenuId { get; set; }

        public virtual AspNetRoles IdNavigation { get; set; }
        public virtual AspNetUsersMenu VMenu { get; set; }
        public virtual ICollection<AspNetUsersMenuPermissionControl> AspNetUsersMenuPermissionControl { get; set; }
    }
}
