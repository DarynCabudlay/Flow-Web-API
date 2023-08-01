using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class MenuViewModel
    {
        public string VMenuID { get; set; }
        public string RoleId { get; set; }
        public string NameWithParent { get; set; }
        public string NvMenuName { get; set; }
        public int ISerialNo { get; set; }
        public string NvFabIcon { get; set; }
        public string VParentMenuID { get; set; }
        public string NvPageUrl { get; set; }
        public string PrefixCode { get; set; }
        public List<MenuViewModel> Child { get; set; }
        public bool Published { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public System.DateTime? ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public bool IsMenu { get; set; }
        public List<IdList> SelectedControls { get; set; }
        public List<IdListStr> SelectedRoleControls { get; set; }
        public List<UserMenuControlViewModel> ButtonControls { get; set; }
        public List<ListData> DsList { get; set; }

        public bool IsChecked { get; set; }
    }

    public class MenuPermisionViewModel
    {
        public string VMenuPermissionId { get; set; }
        public string VMenuId { get; set; }
        public string RoleId { get; set; }
        public string NameWithParent { get; set; }
        public string PageUrl { get; set; }
        public int ButtonControlsCount { get; set; }
        public bool IsChecked { get; set; }
        public List<IdListStr> SelectedControls { get; set; }
        public List<ListData> DsList { get; set; }
    }

    public class UserMenuControlViewModel
    {
        public int Id { get; set; }
        public string MenuControlId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}