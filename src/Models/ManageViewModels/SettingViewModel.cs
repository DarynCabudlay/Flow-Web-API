using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class SettingViewModel
    {
        public bool UserRegister { get; set; }
        public bool EmailVerificationDisable { get; set; }
        [Required]
        public string UserRole { get; set; }
        public bool RecoverPassword { get; set; }
        public bool ChangePassword { get; set; }
        public bool ChangeProfile { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool ExternalLogin { get; set; }

        [Required]
        public string SiteName { get; set; }
        [Required]
        public string SiteCode { get; set; }
        [Required]
        public bool SiteOffline { get; set; }
        public string SiteOfflineMessage { get; set; }
        public string SiteMetaDescription { get; set; }
        public string SiteMetaKeyword { get; set; }
        public string SiteRobots { get; set; }
        public string SiteAuthor { get; set; }
        public string ContentRight { get; set; }
        [Required]
        public string Theme { get; set; }

        [Required]
        public int PasswordValidity { get; set; }
        [Required]
        public int PasswordExpirationReminder { get; set; }
        [Required]
        public string IPAddress { get; set; }
        [Required]
        public string ServerEnvironment { get; set; }
        [Required]
        public string SecretKey { get; set; }
        [Required]
        public string DefaultIndex { get; set; }
        [Required]
        public string ProjectDirectory { get; set; }
        [Required]
        public string UploadPath { get; set; }
        [Required]
        public string ReportPath { get; set; }
        [Required]
        public string SMSApiKey { get; set; }
        [Required]
        public string SMSApiSecret { get; set; }
        [Required]
        public string HttpMethod { get; set; }
        [Required]
        public string CallbackInboundMessage { get; set; }
        [Required]
        public string CallbackDeliveryReceipt { get; set; }
        [Required]
        public string FromEmail { get; set; }
        [Required]
        public string FromName { get; set; }
        [Required]
        public string ReplyToEmail { get; set; }
        [Required]
        public string ReplyName { get; set; }
        [Required]
        public string Mailer { get; set; }
        [Required]
        public string SMTPHost { get; set; }
        [Required]
        public string SMTPPort { get; set; }
        [Required]
        public string SMTPSecurity { get; set; }
        [Required]
        public bool SMTPAuthentication { get; set; }
        [Required]
        public bool SMTPEnableSSL { get; set; }

        [Required]
        public string Company { get; set; }
        public string CorporateAddress { get; set; }
        public string CorporateContactNos { get; set; }
        public string CorporateFaxNos { get; set; }
        public string CorporateEmail { get; set; }
        public string CorporateWebsite { get; set; }
        public string FacebookPage { get; set; }
        public string TwitterPage { get; set; }
        public string InstagramPage { get; set; }
        public string YoutubePage { get; set; }
        public string LinkedinPage { get; set; }
        public string PinterestPage { get; set; }
        //[Required]
        public int MaxLoginAttempts { get; set; }
        //[Required]
        public string Domain { get; set; }
        //[Required]
        public int DormatStateEffectivity { get; set; }
        //[Required]
        public int InitialPasswordValidity { get; set; }

        public int AttachmentMaximumLength { get; set; }

        public string DraftAttachmentDirectory { get; set; }

        public string FinalizedAttachmentDirectory { get; set; }
        public int NoOfAllowedLinksInTransaction { get; set; }
        public int DraftTicketRetentionPeriod { get; set; }

    }
}