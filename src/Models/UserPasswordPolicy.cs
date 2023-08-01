using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public partial class UserPasswordPolicy
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool CanChangePassword { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsPasswordNeverExpires { get; set; }
        public string InitialPassword { get; set; }
        public Nullable<DateTime> InitialPasswordExpirationDate { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
