using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class UserIPAddressPerSessionsViewModel
    {
        public int SpId { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
    }
}
