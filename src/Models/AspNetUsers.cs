using System;
using System.Collections.Generic;

namespace workflow.Models
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
            AspNetUserTokens = new HashSet<AspNetUserTokens>();
            AspNetUsersLoginHistory = new HashSet<AspNetUsersLoginHistory>();
            AspNetUsersPageVisited = new HashSet<AspNetUsersPageVisited>();
            UserPasswordPolicies = new HashSet<UserPasswordPolicy>();
            UserPreferences = new HashSet<UserPreferences>();
            RequestTypeProcessOwners = new HashSet<RequestTypeProcessOwner>();
            LockedRequestTypes = new HashSet<LockedRequestType>();
            UserOrganizationalStructures = new HashSet<UserOrganizationalStructure>();
            UserNotAllowedLinkTypes = new HashSet<UserNotAllowedLinkType>();
            UserRequestTypesGroupings = new HashSet<UserRequestTypesGrouping>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool Status { get; set; }
        public bool BlockedAccount { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime Date { get; set; }
        public int LoginAttempts { get; set; }
        public Nullable<DateTime> LoginAttemptsDate { get; set; }
        public Nullable<DateTime> LastLogin { get; set; }
        public bool Islocked { get; set; }
        public int State { get; set; }
        public virtual AspNetUsersProfile AspNetUsersProfile { get; set; }
        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<AspNetUsersLoginHistory> AspNetUsersLoginHistory { get; set; }
        public virtual ICollection<AspNetUsersPageVisited> AspNetUsersPageVisited { get; set; }
        public virtual ICollection<UserPasswordPolicy> UserPasswordPolicies { get; set; }
        public virtual ICollection<UserPreferences> UserPreferences { get; set; }
        public virtual ICollection<RequestTypeProcessOwner> RequestTypeProcessOwners { get; set; }
        public virtual ICollection<LockedRequestType> LockedRequestTypes { get; set; }
        public virtual ICollection<UserOrganizationalStructure> UserOrganizationalStructures { get; set; }
        public virtual ICollection<UserNotAllowedLinkType> UserNotAllowedLinkTypes { get; set; }
        public virtual ICollection<UserRequestTypesGrouping> UserRequestTypesGroupings { get; set; }
    }
}
