using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class ReasonBasicData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ReasonsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ReasonTypeId { get; set; }
        public string ReasonTypeName { get; set; }
        public bool ReasonTypePublished { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Options ReasonType { get; set; }
    }
}
