using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace workflow.Models.ManageViewModels
{
    public class RequestCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Published { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public List<ListData> DsList { get; set; }
    }    
}
