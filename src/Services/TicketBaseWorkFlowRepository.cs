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
    public class TicketBaseWorkFlowRepository : BaseApi, ITicketBaseWorkFlow
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestType _requestType;
        private readonly ITicketBaseWorkFlowAssignee _ticketBaseWorkFlowAssignee;

        public TicketBaseWorkFlowRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IRequestType requestType, ITicketBaseWorkFlowAssignee ticketBaseWorkFlowAssignee) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _requestType = requestType;
            _ticketBaseWorkFlowAssignee = ticketBaseWorkFlowAssignee;
        }

        public async Task<TicketBaseWorkFlowViewModel> Add(TicketBaseWorkFlowViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (model.TicketId <= 0)
                throw new CustomException("Ticket is required", 400);

            if (model.StepId <= 0)
                throw new CustomException("Step is required", 400);

            if (model.StepTypeId <= 0)
                throw new CustomException("Step Type is required", 400);

            if (model.AssigneeId <= 0)
                throw new CustomException("Assignee is required", 400);

            if (!(GeneralHelper.IsValidAssigneeType(Convert.ToInt32(model.AssigneeType))))
                throw new CustomException("Assignee Type is required", 400);

            if (model.RequiredToExecute <= 0)
                throw new CustomException("Required To Execute is required", 400);

            if (model.TAT <= 0)
                throw new CustomException("TAT is required", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == model.TicketId);

                if (ticket == null)
                    throw new CustomException("Ticket is not found.", 404);

                var step = (await _requestType.GetRequestSteps(ticket.RequestTypeId, ticket.Version, false)).FirstOrDefault(s => s.StepId == model.StepId);

                if (step == null)
                    throw new CustomException("Step is not found.", 404);

                var stepType = await _dbCntxt.WorkFlowStepTypes.FirstOrDefaultAsync(s => s.Id == model.StepTypeId);

                if (stepType == null)
                    throw new CustomException("Step Type is not found.", 404);

                var assignee = await _dbCntxt.RequestStepAssignees.FirstOrDefaultAsync(s => s.Id == model.AssigneeId);

                if (assignee == null)
                    throw new CustomException("Assignee is not found.", 404);

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
                var newTicketBaseWorkFlow = new TicketBaseWorkFlow()
                {
                    TicketId = model.TicketId,
                    StepId = model.StepId,
                    StepTypeId = model.StepTypeId,
                    AssigneeId = model.AssigneeId,
                    AssigneeType = model.AssigneeType,
                    RequiredToExecute = model.RequiredToExecute,
                    TAT = model.TAT,
                    ApplicableButtons = model.ApplicableButtons,
                };

                _dbCntxt.TicketBaseWorkFlows.Add(newTicketBaseWorkFlow);
                await _dbCntxt.SaveChangesAsync();

                if (model.TicketBaseWorkFlowAssignees != null)
                {
                    if (model.TicketBaseWorkFlowAssignees.Count > 0)
                    {
                        model.TicketBaseWorkFlowAssignees
                             .ForEach(t => t.TicketBaseWorkFlowId = newTicketBaseWorkFlow.Id);

                        //Validations
                        var detailValidationMessage = await _ticketBaseWorkFlowAssignee.ValidateDetails(model.TicketBaseWorkFlowAssignees);

                        if (detailValidationMessage.ReturnMessage != "")
                            throw new CustomException(detailValidationMessage.ReturnMessage, detailValidationMessage.ReturnCode);

                        foreach (var detail in model.TicketBaseWorkFlowAssignees)
                        {
                            var addedDetails = await _ticketBaseWorkFlowAssignee.Add(detail, false, transactionToUse);
                        }
                    }
                }

                model.Id = newTicketBaseWorkFlow.Id;

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

                var ticketBaseWorkFlow = await _dbCntxt.TicketBaseWorkFlows.FindAsync(id);

                if (ticketBaseWorkFlow == null)
                    throw new CustomException("Ticket Base Work Flow to delete is not found. ", 404);

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

                _dbCntxt.TicketBaseWorkFlowAssignees.RemoveRange(_dbCntxt.TicketBaseWorkFlowAssignees.Where(r => r.TicketBaseWorkFlowId == id));
                await _dbCntxt.SaveChangesAsync();

                _dbCntxt.TicketBaseWorkFlows.Remove(ticketBaseWorkFlow);
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

        public async Task DeleteAll(TicketBaseWorkFlowViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketBaseWorkFlowViewModel> Find(object obj)
        {
            try
            {
                IQueryable<TicketBaseWorkFlowViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(t => t.Id == id)
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
                            )
                            .AsQueryable();

                return query;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<TicketBaseWorkFlowViewModel> FindAll()
        {
            try
            {
                IQueryable<TicketBaseWorkFlowViewModel> query = null;

                query = this.Get()
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

        public async Task Update(TicketBaseWorkFlowViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        private IQueryable<TicketBaseWorkFlow> Get()
        {
            try
            {
                IQueryable<TicketBaseWorkFlow> query = null;

                query = _dbCntxt.TicketBaseWorkFlows
                                .Include(t => t.TicketBaseWorkFlowAssignees)
                                .Include(t => t.Ticket)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<RequestStepViewModel>> GetTicketBaseWorkFlowSteps(int ticketId)
        {
            List<RequestStepViewModel> ticketSteps = new List<RequestStepViewModel>();
            try
            {
                var ticketInfo = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticketInfo != null)
                {
                    var ticketBaseWorkFlowSteps = await this.FindAll()
                                                  .Where(t => t.TicketId == ticketId)
                                                  .OrderBy(t => t.Id)
                                                  .ToListAsync();

                    var stepWithIds = ticketBaseWorkFlowSteps
                                 .Select
                                 (  
                                    s => new
                                    {
                                        Id = s.Id,
                                        StepId = s.StepId
                                    }
                                 ).ToList();

                    var stepIds = stepWithIds.Select(s => s.StepId).ToList();

                    if (ticketBaseWorkFlowSteps.Count > 0)
                    {
                        var requestSteps = (await _requestType.GetRequestSteps(ticketInfo.RequestTypeId, ticketInfo.Version, true)).ToList();

                        if (requestSteps.Count > 0)
                        {
                            var stepsInTicket = requestSteps.Where(s => stepIds.Contains(s.StepId)).ToList();

                            var ticketBaseWorkWithSteps = new List<TicketBaseWorkFlowWithStepViewModel>();
                            foreach (var step in stepsInTicket)
                            {
                                var ticketBaseWorkWithStep = new TicketBaseWorkFlowWithStepViewModel();

                                ticketBaseWorkWithStep.Step = step;
                                ticketBaseWorkWithStep.Id = stepWithIds.FirstOrDefault(s => s.StepId == step.StepId).Id;

                                ticketBaseWorkWithSteps.Add(ticketBaseWorkWithStep);
                            }

                            if (ticketBaseWorkWithSteps.Count > 0)
                            {
                                foreach(var ticketStep in ticketBaseWorkWithSteps.OrderBy(t => t.Id))
                                {
                                    ticketSteps.Add(ticketStep.Step);
                                }
                            }
                        }   
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return ticketSteps;
        }

        public async Task<StepFlowViewModel> GetTicketBaseStepsAndWorkFlows(int ticketId)
        {
            StepFlowViewModel stepWorkFlows = new StepFlowViewModel();
            stepWorkFlows.StepNodes = new List<StepViewModel>();
            stepWorkFlows.LinkNodes = new List<LinkStepViewModel>();
            List<LinkStepViewModel> listOfNodes = new List<LinkStepViewModel>();

            try
            {
                var ticketInfo = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticketInfo != null)
                {
                    var ticketBaseWorkFlowSteps = await this.FindAll()
                                                 .Where(t => t.TicketId == ticketId)
                                                 .OrderBy(t => t.Id)
                                                 .ToListAsync();

                    var ticketBaseWorkFlowStepIds = ticketBaseWorkFlowSteps.Select(s => s.StepId).ToList();

                    var steps = (await _requestType.GetRequestSteps(ticketInfo.RequestTypeId, ticketInfo.Version, false)).ToList();

                    if (steps.Count > 0)
                    {
                        stepWorkFlows.StepNodes = steps
                                                  .Where(s => ticketBaseWorkFlowStepIds.Contains(s.StepId))
                                                  .Select
                                                  (
                                                     s => new StepViewModel()
                                                     {
                                                         Id = s.StepId.ToString(),
                                                         Label = s.Sequence.ToString() + " - " + s.StepName,
                                                         Color = s.StepTypeColor
                                                     }
                                                  )
                                                  .ToList();

                        if (stepWorkFlows.StepNodes.Count > 0)
                        {
                            //Dummy nodes for entry point
                            stepWorkFlows.StepNodes.Add(new StepViewModel()
                            {
                                Id = "s1",
                                Label = "Request Submission",
                                Color = "#D5D6EA"
                            });

                            var linkStepViewModel = new LinkStepViewModel();

                            int ctr = 0;
                            foreach(var ticketBaseWorkFlowStep in ticketBaseWorkFlowSteps)
                            {
                                if ((ctr + 1) < ticketBaseWorkFlowSteps.Count)
                                {
                                    linkStepViewModel = new LinkStepViewModel();

                                    if (ctr == 0)
                                    {
                                        linkStepViewModel = new LinkStepViewModel();
                                        linkStepViewModel.Id = "ws1";
                                        linkStepViewModel.Source = "s1";
                                        linkStepViewModel.Target = ticketBaseWorkFlowStep.StepId.ToString();
                                        linkStepViewModel.Label = "";
                                        listOfNodes.Add(linkStepViewModel);
                                    }

                                    linkStepViewModel = new LinkStepViewModel();
                                    linkStepViewModel.Id = "w" + ticketBaseWorkFlowStep.Id.ToString();
                                    linkStepViewModel.Source = ticketBaseWorkFlowStep.StepId.ToString();
                                    linkStepViewModel.Target = ticketBaseWorkFlowSteps[ctr + 1].StepId.ToString();
                                    linkStepViewModel.Label = "";
                                    listOfNodes.Add(linkStepViewModel);
                                }

                                ctr++;
                            }

                            stepWorkFlows.LinkNodes = listOfNodes;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return stepWorkFlows;
        }
    }
}