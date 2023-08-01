using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class LoginHistoryViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string VUlhid { get; set; }
        public string UserId { get; set; }
        public DateTime Login { get; set; }
        public DateTime? Logout { get; set; }
        public string IP { get; set; }
    }
}
