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
using workflow.Helpers;
using System.Text.Json;
using System.Data.Entity.Core.Objects;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using workflow.Data;

namespace workflow.Services
{
    public class UserNotAllowedLinkTypeRepository : BaseApi, IUserNotAllowedLinkType
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public UserNotAllowedLinkTypeRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
        }

        public async Task<UserNotAllowedLinkTypeViewModel> Add(UserNotAllowedLinkTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (String.IsNullOrWhiteSpace(model.UserId))
                throw new CustomException("User is required.", 400);

            if (model.LinkTypeId <= 0)
                throw new CustomException("Link Type is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var validLinkType = await _dbCntxt.LinkTypes.FirstOrDefaultAsync(r => r.Id == model.LinkTypeId);

                if (validLinkType == null)
                    throw new CustomException("Link Type is not found.", 404);

                var validUser = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(r => r.Id == model.UserId);

                if (validUser == null)
                    throw new CustomException("Specific user is not found.", 404);

                var checkIfExistingRecord = await _dbCntxt.UserNotAllowedLinkTypes
                                                          .Where
                                                          (
                                                              u => u.UserId == model.UserId &&
                                                              u.LinkTypeId == model.LinkTypeId &&
                                                              u.Category == (int) model.Category
                                                          ).FirstOrDefaultAsync();

                string category = model.Category == UserNotAllowedLinkTypeCategory.Request ? "Request" : "WorkFlow";

                if (checkIfExistingRecord != null)
                    throw new CustomException("Specified user and Link Type already exists under " + category + " category.", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();

                if (saveIpPerSession)
                {
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                    await _userIpAddressPerSession.Save(userIp);
                }

                //Insert User Not Allowed Link Types 
                var userNotAllowedLinkType = new UserNotAllowedLinkType()
                {
                    UserId = model.UserId,
                    LinkTypeId = model.LinkTypeId,
                    Category = (int) model.Category,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.UserNotAllowedLinkTypes.Add(userNotAllowedLinkType);
                await _dbCntxt.SaveChangesAsync();

                model.Id = userNotAllowedLinkType.Id;
                model.CreatedByPK = userNotAllowedLinkType.CreatedByPK;
                model.CreatedDate = userNotAllowedLinkType.CreatedDate;

                if (saveIpPerSession)
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                if (transaction == null)
                    transactionToUse.Commit();

                return model;
            }
            catch (CustomException customex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (transaction == null)
                    transactionToUse.Dispose();
            }
        }

        public async Task<UserNotAllowedLinkTypeViewModel> AddInUser(UserNotAllowedLinkTypeViewModel model)
        {
            if (String.IsNullOrWhiteSpace(model.UserId))
                throw new CustomException("User is required.", 400);

            if (model.LinkTypeId <= 0)
                throw new CustomException("Link Type is required.", 400);

            try
            {
                var validLinkType = await _dbCntxt.LinkTypes.FirstOrDefaultAsync(r => r.Id == model.LinkTypeId);

                if (validLinkType == null)
                    throw new CustomException("Link Type is not found.", 404);

                var validUser = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(r => r.Id == model.UserId);

                if (validUser == null)
                    throw new CustomException("Specific user is not found.", 404);

                var checkIfExistingRecord = await _dbCntxt.UserNotAllowedLinkTypes
                                                          .Where
                                                          (
                                                              u => u.UserId == model.UserId &&
                                                              u.LinkTypeId == model.LinkTypeId
                                                          ).FirstOrDefaultAsync();

                if (checkIfExistingRecord != null)
                    throw new CustomException("Specified user and Link Type already exists.", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //Insert User Not Allowed Link Types 
                var userNotAllowedLinkType = new UserNotAllowedLinkType()
                {
                    UserId = model.UserId,
                    LinkTypeId = model.LinkTypeId,
                    Category = (int) model.Category,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.UserNotAllowedLinkTypes.Add(userNotAllowedLinkType);
                await _dbCntxt.SaveChangesAsync();

                model.Id = userNotAllowedLinkType.Id;
                model.CreatedByPK = userNotAllowedLinkType.CreatedByPK;
                model.CreatedDate = userNotAllowedLinkType.CreatedDate;

                return model;
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

        public async Task Delete(object obj, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                int id = (int)obj;

                var userNotAllowedLinkType = await _dbCntxt.UserNotAllowedLinkTypes.FindAsync(id);

                if (userNotAllowedLinkType == null)
                    throw new CustomException("Entry to delete is not found. ", 404);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();

                if (saveIpPerSession)
                {
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);
                }

                _dbCntxt.UserNotAllowedLinkTypes.Remove(userNotAllowedLinkType);
                await _dbCntxt.SaveChangesAsync();

                if (saveIpPerSession)
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                if (transaction == null)
                    transactionToUse.Commit();
            }
            catch (CustomException customex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new CustomException(customex.Message, customex.StatusCode, customex.Details, customex.ListMessage);
            }
            catch (Exception ex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (transaction == null)
                    transactionToUse.Dispose();
            }
        }

        public async Task DeleteAll(UserNotAllowedLinkTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<UserNotAllowedLinkTypeViewModel> Find(object obj)
        {
            try
            {

                IQueryable<UserNotAllowedLinkTypeViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new UserNotAllowedLinkTypeViewModel
                                {
                                    Id = r.Id,
                                    UserId = r.UserId,
                                    User = new UserBasicInfoModel()
                                    {
                                        Id = r.User.Id,
                                        FirstName = r.User.AspNetUsersProfile.FirstName,
                                        LastName = r.User.AspNetUsersProfile.LastName,
                                        MiddleName = r.User.AspNetUsersProfile.MiddleName,
                                        FullName = r.User.AspNetUsersProfile.FirstName + " " + r.User.AspNetUsersProfile.LastName,
                                        Status = r.User.Status
                                    },
                                    LinkTypeId = r.LinkTypeId,
                                    LinkTypeName = r.LinkType.Name,
                                    LinkTypeURL = r.LinkType.URL,
                                    LinkTypePublished = r.LinkType.Published,
                                    Category = (UserNotAllowedLinkTypeCategory) r.Category,
                                    CategoryDescription = r.Category == 1 ? "Request" : "WorkFlow",
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate
                                }
                            )
                            .AsQueryable();

                return query;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserNotAllowedLinkTypeViewModel> FindAll()
        {
            try
            {
                IQueryable<UserNotAllowedLinkTypeViewModel> query = null;
                query = this.Get()
                           .Select
                            (
                                r => new UserNotAllowedLinkTypeViewModel
                                {
                                    Id = r.Id,
                                    UserId = r.UserId,
                                    User = new UserBasicInfoModel()
                                    {
                                        Id = r.User.Id,
                                        FirstName = r.User.AspNetUsersProfile.FirstName,
                                        LastName = r.User.AspNetUsersProfile.LastName,
                                        MiddleName = r.User.AspNetUsersProfile.MiddleName,
                                        FullName = r.User.AspNetUsersProfile.FirstName + " " + r.User.AspNetUsersProfile.LastName,
                                        Status = r.User.Status
                                    },
                                    LinkTypeId = r.LinkTypeId,
                                    LinkTypeName = r.LinkType.Name,
                                    LinkTypeURL = r.LinkType.URL,
                                    LinkTypePublished = r.LinkType.Published,
                                    Category = (UserNotAllowedLinkTypeCategory)r.Category,
                                    CategoryDescription = r.Category == 1 ? "Request" : "WorkFlow",
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate
                                }
                            )
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            throw new NotImplementedException();
        }

        public async Task Update(UserNotAllowedLinkTypeViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (String.IsNullOrWhiteSpace(model.UserId))
                throw new CustomException("User is required.", 400);

            if (model.LinkTypeId <= 0)
                throw new CustomException("Link Type is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var validLinkType = await _dbCntxt.LinkTypes.FirstOrDefaultAsync(r => r.Id == model.LinkTypeId);

                if (validLinkType == null)
                    throw new CustomException("Link Type is not found.", 404);

                var validUser = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(r => r.Id == model.UserId);

                if (validUser == null)
                    throw new CustomException("Specific user is not found.", 404);

                var checkIfExistingRecord = await _dbCntxt.UserNotAllowedLinkTypes
                                                          .Where
                                                          (
                                                              u => u.UserId == model.UserId &&
                                                              u.LinkTypeId == model.LinkTypeId &&
                                                              u.Category == (int) model.Category &&
                                                              u.Id != model.Id
                                                          ).FirstOrDefaultAsync();

                string category = model.Category == UserNotAllowedLinkTypeCategory.Request ? "Request" : "WorkFlow";

                if (checkIfExistingRecord != null)
                    throw new CustomException("Specified user and Link Type already exists under " + category + " category.", 400);

                var userNotAllowedLinkTypes = await _dbCntxt.UserNotAllowedLinkTypes.FirstOrDefaultAsync(a => a.Id == tableId);

                if (userNotAllowedLinkTypes != null)
                {
                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();

                    if (saveIpPerSession)
                    {
                        // get IP address
                        string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        //add useripperssion
                        var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                        await _userIpAddressPerSession.Save(userIp);
                    }

                    userNotAllowedLinkTypes.UserId = model.UserId;
                    userNotAllowedLinkTypes.LinkTypeId = model.LinkTypeId;
                    userNotAllowedLinkTypes.Category = (int) model.Category;
                    userNotAllowedLinkTypes.Published = model.Published;
                    userNotAllowedLinkTypes.ModifiedByPK = cid;
                    userNotAllowedLinkTypes.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(userNotAllowedLinkTypes).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Entry is not found", 404);
            }
            catch (CustomException customex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (transaction == null)
                    transactionToUse.Dispose();
            }
        }

        private IQueryable<UserNotAllowedLinkType> Get()
        {
            try
            {
                IQueryable<UserNotAllowedLinkType> query = null;

                query = _dbCntxt.UserNotAllowedLinkTypes
                                .Include(u => u.User)
                                .Include(u => u.User.AspNetUsersProfile)
                                .Include(r => r.LinkType)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private bool CheckIfDuplicateDetailInModel(UserNotAllowedLinkTypeViewModel model, List<UserNotAllowedLinkTypeViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                u =>
                                                u.UserId == model.UserId &&
                                                u.LinkTypeId == model.LinkTypeId &&
                                                u.Category == model.Category &&
                                                u.Published == model.Published
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private async Task<bool> CheckIfDuplicateDetailInDatabase(UserNotAllowedLinkTypeViewModel model)
        {
            bool returnValue = false;

            List<UserNotAllowedLinkType> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = await _dbCntxt.UserNotAllowedLinkTypes
                                         .Where
                                         (
                                            u =>
                                            u.UserId == model.UserId &&
                                            u.LinkTypeId == model.LinkTypeId &&
                                            u.Category == (int) model.Category &&
                                            u.Published == model.Published &&
                                            u.Id != model.Id
                                         ).ToListAsync();
            }
            else
            {
                checkDuplicate = await _dbCntxt.UserNotAllowedLinkTypes
                                         .Where
                                         (
                                             u =>
                                            u.UserId == model.UserId &&
                                            u.LinkTypeId == model.LinkTypeId &&
                                            u.Category == (int) model.Category &&
                                            u.Published == model.Published
                                         ).ToListAsync();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }

        public async Task<UserNotAllowedLinkTypeValidationViewModel> ValidateDetails(List<UserNotAllowedLinkTypeViewModel> model)
        {
            UserNotAllowedLinkTypeValidationViewModel returnValue = new UserNotAllowedLinkTypeValidationViewModel();

            returnValue.ReturnMessage = "";
            returnValue.ReturnCode = 0;

            try
            {
                var user = model.Select(u => u.UserId).Distinct().ToList();

                if (user.Count > 1)
                {
                    returnValue.ReturnMessage = "Only 1 specific user is allowed";
                    returnValue.ReturnCode = 400;
                    return returnValue;
                }

                //Validate Entries
                foreach (var detail in model)
                {
                    if (this.CheckIfDuplicateDetailInModel(detail, model))
                    {
                        returnValue.ReturnMessage = "Duplicate Entry in Model";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (await this.CheckIfDuplicateDetailInDatabase(detail))
                    {
                        returnValue.ReturnMessage = "Duplicate Entry in Database";
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

        public async Task SaveUserNotAllowedLinkType(SaveUserNotAllowedLinkTypeWithCheckingDetailEntryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                string userId = model.UserNotAllowedLinkTypes.FirstOrDefault().UserId;
                // get current user id
                string cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();

                if (saveIpPerSession)
                {
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                    await _userIpAddressPerSession.Save(userIp);
                }

                //Insert User Not Allowed Link Types
                bool withDetails = false;
                if (model.UserNotAllowedLinkTypes != null)
                {
                    if (model.UserNotAllowedLinkTypes.Count > 0)
                    {
                        withDetails = true;

                        if (model.WithCheckingOfDetailEntryInDb)
                        {
                            var detailsIds = model.UserNotAllowedLinkTypes.Select(c => c.Id);
                            //Remove GUI deleted in DB
                            var details = await _dbCntxt.UserNotAllowedLinkTypes
                                                        .Where
                                                        (
                                                            u =>
                                                            !detailsIds.Contains(u.Id) &&
                                                            u.UserId == userId
                                                        ).ToListAsync();

                            foreach (var detail in details)
                            {
                                _dbCntxt.UserNotAllowedLinkTypes.Remove(detail);
                                await _dbCntxt.SaveChangesAsync();
                            }
                        }

                        //Validations
                        var detailValidationMessage = await this.ValidateDetails(model.UserNotAllowedLinkTypes);

                        if (detailValidationMessage.ReturnMessage != "")
                            throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                        foreach (var detail in model.UserNotAllowedLinkTypes)
                        {
                            if (detail.Id > 0)
                                await this.Update(detail, detail.Id, false, transactionToUse);
                            else
                            {
                                var addedDetails = await this.Add(detail, false, transactionToUse);
                            }
                        }
                    }
                }

                if (model.WithCheckingOfDetailEntryInDb && !withDetails)
                {
                    //delete user org per user
                    _dbCntxt.UserNotAllowedLinkTypes.RemoveRange(_dbCntxt.UserNotAllowedLinkTypes.Where(u => u.UserId == userId));
                    await _dbCntxt.SaveChangesAsync();
                }

                if (saveIpPerSession)
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                if (transaction == null)
                    transactionToUse.Commit();

            }
            catch (CustomException customException)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new CustomException(customException.Message, customException.StatusCode);
            }
            catch (Exception ex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (transaction == null)
                    transactionToUse.Dispose();
            }
        }
    }
}
