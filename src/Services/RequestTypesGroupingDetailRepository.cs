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
    public class RequestTypesGroupingDetailRepository : BaseApi, IRequestTypesGroupingDetail
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestTypesGroupingCommon _requestTypesGroupingCommon;
        private readonly IRequestType _requestType;

        public RequestTypesGroupingDetailRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IRequestType requestType, IRequestTypesGroupingCommon requestTypesGroupingCommon) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _requestType = requestType;
            _requestTypesGroupingCommon = requestTypesGroupingCommon;
        }

        public async Task<RequestTypesGroupingDetailViewModel> Add(RequestTypesGroupingDetailViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                int requestTypesGroupingId = model.RequestTypesGroupingId;

                var checkIfExistingId = await _requestTypesGroupingCommon
                                               .GetRequestTypesGroupings()
                                               .FirstOrDefaultAsync(u => u.Id == requestTypesGroupingId);

                if (checkIfExistingId == null)
                    throw new CustomException("Request Types Grouping is not found.", 404);

                //Request Type Id
                if (model.RequestTypeId <= 0)
                    throw new CustomException("Request Type is required.", 400);

                //Version
                if (model.Version <= 0)
                    throw new CustomException("Request Type Version is required.", 400);

                var requestType = await _dbCntxt.RequestTypes
                                                .FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestType == null)
                    throw new CustomException("Specified Request Type is not found.", 404);

                var requestTypeDetail = await _dbCntxt.RequestTypesGroupingDetails
                                                .FirstOrDefaultAsync
                                                (
                                                    r => r.RequestTypesGroupingId == requestTypesGroupingId &&
                                                    r.RequestTypeId == model.RequestTypeId &&
                                                    r.Version == model.Version
                                                );

                if (requestTypeDetail != null)
                    throw new CustomException("Request Type and Version is already exists.", 400);

                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();

                if (saveIpPerSession)
                {
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cId, ip);
                    await _userIpAddressPerSession.Save(userIp);
                }

                RequestTypesGroupingDetail newDetail = new RequestTypesGroupingDetail();
                newDetail.RequestTypesGroupingId = model.RequestTypesGroupingId;
                newDetail.RequestTypeId = model.RequestTypeId;
                newDetail.Version = model.Version;
                newDetail.Published = model.Published;
                newDetail.CreatedByPK = cId;
                newDetail.CreatedDate = DateTime.Now;

                _dbCntxt.RequestTypesGroupingDetails.Add(newDetail);
                await _dbCntxt.SaveChangesAsync();

                model.Id = newDetail.Id;
                model.CreatedByPK = newDetail.CreatedByPK;
                model.CreatedDate = newDetail.CreatedDate;

                if (saveIpPerSession)
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                if (transaction == null)
                    transactionToUse.Commit();

                return model;
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

                var requestTypesGroupingDetail = await _dbCntxt.RequestTypesGroupingDetails.FindAsync(id);

                if (requestTypesGroupingDetail == null)
                    throw new CustomException("Request Types Grouping Detail to delete is not found. ", 404);

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

                _dbCntxt.RequestTypesGroupingDetails.Remove(requestTypesGroupingDetail);
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

        public async Task DeleteAll(RequestTypesGroupingDetailViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        private IQueryable<RequestTypesGroupingDetail> Get()
        {
            try
            {
                IQueryable<RequestTypesGroupingDetail> query = null;

                query = _dbCntxt.RequestTypesGroupingDetails
                                .Include(r => r.RequestTypes)
                                .Include(r => r.RequestTypesGrouping)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<RequestTypesGroupingDetailViewModel> Find(object obj)
        {
            try
            {
                IQueryable<RequestTypesGroupingDetailViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new RequestTypesGroupingDetailViewModel
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

        public IQueryable<RequestTypesGroupingDetailViewModel> FindAll()
        {
            try
            {
                IQueryable<RequestTypesGroupingDetailViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                r => new RequestTypesGroupingDetailViewModel
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

        public Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            throw new NotImplementedException();
        }

        public async Task Update(RequestTypesGroupingDetailViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var requestTypesGroupingDetail = await _dbCntxt.RequestTypesGroupingDetails
                                                         .FirstOrDefaultAsync(a => a.Id == tableId);

                if (requestTypesGroupingDetail != null)
                {
                    int requestTypesGroupingId = model.RequestTypesGroupingId;

                    var checkIfExistingId = await _requestTypesGroupingCommon
                                                  .GetRequestTypesGroupings()
                                                  .FirstOrDefaultAsync(u => u.Id == requestTypesGroupingId);

                    if (checkIfExistingId == null)
                        throw new CustomException("Request Types Grouping is not found.", 404);

                    //Request Type Id
                    if (model.RequestTypeId <= 0)
                        throw new CustomException("Request Type is required.", 400);

                    //Version
                    if (model.Version <= 0)
                        throw new CustomException("Request Type Version is required.", 400);

                    var requestType = await _dbCntxt.RequestTypes
                                                    .FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                    if (requestType == null)
                        throw new CustomException("Specified Request Type is not found.", 404);

                    var requestTypeDetail = await _dbCntxt.RequestTypesGroupingDetails
                                                .FirstOrDefaultAsync
                                                (
                                                    r => r.RequestTypesGroupingId == requestTypesGroupingId &&
                                                    r.RequestTypeId == model.RequestTypeId &&
                                                    r.Version == model.Version &&
                                                    r.Id != tableId
                                                );

                    if (requestTypeDetail != null)
                        throw new CustomException("Request Type and Version is already exists.", 400);

                    var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();

                    if (saveIpPerSession)
                    {
                        // get IP address
                        string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        //add useripperssion
                        var userIp = _userIpAddressPerSession.AddData(spId, cId, ip);
                        await _userIpAddressPerSession.Save(userIp);
                    }

                    requestTypesGroupingDetail.RequestTypesGroupingId = model.RequestTypesGroupingId;
                    requestTypesGroupingDetail.RequestTypeId = model.RequestTypeId;
                    requestTypesGroupingDetail.Version = model.Version;
                    requestTypesGroupingDetail.Published = model.Published;
                    requestTypesGroupingDetail.ModifiedByPK = cId;
                    requestTypesGroupingDetail.ModifiedDate = DateTime.Now;

                    _dbCntxt.Entry(requestTypesGroupingDetail).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Request Types Grouping Detail is not found", 404);

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

        public async Task SaveRequestTypesGroupingDetails(SaveRequestTypesGroupingDetailWithCheckingDetailEntryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                int requestTypesGroupingId = model.RequestTypesGroupDetails.FirstOrDefault().RequestTypesGroupingId;
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

                //Insert Request Types Grouping Details
                bool withDetails = false;
                if (model.RequestTypesGroupDetails != null)
                {
                    if (model.RequestTypesGroupDetails.Count > 0)
                    {
                        withDetails = true;

                        if (model.WithCheckingOfDetailEntryInDb)
                        {
                            var detailsIds = model.RequestTypesGroupDetails.Select(c => c.Id);
                            //Remove GUI deleted in DB
                            var details = await _dbCntxt.RequestTypesGroupingDetails
                                                        .Where
                                                        (
                                                            u =>
                                                            !detailsIds.Contains(u.Id) &&
                                                            u.RequestTypesGroupingId == requestTypesGroupingId
                                                        ).ToListAsync();

                            foreach (var detail in details)
                            {
                                _dbCntxt.RequestTypesGroupingDetails.Remove(detail);
                                await _dbCntxt.SaveChangesAsync();
                            }
                        }

                        //Validations
                        var detailValidationMessage = await this.ValidateDetails(model.RequestTypesGroupDetails);

                        if (detailValidationMessage.ReturnMessage != "")
                            throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                        foreach (var detail in model.RequestTypesGroupDetails)
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
                    _dbCntxt.RequestTypesGroupingDetails.RemoveRange(_dbCntxt.RequestTypesGroupingDetails.Where(u => u.RequestTypesGroupingId == requestTypesGroupingId));
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

        private bool CheckIfDuplicateDetailInModel(RequestTypesGroupingDetailViewModel model, List<RequestTypesGroupingDetailViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                u =>
                                                u.RequestTypesGroupingId == model.RequestTypesGroupingId &&
                                                u.RequestTypeId == model.RequestTypeId &&
                                                u.Version == model.Version &&
                                                u.Published == model.Published
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private async Task<bool> CheckIfDuplicateDetailInDatabase(RequestTypesGroupingDetailViewModel model)
        {
            bool returnValue = false;

            List<RequestTypesGroupingDetail> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = await _dbCntxt.RequestTypesGroupingDetails
                                         .Where
                                         (
                                            u =>
                                            u.RequestTypesGroupingId == model.RequestTypesGroupingId &&
                                            u.RequestTypeId == model.RequestTypeId &&
                                            u.Version == model.Version &&
                                            u.Published == model.Published &&
                                            u.Id != model.Id
                                         ).ToListAsync();
            }
            else
            {
                checkDuplicate = await _dbCntxt.RequestTypesGroupingDetails
                                         .Where
                                         (
                                            u =>
                                            u.RequestTypesGroupingId == model.RequestTypesGroupingId &&
                                            u.RequestTypeId == model.RequestTypeId &&
                                            u.Version == model.Version &&
                                            u.Published == model.Published
                                         ).ToListAsync();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }
        public async Task<RequestTypesGroupingDetailValidationViewModel> ValidateDetails(List<RequestTypesGroupingDetailViewModel> model)
        {
            RequestTypesGroupingDetailValidationViewModel returnValue = new RequestTypesGroupingDetailValidationViewModel();

            returnValue.ReturnMessage = "";
            returnValue.ReturnCode = 0;

            try
            {
                var requestTypesGroupings = model.Select(u => u.RequestTypesGroupingId).Distinct().ToList();

                if (requestTypesGroupings.Count > 1)
                {
                    returnValue.ReturnMessage = "Only 1 specific request type grouping is allowed";
                    returnValue.ReturnCode = 400;
                    return returnValue;
                }

                //Validate Entries
                foreach (var detail in model)
                {
                    if (this.CheckIfDuplicateDetailInModel(detail, model))
                    {
                        returnValue.ReturnMessage = "Duplicate Request Types Grouping Detail Entry in Model";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (await this.CheckIfDuplicateDetailInDatabase(detail))
                    {
                        returnValue.ReturnMessage = "Duplicate Request Types Grouping Detail Entry in Database";
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
    }
}
