using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace workflow.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public DateTime date { get; set; }
        public bool Status { get; set; }
        public bool BlockedAccount { get; set; }
        public int State { get; set; }
    }
}
