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
    public class TicketRequestRepository : BaseApi, ITicketRequest
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestType _requestType;
        private readonly ITicketRequestDetail _ticketRequestDetail;
        private readonly ITicketBaseWorkFlow _ticketBaseWorkFlow;
        private readonly ITicketCommon _ticketCommon;

        public TicketRequestRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IRequestType requestType, ITicketRequestDetail ticketRequestDetail, ITicketBaseWorkFlow ticketBaseWorkFlow, ITicketCommon ticketCommon) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _requestType = requestType;
            _ticketRequestDetail = ticketRequestDetail;
            _ticketBaseWorkFlow = ticketBaseWorkFlow;
            _ticketCommon = ticketCommon;
        }

        public async Task<TicketRequestViewModel> Add(TicketRequestViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (model.TicketId <= 0)
                throw new CustomException("Ticket is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(r => r.Id == model.TicketId);

                if (ticket == null)
                    throw new CustomException("Ticket is not found.", 404);

               
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

                //get Max Sequence By Ticket Id
                var ticketRequests = await _dbCntxt.TicketRequests.Where(t => t.TicketId == model.TicketId).ToListAsync();

                int sequence = 0;

                if (ticketRequests.Count > 0)
                    sequence = ticketRequests.Max(r => r.Sequence) + 1;
                else
                    sequence = 1;

                //Insert Ticket Request
                var newTicketRequest = new TicketRequest()
                {
                    TicketId = model.TicketId,
                    Remarks = model.Remarks,
                    Sequence = sequence,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.TicketRequests.Add(newTicketRequest);
                await _dbCntxt.SaveChangesAsync();

                if (model.TicketRequestDetails != null)
                {
                    if (model.TicketRequestDetails.Count > 0)
                    {
                        model.TicketRequestDetails
                             .ForEach(t => t.TicketRequestId = newTicketRequest.Id);

                        //Check if request details same count with request forms in maintenance
                        var requestForms = (await _requestType.GetRequestForms(ticket.RequestTypeId, ticket.Version, true)).ToList();

                        if (model.TicketRequestDetails.Count != requestForms.Count)
                            throw new CustomException("Ticket Detail Items is not equal with Request Forms in the setup.", 400);

                        //get base workflow
                        var ticketInfo = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == model.TicketId);

                        var requestTypeBaseWorkFlows = (await _requestType.GetRequestTypeBaseWorkFlows(ticketInfo.RequestTypeId, ticket.Version, model.TicketRequestDetails, ticketInfo.UserOrganizationalStructureId)).ToList();

                        if (requestTypeBaseWorkFlows.Count <= 0)
                            throw new CustomException("There's no base workflow to be saved.", 400);

                        //If first ticket request record then save base workflow
                        if (sequence == 1)
                        {
                            foreach (var baseWorkFlow in requestTypeBaseWorkFlows)
                            {
                                //save base workflow
                                var ticketBaseWorkFlow = new TicketBaseWorkFlowViewModel()
                                {
                                    TicketId = model.TicketId,
                                    StepId = baseWorkFlow.StepId,
                                    StepTypeId = baseWorkFlow.StepTypeId,
                                    AssigneeId = baseWorkFlow.AssigneeId,
                                    AssigneeType = baseWorkFlow.AssigneeType,
                                    RequiredToExecute = baseWorkFlow.RequiredToExecute,
                                    TAT = baseWorkFlow.TAT,
                                    ApplicableButtons = "",
                                    TicketBaseWorkFlowAssignees = baseWorkFlow.BaseWorkAssignees
                                                                    .Select
                                                                    (
                                                                        b => new TicketBaseWorkFlowAssigneeViewModel()
                                                                        {
                                                                            TicketBaseWorkFlowId = 0,
                                                                            OrganizationalEntityId = b.OrganizationalEntityId,
                                                                            OrganizationalEntity = b.OrganizationalEntity,
                                                                            OrganizationalEntityHierarchy = b.OrganizationalEntityHierarchy,
                                                                            OrganizationalStructureId = b.OrganizationalStructureId,
                                                                            OrganizationalStructure = b.OrganizationalStructure,
                                                                            ApprovalLevelDetailId = b.ApprovalLevelDetailId,
                                                                            ApprovalLevelDetail = b.ApprovalLevelDetail,
                                                                            ApprovalLevelId = b.ApprovalLevelId,
                                                                            ApprovalLevel = b.ApprovalLevel,
                                                                            UserId = b.UserId,
                                                                            SortOrder = b.SortOrder
                                                                        }
                                                                    ).ToList()

                                };

                                var newTicketBaseWorkFlow = await _ticketBaseWorkFlow.Add(ticketBaseWorkFlow, false, transactionToUse);
                            }
                        }
                        else
                        {
                            //Second detail and onwards
                            var ticketBaseWorkFlows = await _dbCntxt.TicketBaseWorkFlows
                                                                    .Where(t => t.TicketId == model.TicketId)
                                                                    .OrderBy(t => t.Id)
                                                                    .ToListAsync();

                            if (ticketBaseWorkFlows.Count > 0)
                            {
                                if (ticketBaseWorkFlows.Count != requestTypeBaseWorkFlows.Count)
                                    throw new CustomException("New details is conflict with the baseline path", 400);

                                bool conflictFound = false;
                                for(int ctr = 0; ctr < ticketBaseWorkFlows.Count; ctr++)
                                {
                                    if (!(ticketBaseWorkFlows[ctr].StepId == requestTypeBaseWorkFlows[ctr].StepId && ticketBaseWorkFlows[ctr].AssigneeId == requestTypeBaseWorkFlows[ctr].AssigneeId))
                                    {
                                        conflictFound = true;
                                        break;
                                    }                                        
                                }

                                if (conflictFound)
                                    throw new CustomException("New details is conflict with the baseline path", 400);
                            }
                        }

                        foreach (var details in model.TicketRequestDetails)
                        {
                            var newTicketRequestDetail = await _ticketRequestDetail.Add(details, false, transactionToUse);
                        }                        
                    }
                    else
                        throw new CustomException("Ticket Detail is required.", 400);
                }
                else
                    throw new CustomException("Ticket Detail is required.", 400);

                if (model.TicketLinks != null)
                {
                    if (model.TicketLinks.Count > 0)
                    {

                    }
                }

                if (model.TicketAttachments != null)
                {
                    if (model.TicketAttachments.Count > 0)
                    {

                    }
                }

                model.Id = newTicketRequest.Id;
                model.CreatedByPK = newTicketRequest.CreatedByPK;
                model.CreatedDate = newTicketRequest.CreatedDate;

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

                var ticketRequest = await _dbCntxt.TicketRequests.FindAsync(id);

                if (ticketRequest == null)
                    throw new CustomException("Ticket Detail to delete is not found. ", 404);

                int ticketId = ticketRequest.TicketId;

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

                int ticketRequestsCount = await _dbCntxt.TicketRequests
                                                        .Where(t => t.TicketId == ticketRequest.TicketId)
                                                        .CountAsync();

                if (ticketRequestsCount == 1)
                {
                    //Delete base ticket base workflows
                    var ticketBaseWorkFlows = await _dbCntxt.TicketBaseWorkFlows.Where(t => t.TicketId == ticketRequest.TicketId).ToListAsync();

                    foreach(var ticketBaseWorkFlow in ticketBaseWorkFlows)
                    {
                        _dbCntxt.TicketBaseWorkFlowAssignees.RemoveRange(_dbCntxt.TicketBaseWorkFlowAssignees.Where(r => r.TicketBaseWorkFlowId == ticketBaseWorkFlow.Id));
                        await _dbCntxt.SaveChangesAsync();

                        _dbCntxt.TicketBaseWorkFlows.Remove(ticketBaseWorkFlow);
                        await _dbCntxt.SaveChangesAsync();
                    }
                }

                int linkCategoryId = (int)TicketLinkCategory.Ticket;
                int attachmentCategoryId = (int)TicketAttachmentCategory.Ticket;

                //Delete ticket links
                _dbCntxt.TicketLinks.RemoveRange(_dbCntxt.TicketLinks.Where(r => r.ReferenceId == id && r.Category == linkCategoryId));
                await _dbCntxt.SaveChangesAsync();

                //Delete ticket attachments
                _dbCntxt.TicketAttachments.RemoveRange(_dbCntxt.TicketAttachments.Where(r => r.ReferenceId == id && r.Category == attachmentCategoryId));
                await _dbCntxt.SaveChangesAsync();

                _dbCntxt.TicketRequestDetails.RemoveRange(_dbCntxt.TicketRequestDetails.Where(r => r.TicketRequestId == id));
                await _dbCntxt.SaveChangesAsync();

                _dbCntxt.TicketRequests.Remove(ticketRequest);
                await _dbCntxt.SaveChangesAsync();

                var ticketRequests = await _dbCntxt.TicketRequests
                                                   .Where(t => t.TicketId == ticketId)
                                                   .ToListAsync();

                if (ticketRequests.Count > 0)
                {
                    int ctr = 1;
                    foreach(var tRequest in ticketRequests)
                    {
                        tRequest.Sequence = ctr;

                        _dbCntxt.Entry(tRequest).State = EntityState.Modified;
                        await _dbCntxt.SaveChangesAsync();

                        ctr++;
                    }
                }

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

        public async Task DeleteAll(TicketRequestViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketRequestViewModel> Find(object obj)
        {
            try
            {
                IQueryable<TicketRequestViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(t => t.Id == id)
                            .Select
                            (
                                t => new TicketRequestViewModel
                                {
                                    Id = t.Id,
                                    TicketId = t.TicketId,
                                    Sequence = t.Sequence,
                                    Remarks = t.Remarks,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    Ticket = new TicketViewModel()
                                    {
                                        Id = t.TicketId,
                                        TicketNo = t.Ticket.TicketNo,
                                        RushTicketNo = t.Ticket.RushTicketNo,
                                        RequestTypeId = t.Ticket.RequestTypeId,
                                        RequestTypeName = t.Ticket.RequestType.Name,
                                        Version = t.Ticket.Version,
                                        Status = (TicketStatus) t.Ticket.Status
                                    },
                                    TicketRequestDetails = t.TicketRequestDetails
                                                           .Select
                                                           (
                                                               t => new TicketRequestDetailViewModel()
                                                               {
                                                                   Id = t.Id,
                                                                   TicketRequestId = t.TicketRequestId,
                                                                   FieldId = t.FieldId,
                                                                   IsLov = t.IsLov,
                                                                   ValueCode = t.ValueCode,
                                                                   Value = t.Value,
                                                                   ValueCode2 = t.ValueCode2,
                                                                   Value2 = t.Value2
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

        public IQueryable<TicketRequestViewModel> FindAll()
        {
            try
            {
                IQueryable<TicketRequestViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                t => new TicketRequestViewModel
                                {
                                    Id = t.Id,
                                    TicketId = t.TicketId,
                                    Sequence = t.Sequence,
                                    Remarks = t.Remarks,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    Ticket = new TicketViewModel()
                                    {
                                        Id = t.TicketId,
                                        TicketNo = t.Ticket.TicketNo,
                                        RushTicketNo = t.Ticket.RushTicketNo,
                                        RequestTypeId = t.Ticket.RequestTypeId,
                                        RequestTypeName = t.Ticket.RequestType.Name,
                                        Version = t.Ticket.Version,
                                        Status = (TicketStatus)t.Ticket.Status
                                    },
                                    TicketRequestDetails = t.TicketRequestDetails
                                                           .Select
                                                           (
                                                               t => new TicketRequestDetailViewModel()
                                                               {
                                                                   Id = t.Id,
                                                                   TicketRequestId = t.TicketRequestId,
                                                                   FieldId = t.FieldId,
                                                                   IsLov = t.IsLov,
                                                                   ValueCode = t.ValueCode,
                                                                   Value = t.Value,
                                                                   ValueCode2 = t.ValueCode2,
                                                                   Value2 = t.Value2
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
            throw new NotImplementedException();
        }

        public async Task Update(TicketRequestViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (model.TicketId <= 0)
                throw new CustomException("Ticket is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(r => r.Id == model.TicketId);

                if (ticket == null)
                    throw new CustomException("Ticket is not found.", 404);

                var ticketRequest = await _dbCntxt.TicketRequests.FirstOrDefaultAsync(a => a.Id == tableId);

                if (ticketRequest != null)
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

                    ticketRequest.TicketId = model.TicketId;
                    ticketRequest.Remarks = model.Remarks;
                    ticketRequest.ModifiedDate = DateTime.Now;
                    ticketRequest.ModifiedByPK = cid;

                    if (model.TicketRequestDetails != null)
                    {
                        if (model.TicketRequestDetails.Count > 0)
                        {
                            model.TicketRequestDetails.ForEach(t => t.TicketRequestId = tableId);

                            //Check if request details same count with request forms in maintenance
                            var requestForms = (await _requestType.GetRequestForms(ticket.RequestTypeId, ticket.Version, true)).ToList();

                            if (model.TicketRequestDetails.Count != requestForms.Count)
                                throw new CustomException("Ticket Detail Items is not equal with Request Forms in the setup.", 400);

                            //Validate base workflow
                            var ticketInfo = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == model.TicketId);

                            //get base workflow
                            var requestTypeBaseWorkFlows = (await _requestType.GetRequestTypeBaseWorkFlows(ticketInfo.RequestTypeId, ticket.Version, model.TicketRequestDetails, ticketInfo.UserOrganizationalStructureId)).ToList();

                            if (requestTypeBaseWorkFlows.Count <= 0)
                                throw new CustomException("There's no base workflow for this request details.", 400);

                            //Second detail and onwards
                            var ticketBaseWorkFlows = await _dbCntxt.TicketBaseWorkFlows
                                                                    .Where(t => t.TicketId == model.TicketId)
                                                                    .OrderBy(t => t.Id)
                                                                    .ToListAsync();

                            if (ticketBaseWorkFlows.Count > 0)
                            {
                                if (ticketBaseWorkFlows.Count != requestTypeBaseWorkFlows.Count)
                                    throw new CustomException("New details is conflict with the baseline path", 400);

                                bool conflictFound = false;
                                for (int ctr = 0; ctr < ticketBaseWorkFlows.Count; ctr++)
                                {
                                    if (!(ticketBaseWorkFlows[ctr].StepId == requestTypeBaseWorkFlows[ctr].StepId && ticketBaseWorkFlows[ctr].AssigneeId == requestTypeBaseWorkFlows[ctr].AssigneeId))
                                    {
                                        conflictFound = true;
                                        break;
                                    }
                                }

                                if (conflictFound)
                                    throw new CustomException("New details is conflict with the baseline path", 400);
                            }

                            if (model.WithCheckingOfDetailEntryInDb)
                            {
                                var detailsIds = model.TicketRequestDetails.Select(t => t.Id);
                                //Remove GUI deleted in DB
                                var details = await _dbCntxt.TicketRequestDetails
                                                            .Where
                                                            (
                                                                u =>
                                                                !detailsIds.Contains(u.Id) &&
                                                                u.TicketRequestId == model.Id
                                                            ).ToListAsync();

                                foreach (var detail in details)
                                {
                                    _dbCntxt.TicketRequestDetails.Remove(detail);
                                    await _dbCntxt.SaveChangesAsync();
                                }
                            }

                            //Validations
                            var detailValidationMessage = await _ticketRequestDetail.ValidateDetails(model.TicketRequestDetails);

                            if (detailValidationMessage.ReturnMessage != "")
                                throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                            foreach (var detail in model.TicketRequestDetails)
                            {
                                if (detail.Id > 0)
                                    await _ticketRequestDetail.Update(detail, detail.Id, false, transactionToUse);
                                else
                                {
                                    var addedDetails = await _ticketRequestDetail.Add(detail, false, transactionToUse);
                                }
                            }
                        }
                        else
                            throw new CustomException("Ticket Detail Items are required.", 400);
                    }
                    else
                        throw new CustomException("Ticket Detail Items are required.", 400);

                    if (model.TicketLinks != null)
                    {
                        if (model.TicketLinks.Count > 0)
                        {

                        }
                    }

                    if (model.TicketAttachments != null)
                    {
                        if (model.TicketAttachments.Count > 0)
                        {

                        }
                    }

                    _dbCntxt.Entry(ticketRequest).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Ticket Detail is not found", 404);
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

        private IQueryable<TicketRequest> Get()
        {
            try
            {
                IQueryable<TicketRequest> query = null;

                query = _dbCntxt.TicketRequests
                                .Include(t => t.Ticket)
                                .Include(r => r.Ticket.RequestType)
                                .Include(t => t.TicketRequestDetails)
                                .ThenInclude(f => f.GeneralField)
                                .ThenInclude(r => r.RequestForms)
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