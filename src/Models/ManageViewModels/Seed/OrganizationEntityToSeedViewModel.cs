using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels.Seed
{
    public class OrganizationEntityToSeedViewModel
    {
        public string Name { get; set; }
        public int Hierarchy { get; set; }
        public bool Published { get; set; }
        public string WebClass { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
