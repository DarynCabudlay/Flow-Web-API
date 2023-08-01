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
    public class TicketBaseWorkFlowAssigneeRepository : BaseApi, ITicketBaseWorkFlowAssignee
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestType _requestType;

        public TicketBaseWorkFlowAssigneeRepository(
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

        public async Task<TicketBaseWorkFlowAssigneeViewModel> Add(TicketBaseWorkFlowAssigneeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (model.TicketBaseWorkFlowId <= 0)
                throw new CustomException("Ticket Base Work Flow is required", 400);

            if (String.IsNullOrWhiteSpace(model.UserId))
                throw new CustomException("User Id is required", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                if (model.OrganizationalEntityId != null)
                {
                    var organizationalEntity = await _dbCntxt.OrganizationEntities.FirstOrDefaultAsync(o => o.Id == Convert.ToInt32(model.OrganizationalEntityId));

                    if (organizationalEntity == null)
                        throw new CustomException("Organization Entity is not found.", 404);

                    if (!(String.IsNullOrWhiteSpace(model.OrganizationalEntity)))
                    {
                        if (model.OrganizationalEntity != organizationalEntity.Name)
                            throw new CustomException("Organization Entity did not match.", 404);
                    }

                    if (model.OrganizationalEntityHierarchy != null)
                    {
                        if (Convert.ToInt32(model.OrganizationalEntityHierarchy) != organizationalEntity.Hierarchy)
                            throw new CustomException("Organization Entity Hierarchy did not match.", 404);
                    }
                }

                if (model.OrganizationalStructureId != null)
                {
                    var organizationalStructure = await _dbCntxt.OrganizationalStructures.FirstOrDefaultAsync(o => o.Id == Convert.ToInt32(model.OrganizationalStructureId));

                    if (organizationalStructure == null)
                        throw new CustomException("Organizational Structure is not found.", 404);

                    if (!(String.IsNullOrWhiteSpace(model.OrganizationalStructure)))
                    {
                        if (model.OrganizationalStructure != organizationalStructure.Name)
                            throw new CustomException("Organization Structure did not match.", 404);
                    }
                }

                string approvalLevelName = "";

                if (model.ApprovalLevelId != null)
                {
                    var approvalLevel = await _dbCntxt.ApprovalLevels.FirstOrDefaultAsync(o => o.Id == Convert.ToInt32(model.ApprovalLevelId));

                    if (approvalLevel == null)
                        throw new CustomException("Approval Level is not found.", 404);

                    if (!(String.IsNullOrWhiteSpace(model.ApprovalLevel)))
                    {
                        if (model.ApprovalLevel != approvalLevel.Name)
                            throw new CustomException("Approval Level did not match.", 404);

                        approvalLevelName = approvalLevel.Name;
                    }
                }

                if (model.ApprovalLevelDetailId != null)
                {
                    var approvalLevelDetail = await _dbCntxt.ApprovalLevelDetails.FirstOrDefaultAsync(o => o.Id == Convert.ToInt32(model.ApprovalLevelDetailId));

                    if (approvalLevelDetail == null)
                        throw new CustomException("Approval Level Detail is not found.", 404);

                    if (!(String.IsNullOrWhiteSpace(model.ApprovalLevelDetail)))
                    {
                        if (model.ApprovalLevelDetail != approvalLevelName + " - " + approvalLevelDetail.Sequence)
                            throw new CustomException("Approval Level Detail did not match.", 404);
                    }
                }

                if (model.UserId.ToUpper() != "REQUESTOR")
                {
                    var user = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(t => t.Id == model.UserId);

                    if (user == null)
                        throw new CustomException("User is not found.", 404);
                }
               
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
                var newTicketBaseWorkFlowAssignee = new TicketBaseWorkFlowAssignee()
                {
                    TicketBaseWorkFlowId = model.TicketBaseWorkFlowId,
                    OrganizationalEntityId = model.OrganizationalEntityId,
                    OrganizationalEntity = model.OrganizationalEntity,
                    OrganizationalEntityHierarchy = model.OrganizationalEntityHierarchy,
                    OrganizationalStructureId = model.OrganizationalStructureId,
                    OrganizationalStructure = model.OrganizationalStructure,
                    ApprovalLevelDetailId = model.ApprovalLevelDetailId,
                    ApprovalLevelDetail = model.ApprovalLevelDetail,
                    ApprovalLevelId = model.ApprovalLevelId,
                    ApprovalLevel = model.ApprovalLevel,
                    UserId = model.UserId,
                    SortOrder = model.SortOrder
                };

                _dbCntxt.TicketBaseWorkFlowAssignees.Add(newTicketBaseWorkFlowAssignee);
                await _dbCntxt.SaveChangesAsync();

                model.Id = newTicketBaseWorkFlowAssignee.Id;

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

                var ticketBaseWorkFlowAssignee = await _dbCntxt.TicketBaseWorkFlowAssignees.FindAsync(id);

                if (ticketBaseWorkFlowAssignee == null)
                    throw new CustomException("Ticket Base Work Flow Assignee to delete is not found. ", 404);

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

                _dbCntxt.TicketBaseWorkFlowAssignees.Remove(ticketBaseWorkFlowAssignee);
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

        public async Task DeleteAll(TicketBaseWorkFlowAssigneeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketBaseWorkFlowAssigneeViewModel> Find(object obj)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketBaseWorkFlowAssigneeViewModel> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            throw new NotImplementedException();
        }

        public async Task Update(TicketBaseWorkFlowAssigneeViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }


        private bool CheckIfDuplicateDetailInModel(TicketBaseWorkFlowAssigneeViewModel model, List<TicketBaseWorkFlowAssigneeViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                u =>
                                                u.TicketBaseWorkFlowId == model.TicketBaseWorkFlowId &&
                                                u.OrganizationalEntityId == model.OrganizationalEntityId &&
                                                u.OrganizationalEntity == model.OrganizationalEntity &&
                                                u.OrganizationalEntityHierarchy == model.OrganizationalEntityHierarchy &&
                                                u.OrganizationalStructureId == model.OrganizationalStructureId &&
                                                u.OrganizationalStructure == model.OrganizationalStructure &&
                                                u.ApprovalLevelDetailId == model.ApprovalLevelDetailId &&
                                                u.ApprovalLevelDetail == model.ApprovalLevelDetail &&
                                                u.ApprovalLevelId == model.ApprovalLevelId &&
                                                u.ApprovalLevel == model.ApprovalLevel &&
                                                u.UserId == model.UserId &&
                                                u.SortOrder == model.SortOrder
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }

        private async Task<bool> CheckIfDuplicateDetailInDatabase(TicketBaseWorkFlowAssigneeViewModel model)
        {
            bool returnValue = false;

            List<TicketBaseWorkFlowAssignee> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = await _dbCntxt.TicketBaseWorkFlowAssignees
                                         .Where
                                         (
                                            u =>
                                            u.TicketBaseWorkFlowId == model.TicketBaseWorkFlowId &&
                                            u.OrganizationalEntityId == model.OrganizationalEntityId &&
                                            u.OrganizationalEntity == model.OrganizationalEntity &&
                                            u.OrganizationalEntityHierarchy == model.OrganizationalEntityHierarchy &&
                                            u.OrganizationalStructureId == model.OrganizationalStructureId &&
                                            u.OrganizationalStructure == model.OrganizationalStructure &&
                                            u.ApprovalLevelDetailId == model.ApprovalLevelDetailId &&
                                            u.ApprovalLevelDetail == model.ApprovalLevelDetail &&
                                            u.ApprovalLevelId == model.ApprovalLevelId &&
                                            u.ApprovalLevel == model.ApprovalLevel &&
                                            u.UserId == model.UserId &&
                                            u.SortOrder == model.SortOrder &&
                                            u.Id != model.Id
                                         ).ToListAsync();
            }
            else
            {
                checkDuplicate = await _dbCntxt.TicketBaseWorkFlowAssignees
                                         .Where
                                         (
                                            u =>
                                            u.TicketBaseWorkFlowId == model.TicketBaseWorkFlowId &&
                                            u.OrganizationalEntityId == model.OrganizationalEntityId &&
                                            u.OrganizationalEntity == model.OrganizationalEntity &&
                                            u.OrganizationalEntityHierarchy == model.OrganizationalEntityHierarchy &&
                                            u.OrganizationalStructureId == model.OrganizationalStructureId &&
                                            u.OrganizationalStructure == model.OrganizationalStructure &&
                                            u.ApprovalLevelDetailId == model.ApprovalLevelDetailId &&
                                            u.ApprovalLevelDetail == model.ApprovalLevelDetail &&
                                            u.ApprovalLevelId == model.ApprovalLevelId &&
                                            u.ApprovalLevel == model.ApprovalLevel &&
                                            u.UserId == model.UserId &&
                                            u.SortOrder == model.SortOrder
                                         ).ToListAsync();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }

        public async Task<TicketBaseWorkFlowAssigneeValidationViewModel> ValidateDetails(List<TicketBaseWorkFlowAssigneeViewModel> model)
        {
            TicketBaseWorkFlowAssigneeValidationViewModel returnValue = new TicketBaseWorkFlowAssigneeValidationViewModel();

            returnValue.ReturnMessage = "";
            returnValue.ReturnCode = 0;

            try
            {
                var ticketBaseWorkFlowsAssignees = model.Select(u => u.TicketBaseWorkFlowId).Distinct().ToList();

                if (ticketBaseWorkFlowsAssignees.Count > 1)
                {
                    returnValue.ReturnMessage = "Only 1 specific ticket base work flow is allowed";
                    returnValue.ReturnCode = 400;
                    return returnValue;
                }

                //Validate Entries
                foreach (var detail in model)
                {
                    if (this.CheckIfDuplicateDetailInModel(detail, model))
                    {
                        returnValue.ReturnMessage = "Duplicate Ticket Base Work Flow Assignee Entry in Model";
                        returnValue.ReturnCode = 400;
                        return returnValue;
                    }

                    if (await this.CheckIfDuplicateDetailInDatabase(detail))
                    {
                        returnValue.ReturnMessage = "Duplicate Ticket Base Work Flow Assignee Entry in Database";
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