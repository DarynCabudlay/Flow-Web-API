using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class UserRequestTypesGroupingViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RequestTypesGroupId { get; set; }
        public string RequestTypesGroupingName { get; set; }
        public string RequestTypesGroupingDescription { get; set; }
        public bool RequestTypesGroupingPublished { get; set; }
        public int RequestTypeGroupingDetailsCount { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public UserBasicInfoModel User { get; set; }
    }

    public class UserRequestTypesGroupingValidationViewModel
    {
        public string ReturnMessage { get; set; }
        public int ReturnCode { get; set; }
    }

    public class SaveUserRequestTypesGroupingWithCheckingDetailEntryViewModel
    {
        public bool WithCheckingOfDetailEntryInDb { get; set; }
        public List<UserRequestTypesGroupingViewModel> UserRequestTypesGroupings { get; set; }
    }
}
