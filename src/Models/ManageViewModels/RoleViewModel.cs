using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace workflow.Models.ManageViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string IndexPage { get; set; }
        public int? TotalUser { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public List<MenuPermisionViewModel> MenuPermission { get; set; }
        public List<ListData> DsList { get; set; }
        public bool IsChecked { get; set; }
    }    
}
