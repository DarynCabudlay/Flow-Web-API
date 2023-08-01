using System;
using System.Collections.Generic;

namespace workflow.Models
{
    public partial class AspNetUsersProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Rank { get; set; }
        public string Mobile { get; set; }
        public string Phone {get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string Photo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public int? OfficeId { get; set; }
        public virtual AspNetUsers IdNavigation { get; set; }
        public virtual Options Office { get; set; }
    }
}
