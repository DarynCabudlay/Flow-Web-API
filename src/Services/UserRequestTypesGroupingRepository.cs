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
    public class UserRequestTypesGroupingRepository : BaseApi, IUserRequestTypesGrouping
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public UserRequestTypesGroupingRepository(
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

        public async Task<UserRequestTypesGroupingViewModel> Add(UserRequestTypesGroupingViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (String.IsNullOrWhiteSpace(model.UserId))
                throw new CustomException("User is required.", 400);

            if (model.RequestTypesGroupId <= 0)
                throw new CustomException("Request Type Grouping is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var validRequestTypesGrouping = await _dbCntxt.RequestTypesGroupings.FirstOrDefaultAsync(r => r.Id == model.RequestTypesGroupId);

                if (validRequestTypesGrouping == null)
                    throw new CustomException("Request Type Grouping is not found.", 404);

                var validUser = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(r => r.Id == model.UserId);

                if (validUser == null)
                    throw new CustomException("Specific user is not found.", 404);

                var checkIfExistingRecord = await _dbCntxt.UserRequestTypesGroupings
                                                          .Where
                                                          (
                                                              u => u.UserId == model.UserId && 
                                                              u.RequestTypesGroupId == model.RequestTypesGroupId
                                                          ).FirstOrDefaultAsync();

                if (checkIfExistingRecord != null)
                    throw new CustomException("Specified user and Request Type Grouping already exists.", 400);

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

                //Insert User Request Types Grouping
                var userRequestTypesGrouping = new UserRequestTypesGrouping()
                {
                    UserId = model.UserId,
                    RequestTypesGroupId = model.RequestTypesGroupId,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.UserRequestTypesGroupings.Add(userRequestTypesGrouping);
                await _dbCntxt.SaveChangesAsync();

                model.Id = userRequestTypesGrouping.Id;
                model.CreatedByPK = userRequestTypesGrouping.CreatedByPK;
                model.CreatedDate = userRequestTypesGrouping.CreatedDate;

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

        public async Task<UserRequestTypesGroupingViewModel> AddInUser(UserRequestTypesGroupingViewModel model)
        {
            if (String.IsNullOrWhiteSpace(model.UserId))
                throw new CustomException("User is required.", 400);

            if (model.RequestTypesGroupId <= 0)
                throw new CustomException("Request Type Grouping is required.", 400);

            try
            {
                var validRequestTypesGrouping = await _dbCntxt.RequestTypesGroupings.FirstOrDefaultAsync(r => r.Id == model.RequestTypesGroupId);

                if (validRequestTypesGrouping == null)
                    throw new CustomException("Request Type Grouping is not found.", 404);

                var validUser = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(r => r.Id == model.UserId);

                if (validUser == null)
                    throw new CustomException("Specific user is not found.", 404);

                var checkIfExistingRecord = await _dbCntxt.UserRequestTypesGroupings
                                                          .Where
                                                          (
                                                              u => u.UserId == model.UserId &&
                                                              u.RequestTypesGroupId == model.RequestTypesGroupId
                                                          ).FirstOrDefaultAsync();

                if (checkIfExistingRecord != null)
                    throw new CustomException("Specified user and Request Type Grouping already exists.", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //Insert User Request Types Grouping
                var userRequestTypesGrouping = new UserRequestTypesGrouping()
                {
                    UserId = model.UserId,
                    RequestTypesGroupId = model.RequestTypesGroupId,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.UserRequestTypesGroupings.Add(userRequestTypesGrouping);
                await _dbCntxt.SaveChangesAsync();

                model.Id = userRequestTypesGrouping.Id;
                model.CreatedByPK = userRequestTypesGrouping.CreatedByPK;
                model.CreatedDate = userRequestTypesGrouping.CreatedDate;

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

                var userRequestTypesGrouping = await _dbCntxt.UserRequestTypesGroupings.FindAsync(id);

                if (userRequestTypesGrouping == null)
                    throw new CustomException("User Request Types Grouping to delete is not found. ", 404);

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

                _dbCntxt.UserRequestTypesGroupings.Remove(userRequestTypesGrouping);
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

        public async Task DeleteAll(UserRequestTypesGroupingViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<UserRequestTypesGroupingViewModel> Find(object obj)
        {
            try
            {

                IQueryable<UserRequestTypesGroupingViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new UserRequestTypesGroupingViewModel
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
                                    RequestTypesGroupId = r.RequestTypesGroupId,
                                    RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                    RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                    RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
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

        public IQueryable<UserRequestTypesGroupingViewModel> FindAll()
        {
            try
            {
                IQueryable<UserRequestTypesGroupingViewModel> query = null;
                query = this.Get()
                           .Select
                            (
                                r => new UserRequestTypesGroupingViewModel
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
                                    RequestTypesGroupId = r.RequestTypesGroupId,
                                    RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                    RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                    RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
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

        public async Task Update(UserRequestTypesGroupingViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (String.IsNullOrWhiteSpace(model.UserId))
                throw new CustomException("USer is required.", 400);

            if (model.RequestTypesGroupId <= 0)
                throw new CustomException("Request Types Grouping is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var validRequestTypesGrouping = await _dbCntxt.RequestTypesGroupings.FirstOrDefaultAsync(r => r.Id == model.RequestTypesGroupId);

                if (validRequestTypesGrouping == null)
                    throw new CustomException("Request Type Grouping is not found.", 404);

                var validUser = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(r => r.Id == model.UserId);

                if (validUser == null)
                    throw new CustomException("Specific user is not found.", 404);

                var checkIfExistingRecord = await _dbCntxt.UserRequestTypesGroupings
                                                          .Where
                                                          (
                                                              u => u.UserId == model.UserId &&
                                                              u.RequestTypesGroupId == model.RequestTypesGroupId &&
                                                              u.Id != model.Id
                                                          ).FirstOrDefaultAsync();

                if (checkIfExistingRecord != null)
                    throw new CustomException("Specified user and Request Type Grouping already exists.", 400);

                var userRequestTypesGrouping = await _dbCntxt.UserRequestTypesGroupings.FirstOrDefaultAsync(a => a.Id == tableId);

                if (userRequestTypesGrouping != null)
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

                    userRequestTypesGrouping.UserId = model.UserId;
                    userRequestTypesGrouping.RequestTypesGroupId = model.RequestTypesGroupId;
                    userRequestTypesGrouping.Published = model.Published;
                    userRequestTypesGrouping.ModifiedByPK = cid;
                    userRequestTypesGrouping.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(userRequestTypesGrouping).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("User Request Type Grouping is not found", 404);
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

        private IQueryable<UserRequestTypesGrouping> Get()
        {
            try
            {
                IQueryable<UserRequestTypesGrouping> query = null;

                query = _dbCntxt.UserRequestTypesGroupings
                                .Include(u => u.User)
                                .Include(u => u.User.AspNetUsersProfile)
                                .Include(r => r.RequestTypesGrouping)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private bool CheckIfDuplicateDetailInModel(UserRequestTypesGroupingViewModel model, List<UserRequestTypesGroupingViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                u =>
                                                u.UserId == model.UserId &&
                                                u.RequestTypesGroupId == model.RequestTypesGroupId &&
                                                u.Published == model.Published
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private async Task<bool> CheckIfDuplicateDetailInDatabase(UserRequestTypesGroupingViewModel model)
        {
            bool returnValue = false;

            List<UserRequestTypesGrouping> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = await _dbCntxt.UserRequestTypesGroupings
                                         .Where
                                         (
                                            u =>
                                            u.UserId == model.UserId &&
                                            u.RequestTypesGroupId == model.RequestTypesGroupId &&
                                            u.Published == model.Published &&
                                            u.Id != model.Id
                                         ).ToListAsync();
            }
            else
            {
                checkDuplicate = await _dbCntxt.UserRequestTypesGroupings
                                         .Where
                                         (
                                            u =>
                                            u.UserId == model.UserId &&
                                            u.RequestTypesGroupId == model.RequestTypesGroupId &&
                                            u.Published == model.Published
                                         ).ToListAsync();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }
        public async Task<UserRequestTypesGroupingValidationViewModel> ValidateDetails(List<UserRequestTypesGroupingViewModel> model)
        {
            UserRequestTypesGroupingValidationViewModel returnValue = new UserRequestTypesGroupingValidationViewModel();

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
                        returnValue.ReturnMessage = "Duplicate User Request Types Grouping Entry in Model";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (await this.CheckIfDuplicateDetailInDatabase(detail))
                    {
                        returnValue.ReturnMessage = "Duplicate User Request Types Grouping Entry in Database";
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

        public async Task SaveUserRequestTypesGrouping(SaveUserRequestTypesGroupingWithCheckingDetailEntryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                string userId = model.UserRequestTypesGroupings.FirstOrDefault().UserId;
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

                //Insert User Request Types Grouping
                bool withDetails = false;
                if (model.UserRequestTypesGroupings != null)
                {
                    if (model.UserRequestTypesGroupings.Count > 0)
                    {
                        withDetails = true;

                        if (model.WithCheckingOfDetailEntryInDb)
                        {
                            var detailsIds = model.UserRequestTypesGroupings.Select(c => c.Id);
                            //Remove GUI deleted in DB
                            var details = await _dbCntxt.UserRequestTypesGroupings
                                                        .Where
                                                        (
                                                            u =>
                                                            !detailsIds.Contains(u.Id) &&
                                                            u.UserId == userId
                                                        ).ToListAsync();

                            foreach (var detail in details)
                            {
                                _dbCntxt.UserRequestTypesGroupings.Remove(detail);
                                await _dbCntxt.SaveChangesAsync();
                            }
                        }

                        //Validations
                        var detailValidationMessage = await this.ValidateDetails(model.UserRequestTypesGroupings);

                        if (detailValidationMessage.ReturnMessage != "")
                            throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                        foreach (var detail in model.UserRequestTypesGroupings)
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
                    _dbCntxt.UserRequestTypesGroupings.RemoveRange(_dbCntxt.UserRequestTypesGroupings.Where(u => u.UserId == userId));
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
