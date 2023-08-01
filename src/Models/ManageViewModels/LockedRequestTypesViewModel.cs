using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class LockedRequestTypesViewModel
    {
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public string User { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserPosition { get; set; }
        public string UserDepartment { get; set; }
        public DateTime? DateLocked { get; set; }
    }
}
