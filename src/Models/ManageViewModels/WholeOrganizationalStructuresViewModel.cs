using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class WholeOrganizationalStructuresViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string UserId { get; set; } //with value once Type is user
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationEntityId { get; set; }
        public string OrganizationEntity { get; set; }
        public bool Published { get; set; }
        public int NodeNumber { get; set; }
        public string Type { get; set; }
        public List<WholeOrganizationalStructuresViewModel> Children { get; set; }
    }
}
