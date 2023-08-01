using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public enum UserNotAllowedLinkTypeCategory
    {
        Request = 1,
        WorkFlow = 2
    }

    public class UserNotAllowedLinkTypeViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int LinkTypeId { get; set; }
        public string LinkTypeName { get; set; }
        public string LinkTypeURL { get; set; }
        public bool LinkTypePublished { get; set; }
        public UserNotAllowedLinkTypeCategory Category { get; set; }
        public string CategoryDescription { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public UserBasicInfoModel User { get; set; }
        public LinkTypeViewModel LinkType { get; set; }
    }

    public class UserNotAllowedLinkTypeValidationViewModel
    {
        public string ReturnMessage { get; set; }
        public int ReturnCode { get; set; }
    }

    public class SaveUserNotAllowedLinkTypeWithCheckingDetailEntryViewModel
    {
        public bool WithCheckingOfDetailEntryInDb { get; set; }
        public List<UserNotAllowedLinkTypeViewModel> UserNotAllowedLinkTypes { get; set; }
    }
}
