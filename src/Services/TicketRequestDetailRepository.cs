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
    public class TicketRequestDetailRepository : BaseApi, ITicketRequestDetail
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestType _requestType;

        public TicketRequestDetailRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IRequestType requestType) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _requestType = requestType;
        }

        public async Task<TicketRequestDetailViewModel> Add(TicketRequestDetailViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (model.TicketRequestId <= 0)
                throw new CustomException("Ticket Detail is required.", 400);

            if (model.FieldId <= 0)
                throw new CustomException("Request Field is required.", 400);

            if (model.IsLov)
            {
                if (String.IsNullOrWhiteSpace(model.ValueCode))
                    throw new CustomException("Field value code is required.", 400);
            }

            if (String.IsNullOrWhiteSpace(model.Value))
                throw new CustomException("Field value is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticketRequest = await _dbCntxt.TicketRequests.FirstOrDefaultAsync(r => r.Id == model.TicketRequestId);

                if (ticketRequest == null)
                    throw new CustomException("Ticket Detail is not found.", 404);

                var field = await _dbCntxt.GeneralFields.FirstOrDefaultAsync(r => r.Id == model.FieldId);

                if (field == null)
                    throw new CustomException("Request Field is not found.", 404);


                //check if field is in particular request type and version
                var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == ticketRequest.TicketId);

                int requestTypeId = ticket.RequestTypeId;
                int version = ticket.Version;

                var requestFormField = await _dbCntxt.RequestForms.FirstOrDefaultAsync
                                                                   (
                                                                        r => r.RequestTypeId == requestTypeId &&
                                                                        r.Version == version &&
                                                                        r.FieldId == model.FieldId
                                                                   );

                if (requestFormField == null)
                    throw new CustomException("Request Field is not found in ticket request type.", 404);

                if (model.IsLov)
                {
                    var lov = JsonSerializer.Deserialize<List<LOVViewModel>>(field.LOVs);

                    var fieldLovs = new Dictionary<string, string>();

                    var splitLovCodes = model.ValueCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var splitLovNames = model.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splitLovCodes.Length != splitLovNames.Length)
                        throw new CustomException("Field LOV Value Codes and Field Value Names are not equal in count.", 404);

                    for (int ctr = 0; ctr < splitLovCodes.Length; ctr++ )
                    {
                        fieldLovs.Add(splitLovCodes[ctr], splitLovNames[ctr]);
                    }

                    foreach(var fieldLov in fieldLovs)
                    {
                        var codeIsExisting = lov.FirstOrDefault(l => l.Value == fieldLov.Key);

                        if (codeIsExisting == null)
                            throw new CustomException("Field LOV Value Code: '" + fieldLov.Key + "' is not found.", 404);

                        if (!codeIsExisting.IsActive)
                            throw new CustomException("Field LOV Value Code: '" + fieldLov.Key + "' is deactivated.", 404);


                        var nameIsExisting = lov.FirstOrDefault(l => l.Name == fieldLov.Value);

                        if (nameIsExisting == null)
                            throw new CustomException("Field LOV Name '" + fieldLov.Value + "' is not found.", 404);

                        if (!nameIsExisting.IsActive)
                            throw new CustomException("Field LOV Name: '" + fieldLov.Value + "' is deactivated.", 404);

                        //No need since entries are already validated in request forms maintenance
                        //var fieldValidationMessage = await _requestType.ValidateFieldForCondition(model.FieldId, model.Value, "");

                        //if (fieldValidationMessage != "")
                        //    throw new CustomException(fieldValidationMessage, 404);
                    }  
                }
                else
                {
                    var fieldValidationMessage =  await _requestType.ValidateFieldForCondition(model.FieldId, model.Value, model.Value2);

                    if (fieldValidationMessage != "")
                        throw new CustomException(fieldValidationMessage, 404);

                }

                var requestDetail = await _dbCntxt.TicketRequestDetails
                                            .FirstOrDefaultAsync
                                            (
                                                r => r.TicketRequestId == model.TicketRequestId &&
                                                r.FieldId == model.FieldId
                                            );

                if (requestDetail != null)
                    throw new CustomException("Request Field is already exists.", 400);

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

                //Insert Ticket Request Detail
                var ticketRequestDetail = new TicketRequestDetail()
                {
                    TicketRequestId = model.TicketRequestId,
                    FieldId = model.FieldId,
                    IsLov = model.IsLov,
                    ValueCode = model.ValueCode,
                    Value = model.Value,
                    ValueCode2 = model.ValueCode2,
                    Value2 = model.Value2,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.TicketRequestDetails.Add(ticketRequestDetail);
                await _dbCntxt.SaveChangesAsync();

                model.Id = ticketRequestDetail.Id;
                model.CreatedByPK = ticketRequestDetail.CreatedByPK;
                model.CreatedDate = ticketRequestDetail.CreatedDate;

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
            throw new NotImplementedException();
        }

        public async Task DeleteAll(TicketRequestDetailViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketRequestDetailViewModel> Find(object obj)
        {
            try
            {
                IQueryable<TicketRequestDetailViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(t => t.Id == id)
                            .Select
                            (
                                t => new TicketRequestDetailViewModel
                                {
                                    Id = t.Id,
                                    TicketRequestId = t.TicketRequestId,
                                    FieldId = t.FieldId,
                                    IsLov = t.IsLov,
                                    ValueCode = t.ValueCode,
                                    Value = t.Value,
                                    ValueCode2 = t.ValueCode2,
                                    Value2 = t.Value2,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    TicketRequest = new TicketRequestViewModel()
                                    {
                                        Id = t.TicketRequest.Id,
                                        TicketId = t.TicketRequest.TicketId,
                                        Sequence = t.TicketRequest.Sequence,
                                        Remarks = t.TicketRequest.Remarks
                                    },
                                    GeneralField = new GeneralFieldViewModel()
                                    {
                                        Id = t.GeneralField.Id,
                                        Name = t.GeneralField.Name,
                                        Slug = t.GeneralField.Slug,
                                        FieldType = (FieldType)t.GeneralField.FieldType,
                                        DataType = t.GeneralField.DataType
                                    }
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

        public IQueryable<TicketRequestDetailViewModel> FindAll()
        {
            try
            {
                IQueryable<TicketRequestDetailViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                t => new TicketRequestDetailViewModel
                                {
                                    Id = t.Id,
                                    TicketRequestId = t.TicketRequestId,
                                    FieldId = t.FieldId,
                                    IsLov = t.IsLov,
                                    ValueCode = t.ValueCode,
                                    Value = t.Value,
                                    ValueCode2 = t.ValueCode2,
                                    Value2 = t.Value2,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    TicketRequest = new TicketRequestViewModel()
                                    {
                                        Id = t.TicketRequest.Id,
                                        TicketId = t.TicketRequest.TicketId,
                                        Sequence = t.TicketRequest.Sequence,
                                        Remarks = t.TicketRequest.Remarks
                                    },
                                    GeneralField = new GeneralFieldViewModel()
                                    {
                                        Id = t.GeneralField.Id,
                                        Name = t.GeneralField.Name,
                                        Slug = t.GeneralField.Slug,
                                        FieldType = (FieldType) t.GeneralField.FieldType,
                                        DataType = t.GeneralField.DataType
                                    }
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

        public async Task Update(TicketRequestDetailViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (model.TicketRequestId <= 0)
                throw new CustomException("Ticket Detail is required.", 400);

            if (model.FieldId <= 0)
                throw new CustomException("Request Field is required.", 400);

            if (model.IsLov)
            {
                if (String.IsNullOrWhiteSpace(model.ValueCode))
                    throw new CustomException("Field value code is required.", 400);
            }

            if (String.IsNullOrWhiteSpace(model.Value))
                throw new CustomException("Field value is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticketRequest = await _dbCntxt.TicketRequests.FirstOrDefaultAsync(r => r.Id == model.TicketRequestId);

                if (ticketRequest == null)
                    throw new CustomException("Ticket Detail is not found.", 404);

                var field = await _dbCntxt.GeneralFields.FirstOrDefaultAsync(r => r.Id == model.FieldId);

                if (field == null)
                    throw new CustomException("Request Field is not found.", 404);

                //check if field is in particular request type and version
                var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == ticketRequest.TicketId);

                int requestTypeId = ticket.RequestTypeId;
                int version = ticket.Version;

                var requestFormField = await _dbCntxt.RequestForms.FirstOrDefaultAsync
                                                                   (
                                                                        r => r.RequestTypeId == requestTypeId &&
                                                                        r.Version == version &&
                                                                        r.FieldId == model.FieldId
                                                                   );

                if (requestFormField == null)
                    throw new CustomException("Request Field is not found in ticket request type.", 404);

                if (model.IsLov)
                {
                    var lov = JsonSerializer.Deserialize<List<LOVViewModel>>(field.LOVs);

                    var fieldLovs = new Dictionary<string, string>();

                    var splitLovCodes = model.ValueCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var splitLovNames = model.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splitLovCodes.Length != splitLovNames.Length)
                        throw new CustomException("Field LOV Value Codes and Field Value Names are not equal in count.", 404);

                    for (int ctr = 0; ctr < splitLovCodes.Length; ctr++)
                    {
                        fieldLovs.Add(splitLovCodes[ctr], splitLovNames[ctr]);
                    }

                    foreach (var fieldLov in fieldLovs)
                    {
                        var codeIsExisting = lov.FirstOrDefault(l => l.Value == fieldLov.Key);

                        if (codeIsExisting == null)
                            throw new CustomException("Field LOV Value Code: '" + fieldLov.Key + "' is not found.", 404);

                        if (!codeIsExisting.IsActive)
                            throw new CustomException("Field LOV Value Code: '" + fieldLov.Key + "' is deactivated.", 404);


                        var nameIsExisting = lov.FirstOrDefault(l => l.Name == fieldLov.Value);

                        if (nameIsExisting == null)
                            throw new CustomException("Field LOV Name '" + fieldLov.Value + "' is not found.", 404);

                        if (!nameIsExisting.IsActive)
                            throw new CustomException("Field LOV Name: '" + fieldLov.Value + "' is deactivated.", 404);

                        //No need since entries are already validated in request forms maintenance
                        //var fieldValidationMessage = await _requestType.ValidateFieldForCondition(model.FieldId, model.Value, "");

                        //if (fieldValidationMessage != "")
                        //    throw new CustomException(fieldValidationMessage, 404);
                    }

                }

                var requestDetail = await _dbCntxt.TicketRequestDetails
                                            .FirstOrDefaultAsync
                                            (
                                                r => r.TicketRequestId == model.TicketRequestId &&
                                                r.FieldId == model.FieldId &&
                                                r.Id != model.Id
                                            );

                if (requestDetail != null)
                    throw new CustomException("Request Field is already exists.", 400);

                var ticketRequestDetail = await _dbCntxt.TicketRequestDetails.FirstOrDefaultAsync(a => a.Id == tableId);

                if (ticketRequestDetail != null)
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

                    ticketRequestDetail.TicketRequestId = model.TicketRequestId;
                    ticketRequestDetail.FieldId = model.FieldId;
                    ticketRequestDetail.IsLov = model.IsLov;
                    ticketRequestDetail.ValueCode = model.ValueCode;
                    ticketRequestDetail.Value = model.Value;
                    ticketRequestDetail.ValueCode2 = model.ValueCode2;
                    ticketRequestDetail.Value2 = model.Value2;
                    ticketRequest.ModifiedDate = DateTime.Now;
                    ticketRequest.ModifiedByPK = cid;

                    _dbCntxt.Entry(ticketRequestDetail).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Ticket Detail Item is not found", 404);
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

        private IQueryable<TicketRequestDetail> Get()
        {
            try
            {
                IQueryable<TicketRequestDetail> query = null;

                query = _dbCntxt.TicketRequestDetails
                                .Include(r => r.TicketRequest)
                                .Include(r => r.GeneralField)
                                .Include(r => r.GeneralField.RequestForms)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private bool CheckIfDuplicateDetailInModel(TicketRequestDetailViewModel model, List<TicketRequestDetailViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                u =>
                                                u.TicketRequestId == model.TicketRequestId &&
                                                u.FieldId == model.FieldId &&
                                                u.IsLov == model.IsLov &&
                                                u.ValueCode == model.ValueCode &&
                                                u.Value == model.Value &&
                                                u.ValueCode2 == model.ValueCode2 &&
                                                u.Value2 == model.Value2
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }

        private async Task<bool> CheckIfDuplicateDetailInDatabase(TicketRequestDetailViewModel model)
        {
            bool returnValue = false;

            List<TicketRequestDetail> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = await _dbCntxt.TicketRequestDetails
                                         .Where
                                         (
                                            u =>
                                            u.TicketRequestId == model.TicketRequestId &&
                                            u.FieldId == model.FieldId &&
                                            u.IsLov == model.IsLov &&
                                            u.ValueCode == model.ValueCode &&
                                            u.Value == model.Value &&
                                            u.ValueCode2 == model.ValueCode2 &&
                                            u.Value2 == model.Value2 &&
                                            u.Id != model.Id
                                         ).ToListAsync();
            }
            else
            {
                checkDuplicate = await _dbCntxt.TicketRequestDetails
                                         .Where
                                         (
                                            u =>
                                            u.TicketRequestId == model.TicketRequestId &&
                                            u.FieldId == model.FieldId &&
                                            u.IsLov == model.IsLov &&
                                            u.ValueCode == model.ValueCode &&
                                            u.Value == model.Value &&
                                            u.ValueCode2 == model.ValueCode2 &&
                                            u.Value2 == model.Value2
                                         ).ToListAsync();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }

        public async Task<TicketRequestDetailValidationViewModel> ValidateDetails(List<TicketRequestDetailViewModel> model)
        {
            TicketRequestDetailValidationViewModel returnValue = new TicketRequestDetailValidationViewModel();

            returnValue.ReturnMessage = "";
            returnValue.ReturnCode = 0;

            try
            {
                var ticketRequests = model.Select(u => u.TicketRequestId).Distinct().ToList();

                if (ticketRequests.Count > 1)
                {
                    returnValue.ReturnMessage = "Only 1 specific ticket detail is allowed";
                    returnValue.ReturnCode = 400;
                    return returnValue;
                }

                //Validate Entries
                foreach (var detail in model)
                {
                    if (this.CheckIfDuplicateDetailInModel(detail, model))
                    {
                        returnValue.ReturnMessage = "Duplicate Ticket Detail Item Entry in Model";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (await this.CheckIfDuplicateDetailInDatabase(detail))
                    {
                        returnValue.ReturnMessage = "Duplicate Ticket Detail Item Entry in Database";
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