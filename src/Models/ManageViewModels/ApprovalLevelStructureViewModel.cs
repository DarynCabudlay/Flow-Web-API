using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class ApprovalLevelStructureViewModel
    {
        public bool Expanded { get; set; }
        public string StyleClass { get; set; }
        public string Type { get; set; }
        public ApprovalLevelStructureDataViewModel Data { get; set; }
        public List<ApprovalLevelStructureViewModel> Children { get; set; }
    }

    public class ApprovalLevelStructureDataViewModel
    {
        public int OrganizationalStructureId { get; set; }
        public int UserOrganizationalStructureId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string OrganizationalStructure { get; set; }
        public string Entity { get; set; }
    }

}

