using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class OrganizationEntityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Hierarchy { get; set; }
        public bool Published { get; set; }
        public string WebClass { get; set; }
        public string Color { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public List<OrganizationalStructureViewModel> OrganizationalStructures { get; set; }
    }
}
