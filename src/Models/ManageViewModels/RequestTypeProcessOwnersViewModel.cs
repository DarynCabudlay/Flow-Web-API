using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RequestTypeProcessOwnersViewModel
    {
        public int Id { get; set; }
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public string Owner { get; set; }
        public List<string> UserList { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName{ get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerFullName2 { get; set; }
        public string OwnerPosition { get; set; }
        public string OwnerDepartment { get; set; }
        public string OwnerEmail { get; set; }
    }
}
