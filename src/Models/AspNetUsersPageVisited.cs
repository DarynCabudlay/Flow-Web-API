using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Models
{
    public partial class AspNetUsersPageVisited
    {
        public string VPageVisitedId { get; set; }
        public string UserId { get; set; }
        public DateTime DDateVisited { get; set; }
        public string NvPageName { get; set; }
        public string NvIpaddress { get; set; }

        public virtual AspNetUsers IdNavigation { get; set; }
    }
}
