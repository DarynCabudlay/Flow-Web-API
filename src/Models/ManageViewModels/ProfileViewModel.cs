using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
       
        public string RoleId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Rank { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }

        public string Photo { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public bool BlockedAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}