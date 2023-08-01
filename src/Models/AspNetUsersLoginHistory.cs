using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Models
{
    public partial class AspNetUsersLoginHistory
    {
        public string VUlhid { get; set; }
        public string UserId { get; set; }
        public DateTime DLogIn { get; set; }
        public DateTime? DLogOut { get; set; }
        public string NvIpaddress { get; set; }

        public virtual AspNetUsers IdNavigation { get; set; }
    }
}
