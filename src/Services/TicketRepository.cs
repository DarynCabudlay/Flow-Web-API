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
    public class TicketRepository : BaseApi, ITicket
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestType _requestType;

        public TicketRepository(
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

        public async Task<TicketViewModel> Add(TicketViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (model.RequestTypeId <= 0 || model.Version <= 0)
                throw new CustomException("Request Type and Version is required.", 400);

            if (model.UserOrganizationalStructureId <= 0)
                throw new CustomException("User organizational structure is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestType == null)
                    throw new CustomException("Request Type and Version is not found.", 404);

                var userOrganizationalStructure = await _dbCntxt.UserOrganizationalStructures.FirstOrDefaultAsync(u => u.Id == model.UserOrganizationalStructureId);

                if (userOrganizationalStructure == null)
                    throw new CustomException("User organizational structure is not found.", 404);

                var organizationalStructure = (await _dbCntxt.OrganizationalStructures.FirstOrDefaultAsync(o => o.Id == userOrganizationalStructure.OrganizationalStructureId));

                string organizationalStructureName = "";

                if (organizationalStructure != null)
                    organizationalStructureName = organizationalStructure.Name;

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

                //Insert Ticket
                var newTicket = new Ticket()
                {
                    RequestTypeId = model.RequestTypeId,
                    Version = model.Version,
                    Status = (int) model.Status,
                    StatusDate = DateTime.Now,
                    UserOrganizationalStructureId = model.UserOrganizationalStructureId,
                    UserOrganizationalStructureName = organizationalStructureName,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.Tickets.Add(newTicket);
                await _dbCntxt.SaveChangesAsync();

                if (model.TicketRequests != null)
                {
                    if (model.TicketRequests.Count > 0)
                    {

                    }
                }

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

                model.Id = newTicket.Id;
                model.CreatedByPK = newTicket.CreatedByPK;
                model.CreatedDate = newTicket.CreatedDate;

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

                var ticket = await _dbCntxt.Tickets.FindAsync(id);

                if (ticket == null)
                    throw new CustomException("Ticket to delete is not found. ", 404);

                var ticketRequest = await _dbCntxt.TicketRequests.FirstOrDefaultAsync(t => t.TicketId == id);

                if (ticketRequest != null)
                    throw new CustomException("Cannot delete this ticket since detail(s) were already created.", 400);

                int dratStatus = (int) TicketStatus.Draft;

                if (ticket.Status != dratStatus)
                    throw new CustomException("Only ticket with draft status is allowed to be deleted.", 400);

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

                int linkCategoryId = (int) TicketLinkCategory.Ticket;
                int attachmentCategoryId = (int) TicketAttachmentCategory.Ticket;

                //Delete ticket links
                _dbCntxt.TicketLinks.RemoveRange(_dbCntxt.TicketLinks.Where(r => r.ReferenceId == id && r.Category == linkCategoryId));
                await _dbCntxt.SaveChangesAsync();

                //Delete ticket attachments
                _dbCntxt.TicketAttachments.RemoveRange(_dbCntxt.TicketAttachments.Where(r => r.ReferenceId == id && r.Category == attachmentCategoryId));
                await _dbCntxt.SaveChangesAsync();

                _dbCntxt.Tickets.Remove(ticket);
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

        public async Task DeleteAll(TicketViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketViewModel> Find(object obj)
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<TicketViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(t => t.Id == id)
                            .Select
                            (
                                t => new TicketViewModel
                                {
                                    Id = t.Id,
                                    TicketNo = t.TicketNo == null ? "" : t.TicketNo,
                                    RushTicketNo = t.RushTicketNo == null ? "" : t.RushTicketNo,
                                    RequestCategoryId = t.RequestType.RequestCategoryId,
                                    RequestCategoryName = t.RequestType.RequestCategory.Name,
                                    RequestTypeId = t.RequestTypeId,
                                    RequestTypeName = t.RequestType.Name,
                                    Version = t.Version,
                                    Status = (TicketStatus) t.Status,
                                    StatusDescription =  (t.Status == 1 ? "Draft" : t.Status == 2 ? "In-process" : t.Status == 3 ? "Rejected" : t.Status == 4 ? "Closed": "Cancelled"),
                                    StatusDate = t.StatusDate,
                                    DetailedStatus = t.DetailedStatus,
                                    CancellationReason = t.CancellationReason,
                                    CancellationReasonName = t.CancellationReason == null ? "" : _dbCntxt.Reasons.FirstOrDefault(r => r.Id == Convert.ToInt32(t.CancellationReason)).Name,
                                    CancellationRemarks = t.CancellationRemarks,
                                    UserOrganizationalStructureId = t.UserOrganizationalStructureId,
                                    UserOrganizationalStructureName = t.UserOrganizationalStructureName,
                                    DateSubmitted = t.DateSubmitted,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    TicketRequests = t.TicketRequests
                                                      .Select
                                                      (
                                                          t => new TicketRequestViewModel
                                                          {
                                                              Id = t.Id,
                                                              TicketId = t.TicketId,
                                                              Sequence = t.Sequence,
                                                              Remarks = t.Remarks,
                                                              TicketRequestDetails = t.TicketRequestDetails
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
                                                                                              Value2 = t.Value2
                                                                                          }
                                                                                      ).ToList()
                                                          }
                                                      ).ToList(),
                                    TicketAttachments = _dbCntxt.TicketAttachments
                                                                .Where(ta => ta.Category == 1 && ta.ReferenceId == t.Id)
                                                                .Select
                                                                (
                                                                    t => new TicketAttachmentViewModel
                                                                    {
                                                                        Id = t.Id,
                                                                        ReferenceId = t.ReferenceId,
                                                                        Category = (TicketAttachmentCategory)t.Category,
                                                                        CategoryDesc = t.Category == 1 ? "Ticket" : t.Category == 2 ? "Ticket Request" : "Workflow",
                                                                        FileTypeId = t.FileTypeId,
                                                                        Notes = t.Notes,
                                                                        OriginalFileName = t.OriginalFileName,
                                                                        MaskedFileName = t.MaskedFileName,
                                                                        FilePath = t.FilePath,
                                                                        FileType = new FileTypeViewModel()
                                                                        {
                                                                            Id = t.FileType.Id,
                                                                            Name = t.FileType.Name,
                                                                            MimeType = t.FileType.MimeType,
                                                                            MimeTypes = t.FileType.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
                                                                            Published = t.FileType.Published
                                                                        }
                                                                    }
                                                                ).ToList(),
                                    TicketBaseWorkFlows = t.TicketBaseWorkFlows
                                                           .Select
                                                           (
                                                               t => new TicketBaseWorkFlowViewModel
                                                               {
                                                                   Id = t.Id,
                                                                   TicketId = t.TicketId,
                                                                   StepId = t.StepId,
                                                                   StepName = _dbCntxt.WorkFlowSteps.FirstOrDefault(s => s.Id == t.StepId).Name,
                                                                   StepTypeId = t.StepTypeId,
                                                                   StepTypeName = _dbCntxt.WorkFlowStepTypes.FirstOrDefault(s => s.Id == t.StepId).Name,
                                                                   AssigneeId = t.AssigneeId,
                                                                   AssigneeType = t.AssigneeType,
                                                                   AssigneeTypeDesc = t.AssigneeType == 1 ? "Name" : "Position",
                                                                   RequiredToExecute = t.RequiredToExecute,
                                                                   TAT = t.TAT,
                                                                   ApplicableButtons = t.ApplicableButtons,
                                                                   TicketBaseWorkFlowAssignees = t.TicketBaseWorkFlowAssignees
                                                                                                  .Select
                                                                                                  (
                                                                                                      t => new TicketBaseWorkFlowAssigneeViewModel
                                                                                                      {
                                                                                                          Id = t.Id,
                                                                                                          TicketBaseWorkFlowId = t.TicketBaseWorkFlowId,
                                                                                                          OrganizationalEntityId = t.OrganizationalEntityId,
                                                                                                          OrganizationalEntity = t.OrganizationalEntity,
                                                                                                          OrganizationalEntityHierarchy = t.OrganizationalEntityHierarchy,
                                                                                                          OrganizationalStructureId = t.OrganizationalStructureId,
                                                                                                          OrganizationalStructure = t.OrganizationalStructure,
                                                                                                          ApprovalLevelDetailId = t.ApprovalLevelDetailId,
                                                                                                          ApprovalLevelDetail = t.ApprovalLevelDetail,
                                                                                                          ApprovalLevelId = t.ApprovalLevelId,
                                                                                                          ApprovalLevel = t.ApprovalLevel,
                                                                                                          UserId = t.UserId,
                                                                                                          User = t.UserId == "Requestor" ? null : _dbCntxt.AspNetUsers
                                                                                                                         .Include(u => u.AspNetUsersProfile)
                                                                                                                         .Where(u => u.Id == t.UserId)
                                                                                                                         .Select
                                                                                                                         (
                                                                                                                            u => new UserBasicInfoModel
                                                                                                                            {
                                                                                                                                Id = u.Id,
                                                                                                                                FirstName = u.AspNetUsersProfile.FirstName,
                                                                                                                                LastName = u.AspNetUsersProfile.LastName,
                                                                                                                                MiddleName = u.AspNetUsersProfile.MiddleName,
                                                                                                                                FullName = u.AspNetUsersProfile.LastName + ", " + u.AspNetUsersProfile.FirstName,
                                                                                                                                Status = u.Status
                                                                                                                            }
                                                                                                                         ).FirstOrDefault(),
                                                                                                          SortOrder = t.SortOrder
                                                                                                      }
                                                                                                  ).ToList()
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

        public IQueryable<TicketViewModel> FindAll()
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<TicketViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                t => new TicketViewModel
                                {
                                    Id = t.Id,
                                    TicketNo = t.TicketNo == null ? "" : t.TicketNo,
                                    RushTicketNo = t.RushTicketNo == null ? "" : t.RushTicketNo,
                                    RequestCategoryId = t.RequestType.RequestCategoryId,
                                    RequestCategoryName = t.RequestType.RequestCategory.Name,
                                    RequestTypeId = t.RequestTypeId,
                                    RequestTypeName = t.RequestType.Name,
                                    Version = t.Version,
                                    Status = (TicketStatus)t.Status,
                                    StatusDescription = (t.Status == 1 ? "Draft" : t.Status == 2 ? "In-process" : t.Status == 3 ? "Rejected" : t.Status == 4 ? "Closed" : "Cancelled"),
                                    StatusDate = t.StatusDate,
                                    DetailedStatus = t.DetailedStatus,
                                    CancellationReason = t.CancellationReason,
                                    CancellationReasonName = t.CancellationReason == null ? "" : _dbCntxt.Reasons.FirstOrDefault(r => r.Id == Convert.ToInt32(t.CancellationReason)).Name,
                                    CancellationRemarks = t.CancellationRemarks,
                                    UserOrganizationalStructureId = t.UserOrganizationalStructureId,
                                    UserOrganizationalStructureName = t.UserOrganizationalStructureName,
                                    DateSubmitted = t.DateSubmitted,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    TicketRequests = t.TicketRequests
                                                      .Select
                                                      (
                                                          t => new TicketRequestViewModel
                                                          {
                                                              Id = t.Id,
                                                              TicketId = t.TicketId,
                                                              Sequence = t.Sequence,
                                                              Remarks = t.Remarks,
                                                              TicketRequestDetails = t.TicketRequestDetails
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
                                                                                              Value2 = t.Value2
                                                                                          }
                                                                                      ).ToList()
                                                          }
                                                      ).ToList(),
                                    TicketAttachments = _dbCntxt.TicketAttachments
                                                                .Where(ta => ta.Category == 1 && ta.ReferenceId == t.Id)
                                                                .Select
                                                                (
                                                                    t => new TicketAttachmentViewModel
                                                                    {
                                                                        Id = t.Id,
                                                                        ReferenceId = t.ReferenceId,
                                                                        Category = (TicketAttachmentCategory)t.Category,
                                                                        CategoryDesc = t.Category == 1 ? "Ticket" : t.Category == 2 ? "Ticket Request" : "Workflow",
                                                                        FileTypeId = t.FileTypeId,
                                                                        Notes = t.Notes,
                                                                        OriginalFileName = t.OriginalFileName,
                                                                        MaskedFileName = t.MaskedFileName,
                                                                        FilePath = t.FilePath,
                                                                        FileType = new FileTypeViewModel()
                                                                        {
                                                                            Id = t.FileType.Id,
                                                                            Name = t.FileType.Name,
                                                                            MimeType = t.FileType.MimeType,
                                                                            MimeTypes = t.FileType.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
                                                                            Published = t.FileType.Published
                                                                        }
                                                                    }
                                                                ).ToList(),
                                    TicketBaseWorkFlows = t.TicketBaseWorkFlows
                                                           .Select
                                                           (
                                                               t => new TicketBaseWorkFlowViewModel
                                                               {
                                                                   Id = t.Id,
                                                                   TicketId = t.TicketId,
                                                                   StepId = t.StepId,
                                                                   StepName = _dbCntxt.WorkFlowSteps.FirstOrDefault(s => s.Id == t.StepId).Name,
                                                                   StepTypeId = t.StepTypeId,
                                                                   StepTypeName = _dbCntxt.WorkFlowStepTypes.FirstOrDefault(s => s.Id == t.StepId).Name,
                                                                   AssigneeId = t.AssigneeId,
                                                                   AssigneeType = t.AssigneeType,
                                                                   AssigneeTypeDesc = t.AssigneeType == 1 ? "Name" : "Position",
                                                                   RequiredToExecute = t.RequiredToExecute,
                                                                   TAT = t.TAT,
                                                                   ApplicableButtons = t.ApplicableButtons,
                                                                   TicketBaseWorkFlowAssignees = t.TicketBaseWorkFlowAssignees
                                                                                                  .Select
                                                                                                  (
                                                                                                      t => new TicketBaseWorkFlowAssigneeViewModel
                                                                                                      {
                                                                                                          Id = t.Id,
                                                                                                          TicketBaseWorkFlowId = t.TicketBaseWorkFlowId,
                                                                                                          OrganizationalEntityId = t.OrganizationalEntityId,
                                                                                                          OrganizationalEntity = t.OrganizationalEntity,
                                                                                                          OrganizationalEntityHierarchy = t.OrganizationalEntityHierarchy,
                                                                                                          OrganizationalStructureId = t.OrganizationalStructureId,
                                                                                                          OrganizationalStructure = t.OrganizationalStructure,
                                                                                                          ApprovalLevelDetailId = t.ApprovalLevelDetailId,
                                                                                                          ApprovalLevelDetail = t.ApprovalLevelDetail,
                                                                                                          ApprovalLevelId = t.ApprovalLevelId,
                                                                                                          ApprovalLevel = t.ApprovalLevel,
                                                                                                          UserId = t.UserId,
                                                                                                          User = t.UserId == "Requestor" ? null : _dbCntxt.AspNetUsers
                                                                                                                         .Include(u => u.AspNetUsersProfile)
                                                                                                                         .Where(u => u.Id == t.UserId)
                                                                                                                         .Select
                                                                                                                         (
                                                                                                                            u => new UserBasicInfoModel
                                                                                                                            {
                                                                                                                                Id = u.Id,
                                                                                                                                FirstName = u.AspNetUsersProfile.FirstName,
                                                                                                                                LastName = u.AspNetUsersProfile.LastName,
                                                                                                                                MiddleName = u.AspNetUsersProfile.MiddleName,
                                                                                                                                FullName = u.AspNetUsersProfile.LastName + ", " + u.AspNetUsersProfile.FirstName,
                                                                                                                                Status = u.Status
                                                                                                                            }
                                                                                                                         ).FirstOrDefault(),
                                                                                                          SortOrder = t.SortOrder
                                                                                                      }
                                                                                                  ).ToList()
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

        public async Task Update(TicketViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (model.RequestTypeId <= 0 || model.Version <= 0)
                throw new CustomException("Request Type and Version is required.", 400);

            if (model.UserOrganizationalStructureId <= 0)
                throw new CustomException("User organizational structure is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestType == null)
                    throw new CustomException("Request Type and Version is not found.", 404);

                var userOrganizationalStructure = await _dbCntxt.UserOrganizationalStructures.FirstOrDefaultAsync(u => u.Id == model.UserOrganizationalStructureId);

                if (userOrganizationalStructure == null)
                    throw new CustomException("User organizational structure is not found.", 404);

                var organizationalStructure = (await _dbCntxt.OrganizationalStructures.FirstOrDefaultAsync(o => o.Id == userOrganizationalStructure.OrganizationalStructureId));

                string organizationalStructureName = "";

                if (organizationalStructure != null)
                    organizationalStructureName = organizationalStructure.Name;

                var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(a => a.Id == tableId);

                if (ticket != null)
                {
                    var ticketRequests = await _dbCntxt.TicketRequests.Where(t => t.TicketId == tableId).ToListAsync();

                    if (ticketRequests.Count > 0)
                        throw new CustomException("Unable to update ticket since details were already added.", 404);

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

                    ticket.RequestTypeId = model.RequestTypeId;
                    ticket.Version = model.Version;
                    ticket.UserOrganizationalStructureId = model.UserOrganizationalStructureId;
                    ticket.UserOrganizationalStructureName = organizationalStructureName;
                    ticket.ModifiedDate = DateTime.Now;
                    ticket.ModifiedByPK = cid;

                    _dbCntxt.Entry(ticket).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (model.TicketRequests != null)
                    {
                        if (model.TicketRequests.Count > 0)
                        {

                        }
                    }

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

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Ticket is not found", 404);
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

        public async Task Cancel(CancelTicketViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (model.ReasonId <= 0)
                throw new CustomException("Reason for cancellation is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticket = await _dbCntxt.Tickets.FindAsync(model.Id);

                if (ticket == null)
                    throw new CustomException("Ticket to cancel is not found. ", 404);

                var reason = await _dbCntxt.Reasons.FirstOrDefaultAsync(r => r.Id == model.ReasonId);

                if (reason == null)
                    throw new CustomException("Reason for cancellation is not found.", 404);

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

                int cancelledStatus = (int)TicketStatus.Cancelled;

                ticket.Status = cancelledStatus;
                ticket.StatusDate = DateTime.Now;
                ticket.CancellationReason = model.ReasonId;
                ticket.CancellationRemarks = model.Remarks;
                ticket.ModifiedByPK = cid;
                ticket.ModifiedDate = DateTime.Now;

                _dbCntxt.Entry(ticket).State = EntityState.Modified;
                await _dbCntxt.SaveChangesAsync();

                //Insert ticket workflow

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

        public async Task Submit(int id, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticket = await _dbCntxt.Tickets.FindAsync(id);

                if (ticket == null)
                    throw new CustomException("Ticket to submit is not found. ", 404);

                var ticketRequest = await _dbCntxt.TicketRequests.FirstOrDefaultAsync(t => t.TicketId == id);

                if (ticketRequest == null)
                    throw new CustomException("Cannot submit this ticket without any details.", 400);

                int dratStatus = (int)TicketStatus.Draft;

                if (ticket.Status != dratStatus)
                    throw new CustomException("Only ticket with draft status is allowed to be submitted.", 400);

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

                int submittedStatus = (int) TicketStatus.Inprocess;

                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;

                string yearInText = year.ToString().Substring(2, 2);
                string monthInText = "";

                if (month.ToString().Length == 1)
                    monthInText = "0" + month.ToString();
                else
                    monthInText = month.ToString();

                string ticketNo = "";

                string zeros = "0000000";

                var tickets = await _dbCntxt.Tickets.ToListAsync();

                int countWithTicketNo = tickets
                                        .Where
                                        (
                                            t =>
                                            !String.IsNullOrWhiteSpace(t.TicketNo) &&
                                            t.CreatedDate.Year == year
                                        ).Count();

                countWithTicketNo = countWithTicketNo + 1;

                ticketNo = yearInText + monthInText + zeros.Substring(0, (zeros.Length - countWithTicketNo.ToString().Length)) + countWithTicketNo.ToString();

                ticket.TicketNo = ticketNo;
                ticket.Status = submittedStatus;
                ticket.StatusDate = DateTime.Now;
                ticket.DateSubmitted = DateTime.Now;
                ticket.ModifiedByPK = cid;
                ticket.ModifiedDate = DateTime.Now;

                _dbCntxt.Entry(ticket).State = EntityState.Modified;
                await _dbCntxt.SaveChangesAsync();

                //Insert ticket workflow

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

        private IQueryable<Ticket> Get()
        {
            try
            {
                IQueryable<Ticket> query = null;

                query = _dbCntxt.Tickets
                                .Include(t => t.TicketRequests)
                                .ThenInclude(t => t.TicketRequestDetails)
                                .ThenInclude(f => f.GeneralField)
                                .ThenInclude(r => r.RequestForms)
                                .Include(r => r.RequestType)
                                .Include(t => t.TicketBaseWorkFlows)
                                .ThenInclude(t => t.TicketBaseWorkFlowAssignees)
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
