using api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using workflow.Models;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Transactions;
using System.Text;
using workflow.Helpers;
using workflow.Data;

namespace workflow.Services
{
    public class UserRepository : BaseApi, IUser
    {
        private readonly IRole _role;
        private readonly IEmailSender _emailSender;
        private readonly IOrganizationalStructure _organizationalStructure;
        private readonly IApprovalLevel _approvalLevel;
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IUserRequestTypesGrouping _userRequestTypesGrouping;
        private readonly IUserNotAllowedLinkType _userNotAllowedLinkType;

        public UserRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<UserRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor,
                IRole role, IEmailSender emailSender, IOrganizationalStructure organizationalStructure, IApprovalLevel approvalLevel, IUserIPAddressPerSession userIpAddressPerSession, IUserRequestTypesGrouping userRequestTypesGrouping, IUserNotAllowedLinkType userNotAllowedLinkType) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _role = role;
            _emailSender = emailSender;
            _organizationalStructure = organizationalStructure;
            _approvalLevel = approvalLevel;
            _userIpAddressPerSession = userIpAddressPerSession;
            _userRequestTypesGrouping = userRequestTypesGrouping;
            _userNotAllowedLinkType = userNotAllowedLinkType;
        }

        private IQueryable<AspNetUsers> Get()
        {
            try
            {
                IQueryable<AspNetUsers> query = null;

                query = _dbCntxt.AspNetUsers
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserRegisterModel> Find(object obj)
        {
            try
            {
                IQueryable<UserRegisterModel> query = null;

                var condition = (obj ?? "").ToString();

                query = this.Get()
                            .Where(r => r.Id == condition)
                            .Select
                            (
                                usr => new UserRegisterModel
                                {
                                    Id = usr.Id,
                                    Email = usr.Email,
                                    UserName = usr.UserName,
                                    TwoFactorEnabled = usr.TwoFactorEnabled,
                                    RoleName = usr.AspNetUserRoles.FirstOrDefault().Role.Name,
                                    RoleId = usr.AspNetUserRoles.FirstOrDefault().Role.Id,
                                    FirstName = usr.AspNetUsersProfile.FirstName,
                                    LastName = usr.AspNetUsersProfile.LastName,
                                    MiddleName = usr.AspNetUsersProfile.MiddleName,
                                    FullName = usr.AspNetUsersProfile.LastName + ", " + usr.AspNetUsersProfile.FirstName,
                                    FullName2 = usr.AspNetUsersProfile.FirstName + " " + usr.AspNetUsersProfile.LastName,
                                    Gender = usr.AspNetUsersProfile.Gender,
                                    Company = usr.AspNetUsersProfile.Company,
                                    Department = usr.AspNetUsersProfile.Department,
                                    Position = usr.AspNetUsersProfile.Position,
                                    Rank = usr.AspNetUsersProfile.Rank,
                                    Mobile = usr.AspNetUsersProfile.Mobile,
                                    Phone = usr.AspNetUsersProfile.Phone,
                                    Country = usr.AspNetUsersProfile.Country,
                                    PhoneNumber = usr.PhoneNumber,
                                    Status = usr.Status,
                                    BlockedAccount = usr.BlockedAccount,
                                    Photo = usr.AspNetUsersProfile.Photo,
                                    CreatedByPK = usr.AspNetUsersProfile.CreatedByPK,
                                    CreatedDate = usr.AspNetUsersProfile != null ? usr.AspNetUsersProfile.CreatedDate : DateTime.Now,
                                    Roles = _dbCntxt.AspNetUserRoles.Where(x => x.UserId == usr.Id)
                                                                    .Select(x => new RoleViewModel { Id = x.RoleId, Name = x.Role.Name })
                                                                   .ToList(),
                                    CanChangePassword = usr.UserPasswordPolicies.FirstOrDefault().CanChangePassword,
                                    ExpirationDate = usr.UserPasswordPolicies.FirstOrDefault().ExpirationDate,
                                    IsPasswordNeverExpires = usr.UserPasswordPolicies.FirstOrDefault().IsPasswordNeverExpires,
                                    LastLogin = usr.LastLogin,
                                    IsLocked = usr.Islocked,
                                    LoginAttempts = usr.LoginAttempts,
                                    LoginAttemptsDate = usr.LoginAttemptsDate,
                                    State = (UserAccountState)usr.State,
                                    InitialPassword = usr.UserPasswordPolicies.FirstOrDefault().InitialPassword,
                                    InitialPasswordExpirationDate = usr.UserPasswordPolicies.FirstOrDefault().InitialPasswordExpirationDate,
                                    OfficeId = usr.AspNetUsersProfile.OfficeId,
                                    Office = usr.AspNetUsersProfile.OfficeId == null ? "" : _dbCntxt.Options.FirstOrDefault(o => o.OptionGroup == "Site Locations" && o.Id == usr.AspNetUsersProfile.OfficeId).Name,
                                    UserOrganizationalStructures = _dbCntxt.UserOrganizationalStructures
                                                                           .Include(o => o.OrganizationalStructure)
                                                                           .Include(o => o.OrganizationalStructure.OrganizationEntity)
                                                                           .Where
                                                                           (
                                                                               u =>
                                                                               u.UserId == usr.Id
                                                                           ).
                                                                           Select
                                                                           (
                                                                               u => new UserOrganizationalStructureViewModel
                                                                               {
                                                                                   Id = u.Id,
                                                                                   UserId = u.UserId,
                                                                                   OrganizationalStructureId = u.OrganizationalStructureId,
                                                                                   OrganizationalStructure = u.OrganizationalStructure.Name,
                                                                                   OrganizationEntityId = u.OrganizationalStructure.OrganizationEntityId,
                                                                                   OrganizationEntityName = u.OrganizationalStructure.OrganizationEntity.Name,
                                                                                   ApprovalLevelDetailId = u.ApprovalLevelDetailId,
                                                                                   ApprovalLevel = _dbCntxt.ApprovalLevelDetails
                                                                                                            .Include(a => a.ApprovelLevel)
                                                                                                            .Where
                                                                                                            (
                                                                                                                a =>
                                                                                                                a.Id == u.ApprovalLevelDetailId
                                                                                                            )
                                                                                                            .Select
                                                                                                            (
                                                                                                                a => new ApprovalLevelDetailViewModel
                                                                                                                {
                                                                                                                    Id = a.Id,
                                                                                                                    ApprovalLevelId = a.ApprovalLevelId,
                                                                                                                    ApprovalLevel = a.ApprovelLevel.Name,
                                                                                                                    Sequence = a.Sequence,
                                                                                                                    ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                                                                                }
                                                                                                            )
                                                                                                            .FirstOrDefault(),
                                                                                   ReportingTo = u.ReportingTo,
                                                                                   ReportingToApprovalLevel = (u.ReportingTo == 9999 ? new ApprovalLevelDetailViewModel
                                                                                   {
                                                                                       Id = 9999,
                                                                                       ApprovalLevelId = 0,
                                                                                       ApprovalLevel = "Next in Line",
                                                                                       Sequence = 0,
                                                                                       ApprovalLevelAndSequence = "Next in Line"

                                                                                   } : _dbCntxt.ApprovalLevelDetails
                                                                                                            .Include(a => a.ApprovelLevel)
                                                                                                            .Where
                                                                                                            (
                                                                                                                a =>
                                                                                                                a.Id == u.ReportingTo
                                                                                                            )
                                                                                                            .Select
                                                                                                            (
                                                                                                                a => new ApprovalLevelDetailViewModel
                                                                                                                {
                                                                                                                    Id = a.Id,
                                                                                                                    ApprovalLevelId = a.ApprovalLevelId,
                                                                                                                    ApprovalLevel = a.ApprovelLevel.Name,
                                                                                                                    Sequence = a.Sequence,
                                                                                                                    ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                                                                                }
                                                                                                            )
                                                                                                            .FirstOrDefault()),
                                                                                   ReportingToUser = _approvalLevel.GetSpecificApprovalLevelReportingTo(u.ApprovalLevelDetailId, u.ReportingTo, u.OrganizationalStructureId, null, null, null, null),
                                                                                   Published = u.Published,
                                                                                   CreatedDate = u.CreatedDate
                                                                               }
                                    ).ToList(),
                                    UserRequestTypesGroupings = _dbCntxt.UserRequestTypesGroupings
                                                                        .Include(r => r.RequestTypesGrouping)
                                                                        .Where
                                                                        (
                                                                            u =>
                                                                            u.UserId == usr.Id
                                                                        )
                                                                        .Select
                                                                        (
                                                                             r => new UserRequestTypesGroupingViewModel
                                                                             {
                                                                                 Id = r.Id,
                                                                                 UserId = r.UserId,
                                                                                 RequestTypesGroupId = r.RequestTypesGroupId,
                                                                                 RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                                                                 RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                                                                 RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                                                                 Published = r.Published,
                                                                                 CreatedDate = r.CreatedDate
                                                                             }
                                                                        ).ToList(),
                                    UserNotAllowedLinkTypes = _dbCntxt.UserNotAllowedLinkTypes
                                                                      .Include(r => r.LinkType)
                                                                      .Where
                                                                        (
                                                                            u =>
                                                                            u.UserId == usr.Id
                                                                        )
                                                                        .Select
                                                                        (
                                                                             r => new UserNotAllowedLinkTypeViewModel
                                                                             {
                                                                                 Id = r.Id,
                                                                                 UserId = r.UserId,
                                                                                 LinkTypeId = r.LinkTypeId,
                                                                                 LinkTypeName = r.LinkType.Name,
                                                                                 LinkTypeURL = r.LinkType.URL,
                                                                                 LinkTypePublished = r.LinkType.Published,
                                                                                 Category = (UserNotAllowedLinkTypeCategory)r.Category,
                                                                                 CategoryDescription = r.Category == 1 ? "Request" : "WorkFlow",
                                                                                 Published = r.Published,
                                                                                 CreatedDate = r.CreatedDate
                                                                             }
                                                                        ).ToList(),

                                }
                            ).AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserRegisterModel> FindAll(object obj = null)
        {
            try
            {
                IQueryable<UserRegisterModel> query = null;

                query = this.Get()
                            .Select
                            (
                                usr => new UserRegisterModel
                                {
                                    Id = usr.Id,
                                    Email = usr.Email,
                                    UserName = usr.UserName,
                                    TwoFactorEnabled = usr.TwoFactorEnabled,
                                    RoleName = usr.AspNetUserRoles.FirstOrDefault().Role.Name,
                                    RoleId = usr.AspNetUserRoles.FirstOrDefault().Role.Id,
                                    FirstName = usr.AspNetUsersProfile.FirstName,
                                    LastName = usr.AspNetUsersProfile.LastName,
                                    MiddleName = usr.AspNetUsersProfile.MiddleName,
                                    FullName = usr.AspNetUsersProfile.LastName + ", " + usr.AspNetUsersProfile.FirstName,
                                    FullName2 = usr.AspNetUsersProfile.FirstName + " " + usr.AspNetUsersProfile.LastName,
                                    Gender = usr.AspNetUsersProfile.Gender,
                                    Company = usr.AspNetUsersProfile.Company,
                                    Department = usr.AspNetUsersProfile.Department,
                                    Position = usr.AspNetUsersProfile.Position,
                                    Rank = usr.AspNetUsersProfile.Rank,
                                    Mobile = usr.AspNetUsersProfile.Mobile,
                                    Phone = usr.AspNetUsersProfile.Phone,
                                    Country = usr.AspNetUsersProfile.Country,
                                    PhoneNumber = usr.PhoneNumber,
                                    Status = usr.Status,
                                    BlockedAccount = usr.BlockedAccount,
                                    Photo = usr.AspNetUsersProfile.Photo,
                                    CreatedByPK = usr.AspNetUsersProfile.CreatedByPK,
                                    CreatedDate = usr.AspNetUsersProfile != null ? usr.AspNetUsersProfile.CreatedDate : DateTime.Now,
                                    Roles = _dbCntxt.AspNetUserRoles.Where(x => x.UserId == usr.Id)
                                                                    .Select(x => new RoleViewModel { Id = x.RoleId, Name = x.Role.Name })
                                                                    .ToList(),
                                    CanChangePassword = usr.UserPasswordPolicies.FirstOrDefault().CanChangePassword,
                                    ExpirationDate = usr.UserPasswordPolicies.FirstOrDefault().ExpirationDate,
                                    IsPasswordNeverExpires = usr.UserPasswordPolicies.FirstOrDefault().IsPasswordNeverExpires,
                                    LastLogin = usr.LastLogin,
                                    IsLocked = usr.Islocked,
                                    LoginAttempts = usr.LoginAttempts,
                                    LoginAttemptsDate = usr.LoginAttemptsDate,
                                    State = (UserAccountState)usr.State,
                                    InitialPassword = usr.UserPasswordPolicies.FirstOrDefault().InitialPassword,
                                    InitialPasswordExpirationDate = usr.UserPasswordPolicies.FirstOrDefault().InitialPasswordExpirationDate,
                                    OfficeId = usr.AspNetUsersProfile.OfficeId,
                                    Office = usr.AspNetUsersProfile.OfficeId == null ? "" : _dbCntxt.Options.FirstOrDefault(o => o.OptionGroup == "Site Locations" && o.Id == usr.AspNetUsersProfile.OfficeId).Name,
                                    UserOrganizationalStructures = _dbCntxt.UserOrganizationalStructures
                                                                           .Include(o => o.OrganizationalStructure)
                                                                           .Include(o => o.OrganizationalStructure.OrganizationEntity)
                                                                           .Where
                                                                           (
                                                                               u =>
                                                                               u.UserId == usr.Id
                                                                           ).
                                                                           Select
                                                                           (
                                                                               u => new UserOrganizationalStructureViewModel
                                                                               {
                                                                                   Id = u.Id,
                                                                                   UserId = u.UserId,
                                                                                   OrganizationalStructureId = u.OrganizationalStructureId,
                                                                                   OrganizationalStructure = u.OrganizationalStructure.Name,
                                                                                   OrganizationEntityId = u.OrganizationalStructure.OrganizationEntityId,
                                                                                   OrganizationEntityName = u.OrganizationalStructure.OrganizationEntity.Name,
                                                                                   ApprovalLevelDetailId = u.ApprovalLevelDetailId,
                                                                                   ApprovalLevel = (u.ApprovalLevelDetailId > 0 ? _dbCntxt.ApprovalLevelDetails
                                                                                                    .Include(a => a.ApprovelLevel)
                                                                                                    .Where
                                                                                                    (
                                                                                                        a =>
                                                                                                        a.Id == u.ApprovalLevelDetailId
                                                                                                    )
                                                                                                    .Select
                                                                                                    (
                                                                                                        a => new ApprovalLevelDetailViewModel
                                                                                                        {
                                                                                                            Id = a.Id,
                                                                                                            ApprovalLevelId = a.ApprovalLevelId,
                                                                                                            ApprovalLevel = a.ApprovelLevel.Name,
                                                                                                            Sequence = a.Sequence,
                                                                                                            ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                                                                        }
                                                                                                    )
                                                                                                    .FirstOrDefault() : new ApprovalLevelDetailViewModel
                                                                                                    {
                                                                                                        Id = 0,
                                                                                                        ApprovalLevelId = 0,
                                                                                                        ApprovalLevel = "Staff",
                                                                                                        Sequence = 0,
                                                                                                        ApprovalLevelAndSequence = "Staff"
                                                                                                    }),
                                                                                   ReportingTo = u.ReportingTo,
                                                                                   ReportingToApprovalLevel = (u.ReportingTo == 9999 ? new ApprovalLevelDetailViewModel
                                                                                   {
                                                                                       Id = 0,
                                                                                       ApprovalLevelId = 0,
                                                                                       ApprovalLevel = "Next in Line",
                                                                                       Sequence = 0,
                                                                                       ApprovalLevelAndSequence = "Next in Line"

                                                                                   } : _dbCntxt.ApprovalLevelDetails
                                                                                                            .Include(a => a.ApprovelLevel)
                                                                                                            .Where
                                                                                                            (
                                                                                                                a =>
                                                                                                                a.Id == u.ReportingTo
                                                                                                            )
                                                                                                            .Select
                                                                                                            (
                                                                                                                a => new ApprovalLevelDetailViewModel
                                                                                                                {
                                                                                                                    Id = a.Id,
                                                                                                                    ApprovalLevelId = a.ApprovalLevelId,
                                                                                                                    ApprovalLevel = a.ApprovelLevel.Name,
                                                                                                                    Sequence = a.Sequence,
                                                                                                                    ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                                                                                }
                                                                                                            )
                                                                                                            .FirstOrDefault()),
                                                                                   ReportingToUser = _approvalLevel.GetSpecificApprovalLevelReportingTo(u.ApprovalLevelDetailId, u.ReportingTo, u.OrganizationalStructureId, null, null, null, null),
                                                                                   Published = u.Published,
                                                                                   CreatedDate = u.CreatedDate
                                                                               }
                                                                           ).ToList(),
                                    UserRequestTypesGroupings = _dbCntxt.UserRequestTypesGroupings
                                                                        .Include(r => r.RequestTypesGrouping)
                                                                        .Where
                                                                        (
                                                                            u =>
                                                                            u.UserId == usr.Id
                                                                        )
                                                                        .Select
                                                                        (
                                                                             r => new UserRequestTypesGroupingViewModel
                                                                             {
                                                                                 Id = r.Id,
                                                                                 UserId = r.UserId,
                                                                                 RequestTypesGroupId = r.RequestTypesGroupId,
                                                                                 RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                                                                 RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                                                                 RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                                                                 Published = r.Published,
                                                                                 CreatedDate = r.CreatedDate
                                                                             }
                                                                        ).ToList(),
                                    UserNotAllowedLinkTypes = _dbCntxt.UserNotAllowedLinkTypes
                                                                      .Include(r => r.LinkType)
                                                                      .Where
                                                                        (
                                                                            u =>
                                                                            u.UserId == usr.Id
                                                                        )
                                                                        .Select
                                                                        (
                                                                             r => new UserNotAllowedLinkTypeViewModel
                                                                             {
                                                                                 Id = r.Id,
                                                                                 UserId = r.UserId,
                                                                                 LinkTypeId = r.LinkTypeId,
                                                                                 LinkTypeName = r.LinkType.Name,
                                                                                 LinkTypeURL = r.LinkType.URL,
                                                                                 LinkTypePublished = r.LinkType.Published,
                                                                                 Category = (UserNotAllowedLinkTypeCategory)r.Category,
                                                                                 CategoryDescription = r.Category == 1 ? "Request" : "WorkFlow",
                                                                                 Published = r.Published,
                                                                                 CreatedDate = r.CreatedDate
                                                                             }
                                                                        ).ToList(),
                                }
                            ).AsQueryable();

                if (obj != null)
                {
                    // default search 
                    PagingRequest paging = (PagingRequest)obj;
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {

                    }

                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IQueryable<UserRegisterModel> GetUserList(PagingRequest paging = null)
        {
            try
            {
                var query = this.FindAll()
                                .AsQueryable();

                if (paging != null)
                {

                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query
                                .Where
                                (
                                    p => p.FullName2.Contains(search) ||
                                    p.Email.Contains(search) ||
                                    p.UserName.Contains(search)
                                );
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserRegisterModel> Add(SaveUserModel model)
        {
            UserRegisterModel userReturn = new UserRegisterModel();

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.EmailVerificationDisabled,
                date = DateTime.UtcNow,
                Status = true,
                BlockedAccount = model.BlockedAccount,
                State = Convert.ToInt32(UserAccountState.Initial_Password) // as default
            };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                bool isEmailDuplicate = this.Get().Where(x => x.Email == model.Email).Any();
                bool isUserNameDuplicate = this.Get().Where(x => x.UserName == model.UserName).Any();
                if (isEmailDuplicate)
                    throw new CustomException("Email already exist!", 400);
                if (isUserNameDuplicate)
                    throw new CustomException("Username already exist!", 400);
                else
                {
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // get current user id
                        var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        // get current session id
                        int spId = await _userIpAddressPerSession.GetSPID();
                        // get IP address
                        string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        //add useripperssion
                        var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                        await _userIpAddressPerSession.Save(userIp);

                        user = await _userManager.FindByEmailAsync(model.Email);

                        // section for inserting role
                        if (model.SelectedRoles.Count > 0)
                        {
                            foreach (var roleId in model.SelectedRoles)
                            {
                                AspNetUserRoles anur = new();
                                anur.UserId = user.Id;
                                anur.RoleId = roleId;
                                _dbCntxt.AspNetUserRoles.Add(anur);
                            }
                            await _dbCntxt.SaveChangesAsync();
                        }
                        else
                            throw new CustomException("Role is required!", 400);

                        bool isImageSaved = UploadImage(model.Photo, user.Id);
                        if (isImageSaved)
                        {
                            AspNetUsersProfile anup = new();
                            anup.Id = user.Id;
                            anup.FirstName = model.FirstName;
                            anup.LastName = model.LastName;
                            anup.MiddleName = model.MiddleName;
                            anup.Mobile = model.MiddleName;
                            anup.Gender = model.Gender;
                            anup.Company = model.Company;
                            anup.Department = model.Department;
                            anup.Position = model.Position;
                            anup.Rank = model.Rank;
                            anup.Mobile = model.Mobile;
                            anup.Photo = model.Phone;
                            anup.Country = model.Country;
                            anup.CreatedDate = DateTime.Now;
                            anup.CreatedByPK = cid;
                            anup.Photo = model.Photo == null ? model.Photo : "../../assets/images/profile_Image/" + user.Id + ".png" + GeneralHelper.GenerateGuidForImage();
                            anup.OfficeId = model.OfficeId;
                            _dbCntxt.AspNetUsersProfile.Add(anup);
                            await _dbCntxt.SaveChangesAsync();
                        }
                        else
                            throw new CustomException("Something Wrong!", 400);

                        UserPasswordPolicy userpass = new()
                        {
                            UserId = user.Id,
                            CanChangePassword = true,
                            IsPasswordNeverExpires = true,
                            ExpirationDate = Convert.ToDateTime("12/31/9999")
                        };

                        _dbCntxt.UserPasswordPolicies.Add(userpass);
                        await _dbCntxt.SaveChangesAsync();

                        //Insert User Organizational Structures
                        if (model.UserOrganizationalStructuresToSave != null)
                        {
                            if (model.UserOrganizationalStructuresToSave.Count > 0)
                            {
                                model.UserOrganizationalStructuresToSave
                                        .ForEach(u => u.UserId = user.Id);

                                //Validations
                                var userOrgValidationMessage = await this.ValidateUserOrganizationalStructures(model.UserOrganizationalStructuresToSave);

                                if (userOrgValidationMessage.ReturnMessage != "")
                                    throw new CustomException(userOrgValidationMessage.ReturnMessage, userOrgValidationMessage.ReturnCode);

                                //Save User Organizational Structures
                                await this.SaveUserOrgStructures(model.UserOrganizationalStructuresToSave);
                            }
                        }

                        //Insert User Request Types Groupings
                        if (model.UserRequestTypesGroupings != null)
                        {
                            if (model.UserRequestTypesGroupings.Count > 0)
                            {
                                model.UserRequestTypesGroupings
                                     .ForEach(u => u.UserId = user.Id);

                                //Validations
                                var detailValidationMessage = await _userRequestTypesGrouping.ValidateDetails(model.UserRequestTypesGroupings);

                                if (detailValidationMessage.ReturnMessage != "")
                                    throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                                foreach (var detail in model.UserRequestTypesGroupings)
                                {
                                    var addedDetails = await _userRequestTypesGrouping.AddInUser(detail);
                                }
                            }
                        }

                        //Insert User Not Allowed Link Types
                        if (model.UserNotAllowedLinkTypes != null)
                        {
                            if (model.UserNotAllowedLinkTypes.Count > 0)
                            {
                                model.UserNotAllowedLinkTypes
                                     .ForEach(u => u.UserId = user.Id);

                                //Validations
                                var detailValidationMessage = await _userNotAllowedLinkType.ValidateDetails(model.UserNotAllowedLinkTypes);

                                if (detailValidationMessage.ReturnMessage != "")
                                    throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                                foreach (var detail in model.UserNotAllowedLinkTypes)
                                {
                                    var addedDetails = await _userNotAllowedLinkType.AddInUser(detail);
                                }
                            }
                        }

                        var emailverification = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_EMAILVERIFICATION");

                        bool isemailverification = Convert.ToBoolean(emailverification.VSettingOption);

                        if (isemailverification)
                        {
                            if (!model.EmailVerificationDisabled)
                            {

                                var settings = await _dbCntxt.Setting.ToListAsync();

                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                                // var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Host.Value);  //Only use when Core & Angular are in same domain
                                var host = _config.GetSection("AppSettings")["AngularPath"];
                                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, host);

                                var webRoot = System.IO.Directory.GetCurrentDirectory();
                                var sPath = System.IO.Path.Combine(webRoot, "EmailTemplate/email-confirmation.html");

                                var builder = new StringBuilder();
                                using (var reader = System.IO.File.OpenText(sPath))
                                {
                                    builder.Append(reader.ReadToEnd());
                                }

                                builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                                builder.Replace("{username}", myTI.ToTitleCase(model.FirstName));

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
                        }

                        await this.SendInitialPassword(user.Id);

                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                        userReturn = this.Find(user.Id)
                                         .FirstOrDefault();

                        scope.Complete();

                    }
                    else
                        throw new CustomException("Something Wrong!", 400);
                }

                return userReturn;
            }
            catch (CustomException customex)
            {
                //dbAppContextTransaction.Rollback();
                //dbContextTransaction.Rollback();
                scope.Dispose();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                //dbAppContextTransaction.Rollback();
                //dbContextTransaction.Rollback();
                scope.Dispose();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(SaveUserModel model)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                //Check if state from front end is a valid UserAccountState Enum
                if (!(GeneralHelper.IsValidUserAccountState(Convert.ToInt32(model.State))))
                    throw new CustomException("User Account State is not valid", 400);

                AspNetUsers anu = _dbCntxt.AspNetUsers.Find(model.Id);

                if (anu != null)
                {
                    bool isEmailDuplicate = this.Get().Where(x => x.Email == model.Email && x.Id != model.Id).Any();
                    bool isUserNameDuplicate = this.Get().Where(x => x.UserName == model.UserName && x.Id != model.Id).Any();
                    if (isEmailDuplicate)
                        throw new CustomException("Email already exist!", 400);
                    if (isUserNameDuplicate)
                        throw new CustomException("Username already exist!", 400);
                    else
                    {

                        // get current user id
                        var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        // get current session id
                        int spId = await _userIpAddressPerSession.GetSPID();
                        // get IP address
                        string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        //add useripperssion
                        var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                        await _userIpAddressPerSession.Save(userIp);

                        anu.Email = model.Email;
                        anu.NormalizedEmail = model.Email.ToUpper();
                        anu.UserName = model.UserName;
                        anu.NormalizedUserName = model.UserName.ToUpper();
                        anu.PhoneNumber = model.PhoneNumber;
                        anu.BlockedAccount = model.BlockedAccount;
                        anu.Status = model.Status;

                        if (model.State == UserAccountState.Dormant_User || model.State == UserAccountState.Active || model.State == UserAccountState.Password_Expired)
                        {
                            anu.LoginAttempts = 0;
                            anu.LoginAttemptsDate = null;
                        }

                        if (model.State == UserAccountState.Initial_Password)
                            //Send initial password
                            if ((UserAccountState)anu.State != UserAccountState.Initial_Password)
                                await this.SendInitialPassword(model.Id);

                        anu.State = Convert.ToInt32(model.State);

                        _dbCntxt.Entry(anu).State = EntityState.Modified;
                        await _dbCntxt.SaveChangesAsync();

                        //User password policies
                        var userpass = await _dbCntxt.UserPasswordPolicies.Where(p => p.UserId == model.Id).FirstOrDefaultAsync();
                        userpass.CanChangePassword = model.CanChangePassword;
                        userpass.IsPasswordNeverExpires = model.IsPasswordNeverExpires;
                        _dbCntxt.Entry(userpass).State = EntityState.Modified;
                        await _dbCntxt.SaveChangesAsync();

                        bool isImageSaved = UploadImage(model.Photo, model.Id);
                        if (isImageSaved)
                        {
                            AspNetUsersProfile anupro = _dbCntxt.AspNetUsersProfile.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (anupro != null)
                            {
                                anupro.Id = model.Id;
                                anupro.FirstName = model.FirstName;
                                anupro.LastName = model.LastName;
                                anupro.MiddleName = model.MiddleName;
                                anupro.Gender = model.Gender;
                                anupro.Company = model.Company;
                                anupro.Department = model.Department;
                                anupro.Position = model.Position;
                                anupro.Rank = model.Rank;
                                anupro.Mobile = model.Mobile;
                                anupro.Photo = model.Phone;
                                anupro.Country = model.Country;
                                anupro.CreatedDate = DateTime.Now;
                                anupro.CreatedByPK = cid;
                                anupro.Photo = model.Photo == null ? model.Photo : "../../assets/images/profile_Image/" + model.Id + ".png" + GeneralHelper.GenerateGuidForImage();
                                anupro.OfficeId = model.OfficeId;
                                _dbCntxt.Entry(anupro).State = EntityState.Modified;
                            }
                            else
                            {
                                AspNetUsersProfile anup = new();
                                anup.Id = model.Id;
                                anup.FirstName = model.FirstName;
                                anup.LastName = model.LastName;
                                anup.Mobile = model.MiddleName;
                                anup.Gender = model.Gender;
                                anupro.Company = model.Company;
                                anupro.Department = model.Department;
                                anupro.Position = model.Position;
                                anupro.Rank = model.Rank;
                                anupro.Mobile = model.Mobile;
                                anupro.Photo = model.Phone;
                                anup.Country = model.Country;
                                anup.ModifiedDate = DateTime.Now;
                                anup.ModifiedByPK = cid;
                                anup.Photo = model.Photo == null ? model.Photo : "../../assets/images/profile_Image/" + model.Id + ".png" + GeneralHelper.GenerateGuidForImage();
                                anup.OfficeId = model.OfficeId;
                                _dbCntxt.AspNetUsersProfile.Add(anup);
                            }

                            await _dbCntxt.SaveChangesAsync();

                            //remove useripperssion
                            await _userIpAddressPerSession.Remove(spId);
                        }
                        else
                        {
                            var isDelete = DeleteImage(model.Id);

                            throw new CustomException("Something Wrong!", 400);
                        }
                    }
                }
                else
                    throw new CustomException("User not found.", 404);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                var isDelete = DeleteImage(model.Id);
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Delete(object obj)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var id = Convert.ToString(obj);

                var user = this.Find(id);

                if (user != null)
                {
                    var roleId = _dbCntxt.AspNetUserRoles.Where(x => x.UserId == id).FirstOrDefault().RoleId;
                    if (roleId == "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03")
                        throw new CustomException("Deletion of Super Admin account is not allowed!", 400);
                    else
                    {
                        // get current user id
                        var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        // get current session id
                        int spId = await _userIpAddressPerSession.GetSPID();
                        // get IP address
                        string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        //add useripperssion
                        var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                        await _userIpAddressPerSession.Save(userIp);

                        //check if user in process owner
                        var processOwners = await _dbCntxt.RequestTypeProcessOwners
                                                   .Include(r => r.RequestType)
                                                   .Where(r => r.Owner == id)
                                                   .ToListAsync();

                        if (processOwners.Count > 0)
                        {
                            List<string> requestTypes = new List<string>();

                            foreach(var processOwner in processOwners)
                            {
                                string message = processOwner.RequestType.Name + " and Version: " + processOwner.RequestType.Version.ToString();
                                requestTypes.Add(message);
                            }

                            throw new CustomException("This user was assigned as process owners under request types. ", 400, null, requestTypes);
                        }

                        //check if user in step assignees
                        var stepAssignees = await _dbCntxt.RequestStepAssigneeDetails
                                                   .Include(r => r.RequestStepAssignee)
                                                   .Include(r => r.RequestStepAssignee.RequestStep)
                                                   .Include(r => r.RequestStepAssignee.RequestStep.WorkFlowStep)
                                                   .Include(r => r.RequestStepAssignee.RequestStep.RequestType)
                                                   .Where(r => r.Assignee == id)
                                                   .ToListAsync();

                        if (stepAssignees.Count > 0)
                        {
                            List<string> requestTypes = new List<string>();

                            foreach (var stepAssignee in stepAssignees)
                            {
                                string message = stepAssignee.RequestStepAssignee.RequestStep.RequestType.Name + " and Version: " + stepAssignee.RequestStepAssignee.RequestStep.RequestType.Version.ToString() + " and Step: " + stepAssignee.RequestStepAssignee.RequestStep.WorkFlowStep.Name;

                                requestTypes.Add(message);
                            }

                            throw new CustomException("This user was assigned as step assignee under request types. ", 400, null, requestTypes);
                        }

                        _dbCntxt.UserOrganizationalStructures.RemoveRange(_dbCntxt.UserOrganizationalStructures.Where(x => x.UserId == id));
                        await _dbCntxt.SaveChangesAsync();

                        _dbCntxt.AspNetUsersLoginHistory.RemoveRange(_dbCntxt.AspNetUsersLoginHistory.Where(x => x.UserId == id));
                        await _dbCntxt.SaveChangesAsync();

                        _dbCntxt.AspNetUsersPageVisited.RemoveRange(_dbCntxt.AspNetUsersPageVisited.Where(x => x.UserId == id));
                        await _dbCntxt.SaveChangesAsync();

                        _dbCntxt.AspNetUserRoles.RemoveRange(_dbCntxt.AspNetUserRoles.Where(x => x.UserId == id));
                        await _dbCntxt.SaveChangesAsync();

                        _dbCntxt.AspNetUsersProfile.RemoveRange(_dbCntxt.AspNetUsersProfile.Where(x => x.Id == id));
                        await _dbCntxt.SaveChangesAsync();

                        _dbCntxt.UserPasswordPolicies.RemoveRange(_dbCntxt.UserPasswordPolicies.Where(x => x.UserId == id));
                        await _dbCntxt.SaveChangesAsync();

                        _dbCntxt.UserPreferences.RemoveRange(_dbCntxt.UserPreferences.Where(x => x.UserId == id));
                        await _dbCntxt.SaveChangesAsync();


                        _dbCntxt.AspNetUsers.RemoveRange(_dbCntxt.AspNetUsers.Where(x => x.Id == id));
                        await _dbCntxt.SaveChangesAsync();

                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                        bool isImageDeleted = this.DeleteImage(id);
                        if (!isImageDeleted)
                        {
                            dbContextTransaction.Rollback();
                            throw new CustomException("Something wrong in Image deletion!", 400);
                        }

                        dbContextTransaction.Commit();
                    }
                }
                else
                    throw new CustomException("User is not found.", 404);

            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details, customex.ListMessage);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<PageVisitedViewModel> GetPageVisited(string id = null, object obj = null)
        {
            try
            {
                IQueryable<PageVisitedViewModel> query = null;

                query = (from anu in _dbCntxt.AspNetUsers
                         join anupv in _dbCntxt.AspNetUsersPageVisited on anu.Id equals anupv.UserId
                         join anup in _dbCntxt.AspNetUsersProfile on anu.Id equals anup.Id into joined
                         from anup in joined.DefaultIfEmpty()
                         select new PageVisitedViewModel
                         {
                             UserId = anu.Id,
                             Name = anup.FirstName + " " + anup.LastName,
                             Email = anu.Email,
                             PageName = anupv.NvPageName,
                             VisitedDate = anupv.DDateVisited,
                             IP = anupv.NvIpaddress,
                         }).OrderByDescending(x => x.VisitedDate).AsQueryable();

                // get specific user 
                if (id != null)
                    query = query.Where(x => x.UserId == id);

                if (obj != null)
                {
                    PagingRequest paging = (PagingRequest)obj;

                    // get only specific user
                    var filter = paging.SearchCriteria.Filter;
                    if (!String.IsNullOrEmpty(filter))
                        query = query.Where(x => x.UserId == filter);

                    // default search 
                    var search = paging.Search.Value.ToLower();
                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        // customer search here
                    }
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<PageVisitedViewModel> PagingPageVisited(IQueryable<PageVisitedViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<PageVisitedViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<PageVisitedViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<LoginHistoryViewModel> GetLoginHistory(string id = null, object obj = null)
        {
            try
            {
                IQueryable<LoginHistoryViewModel> query = null;

                query = (from anu in _dbCntxt.AspNetUsers
                         join anulh in _dbCntxt.AspNetUsersLoginHistory on anu.Id equals anulh.UserId
                         join anup in _dbCntxt.AspNetUsersProfile on anu.Id equals anup.Id into joined
                         from anup in joined.DefaultIfEmpty()
                         select new LoginHistoryViewModel
                         {
                             UserId = anu.Id,
                             Name = anup.FirstName + " " + anup.LastName,
                             Email = anu.Email,
                             Login = anulh.DLogIn,
                             Logout = anulh.DLogOut,
                             IP = anulh.NvIpaddress
                         }).OrderByDescending(x => x.Login).AsQueryable();

                // get specific user 
                if (id != null)
                    query = query.Where(x => x.UserId == id);

                if (obj != null)
                {
                    PagingRequest paging = (PagingRequest)obj;

                    // get only specific user
                    var filter = paging.SearchCriteria.Filter;
                    if (!String.IsNullOrEmpty(filter))
                        query = query.Where(x => x.UserId == filter);

                    // default search 
                    var search = paging.Search.Value.ToLower();
                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        // customer search here
                    }
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<LoginHistoryViewModel> PagingLogHistory(IQueryable<LoginHistoryViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<LoginHistoryViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<LoginHistoryViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<ObjectReturnModel> GetProfileData()
        {
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var visitedList = await this.GetPageVisited(cId)
                                          .Select(x => new { x.PageName, x.VisitedDate, x.IP })
                                          .ToListAsync();

                var loginList = await this.GetLoginHistory(cId)
                                    .Select(x => new { x.Login, x.Logout, x.IP })
                                    .ToListAsync();

                var lastLogin = loginList[0].Login;

                //get password expiration reminder
                var passwordsettings = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_PASSWORDEXPIRATIONREMINDER");
                int reminder = Convert.ToInt32(passwordsettings.VSettingOption);

                var user = await this.Find(cId)
                                //.Where(x => x.Id == cId)
                                .Select(x =>
                                    new
                                    {
                                        x.Id,
                                        x.Email,
                                        x.UserName,
                                        x.TwoFactorEnabled,
                                        x.RoleId,
                                        x.RoleName,
                                        x.FirstName,
                                        x.LastName,
                                        x.MiddleName,
                                        x.FullName,
                                        x.Gender,
                                        x.Company,
                                        x.Department,
                                        x.Position,
                                        x.Rank,
                                        x.Country,
                                        x.Photo,
                                        x.Mobile,
                                        x.Phone,
                                        x.PhoneNumber,
                                        x.BlockedAccount,
                                        x.Status,
                                        x.Roles,
                                        x.IsPasswordNeverExpires,
                                        PasswordExpiration = (x.IsPasswordNeverExpires ? "" : reminder >= Convert.ToInt32((x.ExpirationDate - DateTime.Now).TotalDays) ? "Your password will expire in " + Convert.ToInt32((x.ExpirationDate - DateTime.Now).TotalDays).ToString() + " day(s)" : ""),
                                        LastLogin = lastLogin,
                                        TotalLogin = loginList.Count,
                                        TotalPageVisited = visitedList.Count,
                                        x.CanChangePassword,
                                        x.OfficeId,
                                        Office = x.OfficeId == null ? "" : _dbCntxt.Options.FirstOrDefault(o => o.OptionGroup == "Site Locations" && o.Id == x.OfficeId).Name
                                    }
                                ).FirstOrDefaultAsync();

                // Load Roles
                var Id = _dbCntxt.AspNetUserRoles.Where(x => x.RoleId == "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03").FirstOrDefault().UserId;
                Id = (cId == Id) ? null : Id;
                var roles = await (from role in _dbCntxt.AspNetRoles
                                   where role.Id != ((Id == null) ? null : "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03")
                                   select new
                                   {
                                       role.Id,
                                       role.Name
                                   }).OrderBy(x => x.Name).ToListAsync();

                // Load Preferences
                var isTwoFactorEnabled = Convert.ToBoolean(_dbCntxt.Setting.Where(x => x.VSettingId == "_TWOFACTORENABLED").FirstOrDefault().VSettingOption);
                var DisplaySetPassword = !await _userManager.HasPasswordAsync(await _userManager.FindByIdAsync(cId));

                //Temp commented only
                //var data = new ObjectReturnModel { Object1 = user, Object2 = isTwoFactorEnabled, Object3 = DisplaySetPassword };

                var data = new ObjectReturnModel { Object1 = user, Object2 = roles, Object3 = loginList, Object4 = visitedList, Object5 = isTwoFactorEnabled, Object6 = DisplaySetPassword };

                return data;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task UpdateProfile(ProfileViewModel data)
        {
            try
            {
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isUserSuperAdmin = _dbCntxt.AspNetUserRoles.Where(x => x.RoleId == "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03" && x.UserId == cId).Any();
                bool isAllowed = Convert.ToBoolean(_dbCntxt.Setting.Where(x => x.VSettingId == "_CHANGEPROFILE").FirstOrDefault().VSettingOption);
                if (isUserSuperAdmin || isAllowed)
                {
                    using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
                    try
                    {
                        bool isEmailDuplicate = _dbCntxt.AspNetUsers.Where(x => x.Email == data.Email && x.Id != cId).Any();
                        bool isUserNameDuplicate = _dbCntxt.AspNetUsers.Where(x => x.UserName == data.UserName && x.Id != data.Id).Any();
                        if (isEmailDuplicate)
                            throw new CustomException("Email already exist!", 400);
                        if (isUserNameDuplicate)
                            throw new CustomException("Username already exist!", 400);
                        else
                        {
                            AspNetUsers anu = _dbCntxt.AspNetUsers.Where(x => x.Id == cId).FirstOrDefault();
                            anu.Email = data.Email;
                            anu.NormalizedEmail = data.Email.ToUpper();
                            anu.UserName = data.UserName;
                            anu.NormalizedUserName = data.UserName.ToUpper();
                            anu.PhoneNumber = data.PhoneNumber;
                            _dbCntxt.Entry(anu).State = EntityState.Modified;
                            await _dbCntxt.SaveChangesAsync();

                            bool isImageSaved = UploadImage(data.Photo, cId);
                            if (isImageSaved)
                            {
                                AspNetUsersProfile anupro = _dbCntxt.AspNetUsersProfile.Where(x => x.Id == cId).FirstOrDefault();
                                if (anupro == null)
                                {
                                    AspNetUsersProfile anup = new();
                                    anup.Id = cId;
                                    anup.FirstName = data.FirstName;
                                    anup.LastName = data.LastName;
                                    anup.MiddleName = data.MiddleName;
                                    anup.Gender = data.Gender;
                                    anup.Company = data.Company;
                                    anup.Department = data.Department;
                                    anup.Position = data.Position;
                                    anup.Rank = data.Rank;
                                    anup.Mobile = data.Mobile;
                                    anup.Phone = data.Phone;
                                    anup.Country = data.Country;
                                    anup.Photo = data.Photo == null ? data.Photo : "../../assets/images/profile_Image/" + cId + ".png";
                                    anup.CreatedDate = DateTime.Now;
                                    anup.CreatedByPK = cId;
                                    _dbCntxt.AspNetUsersProfile.Add(anup);
                                }
                                else
                                {
                                    anupro.Id = cId;
                                    anupro.FirstName = data.FirstName;
                                    anupro.LastName = data.LastName;
                                    anupro.MiddleName = data.MiddleName;
                                    anupro.Gender = data.Gender;
                                    anupro.Company = data.Company;
                                    anupro.Department = data.Department;
                                    anupro.Position = data.Position;
                                    anupro.Rank = data.Rank;
                                    anupro.Mobile = data.Mobile;
                                    anupro.Phone = data.Phone;
                                    anupro.Country = data.Country;
                                    anupro.Photo = data.Photo == null ? data.Photo : "../../assets/images/profile_Image/" + cId + ".png";
                                    _dbCntxt.Entry(anupro).State = EntityState.Modified;
                                }
                                await _dbCntxt.SaveChangesAsync();
                            }
                            else
                            {
                                var isDelete = DeleteImage(cId);
                                dbContextTransaction.Rollback();
                                throw new CustomException("Something Wrong!", 400);
                            }
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (CustomException customex)
                    {
                        dbContextTransaction.Rollback();
                        throw new CustomException(customex.Message, customex.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        var isDelete = DeleteImage(cId);
                        dbContextTransaction.Rollback();
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                }
                else
                    throw new CustomException("Change Profile Not Allowed!", 400);
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

        public async Task<IdentityResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(cId);

                var passwordpolicies = _dbCntxt.UserPasswordPolicies.FirstOrDefault(up => up.UserId == cId);

                bool isAllowed = passwordpolicies.CanChangePassword;

                if (isAllowed)
                {
                    var passresult = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                    if (!passresult)
                        throw new CustomException("Old password mismatch.", 400);


                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (!result.Succeeded)
                    {
                        string errors = "";

                        foreach(var error in result.Errors)
                        {
                            errors += error.Description + " " + Environment.NewLine;
                        }

                        throw new CustomException(errors, 400);
                    }

                    var validity = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_PASSWORDVALIDITY");

                    passwordpolicies.ExpirationDate = DateTime.Now.AddDays(Convert.ToInt32(validity.VSettingOption));// should be based from the saved validity in global config plus one get the exact expiration date
                    await _dbCntxt.SaveChangesAsync();

                    return result;
                }
                else
                    throw new CustomException("You are not allowed to change your password.", 400);
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

        private bool UploadImage(string photo, string name)
        {
            try
            {
                if (photo != null)
                {
                    var webRoot = System.IO.Directory.GetCurrentDirectory();
                    var iPath = _config.GetSection("AppSettings")["ImagePath"] + "/assets/images/profile_Image/";
                    var sPath = System.IO.Path.Combine(webRoot, iPath);

                    string imageName = name + ".png";
                    string imgPath = System.IO.Path.Combine(sPath, imageName);

                    if (!System.IO.Directory.Exists(sPath))
                    {
                        System.IO.Directory.CreateDirectory(sPath);
                    }

                    if (photo.Contains(','))
                    {
                        var imgStr = photo.Split(',')[1];
                        byte[] imageBytes = Convert.FromBase64String(imgStr);
                        System.IO.File.WriteAllBytes(imgPath, imageBytes);
                    }
                }
                else
                {
                    this.DeleteImage(name);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DeleteImage(string name)
        {
            try
            {
                var webRoot = System.IO.Directory.GetCurrentDirectory();
                var iPath = _config.GetSection("AppSettings")["ImagePath"] + "/assets/images/profile_Image/";
                var sPath = System.IO.Path.Combine(webRoot, iPath);
                System.IO.File.Delete(sPath + name + ".png");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IQueryable<UserRoleViewModel> GetUserRole(object obj)
        {
            try
            {
                IQueryable<UserRoleViewModel> query = null;

                string userid = obj.ToString();

                query = _dbCntxt.AspNetUserRoles
                                .Include(r => r.Role)
                                .Where
                                (
                                    r =>
                                    r.UserId == userid
                                )
                                .Select
                                (
                                    r => new UserRoleViewModel
                                    {
                                        UserId = r.UserId,
                                        RoleId = r.RoleId,
                                        Role = r.Role.Name
                                    }
                                ).AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<UserPreferencesViewModel> GetUserPreferences()
        {
            try
            {
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var preferences = await _dbCntxt.UserPreferences
                                        .Where
                                        (
                                            p => p.UserId == cId
                                        )
                                        .ToListAsync();

                UserPreferencesViewModel userpreferences = new();
                userpreferences.UserId = cId;

                // Load homepage. set default homepage if user homepage is null or empty
                var checkDefaultHomePage = preferences.FirstOrDefault(c => c.Name == "_DEFAULTHOMEPAGE");
                if (checkDefaultHomePage == null)

                    if (_dbCntxt.Setting.Count() > 0)
                        userpreferences.DefaultHomePage = _dbCntxt.Setting.FirstOrDefault(s => s.VSettingId == "_DEFAULTHOMEPAGE").VSettingOption;
                    else
                        userpreferences.DefaultHomePage = "";
                else
                    userpreferences.DefaultHomePage = checkDefaultHomePage.Value;

                // Load Theme. set default theme if user theme is null or empty 
                var userTheme = preferences.FirstOrDefault(c => c.Name == "_DEFAULTTHEME");
                if (userTheme == null)
                    if (_dbCntxt.Setting.Count() > 0)
                        userpreferences.Theme = _dbCntxt.Setting.FirstOrDefault(s => s.VSettingId == "_DEFAULTTHEME").VSettingOption;
                    else
                        userpreferences.Theme = "";
                else
                    userpreferences.Theme = userTheme.Value;

                return userpreferences;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private void SaveUserPreference(string paramprefname, string paramprefvalue, string paramuserid)
        {
            var newpref = new UserPreferences
            {
                Name = paramprefname,
                Value = paramprefvalue,
                UserId = paramuserid,
                CreatedDate = DateTime.UtcNow
            };
            _dbCntxt.UserPreferences.Add(newpref);
        }

        public async Task SavePreferences(UserPreferencesViewModel model)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var preferencestoremove = await _dbCntxt.UserPreferences.Where(p => p.UserId == model.UserId).ToListAsync();

                _dbCntxt.UserPreferences.RemoveRange(preferencestoremove);
                await _dbCntxt.SaveChangesAsync();

                if (!(model.DefaultHomePage == null || String.IsNullOrEmpty(model.DefaultHomePage)))
                    SaveUserPreference("_DEFAULTHOMEPAGE", model.DefaultHomePage, model.UserId);

                if (!(model.Theme == null || String.IsNullOrEmpty(model.Theme)))
                    SaveUserPreference("_DEFAULTTHEME", model.Theme, model.UserId);

                await _dbCntxt.SaveChangesAsync();

                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task SaveInitialPassword(string userid, string password, DateTime passwordexpirationdate)
        {
            try
            {
                var userindb = await _dbCntxt.AspNetUsers.FindAsync(userid);

                if (userindb != null)
                {

                    var user = await _userManager.FindByIdAsync(userid);
                    userindb.PasswordHash = _signInManager.UserManager.PasswordHasher.HashPassword(user, password);
                    _dbCntxt.Entry(userindb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    var passwordpolicy = await _dbCntxt.UserPasswordPolicies.FirstOrDefaultAsync(p => p.UserId == userid);

                    passwordpolicy.InitialPassword = password;
                    passwordpolicy.InitialPasswordExpirationDate = passwordexpirationdate;

                    _dbCntxt.Entry(passwordpolicy).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
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

        }

        public async Task SendInitialPassword(string userid)
        {
            try
            {
                var userindb = await this.Find(userid).FirstOrDefaultAsync();

                if (userindb != null)
                {

                    var validity = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_INITIALPASSWORDVALIDITY");

                    DateTime passwordexpirationdate = DateTime.Now.AddDays(Convert.ToInt32(validity.VSettingOption));
                    string password = "";

                    //Generate random password
                    password = GeneralHelper.CreateRandomPassword(6);

                    //Save initial password first
                    await SaveInitialPassword(userid, password, passwordexpirationdate);


                    var settings = await _dbCntxt.Setting.ToListAsync();

                    var webRoot = System.IO.Directory.GetCurrentDirectory();
                    var sPath = System.IO.Path.Combine(webRoot, "EmailTemplate/initial-password.html");

                    var builder = new StringBuilder();
                    using (var reader = System.IO.File.OpenText(sPath))
                    {
                        builder.Append(reader.ReadToEnd());
                    }

                    builder.Replace("{logo}", _config.GetSection("AppSettings")["Logo"]);
                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                    builder.Replace("{username}", myTI.ToTitleCase(userindb.FirstName));

                    var appcode = settings.Where(s => s.VSettingId == "_SITECODE").Select(s => s.VSettingOption).FirstOrDefault();

                    if (appcode == "")
                        appcode = "FLOW";

                    var appname = settings.Where(s => s.VSettingId == "_SITENAME").Select(s => s.VSettingOption).FirstOrDefault();

                    if (appname == "")
                        appname = _config.GetSection("AppSettings")["AppName"];

                    builder.Replace("{appname}", appname); //_SITENAME
                    builder.Replace("{appcode}", "(" + appcode + ")"); //(_SITECODE)                                         
                    builder.Replace("{tempassword}", password);
                    builder.Replace("{tempasswordexpirationdate}", passwordexpirationdate.ToString("MM/dd/yyyy hh:mm tt"));

                    var domain = settings.Where(s => s.VSettingId == "_DOMAIN").Select(s => s.VSettingOption).FirstOrDefault();

                    //var domain = _config.GetSection("AppSettings")["AngularPath"];

                    if (domain == "")
                        domain = _config.GetSection("AppSettings")["AngularPath"];

                    builder.Replace("{loginlink}", domain);

                    var twitter = settings.Where(s => s.VSettingId == "_TWITTERPAGE").Select(s => s.VSettingOption).FirstOrDefault();

                    if (twitter == "")
                        twitter = _config.GetSection("Company")["SocialTwitter"];

                    builder.Replace("{twitterlink}", twitter); //Twitter Page / _TWITTERPAGE

                    var fbpage = settings.Where(s => s.VSettingId == "_FACEBOOKPAGE").Select(s => s.VSettingOption).FirstOrDefault();

                    if (fbpage == "")
                        fbpage = _config.GetSection("Company")["SocialFacebook"];

                    builder.Replace("{facebooklink}", fbpage); // Facebook page / _FACEBOOKPAGE

                    var company = settings.Where(s => s.VSettingId == "_COMPANY").Select(s => s.VSettingOption).FirstOrDefault();

                    if (company == "")
                        company = _config.GetSection("Company")["Company"];

                    builder.Replace("{companyname}", company); //Company /_COMPANY

                    var companyemail = settings.Where(s => s.VSettingId == "_CORPORATEEMAIL").Select(s => s.VSettingOption).FirstOrDefault();

                    if (companyemail == "")
                        companyemail = _config.GetSection("Company")["MailAddress"];

                    builder.Replace("{companyemail}", companyemail); //Corporate Email / _CORPORATEEMAIL

                    //Send email
                    await _emailSender.SendTemporaryCredentialsAsync(userindb.Email, builder.ToString());
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
        }

        public async Task ResendInitialPassword(string userid)
        {
            using (var trans = _dbCntxt.Database.BeginTransaction())
            {
                try
                {
                    var userindb = await _dbCntxt.AspNetUsers.FindAsync(userid);

                    if (userindb != null)
                    {
                        await this.SendInitialPassword(userid);
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

        public async Task<IEnumerable<UserListViewModel>> GetUsers()
        {
            List<UserListViewModel> users = new List<UserListViewModel>();

            try
            {
                users = await this.FindAll()
                            .Select
                            (
                                s => new UserListViewModel()
                                {
                                    Id = s.Id,
                                    FirstName = s.FirstName,
                                    LastName = s.LastName,
                                    MiddleName = s.MiddleName,
                                    Name = s.FullName,
                                    Name2 = s.FullName2,
                                    Position = s.Position,
                                    Department = s.Department,
                                    Email = s.Email,
                                    Status = s.Status,
                                    Office = s.Office
                                }
                            )
                            .OrderBy(u => u.Name)
                            .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return users;
        }

        private bool CheckIfDuplicateUserOrganizationalStructureInModel(SaveUserOrganizationalStructureViewModel model, List<SaveUserOrganizationalStructureViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                u => 
                                                u.UserId == model.UserId &&
                                                u.OrganizationalStructureId == model.OrganizationalStructureId &&
                                                u.ApprovalLevelDetailId == model.ApprovalLevelDetailId &&
                                                u.ReportingTo == model.ReportingTo
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateUserRoleInModel(UserRoleViewModel model, List<UserRoleViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                u =>
                                                u.UserId == model.UserId &&
                                                u.RoleId == model.RoleId
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }

        private bool CheckIfDuplicateUserOrganizationalStructureInDatabase(SaveUserOrganizationalStructureViewModel model)
        {
            bool returnValue = false;

            List<UserOrganizationalStructure> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = _dbCntxt.UserOrganizationalStructures
                                         .Where
                                         (
                                            u =>
                                            u.UserId == model.UserId &&
                                            u.OrganizationalStructureId == model.OrganizationalStructureId &&
                                            u.ApprovalLevelDetailId == model.ApprovalLevelDetailId &&
                                            u.ReportingTo == model.ReportingTo &&
                                            u.Id != model.Id
                                         ).ToList();
            }
            else
            {
                checkDuplicate = _dbCntxt.UserOrganizationalStructures
                                         .Where
                                         (
                                            u =>
                                            u.UserId == model.UserId &&
                                            u.OrganizationalStructureId == model.OrganizationalStructureId &&
                                            u.ApprovalLevelDetailId == model.ApprovalLevelDetailId &&
                                            u.ReportingTo == model.ReportingTo
                                         ).ToList();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }

        private async Task<UserOrganizationalStructureValidationViewModel> ValidateUserOrganizationalStructures(List<SaveUserOrganizationalStructureViewModel> model)
        {
            UserOrganizationalStructureValidationViewModel returnValue = new UserOrganizationalStructureValidationViewModel();

            returnValue.ReturnMessage = "";
            returnValue.ReturnCode = 0;

            try
            {
                var users = model.Select(u => u.UserId).Distinct().ToList();

                if (users.Count > 1)
                {
                    returnValue.ReturnMessage = "Only 1 specific user is allowed";
                    returnValue.ReturnCode = 400;
                    return returnValue;
                }

                string userId = model.FirstOrDefault().UserId;

                var checkIfExistingUser = await this.Get().FirstOrDefaultAsync(u => u.Id == userId);

                if (checkIfExistingUser == null)
                {
                    returnValue.ReturnMessage = "User is not found.";
                    returnValue.ReturnCode = 404;
                    return returnValue;
                }

                var organizationalStructures = await this._organizationalStructure.GetOrganizationalStructures(null, true, true)
                                                         .ToListAsync();
                var approvalLevelDetails = (await this._approvalLevel
                                                     .GetApprovalLevelDetailsInSequentialMannerAsync(true, true))
                                                     .ToList();

                var organizationalStructure = new OrganizationalStructureViewModel();
                var approvalLevelDetail = new ApprovalLevelDetailViewModel();

                //Validate Entries
                foreach (var userOrganizationalStructure in model)
                {
                    //Organizational Structure Id
                    if (userOrganizationalStructure.OrganizationalStructureId <= 0)
                    {
                        returnValue.ReturnMessage = "Organizational Structure is required.";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    organizationalStructure = organizationalStructures.FirstOrDefault(o => o.Id == userOrganizationalStructure.OrganizationalStructureId);

                    if (organizationalStructure == null)
                    {
                        returnValue.ReturnMessage = "Specified Organizational Structure is not found.";
                        returnValue.ReturnCode = 404;
                        return returnValue;
                    }
                    //Approval Level
                    if (userOrganizationalStructure.ApprovalLevelDetailId < 0)
                    {
                        returnValue.ReturnMessage = "Approval Level is required.";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }
                     
                    if (userOrganizationalStructure.ApprovalLevelDetailId > 0)
                    {
                        approvalLevelDetail = approvalLevelDetails.FirstOrDefault(o => o.Id == userOrganizationalStructure.ApprovalLevelDetailId);

                        if (approvalLevelDetail == null)
                        {
                            returnValue.ReturnMessage = "Specified Approval Level Detail is not found.";
                            returnValue.ReturnCode = 404;
                            return returnValue;
                        }
                        //Check if Desired Approval Level Entity is equivalent with Organizational Structure entity
                        //This is to prevent misaligned of organizational entity between approval level and organizational structure
                        if (userOrganizationalStructure.ApprovalLevelDetailId > 0)
                        {
                            if (organizationalStructure.OrganizationEntityId != approvalLevelDetail.OrganizationEntityId)
                            {
                                returnValue.ReturnMessage = "Approval Level Entity is not aligned with Organizational Structure Entity.";
                                returnValue.ReturnCode = 400;
                                return returnValue;
                            }
                        }
                    }

                    //Reporting To
                    if (userOrganizationalStructure.ReportingTo < 0)
                    {
                        returnValue.ReturnMessage = "Reporting To is required";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (userOrganizationalStructure.ReportingTo != 9999 && userOrganizationalStructure.ReportingTo > 0)
                    //9999 means NI
                    {
                        var checkReportingTo = approvalLevelDetails.FirstOrDefault(o => o.Id == userOrganizationalStructure.ReportingTo);

                        if (checkReportingTo == null)
                        {
                            returnValue.ReturnMessage = "Specified Reporting To is not found.";
                            returnValue.ReturnCode = 404;
                            return returnValue;
                        }

                        if (userOrganizationalStructure.ApprovalLevelDetailId > 0)
                        {
                            if (userOrganizationalStructure.ApprovalLevelDetailId == userOrganizationalStructure.ReportingTo)
                            {
                                returnValue.ReturnMessage = "Reporting To is the same with selected Approval Level.";
                                returnValue.ReturnCode = 400;
                                return returnValue;
                            }

                            if (approvalLevelDetail.RowNumber > checkReportingTo.RowNumber)
                            {
                                returnValue.ReturnMessage = "Desired reporting to is lower level with the selected user approval level.";
                                returnValue.ReturnCode = 400;
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (organizationalStructure.OrganizationEntityHierarchy < checkReportingTo.OrganizationEntityHierarchy)
                            {
                                returnValue.ReturnMessage = "Desired reporting to is not aligned with the entity of the selected organizational structure. ";
                                returnValue.ReturnCode = 400;
                                return returnValue;
                            }
                        }
                    }

                    if (this.CheckIfDuplicateUserOrganizationalStructureInModel(userOrganizationalStructure, model))
                    {
                        returnValue.ReturnMessage = "Duplicate User Organizational Structure Entry in Model";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (this.CheckIfDuplicateUserOrganizationalStructureInDatabase(userOrganizationalStructure))
                    {
                        returnValue.ReturnMessage = "Duplicate User Organizational Structure Entry in Database";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    var checkDuplicateUserOrg = new List<UserOrganizationalStructure>();
                    if (userOrganizationalStructure.Id > 0)
                        checkDuplicateUserOrg = await _dbCntxt.UserOrganizationalStructures
                                                       .Where
                                                       (
                                                           u =>
                                                           u.UserId == userId &&
                                                           u.OrganizationalStructureId == userOrganizationalStructure.OrganizationalStructureId &&
                                                           u.Id != userOrganizationalStructure.Id
                                                       ).ToListAsync();
                    else
                        checkDuplicateUserOrg = await _dbCntxt.UserOrganizationalStructures
                                                           .Where
                                                           (
                                                               u =>
                                                               u.UserId == userId &&
                                                               u.OrganizationalStructureId == userOrganizationalStructure.OrganizationalStructureId 
                                                           ).ToListAsync();

                    if (checkDuplicateUserOrg.Count > 0)
                    {
                        returnValue.ReturnMessage = "User cannot have the same organizational structure entry";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (userOrganizationalStructure.ApprovalLevelDetailId > 0)
                    {

                        var checkDuplicateUserApprovalLevel = new List<UserOrganizationalStructure>();

                        if (userOrganizationalStructure.Id > 0)
                        {
                            checkDuplicateUserApprovalLevel = await _dbCntxt.UserOrganizationalStructures
                                                        .Where
                                                        (
                                                            u =>
                                                            u.OrganizationalStructureId == userOrganizationalStructure.OrganizationalStructureId &&
                                                            u.ApprovalLevelDetailId == userOrganizationalStructure.ApprovalLevelDetailId &&
                                                            u.Id != userOrganizationalStructure.Id
                                                        ).ToListAsync();
                        }
                        else
                        {
                            checkDuplicateUserApprovalLevel = await _dbCntxt.UserOrganizationalStructures
                                                        .Where
                                                        (
                                                            u =>
                                                            u.OrganizationalStructureId == userOrganizationalStructure.OrganizationalStructureId &&
                                                            u.ApprovalLevelDetailId == userOrganizationalStructure.ApprovalLevelDetailId
                                                        ).ToListAsync();
                        }
                            
                        if (checkDuplicateUserApprovalLevel.Count > 0)
                        {
                            returnValue.ReturnMessage = "Approval level was already assigned on this organizational structure.";
                            returnValue.ReturnCode = 400;
                            return returnValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }

        private async Task SaveUserOrgStructures (List<SaveUserOrganizationalStructureViewModel> model)
        {
            try
            {
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //save User organizational structures
                foreach (var userOrg in model)
                {
                    //new assignee
                    var userOrganizationalStructure = new UserOrganizationalStructure();

                    if (userOrg.Id <= 0)
                    {
                        userOrganizationalStructure.UserId = userOrg.UserId;
                        userOrganizationalStructure.OrganizationalStructureId = userOrg.OrganizationalStructureId;
                        userOrganizationalStructure.ApprovalLevelDetailId = userOrg.ApprovalLevelDetailId;
                        userOrganizationalStructure.ReportingTo = userOrg.ReportingTo;
                        userOrganizationalStructure.Published = userOrg.Published;
                        userOrganizationalStructure.CreatedByPK = cId;
                        userOrganizationalStructure.CreatedDate = DateTime.Now;

                        _dbCntxt.UserOrganizationalStructures.Add(userOrganizationalStructure);
                        await _dbCntxt.SaveChangesAsync();
                    }
                    else
                    {
                        //update assignee
                        userOrganizationalStructure = _dbCntxt.UserOrganizationalStructures.FirstOrDefault(r => r.Id == userOrg.Id);

                        if (userOrganizationalStructure != null)
                        {
                            userOrganizationalStructure.UserId = userOrg.UserId;
                            userOrganizationalStructure.OrganizationalStructureId = userOrg.OrganizationalStructureId;
                            userOrganizationalStructure.ApprovalLevelDetailId = userOrg.ApprovalLevelDetailId;
                            userOrganizationalStructure.ReportingTo = userOrg.ReportingTo;
                            userOrganizationalStructure.Published = userOrg.Published;
                            userOrganizationalStructure.ModifiedByPK = cId;
                            userOrganizationalStructure.ModifiedDate = DateTime.Now;
                            _dbCntxt.Entry(userOrganizationalStructure).State = EntityState.Modified;
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task SaveUserRoles(List<UserRoleViewModel> model)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (model != null)
                {
                    if (model.Count > 0)
                    {
                        // get current user id
                        var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        // get current session id
                        int spId = await _userIpAddressPerSession.GetSPID();
                        // get IP address
                        string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        //add useripperssion
                        var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                        await _userIpAddressPerSession.Save(userIp);

                        //Validations
                        var userRoleValidationMessage = await this.ValidateUserRoles(model);

                        if (userRoleValidationMessage.ReturnMessage != "")
                            throw new CustomException(userRoleValidationMessage.ReturnMessage, userRoleValidationMessage.ReturnCode);

                        //Save User Organizational Structures
                        await this.SaveUserRolesInDb(model);

                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);
                    }
                }

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task SaveUserRolesInDb(List<UserRoleViewModel> model)
        {
            try
            {
                string userId = model.FirstOrDefault().UserId;

                //Delete Insert
                _dbCntxt.AspNetUserRoles.RemoveRange(_dbCntxt.AspNetUserRoles.Where(u => u.UserId == userId).ToList());
                await _dbCntxt.SaveChangesAsync();

                //save User organizational structures
                foreach (var uRole in model)
                {
                    //new assignee
                    var userRole = new AspNetUserRoles();

                     userRole.RoleId = uRole.RoleId;
                     userRole.UserId = uRole.UserId;

                    _dbCntxt.AspNetUserRoles.Add(userRole);
                    await _dbCntxt.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task SaveUserOrganizationalStructures(SaveUserOrganizationalStructureWithDBValidationViewModel model)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                string userId = model.UserOrganizationalStructures.FirstOrDefault().UserId;

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                await _userIpAddressPerSession.Save(userIp);

                //Insert User Organizational Structures
                bool withUserOrg = false;
                if (model.UserOrganizationalStructures != null)
                {
                    if (model.UserOrganizationalStructures.Count > 0)
                    {
                        withUserOrg = true;

                        if (model.WithCheckingOfEntryInDb)
                        {
                            var userOrganizationalStructuresIds = model.UserOrganizationalStructures.Select(c => c.Id);
                            //Remove GUI deleted user organizational structures in DB
                            var userOrganizationalStructures = await _dbCntxt.UserOrganizationalStructures
                                                                 .Where
                                                                 (
                                                                    u =>
                                                                    !userOrganizationalStructuresIds.Contains(u.Id) &&
                                                                    u.UserId == userId
                                                                 ).ToListAsync();

                            foreach (var userOrganizationalStructure in userOrganizationalStructures)
                            {
                                _dbCntxt.UserOrganizationalStructures.Remove(userOrganizationalStructure);
                                await _dbCntxt.SaveChangesAsync();
                            }
                        }

                        //Validations
                        var userOrgValidationMessage = await this.ValidateUserOrganizationalStructures(model.UserOrganizationalStructures);

                        if (userOrgValidationMessage.ReturnMessage != "")
                            throw new CustomException(userOrgValidationMessage.ReturnMessage, userOrgValidationMessage.ReturnCode);

                        //Save User Organizational Structures
                        await this.SaveUserOrgStructures(model.UserOrganizationalStructures);
                    }
                }

                if (model.WithCheckingOfEntryInDb && !withUserOrg)
                {
                    //delete user org per user
                    _dbCntxt.UserOrganizationalStructures.RemoveRange(_dbCntxt.UserOrganizationalStructures.Where(u => u.UserId == userId));
                    await _dbCntxt.SaveChangesAsync();
                }

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task<UserRoleValidationViewModel> ValidateUserRoles(List<UserRoleViewModel> model)
        {
            UserRoleValidationViewModel returnValue = new UserRoleValidationViewModel();

            returnValue.ReturnMessage = "";
            returnValue.ReturnCode = 0;

            try
            {
                var users = model.Select(u => u.UserId).Distinct().ToList();

                if (users.Count > 1)
                {
                    returnValue.ReturnMessage = "Only 1 specific user is allowed";
                    returnValue.ReturnCode = 400;
                    return returnValue;
                }

                foreach (var userRole in model)
                {
                    if (String.IsNullOrWhiteSpace(userRole.UserId))
                    {
                        returnValue.ReturnMessage = "User is required";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (String.IsNullOrWhiteSpace(userRole.RoleId))
                    {
                        returnValue.ReturnMessage = "Role is required";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    var checkIfExistingUser = await this.Get().FirstOrDefaultAsync(u => u.Id == userRole.UserId);

                    if (checkIfExistingUser == null)
                    {
                        returnValue.ReturnMessage = "User is not found";
                        returnValue.ReturnCode = 404;
                        return returnValue;
                    }

                    var checkIfExistingRole = await this._role.GetRole(false).FirstOrDefaultAsync(u => u.Id == userRole.RoleId);

                    if (checkIfExistingRole == null)
                    {
                        returnValue.ReturnMessage = "Role is not found";
                        returnValue.ReturnCode = 404;
                        return returnValue;
                    }

                    if (this.CheckIfDuplicateUserRoleInModel(userRole, model))
                    {
                        returnValue.ReturnMessage = "Duplicate User Role Entry in Model";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }

        public IQueryable<UserOrganizationalStructureViewModel> GetUserOrganizationalStructuresPerUser(string id, PagingRequest paging = null, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null)
        {
            try
            {
                IQueryable<UserOrganizationalStructureViewModel> query = null;

                query = this.GetUserOrganizationalStructures(published, organizationalStructurePublished, organizationEntityPublished)
                            .Where
                            (
                                u =>
                                u.UserId == id
                            ).AsQueryable();

                if (paging != null)
                {

                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query
                                .Where
                                (
                                    p => p.OrganizationalStructure.Contains(search) || 
                                    p.OrganizationEntityName.Contains(search) ||
                                    p.ApprovalLevel.ApprovalLevel.Contains(search) ||
                                    p.ApprovalLevel.ApprovalLevelAndSequence.Contains(search) ||
                                    p.ReportingToApprovalLevel.ApprovalLevel.Contains(search) ||
                                    p.ReportingToApprovalLevel.ApprovalLevelAndSequence.Contains(search)
                                );
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public IQueryable<UserOrganizationalStructureViewModel> GetUserOrganizationalStructures(bool? published = null, bool? organizationalStructurePublished = null, bool? organizationalEntityPublished = null)
        {
            try
            {
                IQueryable<UserOrganizationalStructureViewModel> query = null;

                query = _dbCntxt.UserOrganizationalStructures
                                .Include(o => o.OrganizationalStructure)
                                .Include(o => o.OrganizationalStructure.OrganizationEntity)
                                .Select
                                (
                                    u => new UserOrganizationalStructureViewModel
                                    {
                                        Id = u.Id,
                                        UserId = u.UserId,
                                        User = new UserRegisterModel
                                        {
                                            Id = u.User.Id,
                                            UserName = u.User.UserName,
                                            Email = u.User.Email,
                                            FirstName = u.User.AspNetUsersProfile.FirstName,
                                            LastName = u.User.AspNetUsersProfile.LastName,
                                            Status = u.User.Status
                                        },
                                        OrganizationalStructureId = u.OrganizationalStructureId,
                                        OrganizationalStructure = u.OrganizationalStructure.Name,
                                        OrganizationalStructurePublished = u.OrganizationalStructure.Published,
                                        OrganizationEntityId = u.OrganizationalStructure.OrganizationEntityId,
                                        OrganizationEntityName = u.OrganizationalStructure.OrganizationEntity.Name,
                                        OrganizationEntityPublished = u.OrganizationalStructure.OrganizationEntity.Published,
                                        ApprovalLevelDetailId = u.ApprovalLevelDetailId,
                                        ApprovalLevel = (u.ApprovalLevelDetailId > 0 ? _dbCntxt.ApprovalLevelDetails
                                                        .Include(a => a.ApprovelLevel)
                                                        .Where
                                                        (
                                                            a =>
                                                            a.Id == u.ApprovalLevelDetailId
                                                        )
                                                        .Select
                                                        (
                                                            a => new ApprovalLevelDetailViewModel
                                                            {
                                                                Id = a.Id,
                                                                ApprovalLevelId = a.ApprovalLevelId,
                                                                ApprovalLevel = a.ApprovelLevel.Name,
                                                                Sequence = a.Sequence,
                                                                ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                            }
                                                        )
                                                        .FirstOrDefault() : new ApprovalLevelDetailViewModel
                                                        {
                                                            Id = 0,
                                                            ApprovalLevelId = 0,
                                                            ApprovalLevel = "Staff",
                                                            Sequence = 0,
                                                            ApprovalLevelAndSequence = "Staff"
                                                        }),
                                        ReportingTo = u.ReportingTo,
                                        ReportingToApprovalLevel = (u.ReportingTo == 9999 ? new ApprovalLevelDetailViewModel
                                        {
                                            Id = 0,
                                            ApprovalLevelId = 0,
                                            ApprovalLevel = "Next in Line",
                                            Sequence = 0,
                                            ApprovalLevelAndSequence = "Next in Line"

                                        } : _dbCntxt.ApprovalLevelDetails
                                                                .Include(a => a.ApprovelLevel)
                                                                .Where
                                                                (
                                                                    a =>
                                                                    a.Id == u.ReportingTo
                                                                )
                                                                .Select
                                                                (
                                                                    a => new ApprovalLevelDetailViewModel
                                                                    {
                                                                        Id = a.Id,
                                                                        ApprovalLevelId = a.ApprovalLevelId,
                                                                        ApprovalLevel = a.ApprovelLevel.Name,
                                                                        Sequence = a.Sequence,
                                                                        ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                                    }
                                                                )
                                                                .FirstOrDefault()),
                                        ReportingToUser = _approvalLevel.GetSpecificApprovalLevelReportingTo(u.ApprovalLevelDetailId, u.ReportingTo, u.OrganizationalStructureId, null, null, null, null),
                                        Published = u.Published,
                                        CreatedDate = u.CreatedDate
                                    }
                                ).AsQueryable();

                if (published != null)
                    query = query.Where(u => u.Published == published);

                if (organizationalStructurePublished != null)
                    query = query.Where(u => u.OrganizationalStructurePublished == organizationalStructurePublished);

                if (organizationalEntityPublished != null)
                    query = query.Where(u => u.OrganizationEntityPublished == organizationalEntityPublished);

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<UserOrganizationalStructureViewModel> UserOrganizationalStructuresPagingFeature(IQueryable<UserOrganizationalStructureViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<UserOrganizationalStructureViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<UserOrganizationalStructureViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public PagingResponse<UserRegisterModel> PagingFeature(IQueryable<UserRegisterModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<UserRegisterModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<UserRegisterModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task DeleteUserOrganizationalStructure(int id)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {

                var userOrganizationalStructure = await _dbCntxt.UserOrganizationalStructures.FirstOrDefaultAsync(u => u.Id == id);

                if (userOrganizationalStructure == null)
                    throw new CustomException("User Organizational Structure to delete is not found", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                await _userIpAddressPerSession.Save(userIp);

                _dbCntxt.UserOrganizationalStructures.Remove(userOrganizationalStructure);
                await _dbCntxt.SaveChangesAsync();

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<RequestTypeBaseWorkAssigneeFlowViewModel>> GetUserOrganizationalStructurePath(int userOrganizationalStructureId, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationalEntityPublished = null, bool? userPublished = null, bool? approvalLevelPublished = null)
        {

            List<RequestTypeBaseWorkAssigneeFlowViewModel> assignees = new List<RequestTypeBaseWorkAssigneeFlowViewModel>();

            try 
            {
                var userOrganizationalStructure = await this.GetUserOrganizationalStructures(published, organizationalStructurePublished, organizationalEntityPublished).FirstOrDefaultAsync(u => u.Id == userOrganizationalStructureId);

                int organizationalStructureId = 0;

                if (userOrganizationalStructure != null)
                    organizationalStructureId = userOrganizationalStructure.OrganizationalStructureId;

                var orgWithUsers = (await _approvalLevel.GetOrganizationalStructurePathWithUser(organizationalStructureId, organizationalStructurePublished, organizationalEntityPublished, userPublished, approvalLevelPublished)).ToList();

                var approvalLevelDetails = (await _approvalLevel.GetApprovalLevelDetailsInSequentialMannerAsync(approvalLevelPublished, organizationalEntityPublished))
                                           .ToList();

                int userApprovalLevelRowNumber = _approvalLevel.GetApprovalDetailRowNumber(userOrganizationalStructure.ApprovalLevelDetailId, approvalLevelDetails);

                int userReportingToRowNumber = _approvalLevel.GetApprovalDetailRowNumber(userOrganizationalStructure.ReportingTo, approvalLevelDetails);

                var usersInPath = orgWithUsers
                                .Where
                                (
                                    u =>
                                    u.ApprovalLevelDetailRowNumber > userApprovalLevelRowNumber
                                )
                                .OrderBy(u => u.ApprovalLevelDetailRowNumber)
                                .ToList();

                if (usersInPath.Count > 0)
                {
                    if (userOrganizationalStructure.ReportingTo > 0)
                    {
                        int sortOrder = 1;

                        var nextUserInPath = new OrganizationalStructureWithUserViewModel();

                        if (userOrganizationalStructure.ReportingTo == 9999)
                        {
                            nextUserInPath = usersInPath
                                            .FirstOrDefault();

                            if (nextUserInPath != null)
                            {
                                assignees.Add(new RequestTypeBaseWorkAssigneeFlowViewModel()
                                {
                                    OrganizationalEntityId = nextUserInPath.OrganizationEntityId,
                                    OrganizationalEntity = nextUserInPath.OrganizationEntityName,
                                    OrganizationalEntityHierarchy = nextUserInPath.OrganizationEntityHierarchy,
                                    OrganizationalStructureId = nextUserInPath.Id,
                                    OrganizationalStructure = nextUserInPath.Name,
                                    ApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                    ApprovalLevelDetail = nextUserInPath.ApprovalLevel.ApprovalLevelAndSequence,
                                    ApprovalLevelId = nextUserInPath.ApprovalLevel.ApprovalLevelId,
                                    ApprovalLevel = nextUserInPath.ApprovalLevel.ApprovalLevel,
                                    UserId = nextUserInPath.UserId,
                                    SortOrder = sortOrder
                                });

                                assignees.AddRange(this.GetUserOrganizationalStructurePathDetails(nextUserInPath, usersInPath, sortOrder));
                            }
                        }
                        else
                        {
                            nextUserInPath = usersInPath
                                             .Where
                                             (
                                                u =>
                                                u.ApprovalLevelDetailId == userOrganizationalStructure.ReportingTo
                                             )
                                             .FirstOrDefault();

                            if (nextUserInPath != null)
                            {
                                assignees.Add(new RequestTypeBaseWorkAssigneeFlowViewModel()
                                {
                                    OrganizationalEntityId = nextUserInPath.OrganizationEntityId,
                                    OrganizationalEntity = nextUserInPath.OrganizationEntityName,
                                    OrganizationalEntityHierarchy = nextUserInPath.OrganizationEntityHierarchy,
                                    OrganizationalStructureId = nextUserInPath.Id,
                                    OrganizationalStructure = nextUserInPath.Name,
                                    ApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                    ApprovalLevelDetail = nextUserInPath.ApprovalLevel.ApprovalLevelAndSequence,
                                    ApprovalLevelId = nextUserInPath.ApprovalLevel.ApprovalLevelId,
                                    ApprovalLevel = nextUserInPath.ApprovalLevel.ApprovalLevel,
                                    UserId = nextUserInPath.UserId,
                                    SortOrder = sortOrder
                                });

                                assignees.AddRange(this.GetUserOrganizationalStructurePathDetails(nextUserInPath, usersInPath, sortOrder));
                            }
                            else
                            {
                                nextUserInPath = usersInPath
                                                .Where
                                                (
                                                   u =>
                                                   u.ApprovalLevelDetailRowNumber > userReportingToRowNumber
                                                )
                                                .FirstOrDefault();

                                if (nextUserInPath != null)
                                {
                                    assignees.Add(new RequestTypeBaseWorkAssigneeFlowViewModel()
                                    {
                                        OrganizationalEntityId = nextUserInPath.OrganizationEntityId,
                                        OrganizationalEntity = nextUserInPath.OrganizationEntityName,
                                        OrganizationalEntityHierarchy = nextUserInPath.OrganizationEntityHierarchy,
                                        OrganizationalStructureId = nextUserInPath.Id,
                                        OrganizationalStructure = nextUserInPath.Name,
                                        ApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                        ApprovalLevelDetail = nextUserInPath.ApprovalLevel.ApprovalLevelAndSequence,
                                        ApprovalLevelId = nextUserInPath.ApprovalLevel.ApprovalLevelId,
                                        ApprovalLevel = nextUserInPath.ApprovalLevel.ApprovalLevel,
                                        UserId = nextUserInPath.UserId,
                                        SortOrder = sortOrder
                                    });

                                    assignees.AddRange(this.GetUserOrganizationalStructurePathDetails(nextUserInPath, usersInPath, sortOrder));
                                }
                            }
                        }
                    }
                }
                
                return assignees;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private List<RequestTypeBaseWorkAssigneeFlowViewModel> GetUserOrganizationalStructurePathDetails(OrganizationalStructureWithUserViewModel userInPath, List<OrganizationalStructureWithUserViewModel> usersInPath, int approvalSortOrder)
        {
            List<RequestTypeBaseWorkAssigneeFlowViewModel> usersToReturn = new List<RequestTypeBaseWorkAssigneeFlowViewModel>();

            int sortOrder = 0;

            if (userInPath != null)
            {
                if (userInPath.ReportingTo > 0)
                {
                    sortOrder = approvalSortOrder + 1;

                    var nextUsersInPath = usersInPath
                                   .Where(u => u.ApprovalLevelDetailRowNumber > userInPath.ApprovalLevelDetailRowNumber)
                                   .OrderBy(u => u.ApprovalLevelDetailRowNumber)
                                   .ToList();

                    if (userInPath.ReportingTo == 9999)
                    {
                        var nextUserInPath = nextUsersInPath.FirstOrDefault();

                        if (nextUserInPath != null)
                        {
                            usersToReturn.Add(new RequestTypeBaseWorkAssigneeFlowViewModel()
                            {
                                OrganizationalEntityId = nextUserInPath.OrganizationEntityId,
                                OrganizationalEntity = nextUserInPath.OrganizationEntityName,
                                OrganizationalEntityHierarchy = nextUserInPath.OrganizationEntityHierarchy,
                                OrganizationalStructureId = nextUserInPath.Id,
                                OrganizationalStructure = nextUserInPath.Name,
                                ApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                ApprovalLevelDetail = nextUserInPath.ApprovalLevel.ApprovalLevelAndSequence,
                                ApprovalLevelId = nextUserInPath.ApprovalLevel.ApprovalLevelId,
                                ApprovalLevel = nextUserInPath.ApprovalLevel.ApprovalLevel,
                                UserId = nextUserInPath.UserId,
                                SortOrder = sortOrder
                            });

                            usersToReturn.AddRange(this.GetUserOrganizationalStructurePathDetails(nextUserInPath, usersInPath, sortOrder));
                        }
                    }
                    else
                    {
                        var nextUserInPath = nextUsersInPath
                                             .Where
                                             (
                                                u =>
                                                u.ApprovalLevelDetailId == userInPath.ReportingTo
                                             )
                                             .FirstOrDefault();

                        if (nextUserInPath != null)
                        {
                            usersToReturn.Add(new RequestTypeBaseWorkAssigneeFlowViewModel()
                            {
                                OrganizationalEntityId = nextUserInPath.OrganizationEntityId,
                                OrganizationalEntity = nextUserInPath.OrganizationEntityName,
                                OrganizationalEntityHierarchy = nextUserInPath.OrganizationEntityHierarchy,
                                OrganizationalStructureId = nextUserInPath.Id,
                                OrganizationalStructure = nextUserInPath.Name,
                                ApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                ApprovalLevelDetail = nextUserInPath.ApprovalLevel.ApprovalLevelAndSequence,
                                ApprovalLevelId = nextUserInPath.ApprovalLevel.ApprovalLevelId,
                                ApprovalLevel = nextUserInPath.ApprovalLevel.ApprovalLevel,
                                UserId = nextUserInPath.UserId,
                                SortOrder = sortOrder
                            });

                            usersToReturn.AddRange(this.GetUserOrganizationalStructurePathDetails(nextUserInPath, usersInPath, sortOrder));
                        }
                        else
                        {
                            nextUserInPath = nextUsersInPath
                                             .Where
                                             (
                                                u =>
                                                u.ApprovalLevelDetailRowNumber > userInPath.ReportingToRowNumber
                                             )
                                             .FirstOrDefault();

                            if (nextUserInPath != null)
                            {
                                usersToReturn.Add(new RequestTypeBaseWorkAssigneeFlowViewModel()
                                {
                                    OrganizationalEntityId = nextUserInPath.OrganizationEntityId,
                                    OrganizationalEntity = nextUserInPath.OrganizationEntityName,
                                    OrganizationalEntityHierarchy = nextUserInPath.OrganizationEntityHierarchy,
                                    OrganizationalStructureId = nextUserInPath.Id,
                                    OrganizationalStructure = nextUserInPath.Name,
                                    ApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                    ApprovalLevelDetail = nextUserInPath.ApprovalLevel.ApprovalLevelAndSequence,
                                    ApprovalLevelId = nextUserInPath.ApprovalLevel.ApprovalLevelId,
                                    ApprovalLevel = nextUserInPath.ApprovalLevel.ApprovalLevel,
                                    UserId = nextUserInPath.UserId,
                                    SortOrder = sortOrder
                                });

                                usersToReturn.AddRange(this.GetUserOrganizationalStructurePathDetails(nextUserInPath, usersInPath, sortOrder));
                            }
                        }
                    }
                }
            }

            return usersToReturn;
        }
    }
}
