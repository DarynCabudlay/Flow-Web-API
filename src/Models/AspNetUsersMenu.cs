using System;
using System.Collections.Generic;

namespace workflow.Models
{
    public partial class AspNetUsersMenu
    {
        public AspNetUsersMenu()
        {
            AspNetUsersMenuPermission = new HashSet<AspNetUsersMenuPermission>();
            AspNetUsersMenuControl = new HashSet<AspNetUsersMenuControl>();
            InverseVParentMenu = new HashSet<AspNetUsersMenu>();
        }

        public string VMenuId { get; set; }
        public string NvMenuName { get; set; }
        public int ISerialNo { get; set; }
        public string NvFabIcon { get; set; }
        public string VParentMenuId { get; set; }
        public string NvPageUrl { get; set; }
        public string PrefixCode { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public bool IsMenu { get; set; }
        public virtual AspNetUsersMenu VParentMenu { get; set; }
        public virtual ICollection<AspNetUsersMenuPermission> AspNetUsersMenuPermission { get; set; }
        public virtual ICollection<AspNetUsersMenuControl> AspNetUsersMenuControl { get; set; }
        public virtual ICollection<AspNetUsersMenu> InverseVParentMenu { get; set; }
    }
}
