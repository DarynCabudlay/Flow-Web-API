using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Models
{
    public partial class AspNetUsersMenuPermissionControl
    {
        public string Id { get; set; }
        public string VMenuPermissionId { get; set; }
        public string MenuControlId { get; set; }

        public virtual AspNetUsersMenuPermission MenuPermission { get; set; }
        public virtual AspNetUsersMenuControl MenuControl { get; set; }
    }
}
