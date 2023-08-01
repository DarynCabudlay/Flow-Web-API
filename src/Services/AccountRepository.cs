using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using workflow.Models;
using workflow.Models.ManageViewModels;
using workflow.Models.ManageViewModels.Seed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using workflow.Models.AccountViewModels;
using System.Text.Encodings.Web;
using workflow.Helpers;
using Microsoft.EntityFrameworkCore.Storage;
using workflow.Data;

namespace workflow.Services
{
    public class AccountRepository : BaseApi, IAccount
    {
        readonly IUser _user;
        readonly IEmailSender _emailSender;
        readonly UrlEncoder _urlEncoder;

        public AccountRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<AccountRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor,
                IEmailSender emailSender,
                UrlEncoder urlEncoder,
                IUser user) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
            _user = user;
        }

        public async Task<Boolean> Register(RegisterViewModel model)
        {
            try
            {
                if (!_dbCntxt.Setting.Any())
                {
                    var settingData = System.IO.File.ReadAllText("Data/SettingSeedData.json");
                    var settings = JsonConvert.DeserializeObject<List<Setting>>(settingData);
                    foreach (var setting in settings)
                    {
                        _dbCntxt.Setting.Add(setting);
                    }
                    await _dbCntxt.SaveChangesAsync();
                }

                if (!_dbCntxt.AspNetUsersMenu.Any())
                {
                    var menuData = System.IO.File.ReadAllText("Data/MenuSeedData.json");
                    var menus = JsonConvert.DeserializeObject<List<AspNetUsersMenu>>(menuData);
                    foreach (var AspNetUsersMenu in menus)
                    {
                        _dbCntxt.AspNetUsersMenu.Add(AspNetUsersMenu);
                    }
                    await _dbCntxt.SaveChangesAsync();
                }

                if (!_dbCntxt.Options.Any())
                {
                    var optionData = System.IO.File.ReadAllText("Data/OptionSeedData.json");
                    var options = JsonConvert.DeserializeObject<List<Options>>(optionData);
                    foreach (var Option in options)
                    {
                        _dbCntxt.Options.Add(Option);
                    }
                    await _dbCntxt.SaveChangesAsync();
                }

                if (!_dbCntxt.AspNetUsersMenuControl.Any())
                {
                    var menuControl = System.IO.File.ReadAllText("Data/MenuControlSeedData.json");
                    var menuControls = JsonConvert.DeserializeObject<List<MenuControlToSeedViewModel>>(menuControl);

                    foreach (var control in menuControls)
                    {
                        int buttonid = _dbCntxt.Options
                                                       .FirstOrDefault
                                                       (
                                                            o => o.OptionGroup == "Button Controls" &&
                                                            o.Name == control.ActionButton
                                                       )
                                                       .Id;

                        var controlindb = new AspNetUsersMenuControl();
                        
                        controlindb.MenuControlId = control.MenuControlId;
                        controlindb.VMenuId = control.MenuId;
                        controlindb.OptionId = buttonid;

                        _dbCntxt.AspNetUsersMenuControl.Add(controlindb);
                    }
                    await _dbCntxt.SaveChangesAsync();
                }

                if (!_dbCntxt.WorkFlowStepTypes.Any())
                {
                    var workflowTypeData = System.IO.File.ReadAllText("Data/WorkFlowStepTypeSeedData.json");
                    var workflowTypesData = JsonConvert.DeserializeObject<List<WorkFlowStepTypeToSeedViewModel>>(workflowTypeData);
                    foreach (var workflowtype in workflowTypesData)
                    {
                        var workflowtypedb = new WorkFlowStepType();
                        workflowtypedb.Name = workflowtype.Name;
                        workflowtypedb.Color = workflowtype.Color;
                        workflowtypedb.Published = workflowtype.Published;
                        workflowtypedb.ApplyApprovalPolicyForAssignees = workflowtype.ApplyApprovalPolicyForAssignees;

                        _dbCntxt.WorkFlowStepTypes.Add(workflowtypedb);
                        _dbCntxt.SaveChanges();

                        if (workflowtype.ActionButtons.Count > 0)
                        {
                            foreach(var button in workflowtype.ActionButtons)
                            {

                                int buttonid = _dbCntxt.Options
                                                       .FirstOrDefault
                                                       (
                                                            o => o.OptionGroup == "WorkFlow Step Actions" && 
                                                            o.Name == button.ActionButton
                                                       )
                                                       .Id;

                                var workflowtypebutton = new WorkFlowStepTypeButton();
                                workflowtypebutton.StepTypeId = workflowtypedb.Id;
                                workflowtypebutton.ActionButtonId = buttonid;
                                workflowtypebutton.IsActionToNextStep = button.IsActionToNextStep;

                                _dbCntxt.WorkFlowStepTypeButtons.Add(workflowtypebutton);
                                _dbCntxt.SaveChanges();
                            }
                        }
                    }
                    
                }

                if (!_dbCntxt.ComparisonOperators.Any())
                {
                    var comparisonOperatorData = System.IO.File.ReadAllText("Data/ComparisonOperatorSeedData.json");
                    var comparisonOperatorsData = JsonConvert.DeserializeObject<List<ComparisonOperatorToSeedViewModel>>(comparisonOperatorData);

                    foreach (var comparisonoperator in comparisonOperatorsData)
                    {
                        ComparisonOperator ops = new()
                        {
                            Name = comparisonoperator.Name,
                            ApplicableDataTypes = comparisonoperator.ApplicableDataTypes,
                            ApplicableFieldTypes = comparisonoperator.ApplicableFieldTypes,
                            TechnicalEquivalent = comparisonoperator.TechnicalEquivalent,
                            TechnicalEquivalent2 = comparisonoperator.TechnicalEquivalent2,
                            UseInMultipleTypes = comparisonoperator.UseInMultipleTypes
                        };

                        _dbCntxt.ComparisonOperators.Add(ops);
                    }
                    await _dbCntxt.SaveChangesAsync();

                }

                if (!_dbCntxt.OrganizationEntities.Any())
                {
                    var organizationEntityData = System.IO.File.ReadAllText("Data/OrganizationEntitySeedData.json");
                    var organizationEntitiesData = JsonConvert.DeserializeObject<List<OrganizationEntityToSeedViewModel>>(organizationEntityData);

                    foreach (var organizationEntity in organizationEntitiesData)
                    {
                        OrganizationEntity org = new()
                        {
                            Name = organizationEntity.Name,
                            Hierarchy = organizationEntity.Hierarchy,
                            Published = organizationEntity.Published,
                            WebClass = organizationEntity.WebClass,
                            CreatedByPK = organizationEntity.CreatedBy,
                            CreatedDate = DateTime.Now,
                            ModifiedByPK = organizationEntity.ModifiedBy,
                            ModifiedDate = organizationEntity.ModifiedDate
                        };

                        _dbCntxt.OrganizationEntities.Add(org);
                    }
                    await _dbCntxt.SaveChangesAsync();
                }

                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var roleId = "";
                    bool isEmailVerificationDisabled = false;
                    if (_user.FindAll().Any())
                    {
                        if (_user.FindAll().Where(x => x.Email == model.Email).Any())
                            throw new CustomException("User with the same email already taken.", 400);
                        if (_user.FindAll().Where(x => x.UserName == model.UserName).Any())
                            throw new CustomException("User with the same username already taken.", 400);
                        else
                        {
                            roleId = _dbCntxt.Setting.Where(x => x.VSettingId == "_USERROLE").FirstOrDefault().VSettingOption;

                            if (!_dbCntxt.AspNetRoles.Where(x => x.Id == roleId).Any())
                                throw new CustomException("User registration role not set at settings. Please go to settings page and set the user registration role.", 400);
                        }
                        isEmailVerificationDisabled = Convert.ToBoolean(_dbCntxt.Setting.Where(x => x.VSettingId == "_EMAILVERIFICATION").FirstOrDefault().VSettingOption);
                    }
                    else
                    {
                        roleId = "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03";

                        List<AspNetUsersMenu> anuml = _dbCntxt.AspNetUsersMenu.OrderBy(x => x.ISerialNo).ToList();

                        AspNetRoles role = new()
                        {
                            Id = roleId,
                            Name = "Super Admin",
                            IndexPage = _dbCntxt.AspNetUsersMenu.Where(x => x.VMenuId == "03D5B7E1-117D-429F-89AC-DE59E30B9386").FirstOrDefault().NvPageUrl,
                            Published = true,
                            CreatedDate = DateTime.Now,
                            CreatedByPK = "SYSTEMDEFAULT"
                        };
                        _dbCntxt.AspNetRoles.Add(role);
                        _dbCntxt.SaveChanges();

                        foreach (var mn in anuml)
                        {
                            AspNetUsersMenuPermission anump = new()
                            {
                                VMenuPermissionId = GeneralHelper.GenerateGuidAsUniqueKey(),
                                RoleId = roleId,
                                VMenuId = mn.VMenuId
                            };
                            _dbCntxt.AspNetUsersMenuPermission.Add(anump);
                            _dbCntxt.SaveChanges();

                            var controlsinmenu = _dbCntxt.AspNetUsersMenuControl.Where(m => m.VMenuId == mn.VMenuId).ToList();

                            if (controlsinmenu.Count > 0)
                            {
                                foreach(var controlinmenu in controlsinmenu)
                                {
                                    AspNetUsersMenuPermissionControl permissioncontrol = new()
                                    {
                                        Id = GeneralHelper.GenerateGuidAsUniqueKey(),
                                        VMenuPermissionId = anump.VMenuPermissionId,
                                        MenuControlId = controlinmenu.MenuControlId
                                    };
                                    _dbCntxt.AspNetUsersMenuPermissionControl.Add(permissioncontrol);
                                    _dbCntxt.SaveChanges();
                                }
                            }
                        }
                    }

                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = isEmailVerificationDisabled, date = DateTime.UtcNow };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        user = await _userManager.FindByEmailAsync(model.Email);

                        var aspuser = _dbCntxt.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();

                        aspuser.Status = true;
                        aspuser.State = Convert.ToInt32(UserAccountState.Active); // as default
                        _dbCntxt.Entry(aspuser).State = EntityState.Modified;
                        await _dbCntxt.SaveChangesAsync();

                        AspNetUserRoles anur = new()
                        {
                            UserId = aspuser.Id,
                            RoleId = roleId
                        };
                        _dbCntxt.AspNetUserRoles.Add(anur);
                        _dbCntxt.SaveChanges();

                        UserPasswordPolicy userpass = new()
                        {
                            UserId = aspuser.Id,
                            CanChangePassword = true,
                            ExpirationDate = Convert.ToDateTime("12/31/9999"),
                            IsPasswordNeverExpires = true
                        };
                        _dbCntxt.UserPasswordPolicies.Add(userpass);
                        _dbCntxt.SaveChanges();

                        if (!isEmailVerificationDisabled)
                        {
                            var settings = await _dbCntxt.Setting.ToListAsync();

                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            // var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Host.Value);  //Only use when Core & Angular are in same domain
                            var host = _config.GetSection("AppSettings")["AngularPath"];
                            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, host);

                            var webRoot = System.IO.Directory.GetCurrentDirectory();
                            var sPath = System.IO.Path.Combine(webRoot, "EmailTemplate/email-confirmation.html");

                            //var builder = new StringBuilder();
                            //using (var reader = System.IO.File.OpenText(sPath))
                            //{
                            //    builder.Append(reader.ReadToEnd());
                            //}

                            //builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                            //TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                            //builder.Replace("{username}", myTI.ToTitleCase(model.UserName));
                            //builder.Replace("{appname}", _config.GetSection("AppSettings")["AppName"]);
                            //builder.Replace("{verifylink}", callbackUrl);
                            //builder.Replace("{twitterlink}", _config.GetSection("Company")["SocialTwitter"]);
                            //builder.Replace("{facebooklink}", _config.GetSection("Company")["SocialFacebook"]);
                            //builder.Replace("{companyname}", _config.GetSection("Company")["Company"]);
                            //builder.Replace("{companyemail}", _config.GetSection("Company")["MailAddress"]);

                            var builder = new StringBuilder();
                            using (var reader = System.IO.File.OpenText(sPath))
                            {
                                builder.Append(reader.ReadToEnd());
                            }

                            builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                            builder.Replace("{username}", myTI.ToTitleCase(model.UserName));

                            var appcode = settings.Where(s => s.VSettingId == "_SITECODE").Select(s => s.VSettingOption).FirstOrDefault();

                            if (appcode == "")
                                appcode = "FLOW";

                            var appname = settings.Where(s => s.VSettingId == "_SITENAME").Select(s => s.VSettingOption).FirstOrDefault();

                            if (appname == "")
                                appname = _config.GetSection("AppSettings")["AppName"];

                            builder.Replace("{appname}", appname);
                            builder.Replace("{verifylink}", callbackUrl);

                            var twitter = settings.Where(s => s.VSettingId == "_TWITTERPAGE").Select(s => s.VSettingOption).FirstOrDefault();

                            if (twitter == "")
                                twitter = _config.GetSection("Company")["SocialTwitter"];

                            builder.Replace("{twitterlink}", twitter);

                            var fbpage = settings.Where(s => s.VSettingId == "_FACEBOOKPAGE").Select(s => s.VSettingOption).FirstOrDefault();

                            if (fbpage == "")
                                fbpage = _config.GetSection("Company")["SocialFacebook"];

                            builder.Replace("{facebooklink}", fbpage);

                            var company = settings.Where(s => s.VSettingId == "_COMPANY").Select(s => s.VSettingOption).FirstOrDefault();

                            if (company == "")
                                company = _config.GetSection("Company")["Company"];

                            builder.Replace("{companyname}", company);

                            var companyemail = settings.Where(s => s.VSettingId == "_CORPORATEEMAIL").Select(s => s.VSettingOption).FirstOrDefault();

                            if (companyemail == "")
                                companyemail = _config.GetSection("Company")["MailAddress"];

                            builder.Replace("{companyemail}", companyemail);

                            await _emailSender.SendEmailConfirmationAsync(model.Email, builder.ToString());

                            await _signInManager.SignInAsync(user, isPersistent: false);
                        }

                        scope.Complete();

                        return isEmailVerificationDisabled;
                    }
                    else
                    {
                        scope.Dispose();
                        string errors = "";

                        foreach (var error in result.Errors)
                        {
                            errors += error.Description + " " + Environment.NewLine;
                        }

                        throw new CustomException(errors, 400);
                    }
                }
                catch (CustomException customex)
                {
                    scope.Dispose();
                    throw new CustomException(customex.Message, customex.StatusCode);
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task ConfirmEmail(string userId, string code)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    throw new CustomException("User not found in system to verify email.", 404);

                var result = await _userManager.ConfirmEmailAsync(user, code.Replace(" ", "+"));
                if (result.Succeeded)
                {
                    //Set state as active after confirmation
                    var useridb = _dbCntxt.AspNetUsers.Find(user.Id);
                    useridb.State = Convert.ToInt32(UserAccountState.Active);
                    _dbCntxt.Entry(useridb).State = EntityState.Modified;
                    StatusCode(201); //must be change to 204
                }
                else
                {
                    var codeparam = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, codeparam, Request.Host.Value);

                    var webRoot = System.IO.Directory.GetCurrentDirectory();
                    var sPath = System.IO.Path.Combine(webRoot, "EmailTemplate/email-confirmation.html");

                    var builder = new StringBuilder();
                    using (var reader = System.IO.File.OpenText(sPath))
                        builder.Append(reader.ReadToEnd());

                    builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                    builder.Replace("{username}", myTI.ToTitleCase(user.UserName));
                    builder.Replace("{appname}", _config.GetSection("AppSettings")["AppName"]);
                    builder.Replace("{verifylink}", HtmlEncoder.Default.Encode(callbackUrl));
                    builder.Replace("{twitterlink}", _config.GetSection("Company")["SocialTwitter"]);
                    builder.Replace("{facebooklink}", _config.GetSection("Company")["SocialFacebook"]);
                    builder.Replace("{companyname}", _config.GetSection("Company")["Company"]);
                    builder.Replace("{companyemail}", _config.GetSection("Company")["MailAddress"]);

                    await _emailSender.SendEmailConfirmationAsync(user.Email, builder.ToString());

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    throw new CustomException("Your mail verification token has expired. We send you another confirmation mail link to your email address.", 400);
                }
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<Object> Login(LoginViewModel model)
        {

            using (var _dbCntxtTransaction = _dbCntxt.Database.BeginTransaction())
            {
                try
                {
                    //TODO: once we'll implement username and email add during login
                    //bool isEmail = false;
                    //bool isUserName = false;
                    //ApplicationUser user = null;

                    //user = await _userManager.FindByEmailAsync(model.Email);

                    //if (user != null)
                    //    isEmail = true;
                    
                    //if (!isEmail)
                    //{
                    //    user = await _userManager.FindByNameAsync(model.Email);

                    //    if (user != null)
                    //        isUserName = true;
                    //}

                    //To be continued...
                    
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        //Get user info
                        var userindb = await _user.Find(user.Id).FirstOrDefaultAsync();

                        UserAccountState userstate = (UserAccountState)userindb.State;

                        //Check if user is deactivated
                        if (!userindb.Status)
                            throw new CustomException("User is already deactivated", 400, GeneralHelper.SetLoginReturn(7));

                        DateTime temporarypasswordexpirationdate = Convert.ToDateTime(userindb.InitialPasswordExpirationDate == null ? "12/31/9999" : userindb.InitialPasswordExpirationDate);

                        //Check if account is Initial Password Expired
                        if (userstate == UserAccountState.Initial_Password_Expired || (DateTime.Now > temporarypasswordexpirationdate && userstate != UserAccountState.Initial_Password_Expired))
                        {
                            if (userstate != UserAccountState.Initial_Password_Expired)
                                await this.UpdateUserAccountState(user.Id, UserAccountState.Initial_Password_Expired);

                            throw new CustomException("Your temporary credentials is already expired. Please contact your administrator", 400, GeneralHelper.SetLoginReturn(10));
                        }

                        if (userstate == UserAccountState.Initial_Password)
                        {
                            //var passresult = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

                            var passresult = await _userManager.CheckPasswordAsync(user, model.Password);
                            if (passresult)
                            {
                                //proceed even not yet confirmed since confirmation will be done after changing of password
                                var is2faenabled = user.TwoFactorEnabled;
                                var ULHID = "";

                                ULHID = GeneralHelper.GenerateGuidAsUniqueKey();

                                try
                                {

                                    DateTime loginDateTime = DateTime.UtcNow;
                                    //Update Last Login
                                    var newuser = _dbCntxt.AspNetUsers.Where(u => u.Id == user.Id).FirstOrDefault();
                                    newuser.LastLogin = loginDateTime;
                                    _dbCntxt.SaveChanges();

                                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                                    AspNetUsersLoginHistory anulh = new();
                                    anulh.VUlhid = ULHID;
                                    anulh.UserId = user.Id;
                                    anulh.DLogIn = loginDateTime;
                                    anulh.NvIpaddress = ip;
                                    _dbCntxt.AspNetUsersLoginHistory.Add(anulh);
                                    _dbCntxt.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message, ex.InnerException);
                                }

                                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings")["Token"]));
                                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

                                var roles = await _userManager.GetRolesAsync(user);
                                var rolesString = JsonConvert.SerializeObject(roles);

                                var tokeOptions = new JwtSecurityToken(
                                            issuer: _httpContextAccessor.HttpContext.Request.Host.Value,
                                            audience: _httpContextAccessor.HttpContext.Request.Host.Value,
                                            claims: new List<Claim>(
                                                new List<Claim> {
                                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                                    new Claim("userName",user.UserName),
                                    new Claim("email",user.Email),
                                    new Claim("role", rolesString),
                                    new Claim(JwtRegisteredClaimNames.Jti, GeneralHelper.GenerateGuidAsUniqueKey())
                                                }
                                            ),
                                            expires: DateTime.Now.AddDays(1),
                                            signingCredentials: signinCredentials
                                );

                                var tokenHandler = new JwtSecurityTokenHandler();

                                var loginresult = new
                                {
                                    token = tokenHandler.WriteToken(tokeOptions),
                                    user.Id,
                                    index = "/home/initialPassword",
                                    ulhid = ULHID,
                                    is2faenabled
                                };

                                await _signInManager.SignInAsync(user, isPersistent: false);

                                _dbCntxtTransaction.Commit();

                                return loginresult;
                            }
                            else
                                throw new CustomException("Temporary credentials did not match", 401, GeneralHelper.SetLoginReturn(9));
                        }

                        if (!user.EmailConfirmed)
                            throw new CustomException("Email is not yet verified", 400, GeneralHelper.SetLoginReturn(1));

                        //Check if account is blocked by admin
                        if (userindb.BlockedAccount)
                            throw new CustomException("User is blocked by administrator", 400, GeneralHelper.SetLoginReturn(5));



                        var settings = await _dbCntxt.Setting.ToListAsync();

                        int dormateffectivity = Convert.ToInt32(settings.FirstOrDefault(s => s.VSettingId == "_DORMANTSTATEEFFECTIVITY").VSettingOption);

                        //Check if dormant
                        if (userstate == UserAccountState.Dormant_User || ((DateTime.Now.Date - Convert.ToDateTime(userindb.LastLogin == null ? "12/31/9999" : userindb.LastLogin).Date).TotalDays > dormateffectivity && userstate != UserAccountState.Dormant_User))
                        {
                            if (userstate != UserAccountState.Dormant_User)
                            {
                                await this.UpdateLoginAttempts(user.Id, 0, null);
                                //update state to dormant
                                await this.UpdateUserAccountState(user.Id, UserAccountState.Dormant_User);
                            }
                            throw new CustomException("Your account is considered as stale/dormant account. Please contact your administrator", 400, GeneralHelper.SetLoginReturn(8));
                        }

                        //Check if hard locked
                        if (userstate == UserAccountState.Locked)
                            throw new CustomException("Your account is already locked. Please contact administrator", 400, GeneralHelper.SetLoginReturn(4));

                        int maxloginattempt = Convert.ToInt32(settings.FirstOrDefault(s => s.VSettingId == "_MAXLOGINATTEMPTS").VSettingOption);
                        int loginattemptsindb = userindb.LoginAttempts;
                        DateTime loginattemptsdateindb = Convert.ToDateTime(userindb.LoginAttemptsDate == null ? "1/1/1900" : userindb.LoginAttemptsDate);

                        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            //Check if soft locked
                            if (userstate == UserAccountState.Soft_Locked)
                            {
                                int logininterval = Convert.ToInt32((DateTime.Now - loginattemptsdateindb).TotalMinutes);

                                if (logininterval <= 15)
                                {
                                    throw new CustomException("Your account is already locked. Please login after " + (15 - logininterval) + " mins.", 400, GeneralHelper.SetLoginReturn(4));
                                }
                            }

                            bool isexpiredandcanchange = false;

                            if (!userindb.IsPasswordNeverExpires)
                            {
                                //Check if password is expired
                                if ((userstate == UserAccountState.Password_Expired && !userindb.CanChangePassword) || (DateTime.Now > userindb.ExpirationDate && !userindb.CanChangePassword && userstate != UserAccountState.Password_Expired))
                                {
                                    if (userstate != UserAccountState.Password_Expired)
                                    {
                                        await this.UpdateLoginAttempts(user.Id, 0, null);
                                        await this.UpdateUserAccountState(user.Id, UserAccountState.Password_Expired);
                                    }
                                    throw new CustomException("Password is already expired. Please contact administrator to reset your password.", 400, GeneralHelper.SetLoginReturn(2));
                                }
                                else if ((userstate == UserAccountState.Password_Expired && userindb.CanChangePassword) || (DateTime.Now > userindb.ExpirationDate && userindb.CanChangePassword && userstate != UserAccountState.Password_Expired))
                                {
                                    if (userstate != UserAccountState.Password_Expired)
                                    {
                                        await this.UpdateLoginAttempts(user.Id, 0, null);
                                        await this.UpdateUserAccountState(user.Id, UserAccountState.Password_Expired);
                                    }
                                    isexpiredandcanchange = true;
                                }
                            }

                            var loginResult = await LoginSuccess(user, isexpiredandcanchange);

                            //update state to active
                            if (!isexpiredandcanchange)
                            {
                                await this.UpdateLoginAttempts(user.Id, 0, null);
                                await this.UpdateUserAccountState(user.Id, UserAccountState.Active);
                            }

                            await _signInManager.SignInAsync(user, isPersistent: false);

                            _dbCntxtTransaction.Commit();
                            return loginResult;
                        }
                        else if (result.RequiresTwoFactor)
                        {
                            await this.UpdateLoginAttempts(user.Id, 0, null);
                            //update state to active
                            if (userstate != UserAccountState.Active)
                                await this.UpdateUserAccountState(user.Id, UserAccountState.Active);

                            _dbCntxtTransaction.Commit();
                            return new { is2faenabled = true, user.Id };
                        }
                        else
                        {
                            //Remove login attemtps every midnight not hard locked and dormant account
                            if ((DateTime.Now.Date - loginattemptsdateindb.Date).TotalDays > 0 && loginattemptsindb > 0)
                            {
                                await this.UpdateLoginAttempts(user.Id, 0, null);
                                //update state to active
                                await this.UpdateUserAccountState(user.Id, UserAccountState.Active);
                                userstate = UserAccountState.Active;
                                loginattemptsindb = 0;
                            }

                            //Check if soft locked
                            if (userstate == UserAccountState.Soft_Locked)
                            {
                                int logininterval = Convert.ToInt32((DateTime.Now - loginattemptsdateindb).TotalMinutes);

                                if (logininterval <= 15)
                                {
                                    await this.UpdateUserAccountState(user.Id, UserAccountState.Locked);
                                    throw new CustomException("Your account is already locked. Please contact administrator", 400, GeneralHelper.SetLoginReturn(4));
                                }
                                else
                                {
                                    await this.UpdateLoginAttempts(user.Id, 0, null);
                                    await this.UpdateUserAccountState(user.Id, UserAccountState.Active);
                                    userstate = UserAccountState.Active;
                                    loginattemptsindb = 0;
                                }
                            }

                            if (maxloginattempt > 0)
                            {
                                if (loginattemptsindb < maxloginattempt)
                                {
                                    //Increment 
                                    loginattemptsindb += 1;

                                    if (loginattemptsindb == maxloginattempt)
                                    {
                                        //user is already lock after all tries
                                        await this.UpdateLoginAttempts(user.Id, loginattemptsindb, DateTime.Now);
                                        //update state to SoftLocked
                                        await this.UpdateUserAccountState(user.Id, UserAccountState.Soft_Locked);
                                        throw new CustomException("Your account is already locked. Please login after 15 mins.", 400, GeneralHelper.SetLoginReturn(4));
                                    }
                                    else
                                    {
                                        await this.UpdateLoginAttempts(user.Id, loginattemptsindb, DateTime.Now);

                                        int tries = maxloginattempt - loginattemptsindb;

                                        throw new CustomException("Email or password did not match, kindly double check first since you only have " + tries + " tries left before lockout. ", 401, GeneralHelper.SetLoginReturn(3));
                                    }
                                }
                                else
                                {
                                    //user is already lock after all tries
                                    await this.UpdateLoginAttempts(user.Id, loginattemptsindb, DateTime.Now);
                                    //update state to SoftLocked
                                    await this.UpdateUserAccountState(user.Id, UserAccountState.Soft_Locked);
                                    throw new CustomException("Your account is already locked. Please login after 15 mins.", 400, GeneralHelper.SetLoginReturn(4));
                                }
                                    
                            }
                            else
                            {
                                throw new CustomException("Email or password did not match, kindly double check first.", 401, GeneralHelper.SetLoginReturn(3));
                            }
                        }
                    }
                    else
                        throw new CustomException("Email is not registered in the system.", 401, GeneralHelper.SetLoginReturn(6));
                }
                catch (CustomException customex)
                {
                    _dbCntxtTransaction.Commit();
                    throw new CustomException(customex.Message, customex.StatusCode, customex.Details);
                }
                catch (Exception ex)
                {
                    _dbCntxtTransaction.Rollback();
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
        }

        public Boolean IsAuthorized(string url)
        {
            try
            {
                var isAuthorized = true;
                if (url != "/home/profile")
                {
                    // get current user id
                    var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var roleIds = _dbCntxt.AspNetUserRoles.Where(x => x.UserId == cId).Select(r => r.RoleId).ToList();
                    var menus = _dbCntxt.AspNetUsersMenu.Where(x => x.NvPageUrl == url).ToList();
                    foreach (var menu in menus)
                    {
                        isAuthorized = _dbCntxt.AspNetUsersMenuPermission.Where(x => x.VMenuId == menu.VMenuId && roleIds.Contains(x.RoleId)).Any();
                        if (isAuthorized)
                            break;
                    }
                }

                return isAuthorized;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public Boolean IsAllowed(string id)
        {
            try
            {
                var isAllowed = true;
                if (_dbCntxt.AspNetUsers.Any())
                    isAllowed = Convert.ToBoolean(_dbCntxt.Setting.Where(x => x.VSettingId == id).FirstOrDefault().VSettingOption);

                return isAllowed;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Logout(string ulhid)
        {
            try
            {
                using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
                try
                {
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    AspNetUsersLoginHistory anulh = _dbCntxt.AspNetUsersLoginHistory.Where(x => x.VUlhid == ulhid && x.NvIpaddress == ip).FirstOrDefault();
                    anulh.DLogOut = DateTime.UtcNow;
                    _dbCntxt.Entry(anulh).State = EntityState.Modified;
                    _dbCntxt.SaveChanges();
                    await _signInManager.SignOutAsync();

                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    throw new CustomException("No user found with this specified email.", 404);
                }
                else if (user != null && !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    throw new CustomException("Email is not yet verified.", 400);
                }
                else
                {
                    //Get user info
                    var userindb = await _user.Find(user.Id).FirstOrDefaultAsync();

                    //Check if user is deactivated
                    if (!userindb.Status)
                        throw new CustomException("User is already deactivated", 400);

                    if (userindb.State == UserAccountState.Initial_Password || userindb.State == UserAccountState.Initial_Password_Expired || userindb.State == UserAccountState.Dormant_User || userindb.State == UserAccountState.Locked)
                    {

                        string state = "";

                        if (userindb.State == UserAccountState.Initial_Password)
                            state = "Initial Password";
                        else if (userindb.State == UserAccountState.Initial_Password_Expired)
                            state = "Initial Password Expired";
                        else if (userindb.State == UserAccountState.Dormant_User)
                            state = "Dormant User";
                        else
                            state = "Locked User";

                        throw new CustomException("Unable to proceed with the request due to user's current state: '" + state + "'. Please contact your administrator", 400);
                    }

                    bool isUserSuperAdmin = _dbCntxt.AspNetUserRoles.Where(x => x.RoleId == "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03" && x.UserId == user.Id).Any();
                    bool isAllowed = Convert.ToBoolean(_dbCntxt.Setting.Where(x => x.VSettingId == "_RECOVERPASSWORD").FirstOrDefault().VSettingOption);
                    if (isUserSuperAdmin || isAllowed)
                    {
                        var settings = await _dbCntxt.Setting.ToListAsync();

                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                        var domain = settings.Where(s => s.VSettingId == "_DOMAIN").Select(s => s.VSettingOption).FirstOrDefault();

                        //var domain = _config.GetSection("AppSettings")["AngularPath"];

                        if (domain == "")
                            domain = _config.GetSection("AppSettings")["AngularPath"];

                        var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, domain);

                        //var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Host.Value);
                        //var callbackUrl = Url.Action("ResetPassword", "Account", new { code, email = user.Email }, Request.Host.Value);
                        var webRoot = System.IO.Directory.GetCurrentDirectory();
                        var sPath = System.IO.Path.Combine(webRoot, "EmailTemplate/reset-password.html");

                        var builder = new StringBuilder();
                        using (var reader = System.IO.File.OpenText(sPath))
                        {
                            builder.Append(reader.ReadToEnd());
                        }

                        //builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                        //TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                        //builder.Replace("{username}", myTI.ToTitleCase(user.UserName));
                        //builder.Replace("{verifylink}", callbackUrl);
                        //builder.Replace("{twitterlink}", _config.GetSection("Company")["SocialTwitter"]);
                        //builder.Replace("{facebooklink}", _config.GetSection("Company")["SocialFacebook"]);
                        //builder.Replace("{companyname}", _config.GetSection("Company")["Company"]);
                        //builder.Replace("{companyemail}", _config.GetSection("Company")["MailAddress"]);

                        builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                        TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                        builder.Replace("{username}", myTI.ToTitleCase(user.UserName));

                        var appcode = settings.Where(s => s.VSettingId == "_SITECODE").Select(s => s.VSettingOption).FirstOrDefault();

                        if (appcode == "")
                            appcode = "FLOW";

                        var appname = settings.Where(s => s.VSettingId == "_SITENAME").Select(s => s.VSettingOption).FirstOrDefault();

                        if (appname == "")
                            appname = _config.GetSection("AppSettings")["AppName"];

                        builder.Replace("{appname}", appname);
                        builder.Replace("{verifylink}", callbackUrl);

                        var twitter = settings.Where(s => s.VSettingId == "_TWITTERPAGE").Select(s => s.VSettingOption).FirstOrDefault();

                        if (twitter == "")
                            twitter = _config.GetSection("Company")["SocialTwitter"];

                        builder.Replace("{twitterlink}", twitter);

                        var fbpage = settings.Where(s => s.VSettingId == "_FACEBOOKPAGE").Select(s => s.VSettingOption).FirstOrDefault();

                        if (fbpage == "")
                            fbpage = _config.GetSection("Company")["SocialFacebook"];

                        builder.Replace("{facebooklink}", fbpage);

                        var company = settings.Where(s => s.VSettingId == "_COMPANY").Select(s => s.VSettingOption).FirstOrDefault();

                        if (company == "")
                            company = _config.GetSection("Company")["Company"];

                        builder.Replace("{companyname}", company);

                        var companyemail = settings.Where(s => s.VSettingId == "_CORPORATEEMAIL").Select(s => s.VSettingOption).FirstOrDefault();

                        if (companyemail == "")
                            companyemail = _config.GetSection("Company")["MailAddress"];

                        builder.Replace("{companyemail}", companyemail);

                        await _emailSender.SendEmailAsync(model.Email, "Reset Password", builder.ToString());
                    }
                    else
                        throw new CustomException("Recovery password not allowed", 400);
                }
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.id);
                if (user == null)
                    throw new CustomException("User not found in system to change password.", 404);
                else if (user != null && !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    throw new CustomException("Email is not yet verified.", 400);
                }
                else
                {
                    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    var result = await _userManager.ResetPasswordAsync(user, model.Code.Replace(" ", "+"), model.Password);

                    if (result.Succeeded)
                    {
                        AspNetUsers anu = _dbCntxt.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault();
                        anu.LoginAttempts = 0;
                        anu.LoginAttemptsDate = null;
                        anu.State = Convert.ToInt32(UserAccountState.Active);
                        anu.TwoFactorEnabled = false;
                        _dbCntxt.Entry(anu).State = EntityState.Modified;
                        await _dbCntxt.SaveChangesAsync();

                        var userpass = await _dbCntxt.UserPasswordPolicies.FirstOrDefaultAsync(p => p.UserId == user.Id);

                        var validity = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_PASSWORDVALIDITY");

                        userpass.ExpirationDate = DateTime.Now.AddDays(Convert.ToInt32(validity.VSettingOption));// should be based from the saved validity in global config plus one get the exact expiration date
                        await _dbCntxt.SaveChangesAsync();

                        scope.Complete();
                    }
                    else
                    {
                        scope.Dispose();
                        string errors = "";

                        foreach (var error in result.Errors)
                        {
                            errors += error.Description + " " + Environment.NewLine;
                        }

                        throw new CustomException(errors, 400);
                    }
                }
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<ObjectReturnModel> GetCurrentUser()
        {
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _user.FindAll()
                                .Where(x => x.Id == cId)
                                .Select(x => new
                                {
                                    x.Id,
                                    Name = x.FullName,
                                    x.Photo,
                                    x.Email,
                                    Role = "",
                                    Position = ""
                                }).ToListAsync();

                var data = new ObjectReturnModel { Object1 = user };

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<String> GenerateQRCodeUri()
        {
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user = await _userManager.FindByIdAsync(cId);
                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(unformattedKey))
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }
                var AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
                var uri = string.Format(
                    AuthenticatorUriFormat,
                    _urlEncoder.Encode("FLOW"),
                    _urlEncoder.Encode(user.Email),
                    unformattedKey);

                return uri;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task VerifyAuthApp(TwoFactorAuthModel TFM)
        {
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(cId);
                var is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, TFM.TFACode);

                if (is2FaTokenValid)
                    await _userManager.SetTwoFactorEnabledAsync(user, true);
                else
                    throw new CustomException("This code is not valid. Please try again.", 400);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Disable2fa()
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                AspNetUsers anu = _dbCntxt.AspNetUsers.Where(x => x.Id == cId).FirstOrDefault();
                anu.TwoFactorEnabled = false;
                _dbCntxt.Entry(anu).State = EntityState.Modified;
                await _dbCntxt.SaveChangesAsync();
                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public AuthenticationProperties ExternalLogin(string provider, string redirectUrl = null)
        {
            try
            {
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

                return properties;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<String> Callback(string returnUrl = null, string remoteError = null)
        {
            try
            {
                var redirectUrl = _config.GetSection("AppSettings")["AngularPath"];

                _ = returnUrl ?? Url.Content("~/");
                if (remoteError != null)
                {
                    return redirectUrl + "/login?error=Something wrong";
                }

                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return redirectUrl + "/login?error=Something wrong";
                }

                var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(userEmail))
                {
                    return redirectUrl + "/login?error=Something wrong";
                }

                var user = await _userManager.FindByEmailAsync(userEmail);

                // Sign in the user with this external login provider if the user already has a login.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                    isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    var loginResult = await LoginSuccess(user);
                    var token = loginResult.GetType().GetProperty("token").GetValue(loginResult, null);
                    var index = loginResult.GetType().GetProperty("index").GetValue(loginResult, null);
                    var ulhid = loginResult.GetType().GetProperty("ulhid").GetValue(loginResult, null);
                    return redirectUrl + "/login?token=" + token + "&index=" + index + "&ulhid=" + ulhid;
                }

                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        // var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Host.Value);  //Only use when Core & Angular are in same domain
                        var host = _config.GetSection("AppSettings")["AngularPath"];
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, host);

                        var webRoot = System.IO.Directory.GetCurrentDirectory();
                        var pathToFile = System.IO.Path.Combine(webRoot, "EmailTemplate/email-confirmation.html");
                        var builder = new StringBuilder();
                        using (var reader = System.IO.File.OpenText(pathToFile))
                        {
                            builder.Append(reader.ReadToEnd());
                        }

                        builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                        TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                        builder.Replace("{username}", myTI.ToTitleCase(user.UserName));
                        builder.Replace("{appname}", _config.GetSection("AppSettings")["AppName"]);
                        builder.Replace("{verifylink}", callbackUrl);
                        builder.Replace("{twitterlink}", _config.GetSection("Company")["SocialTwitter"]);
                        builder.Replace("{facebooklink}", _config.GetSection("Company")["SocialFacebook"]);
                        builder.Replace("{companyname}", _config.GetSection("Company")["Company"]);
                        builder.Replace("{companyemail}", _config.GetSection("Company")["MailAddress"]);

                        await _emailSender.SendEmailConfirmationAsync(user.Email, builder.ToString());

                        return redirectUrl + "/login?error=Email not confirmed";
                    }

                    // Add the external provider
                    await _userManager.AddLoginAsync(user, info);

                    result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                                    isPersistent: false, bypassTwoFactor: true);
                    if (result.Succeeded)
                    {
                        var loginResult = await LoginSuccess(user);
                        var token = loginResult.GetType().GetProperty("token").GetValue(loginResult, null);
                        var index = loginResult.GetType().GetProperty("index").GetValue(loginResult, null);
                        var ulhid = loginResult.GetType().GetProperty("ulhid").GetValue(loginResult, null);
                        return redirectUrl + "/login?token=" + token + "&index=" + index + "&ulhid=" + ulhid;
                    }
                }
                return redirectUrl + "/register?associate=" + userEmail + "&loginProvider=" + info.LoginProvider + "&providerDisplayName=" + info.ProviderDisplayName + "&providerKey=" + info.ProviderKey;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IActionResult> Providers()
        {
            try
            {
                if (_dbCntxt.Setting.Count() > 0)
                {
                    bool isAllowed = Convert.ToBoolean(_dbCntxt.Setting.Where(x => x.VSettingId == "_EXTERNALLOGIN").FirstOrDefault().VSettingOption);
                    if (isAllowed)
                    {
                        var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
                        return Ok(schemes.Select(s => s.DisplayName).ToList());
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        public async Task<Object> Associate(AssociateViewModel associate)
        {
            var user = new ApplicationUser { UserName = associate.UserName, Email = associate.Email, date = DateTime.UtcNow };
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var roleId = "";
                if (_dbCntxt.AspNetUsers.Where(x => x.UserName == associate.UserName).Any())
                {
                    throw new CustomException("User with the same username already taken.", 400);
                }
                else
                {
                    roleId = _dbCntxt.Setting.Where(x => x.VSettingId == "_USERROLE").FirstOrDefault().VSettingOption;
                    if (!_dbCntxt.AspNetRoles.Where(x => x.Id == roleId).Any())
                        throw new CustomException("User registration role not set at settings. Please go to settings page and set the user registration role.", 400);
                }

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    AspNetUserRoles anur = new()
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    };
                    _dbCntxt.AspNetUserRoles.Add(anur);
                    _dbCntxt.SaveChanges();

                    result =
                        await _userManager.AddLoginAsync(user,
                            new ExternalLoginInfo(null, associate.LoginProvider, associate.ProviderKey,
                                associate.ProviderDisplayName));

                    if (result.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);

                        await _signInManager.ExternalLoginSignInAsync(associate.LoginProvider, associate.ProviderKey, false);

                        scope.Complete();
                    }
                    else
                    {
                        scope.Dispose();
                        throw new CustomException(result.Errors.ToString(), 400);
                    }
                }
                else
                {
                    scope.Dispose();
                    throw new CustomException(result.Errors.ToString(), 400);
                }
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                scope.Dispose();
                throw new Exception(ex.Message, ex.InnerException);
            }

            var loginResult = await LoginSuccess(user);
            return loginResult;
        }

        private async Task<Object> LoginSuccess(ApplicationUser user, bool isexpiredandcanchange = false)
        {
            try
            {
                var roleId = _dbCntxt.AspNetUserRoles.Where(x => x.UserId == user.Id).FirstOrDefault().RoleId;
                var index = _dbCntxt.AspNetRoles.Where(x => x.Id == roleId).FirstOrDefault().IndexPage;

                string defaulthomepage = "";

                defaulthomepage = _dbCntxt.Setting.Where(s => s.VSettingId == "_DEFAULTHOMEPAGE").FirstOrDefault().VSettingOption;

                //get from user
                var checkifnull = _dbCntxt.UserPreferences.FirstOrDefault(p => p.UserId == user.Id && p.Name == "_DEFAULTHOMEPAGE");
                if (checkifnull != null)
                {
                    defaulthomepage = checkifnull.Value;
                }

                index = defaulthomepage;
                var is2faenabled = user.TwoFactorEnabled;

                var ULHID = "";

                ULHID = GeneralHelper.GenerateGuidAsUniqueKey();

                //using (var _dbCntxtTransaction = _dbCntxt.Database.BeginTransaction())
                //{
                try
                {

                    DateTime loginDateTime = DateTime.UtcNow;
                    //Update Last Login
                    var newuser = _dbCntxt.AspNetUsers.Where(u => u.Id == user.Id).FirstOrDefault();
                    newuser.LastLogin = loginDateTime;
                    _dbCntxt.SaveChanges();

                    var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    AspNetUsersLoginHistory anulh = new();
                    anulh.VUlhid = ULHID;
                    anulh.UserId = user.Id;
                    anulh.DLogIn = loginDateTime;
                    anulh.NvIpaddress = ip;
                    _dbCntxt.AspNetUsersLoginHistory.Add(anulh);
                    _dbCntxt.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
                //}


                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings")["Token"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

                var roles = await _userManager.GetRolesAsync(user);
                var rolesString = JsonConvert.SerializeObject(roles);

                var tokeOptions = new JwtSecurityToken(
                            issuer: _httpContextAccessor.HttpContext.Request.Host.Value,
                            audience: _httpContextAccessor.HttpContext.Request.Host.Value,
                            claims: new List<Claim>(
                                new List<Claim> {
                                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                                    new Claim("userName",user.UserName),
                                    new Claim("email",user.Email),
                                    new Claim("role", rolesString),
                                    new Claim(JwtRegisteredClaimNames.Jti, GeneralHelper.GenerateGuidAsUniqueKey())
                                }
                            ),
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: signinCredentials
                );

                var tokenHandler = new JwtSecurityTokenHandler();

                return new
                {
                    token = tokenHandler.WriteToken(tokeOptions),
                    user.Id,
                    index = isexpiredandcanchange ? "/home/changePasswordExpired" : index,
                    ulhid = ULHID,
                    is2faenabled
                };
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IdentityResult> SetPassword(SetPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);

                var userpass = await _dbCntxt.UserPasswordPolicies.FirstOrDefaultAsync(p => p.UserId == user.Id);

                var validity = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_PASSWORDVALIDITY");

                userpass.ExpirationDate = DateTime.Now.AddDays(Convert.ToInt32(validity.VSettingOption));// should be based from the saved validity in global config plus one get the exact expiration date
                await _dbCntxt.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task UpdateLoginAttempts(string userid, int loginAttempts, DateTime? loginAttemptsDate)
        {

            try
            {
                var userindb = await _dbCntxt.AspNetUsers.FindAsync(userid);

                if (userindb != null)
                {
                    userindb.LoginAttempts = loginAttempts;
                    userindb.LoginAttemptsDate = loginAttemptsDate;
                    _dbCntxt.Entry(userindb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task UpdateUserAccountState(string userid, UserAccountState state)
        {
            try
            {
                var userindb = await _dbCntxt.AspNetUsers.FindAsync(userid);

                if (userindb != null)
                {
                    userindb.State = Convert.ToInt32(state);
                    _dbCntxt.Entry(userindb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                    throw new CustomException("User not found.", 404);
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task UpdateUserState(string userid, UserAccountState state)
        {

            using (var trans = _dbCntxt.Database.BeginTransaction())
            {
                try
                {

                    //Check if state from front end is a valid UserAccountState Enum
                    if (!(GeneralHelper.IsValidUserAccountState(Convert.ToInt32(state))))
                        throw new CustomException("User Account State is not valid", 400);

                    var userindb = await _dbCntxt.AspNetUsers.FindAsync(userid);

                    if (userindb != null)
                    {
                        await this.UpdateUserAccountState(userid, state);

                        if (state == UserAccountState.Dormant_User || state == UserAccountState.Active || state == UserAccountState.Password_Expired)
                            await this.UpdateLoginAttempts(userid, 0, null);

                        if (state == UserAccountState.Initial_Password)
                            await _user.SendInitialPassword(userid);
                    }
                    else
                        throw new CustomException("User not found.", 404);

                    trans.Commit();

                }
                catch (CustomException customex)
                {
                    trans.Rollback();
                    throw new CustomException(customex.Message, customex.StatusCode);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
        }

        public async Task<List<UserAccountState>> GetApplicableUserState(string userid, UserAccountState fromstate)
        {
            List<UserAccountState> userstates = new List<UserAccountState>();
            try
            {
                //Check if state from front end is a valid UserAccountState Enum
                if (!(GeneralHelper.IsValidUserAccountState(Convert.ToInt32(fromstate))))
                    throw new CustomException("User Account State is not valid", 400);

                //Get user info
                var user = await _user.Find(userid)
                                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    if (user.Status == true)
                    {

                        if (user.State != fromstate)
                            throw new CustomException("User state did not match. Possible update was done on this user account.", 400);

                        if ((DateTime.Now.Date - Convert.ToDateTime(user.LastLogin == null ? "12/31/9999" : user.LastLogin).Date).TotalDays > 90 && user.State != UserAccountState.Dormant_User)
                            //user is dormant
                            userstates.Add(UserAccountState.Dormant_User);
                        else
                        {
                            //check if password is expired
                            if (user.IsPasswordNeverExpires == false && (DateTime.Now > user.ExpirationDate && user.State != UserAccountState.Password_Expired))
                                userstates.Add(UserAccountState.Password_Expired);
                            else
                            {
                                if (fromstate == UserAccountState.Locked || fromstate == UserAccountState.Soft_Locked || (user.LoginAttempts > 0 && (DateTime.Now.Date - Convert.ToDateTime(user.LoginAttemptsDate == null ? "1/1/1900" : user.LoginAttemptsDate).Date).TotalDays > 0))
                                    userstates.Add(UserAccountState.Active);

                                if (fromstate != UserAccountState.Initial_Password)
                                    //only for not initial password
                                    userstates.Add(UserAccountState.Initial_Password);

                                //if (fromstate == UserAccountState.Active)
                                //    //if there's a need to manually tagged as locked
                                //    userstates.Add(UserAccountState.Locked);
                            }
                        }
                    }
                }
                else
                    throw new CustomException("User not found.", 404);
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return userstates;
        }

        public async Task<Object> InitialPassword(InitialPasswordViewModel model)
        {
            using (var _dbCntxtTransaction = _dbCntxt.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null)
                        throw new CustomException("User not found.", 404);

                    if (!user.EmailConfirmed)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var result = await _userManager.ConfirmEmailAsync(user, code.Replace(" ", "+"));
                        if (!result.Succeeded)
                        {
                            throw new CustomException("Failed to confirm email", 400);
                        }
                    }

                    UserPasswordPolicy passwordpolicy = await _dbCntxt.UserPasswordPolicies.FirstOrDefaultAsync(p => p.UserId == model.Id);

                    var changepassresult = await _userManager.ChangePasswordAsync(user, passwordpolicy.InitialPassword, model.Password);

                    if (!changepassresult.Succeeded)
                    {
                        string errors = "";

                        foreach (var error in changepassresult.Errors)
                        {
                            errors += error.Description + " " + Environment.NewLine;
                        }

                        throw new CustomException(errors, 400);
                    }

                    // Update password
                    AspNetUsers usertoupdate = await _dbCntxt.AspNetUsers.FindAsync(model.Id);
                    //usertoupdate.PasswordHash = _signInManager.UserManager.PasswordHasher.HashPassword(user, model.Password);
                    usertoupdate.State = Convert.ToInt32(UserAccountState.Active);
                    usertoupdate.LoginAttempts = 0;
                    usertoupdate.LoginAttemptsDate = null;
                    _dbCntxt.Entry(usertoupdate).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    //Set Password expiration date and remove initial expiration details
                    var validity = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_PASSWORDVALIDITY");
                    
                    passwordpolicy.ExpirationDate = DateTime.Now.AddDays(Convert.ToInt32(validity.VSettingOption));// should be based from the saved validity 
                    passwordpolicy.InitialPassword = null;
                    passwordpolicy.InitialPasswordExpirationDate = null;
                    _dbCntxt.Entry(passwordpolicy).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    var roleId = _dbCntxt.AspNetUserRoles.Where(x => x.UserId == user.Id).FirstOrDefault().RoleId;
                    var index = _dbCntxt.AspNetRoles.Where(x => x.Id == roleId).FirstOrDefault().IndexPage;

                    string defaulthomepage = "";

                    defaulthomepage = _dbCntxt.Setting.Where(s => s.VSettingId == "_DEFAULTHOMEPAGE").FirstOrDefault().VSettingOption;

                    //get from user
                    var checkifnull = _dbCntxt.UserPreferences.FirstOrDefault(p => p.UserId == user.Id && p.Name == "_DEFAULTHOMEPAGE");
                    if (checkifnull != null)
                    {
                        defaulthomepage = checkifnull.Value;
                    }

                    index = defaulthomepage;

                    _dbCntxtTransaction.Commit();

                    return new { index };
                }
                catch (CustomException customex)
                {
                    _dbCntxtTransaction.Rollback();
                    throw new CustomException(customex.Message, customex.StatusCode);
                }
                catch (Exception ex)
                {
                    _dbCntxtTransaction.Rollback();
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
        }

        public async Task<Object> ChangePasswordExpired(ChangePasswordExpiredViewModel model)
        {
            using (var _dbCntxtTransaction = _dbCntxt.Database.BeginTransaction())
            {
                try
                {

                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null)
                        throw new CustomException("User not found", 404);

                    var passwordpolicies = _dbCntxt.UserPasswordPolicies.FirstOrDefault(up => up.UserId == user.Id);

                    bool isAllowed = passwordpolicies.CanChangePassword;

                    if (!isAllowed)
                        throw new CustomException("You're not allowed to change your password. Please contact administrator", 400);

                    var passresult = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                    if (!passresult)
                        throw new CustomException("Old password mismatch.", 400);

                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (!result.Succeeded)
                    {
                        string errors = "";

                        foreach (var error in result.Errors)
                        {
                            errors += error.Description + " " + Environment.NewLine;
                        }

                        throw new CustomException(errors, 400);
                    }

                    //Update Last login and other info
                    var usertoupdate = await _dbCntxt.AspNetUsers.FindAsync(user.Id);
                    usertoupdate.State = Convert.ToInt32(UserAccountState.Active);
                    usertoupdate.LoginAttempts = 0;
                    usertoupdate.LoginAttemptsDate = null;
                    await _dbCntxt.SaveChangesAsync();

                    var validity = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_PASSWORDVALIDITY");

                    passwordpolicies.ExpirationDate = DateTime.Now.AddDays(Convert.ToInt32(validity.VSettingOption));// should be based from the saved validity in global config plus one get the exact expiration date
                    passwordpolicies.InitialPassword = null;
                    passwordpolicies.InitialPasswordExpirationDate = null;
                    _dbCntxt.Entry(passwordpolicies).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    var roleId = _dbCntxt.AspNetUserRoles.Where(x => x.UserId == user.Id).FirstOrDefault().RoleId;
                    var index = _dbCntxt.AspNetRoles.Where(x => x.Id == roleId).FirstOrDefault().IndexPage;

                    string defaulthomepage = "";

                    defaulthomepage = _dbCntxt.Setting.Where(s => s.VSettingId == "_DEFAULTHOMEPAGE").FirstOrDefault().VSettingOption;

                    //get from user
                    var checkifnull = _dbCntxt.UserPreferences.FirstOrDefault(p => p.UserId == user.Id && p.Name == "_DEFAULTHOMEPAGE");
                    if (checkifnull != null)
                    {
                        defaulthomepage = checkifnull.Value;
                    }

                    index = defaulthomepage;

                    _dbCntxtTransaction.Commit();

                    return new { index };

                }
                catch (CustomException customex)
                {
                    _dbCntxtTransaction.Rollback();
                    throw new CustomException(customex.Message, customex.StatusCode);
                }
                catch (Exception ex)
                {
                    _dbCntxtTransaction.Rollback();
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
        }

        public async Task<object> GetCurrentUserState(string userid)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userid);
                if (user == null)
                    throw new CustomException("User not found.", 404);

                var userinfo = await _dbCntxt.AspNetUsers.FindAsync(userid);

                var userstate = (UserAccountState) userinfo.State;

                string index = "";

                if (userstate == UserAccountState.Initial_Password)
                    index = "/home/initialPassword";
                else if (userstate == UserAccountState.Password_Expired)
                    index = "/home/changePasswordExpired";
                else
                    index = "";

                return new { userid = userinfo.Id, userstate = userstate, index = index };

            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
