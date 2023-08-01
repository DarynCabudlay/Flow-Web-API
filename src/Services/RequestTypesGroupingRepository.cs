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

namespace workflow.Services
{
    public class RequestTypesGroupingRepository : BaseApi, IRequestTypesGrouping
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestTypesGroupingDetail _requestTypesGroupingDetail;

        public RequestTypesGroupingRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IRequestTypesGroupingDetail requestTypesGroupingDetail) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _requestTypesGroupingDetail = requestTypesGroupingDetail;
        }

        public async Task<RequestTypesGroupingViewModel> Add(RequestTypesGroupingViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                if (await this.IfExists(model.Name))
                    throw new CustomException("Request Types Grouping: " + model.Name + " is already exists.", 400);

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

                //Insert request types grouping
                var requestTypesGrouping = new RequestTypesGrouping()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.RequestTypesGroupings.Add(requestTypesGrouping);
                await _dbCntxt.SaveChangesAsync();

                if (model.RequestTypesGroupingDetails != null)
                {
                    if (model.RequestTypesGroupingDetails.Count > 0)
                    {
                        model.RequestTypesGroupingDetails
                             .ForEach(u => u.RequestTypesGroupingId = requestTypesGrouping.Id);

                        //Validations
                        var detailValidationMessage = await _requestTypesGroupingDetail.ValidateDetails(model.RequestTypesGroupingDetails);

                        if (detailValidationMessage.ReturnMessage != "")
                            throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                        foreach(var detail in model.RequestTypesGroupingDetails)
                        {
                            var addedDetails = await _requestTypesGroupingDetail.Add(detail, false, transactionToUse);
                        }
                    }
                }

                model.Id = requestTypesGrouping.Id;
                model.CreatedByPK = requestTypesGrouping.CreatedByPK;
                model.CreatedDate = requestTypesGrouping.CreatedDate;

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

                var requestTypesGrouping = await _dbCntxt.RequestTypesGroupings.FindAsync(id);

                if (requestTypesGrouping == null)
                    throw new CustomException("Request Types Grouping to delete is not found. ", 404);

                //Check if used as user approval level and reporting to
                var userRequestTypesGrouping = await _dbCntxt.UserRequestTypesGroupings
                                                      .FirstOrDefaultAsync(u => u.RequestTypesGroupId == id);

                if (userRequestTypesGrouping != null)
                    throw new CustomException("This request types grouping is currently being used in user maintenance.", 400);

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

                _dbCntxt.RequestTypesGroupingDetails.RemoveRange(_dbCntxt.RequestTypesGroupingDetails.Where(r => r.RequestTypesGroupingId == id));
                await _dbCntxt.SaveChangesAsync();

                _dbCntxt.RequestTypesGroupings.Remove(requestTypesGrouping);
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

        public async Task DeleteAll(RequestTypesGroupingViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<RequestTypesGroupingViewModel> Find(object obj)
        {
            try
            {
                IQueryable<RequestTypesGroupingViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new RequestTypesGroupingViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = String.IsNullOrWhiteSpace(r.Description) ? "" : r.Description,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate,
                                    RequestTypesGroupingDetails = r.RequestTypesGroupingDetails
                                                                  .Select
                                                                  (
                                                                      r => new RequestTypesGroupingDetailViewModel()
                                                                      {
                                                                          Id = r.Id,
                                                                          RequestTypesGroupingId = r.RequestTypesGroupingId,
                                                                          RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                                                          RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                                                          RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                                                          RequestTypeId = r.RequestTypeId,
                                                                          RequestTypeName = r.RequestTypes.Name,
                                                                          Version = r.Version,
                                                                          Published = r.Published,
                                                                          CreatedDate = r.CreatedDate,
                                                                          ModifiedDate = r.ModifiedDate
                                                                      }
                                                                  ).ToList()

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

        public IQueryable<RequestTypesGroupingViewModel> FindAll()
        {
            try
            {
                IQueryable<RequestTypesGroupingViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                r => new RequestTypesGroupingViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = String.IsNullOrWhiteSpace(r.Description) ? "" : r.Description,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate,
                                    RequestTypesGroupingDetails = r.RequestTypesGroupingDetails
                                                                  .Select
                                                                  (
                                                                      r => new RequestTypesGroupingDetailViewModel()
                                                                      {
                                                                          Id = r.Id,
                                                                          RequestTypesGroupingId = r.RequestTypesGroupingId,
                                                                          RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                                                          RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                                                          RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                                                          RequestTypeId = r.RequestTypeId,
                                                                          RequestTypeName = r.RequestTypes.Name,
                                                                          Version = r.Version,
                                                                          Published = r.Published,
                                                                          CreatedDate = r.CreatedDate,
                                                                          ModifiedDate = r.ModifiedDate
                                                                      }
                                                                  ).ToList()
                                    
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
            bool isexists = false;
            int condition = 0;
            try
            {
                if (entityPrimaryKey == null)
                {
                    var requesttypesindb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch);

                    if (requesttypesindb != null)
                        isexists = true;
                }
                else
                {
                    condition = Convert.ToInt32(entityPrimaryKey);

                    var requesttypesindb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch && r.Id != condition);

                    if (requesttypesindb != null)
                        isexists = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return isexists;
        }

        public async Task Update(RequestTypesGroupingViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                if (await this.IfExists(model.Name, tableId))
                    throw new CustomException("Request Types Grouping: " + model.Name + " is already exists.", 400);

                var requestTypesGrouping = await _dbCntxt.RequestTypesGroupings.FirstOrDefaultAsync(a => a.Id == tableId);

                if (requestTypesGrouping != null)
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
                   
                    requestTypesGrouping.Name = model.Name;
                    requestTypesGrouping.Description = model.Description;
                    requestTypesGrouping.Published = model.Published;
                    requestTypesGrouping.ModifiedByPK = cid;
                    requestTypesGrouping.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypesGrouping).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    //Insert Request Types Grouping Details
                    bool withDetails = false;
                    if (model.RequestTypesGroupingDetails != null)
                    {
                        if (model.RequestTypesGroupingDetails.Count > 0)
                        {
                            withDetails = true;

                            if (model.WithCheckingOfDetailEntryInDb)
                            {
                                var detailsIds = model.RequestTypesGroupingDetails.Select(c => c.Id);
                                //Remove GUI deleted in DB
                                var details = await _dbCntxt.RequestTypesGroupingDetails
                                                            .Where
                                                            (
                                                                u =>
                                                                !detailsIds.Contains(u.Id) &&
                                                                u.RequestTypesGroupingId == model.Id
                                                            ).ToListAsync();

                                foreach (var detail in details)
                                {
                                    _dbCntxt.RequestTypesGroupingDetails.Remove(detail);
                                    await _dbCntxt.SaveChangesAsync();
                                }
                            }

                            //Validations
                            var detailValidationMessage = await _requestTypesGroupingDetail.ValidateDetails(model.RequestTypesGroupingDetails);

                            if (detailValidationMessage.ReturnMessage != "")
                                throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                            foreach(var detail in model.RequestTypesGroupingDetails)
                            {
                                if (detail.Id > 0)
                                    await _requestTypesGroupingDetail.Update(detail, detail.Id, false, transactionToUse);
                                else
                                {
                                    var addedDetails = await _requestTypesGroupingDetail.Add(detail, false, transactionToUse);
                                }  
                            }
                        }
                    }

                    if (model.WithCheckingOfDetailEntryInDb && !withDetails)
                    {
                        //delete user org per user
                        _dbCntxt.RequestTypesGroupingDetails.RemoveRange(_dbCntxt.RequestTypesGroupingDetails.Where(u => u.RequestTypesGroupingId == tableId));
                        await _dbCntxt.SaveChangesAsync();
                    }

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Request Types Grouping is not found", 404);
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

        private IQueryable<RequestTypesGrouping> Get()
        {
            try
            {
                IQueryable<RequestTypesGrouping> query = null;

                query = _dbCntxt.RequestTypesGroupings
                                .Include(r => r.RequestTypesGroupingDetails)
                                .ThenInclude(r => r.RequestTypes)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
