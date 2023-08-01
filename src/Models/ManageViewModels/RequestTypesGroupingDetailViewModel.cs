using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RequestTypesGroupingDetailViewModel
    {
        public int Id { get; set; }
        public int RequestTypesGroupingId { get; set; }
        public string RequestTypesGroupingName { get; set; }
        public string RequestTypesGroupingDescription { get; set; }
        public bool RequestTypesGroupingPublished { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public int Version { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }

    public class SaveRequestTypesGroupingDetailViewModel
    {
        public int Id { get; set; }
        public int RequestTypesGroupingId { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public bool Published { get; set; }
    }
    public class RequestTypesGroupingDetailValidationViewModel
    {
        public string ReturnMessage { get; set; }
        public int ReturnCode { get; set; }
    }

    public class SaveRequestTypesGroupingDetailWithCheckingDetailEntryViewModel
    {
        public bool WithCheckingOfDetailEntryInDb { get; set; }
        public List<RequestTypesGroupingDetailViewModel> RequestTypesGroupDetails { get; set; }
    }
}
