using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{

    public enum UserAccountState
    {
        Active = 1,
        Initial_Password = 2,
        Initial_Password_Expired = 4,
        Password_Expired = 8,
        Dormant_User = 16,
        Soft_Locked = 32,
        Locked = 64
    }

    public class UserStateParam
    {
        public string Id { get; set; }
        public UserAccountState State { get; set; }
    }

    public class UserRegisterModel
    {
        public string  Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string FullName2 { get; set; }
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
        public bool EmailVerificationDisabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool Status { get; set; }
        [Required]
        public bool BlockedAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public List<string> SelectedRoles { get; set; }
        public List<RoleViewModel> Roles { get; set; }
        public bool CanChangePassword { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsPasswordNeverExpires { get; set; }
        public Nullable<DateTime> LastLogin { get; set; }
        public bool IsLocked { get; set; }
        public UserAccountState State { get; set; }
        public int LoginAttempts { get; set; }
        public Nullable<DateTime> LoginAttemptsDate { get; set; }
        public string InitialPassword { get; set; }
        public Nullable<DateTime> InitialPasswordExpirationDate { get; set; }
        public int? OfficeId { get; set; }
        public string Office { get; set; }
        public List<SaveUserOrganizationalStructureViewModel> UserOrganizationalStructuresToSave { get; set; }
        public List<UserOrganizationalStructureViewModel> UserOrganizationalStructures { get; set; }
        public List<UserRequestTypesGroupingViewModel> UserRequestTypesGroupings { get; set; }
        public List<UserNotAllowedLinkTypeViewModel> UserNotAllowedLinkTypes { get; set; }
    }

    public class SaveUserModel
    {
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
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
        public bool EmailVerificationDisabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool Status { get; set; }
        [Required]
        public bool BlockedAccount { get; set; }
        public List<string> SelectedRoles { get; set; }
        public bool CanChangePassword { get; set; }
        public bool IsPasswordNeverExpires { get; set; }
        public bool IsLocked { get; set; }
        public UserAccountState State { get; set; }
        public int? OfficeId { get; set; }
        public List<SaveUserOrganizationalStructureViewModel> UserOrganizationalStructuresToSave { get; set; }
        public List<UserRequestTypesGroupingViewModel> UserRequestTypesGroupings { get; set; }
        public List<UserNotAllowedLinkTypeViewModel> UserNotAllowedLinkTypes { get; set; }
    }

    public class UserBasicInfoModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public bool Status { get; set; }
    }

    public class UserListViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public bool Status { get; set; }
    }
}