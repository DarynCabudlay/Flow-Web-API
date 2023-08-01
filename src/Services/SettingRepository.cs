using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;
using workflow.Models.ManageViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using workflow.Data;

namespace workflow.Services
{
    public class SettingRepository : BaseApi, ISetting
    {

        public SettingRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<SettingRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public List<SettingViewModel> GetSettingData()
        {
            try
            {
                List<SettingViewModel> CustomSettingsList = new();

                var settings = _dbCntxt.Setting.ToList();

                if (settings.Count > 0)
                {
                    SettingViewModel customSettings = new()
                    {
                        UserRegister = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_USERREGISTER").FirstOrDefault().VSettingOption),
                        EmailVerificationDisable = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_EMAILVERIFICATION").FirstOrDefault().VSettingOption),
                        UserRole = settings.Where(x => x.VSettingId == "_USERROLE").FirstOrDefault().VSettingOption,
                        RecoverPassword = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_RECOVERPASSWORD").FirstOrDefault().VSettingOption),
                        ChangePassword = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_CHANGEPASSWORD").FirstOrDefault().VSettingOption),
                        ChangeProfile = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_CHANGEPROFILE").FirstOrDefault().VSettingOption),
                        TwoFactorEnabled = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_TWOFACTORENABLED").FirstOrDefault().VSettingOption),
                        ExternalLogin = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_EXTERNALLOGIN").FirstOrDefault().VSettingOption),
                        SiteName = settings.Where(x => x.VSettingId == "_SITENAME").FirstOrDefault().VSettingOption,
                        SiteCode = settings.Where(x => x.VSettingId == "_SITECODE").FirstOrDefault().VSettingOption,
                        SiteOffline = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_SITEOFFLINE").FirstOrDefault().VSettingOption),
                        SiteOfflineMessage = settings.Where(x => x.VSettingId == "_SITEOFFLINEMESSAGE").FirstOrDefault().VSettingOption,
                        SiteMetaDescription = settings.Where(x => x.VSettingId == "_SITEMETADESCRIPTION").FirstOrDefault().VSettingOption,
                        SiteMetaKeyword = settings.Where(x => x.VSettingId == "_SITEMETAKEYWORD").FirstOrDefault().VSettingOption,
                        SiteRobots = settings.Where(x => x.VSettingId == "_SITEROBOTS").FirstOrDefault().VSettingOption,
                        SiteAuthor = settings.Where(x => x.VSettingId == "_SITEAUTHOR").FirstOrDefault().VSettingOption,
                        ContentRight = settings.Where(x => x.VSettingId == "_CONTENTRIGHT").FirstOrDefault().VSettingOption,
                        Theme = settings.Where(x => x.VSettingId == "_DEFAULTTHEME").FirstOrDefault().VSettingOption,
                        PasswordValidity = Convert.ToInt16(settings.Where(x => x.VSettingId == "_PASSWORDVALIDITY").FirstOrDefault().VSettingOption),
                        PasswordExpirationReminder = Convert.ToInt16(settings.Where(x => x.VSettingId == "_PASSWORDEXPIRATIONREMINDER").FirstOrDefault().VSettingOption),
                        IPAddress = settings.Where(x => x.VSettingId == "_IPADDRESS").FirstOrDefault().VSettingOption,
                        ServerEnvironment = settings.Where(x => x.VSettingId == "_SERVERENVIRONMENT").FirstOrDefault().VSettingOption,
                        DefaultIndex = settings.Where(x => x.VSettingId == "_DEFAULTHOMEPAGE").FirstOrDefault().VSettingOption,
                        SecretKey = settings.Where(x => x.VSettingId == "_SECRETKEY").FirstOrDefault().VSettingOption,
                        ProjectDirectory = settings.Where(x => x.VSettingId == "_PROJECTDIR").FirstOrDefault().VSettingOption,
                        UploadPath = settings.Where(x => x.VSettingId == "_UPLOADPATH").FirstOrDefault().VSettingOption,
                        ReportPath = settings.Where(x => x.VSettingId == "_REPORTHPATH").FirstOrDefault().VSettingOption,
                        SMSApiKey = settings.Where(x => x.VSettingId == "_SMSAPIKEY").FirstOrDefault().VSettingOption,
                        SMSApiSecret = settings.Where(x => x.VSettingId == "_SMSAPISECRET").FirstOrDefault().VSettingOption,
                        HttpMethod = settings.Where(x => x.VSettingId == "_HTTPMETHOD").FirstOrDefault().VSettingOption,
                        CallbackInboundMessage = settings.Where(x => x.VSettingId == "_CALLBACKINBOUNDMESSAGE").FirstOrDefault().VSettingOption,
                        CallbackDeliveryReceipt = settings.Where(x => x.VSettingId == "_CALLBACKDELIVERYRECEIPT").FirstOrDefault().VSettingOption,
                        FromEmail = settings.Where(x => x.VSettingId == "_FROMEMAIL").FirstOrDefault().VSettingOption,
                        FromName = settings.Where(x => x.VSettingId == "_FROMNAME").FirstOrDefault().VSettingOption,
                        ReplyToEmail = settings.Where(x => x.VSettingId == "_REPLYTOEMAIL").FirstOrDefault().VSettingOption,
                        ReplyName = settings.Where(x => x.VSettingId == "_REPLYNAME").FirstOrDefault().VSettingOption,
                        Mailer = settings.Where(x => x.VSettingId == "_MAILER").FirstOrDefault().VSettingOption,
                        SMTPHost = settings.Where(x => x.VSettingId == "_SMTPHOST").FirstOrDefault().VSettingOption,
                        SMTPPort = settings.Where(x => x.VSettingId == "_SMTPPORT").FirstOrDefault().VSettingOption,
                        SMTPSecurity = settings.Where(x => x.VSettingId == "_SMTPSECURITY").FirstOrDefault().VSettingOption,
                        SMTPAuthentication = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_SMTPAUTHENTICATION").FirstOrDefault().VSettingOption),
                        SMTPEnableSSL = Convert.ToBoolean(settings.Where(x => x.VSettingId == "_SMTPENABLESSL").FirstOrDefault().VSettingOption),
                        Company = settings.Where(x => x.VSettingId == "_COMPANY").FirstOrDefault().VSettingOption,
                        CorporateAddress = settings.Where(x => x.VSettingId == "_CORPORATEADDRESS").FirstOrDefault().VSettingOption,
                        CorporateContactNos = settings.Where(x => x.VSettingId == "_CORPORATECONTACTNOS").FirstOrDefault().VSettingOption,
                        CorporateFaxNos = settings.Where(x => x.VSettingId == "_CORPORATEFAXNOS").FirstOrDefault().VSettingOption,
                        CorporateEmail = settings.Where(x => x.VSettingId == "_CORPORATEEMAIL").FirstOrDefault().VSettingOption,
                        CorporateWebsite = settings.Where(x => x.VSettingId == "_CORPORATEWEBSITE").FirstOrDefault().VSettingOption,
                        FacebookPage = settings.Where(x => x.VSettingId == "_FACEBOOKPAGE").FirstOrDefault().VSettingOption,
                        TwitterPage = settings.Where(x => x.VSettingId == "_TWITTERPAGE").FirstOrDefault().VSettingOption,
                        InstagramPage = settings.Where(x => x.VSettingId == "_INSTAGRAMPAGE").FirstOrDefault().VSettingOption,
                        YoutubePage = settings.Where(x => x.VSettingId == "_YOUTUBEPAGE").FirstOrDefault().VSettingOption,
                        LinkedinPage = settings.Where(x => x.VSettingId == "_LINKEDINPAGE").FirstOrDefault().VSettingOption,
                        PinterestPage = settings.Where(x => x.VSettingId == "_PINTERESTPAGE").FirstOrDefault().VSettingOption,
                        MaxLoginAttempts = Convert.ToInt32(settings.Where(x => x.VSettingId == "_MAXLOGINATTEMPTS").FirstOrDefault().VSettingOption),
                        Domain = settings.Where(x => x.VSettingId == "_DOMAIN").FirstOrDefault().VSettingOption,
                        DormatStateEffectivity = Convert.ToInt32(settings.Where(x => x.VSettingId == "_DORMANTSTATEEFFECTIVITY").FirstOrDefault().VSettingOption),
                        InitialPasswordValidity = Convert.ToInt32(settings.Where(x => x.VSettingId == "_INITIALPASSWORDVALIDITY").FirstOrDefault().VSettingOption),

                        AttachmentMaximumLength = Convert.ToInt32(settings.Where(x => x.VSettingId == "_ATTACHMENTMAXIMUMLENGTH").FirstOrDefault().VSettingOption),

                        DraftAttachmentDirectory = settings.Where(x => x.VSettingId == "_DRAFTATTACHMENTDIRECTORY").FirstOrDefault().VSettingOption,

                        FinalizedAttachmentDirectory = settings.Where(x => x.VSettingId == "_FINALIZEDATTACHMENTDIRECTORY").FirstOrDefault().VSettingOption,

                        NoOfAllowedLinksInTransaction = Convert.ToInt32(settings.Where(x => x.VSettingId == "_NOOFALLOWEDLINKSINTRANSACTION").FirstOrDefault().VSettingOption),

                        DraftTicketRetentionPeriod = Convert.ToInt32(settings.Where(x => x.VSettingId == "_DRAFTTICKETRETENTIONPERIOD").FirstOrDefault().VSettingOption),

                    };
                    
                    CustomSettingsList.Add(customSettings);
                }

                return CustomSettingsList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        public async Task Update(SettingViewModel model)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var settings = _dbCntxt.Setting.ToList();
                bool preTFE = false;
                bool postTFE = false;
                foreach (var st in settings)
                {
                    if (st.VSettingId == "_USERREGISTER")
                        st.VSettingOption = model.UserRegister.ToString();
                    else if (st.VSettingId == "_EMAILVERIFICATION")
                        st.VSettingOption = model.EmailVerificationDisable.ToString();
                    else if (st.VSettingId == "_USERROLE")
                        st.VSettingOption = model.UserRole;
                    else if (st.VSettingId == "_RECOVERPASSWORD")
                        st.VSettingOption = model.RecoverPassword.ToString();
                    else if (st.VSettingId == "_CHANGEPASSWORD")
                        st.VSettingOption = model.ChangePassword.ToString();
                    else if (st.VSettingId == "_CHANGEPROFILE")
                        st.VSettingOption = model.ChangeProfile.ToString();
                    else if (st.VSettingId == "_TWOFACTORENABLED")
                    {
                        preTFE = Convert.ToBoolean(st.VSettingOption);
                        postTFE = model.TwoFactorEnabled;
                        st.VSettingOption = model.TwoFactorEnabled.ToString();
                    }
                    else if (st.VSettingId == "_EXTERNALLOGIN")
                        st.VSettingOption = model.ExternalLogin.ToString();

                    else if (st.VSettingId == "_SITENAME")
                        st.VSettingOption = model.SiteName;
                    else if (st.VSettingId == "_SITECODE")
                        st.VSettingOption = model.SiteCode;
                    else if (st.VSettingId == "_SITEOFFLINE")
                        st.VSettingOption = model.SiteOffline.ToString();
                    else if (st.VSettingId == "_SITEOFFLINEMESSAGE")
                        st.VSettingOption = model.SiteOfflineMessage;
                    else if (st.VSettingId == "_SITEMETADESCRIPTION")
                        st.VSettingOption = model.SiteMetaDescription;
                    else if (st.VSettingId == "_SITEMETAKEYWORD")
                        st.VSettingOption = model.SiteMetaKeyword;
                    else if (st.VSettingId == "_SITEROBOTS")
                        st.VSettingOption = model.SiteRobots;
                    else if (st.VSettingId == "_SITEAUTHOR")
                        st.VSettingOption = model.SiteAuthor;
                    else if (st.VSettingId == "_CONTENTRIGHT")
                        st.VSettingOption = model.ContentRight;
                    else if (st.VSettingId == "_DEFAULTTHEME")
                        st.VSettingOption = model.Theme;
                    else if (st.VSettingId == "_PASSWORDVALIDITY")
                        st.VSettingOption = model.PasswordValidity.ToString();
                    else if (st.VSettingId == "_PASSWORDEXPIRATIONREMINDER")
                        st.VSettingOption = model.PasswordExpirationReminder.ToString();
                    else if (st.VSettingId == "_IPADDRESS")
                        st.VSettingOption = model.IPAddress;
                    else if (st.VSettingId == "_SERVERENVIRONMENT")
                        st.VSettingOption = model.ServerEnvironment;
                    else if (st.VSettingId == "_DEFAULTHOMEPAGE")
                        st.VSettingOption = model.DefaultIndex;
                    else if (st.VSettingId == "_SECRETKEY")
                        st.VSettingOption = model.SecretKey;
                    else if (st.VSettingId == "_PROJECTDIR")
                        st.VSettingOption = model.ProjectDirectory;
                    else if (st.VSettingId == "_UPLOADPATH")
                        st.VSettingOption = model.UploadPath;
                    else if (st.VSettingId == "_REPORTHPATH")
                        st.VSettingOption = model.ReportPath;
                    else if (st.VSettingId == "_SMSAPIKEY")
                        st.VSettingOption = model.SMSApiKey;
                    else if (st.VSettingId == "_SMSAPISECRET")
                        st.VSettingOption = model.SMSApiSecret;
                    else if (st.VSettingId == "_HTTPMETHOD")
                        st.VSettingOption = model.HttpMethod;
                    else if (st.VSettingId == "_CALLBACKINBOUNDMESSAGE")
                        st.VSettingOption = model.CallbackInboundMessage;
                    else if (st.VSettingId == "_CALLBACKDELIVERYRECEIPT")
                        st.VSettingOption = model.CallbackDeliveryReceipt;
                    else if (st.VSettingId == "_FROMEMAIL")
                        st.VSettingOption = model.FromEmail;
                    else if (st.VSettingId == "_FROMNAME")
                        st.VSettingOption = model.FromName;
                    else if (st.VSettingId == "_REPLYTOEMAIL")
                        st.VSettingOption = model.ReplyToEmail;
                    else if (st.VSettingId == "_REPLYNAME")
                        st.VSettingOption = model.ReplyName;
                    else if (st.VSettingId == "_MAILER")
                        st.VSettingOption = model.Mailer;
                    else if (st.VSettingId == "_SMTPHOST")
                        st.VSettingOption = model.SMTPHost;
                    else if (st.VSettingId == "_SMTPPORT")
                        st.VSettingOption = model.SMTPPort;
                    else if (st.VSettingId == "_SMTPSECURITY")
                        st.VSettingOption = model.SMTPSecurity;
                    else if (st.VSettingId == "_SMTPAUTHENTICATION")
                        st.VSettingOption = model.SMTPAuthentication.ToString();
                    else if (st.VSettingId == "_SMTPENABLESSL")
                        st.VSettingOption = model.SMTPEnableSSL.ToString();
                    else if (st.VSettingId == "_COMPANY")
                        st.VSettingOption = model.Company;
                    else if (st.VSettingId == "_CORPORATEADDRESS")
                        st.VSettingOption = model.CorporateAddress;
                    else if (st.VSettingId == "_CORPORATECONTACTNOS")
                        st.VSettingOption = model.CorporateContactNos;
                    else if (st.VSettingId == "_CORPORATEFAXNOS")
                        st.VSettingOption = model.CorporateFaxNos;
                    else if (st.VSettingId == "_CORPORATEEMAIL")
                        st.VSettingOption = model.CorporateEmail;
                    else if (st.VSettingId == "_CORPORATEWEBSITE")
                        st.VSettingOption = model.CorporateWebsite;
                    else if (st.VSettingId == "_FACEBOOKPAGE")
                        st.VSettingOption = model.FacebookPage;
                    else if (st.VSettingId == "_TWITTERPAGE")
                        st.VSettingOption = model.TwitterPage;
                    else if (st.VSettingId == "_INSTAGRAMPAGE")
                        st.VSettingOption = model.InstagramPage;
                    else if (st.VSettingId == "_YOUTUBEPAGE")
                        st.VSettingOption = model.YoutubePage;
                    else if (st.VSettingId == "_LINKEDINPAGE")
                        st.VSettingOption = model.LinkedinPage;
                    else if (st.VSettingId == "_PINTERESTPAGE")
                        st.VSettingOption = model.PinterestPage;
                    else if (st.VSettingId == "_MAXLOGINATTEMPTS")
                    {
                        if (model.MaxLoginAttempts == 0)
                            model.MaxLoginAttempts = 3;
                        st.VSettingOption = model.MaxLoginAttempts.ToString();
                    }

                    else if (st.VSettingId == "_DOMAIN")
                    {
                        if (String.IsNullOrWhiteSpace(model.Domain))
                            model.Domain = "http://localhost:4200";
                        st.VSettingOption = model.Domain;
                    }
                    else if (st.VSettingId == "_DORMANTSTATEEFFECTIVITY")
                    {
                        if (model.DormatStateEffectivity == 0)
                            model.DormatStateEffectivity = 90;
                        st.VSettingOption = model.DormatStateEffectivity.ToString();
                    }
                    else if (st.VSettingId == "_INITIALPASSWORDVALIDITY")
                    {
                        if (model.InitialPasswordValidity == 0)
                            model.InitialPasswordValidity = 7;
                        st.VSettingOption = model.InitialPasswordValidity.ToString();
                    }
                    else if (st.VSettingId == "_ATTACHMENTMAXIMUMLENGTH")
                    {
                        if (model.AttachmentMaximumLength == 0)
                            model.AttachmentMaximumLength = 20;
                        st.VSettingOption = model.AttachmentMaximumLength.ToString();
                    }
                    else if (st.VSettingId == "_DRAFTATTACHMENTDIRECTORY")
                        st.VSettingOption = model.DraftAttachmentDirectory;
                    else if (st.VSettingId == "_FINALIZEDATTACHMENTDIRECTORY")
                        st.VSettingOption = model.FinalizedAttachmentDirectory;
                    else if (st.VSettingId == "_NOOFALLOWEDLINKSINTRANSACTION")
                    {
                        if (model.NoOfAllowedLinksInTransaction == 0)
                            model.NoOfAllowedLinksInTransaction = 5;
                        st.VSettingOption = model.NoOfAllowedLinksInTransaction.ToString();
                    }
                    else if (st.VSettingId == "_DRAFTTICKETRETENTIONPERIOD")
                    {
                        if (model.DraftTicketRetentionPeriod == 0)
                            model.DraftTicketRetentionPeriod = 1;
                        st.VSettingOption = model.DraftTicketRetentionPeriod.ToString();
                    }

                    _dbCntxt.Entry(st).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }

                if (preTFE)
                {
                    if (!postTFE)
                    {
                        _dbCntxt.AspNetUsers
                        .Where(x => x.TwoFactorEnabled == true)
                        .ToList()
                        .ForEach(y => y.TwoFactorEnabled = false);

                        _dbCntxt.SaveChanges();
                    }
                }

                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
