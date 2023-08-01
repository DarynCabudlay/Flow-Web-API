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
    public class TicketCommonRepository : BaseApi, ITicketCommon
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestType _requestType;

        public TicketCommonRepository(
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

        #region Find

        public IQueryable<TicketRequestDetailViewModel> FindTicketRequestDetail(object obj)
        {
            try
            {
                IQueryable<TicketRequestDetailViewModel> query = null;

                var id = (int)obj;

                query = this.GetTicketRequestDetailsBaseData()
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

        public IQueryable<TicketRequestDetailViewModel> FindAllTicketRequestDetails()
        {
            try
            {
                IQueryable<TicketRequestDetailViewModel> query = null;
                query = this.GetTicketRequestDetailsBaseData()
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

        public IQueryable<TicketRequestViewModel> FindTicketRequest(object obj)
        {
            try
            {
                IQueryable<TicketRequestViewModel> query = null;

                var id = (int)obj;

                query = this.GetTicketRequestsBaseData()
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

        public IQueryable<TicketRequestViewModel> FindAllTicketRequests()
        {
            try
            {
                IQueryable<TicketRequestViewModel> query = null;
                query = this.GetTicketRequestsBaseData()
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

        public IQueryable<TicketViewModel> FindTicket(object obj)
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<TicketViewModel> query = null;

                var id = (int)obj;

                query = this.GetTicketsBaseData()
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
                                    Status = (TicketStatus)t.Status,
                                    StatusDescription = (t.Status == 1 ? "Draft" : t.Status == 2 ? "In-process" : t.Status == 3 ? "Rejected" : t.Status == 4 ? "Closed" : "Cancelled"),
                                    StatusDate = t.StatusDate,
                                    DetailedStatus = t.DetailedStatus,
                                    CancellationReason = t.CancellationReason,
                                    CancellationReasonName = t.CancellationReason == null ? "" : _dbCntxt.Reasons.FirstOrDefault(r => r.Id == t.CancellationReason).Name,
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

        public IQueryable<TicketViewModel> FindAllTickets()
        {
            try
            {
                char[] delimeterChars = { ',' };
                IQueryable<TicketViewModel> query = null;
                query = this.GetTicketsBaseData()
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
                                    CancellationReasonName = t.CancellationReason == null ? "" : _dbCntxt.Reasons.FirstOrDefault(r => r.Id == t.CancellationReason).Name,
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

        public IQueryable<TicketAttachmentViewModel> FindTicketAttachment(object obj)
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<TicketAttachmentViewModel> query = null;

                var id = (int)obj;

                query = this.GetTicketAttachmentsBaseData()
                            .Where(t => t.Id == id)
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
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    FileType = new FileTypeViewModel()
                                    {
                                        Id = t.FileType.Id,
                                        Name = t.FileType.Name,
                                        MimeType = t.FileType.MimeType,
                                        MimeTypes = t.FileType.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
                                        Published = t.FileType.Published
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

        public IQueryable<TicketAttachmentViewModel> FindAllTicketAttachments()
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<TicketAttachmentViewModel> query = null;
                query = this.GetTicketAttachmentsBaseData()
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
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    FileType = new FileTypeViewModel()
                                    {
                                        Id = t.FileType.Id,
                                        Name = t.FileType.Name,
                                        MimeType = t.FileType.MimeType,
                                        MimeTypes = t.FileType.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
                                        Published = t.FileType.Published
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

        #endregion

        #region Get

        public IQueryable<TicketViewModel> GetTickets(PagingRequest paging = null, int status = 0)
        {
            try
            {
                IQueryable<TicketViewModel> query = null;
                query = this.FindAllTickets()
                            .AsQueryable();

                if (status > 0)
                {
                    if (!GeneralHelper.IsValidTicketStatus(status))
                        throw new CustomException("Invalid Ticket Status.", 400);

                    query = query.Where(t => t.Status == (TicketStatus)status);
                }

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.TicketNo.Contains(search) || p.StatusDescription.Contains(search) || p.RequestTypeName.Contains(search) || p.Version.ToString().Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<TicketViewModel> GetTicket(int id)
        {
            try
            {
                IQueryable<TicketViewModel> query = null;
                query = this.FindTicket(id)
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<TicketRequestDetailViewModel> GetTicketRequestDetails()
        {
            try
            {
                IQueryable<TicketRequestDetailViewModel> query = null;
                query = this.FindAllTicketRequestDetails()
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<TicketRequestDetailViewModel> GetTicketRequestDetail(int id)
        {
            try
            {
                IQueryable<TicketRequestDetailViewModel> query = null;
                query = this.FindTicketRequestDetail(id)
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<TicketAttachmentViewModel> GetTicketAttachments(PagingRequest paging = null, int category = 0)
        {
            try
            {
                IQueryable<TicketAttachmentViewModel> query = null;
                query = this.FindAllTicketAttachments()
                            .AsQueryable();

                if (category > 0)
                    query = query.Where(t => t.Category == (TicketAttachmentCategory)category);

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.OriginalFileName.Contains(search) || p.FileType.Name.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<TicketAttachmentViewModel> GetTicketAttachment(int id)
        {
            try
            {
                IQueryable<TicketAttachmentViewModel> query = null;
                query = this.FindTicketAttachment(id)
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        #endregion

        #region Paging
        public PagingResponse<TicketViewModel> TicketsPagingFeature(IQueryable<TicketViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<TicketViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<TicketViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public PagingResponse<TicketAttachmentViewModel> TicketAttachmentsPagingFeature(IQueryable<TicketAttachmentViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<TicketAttachmentViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<TicketAttachmentViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        #endregion

        #region BaseData

        private IQueryable<TicketRequestDetail> GetTicketRequestDetailsBaseData()
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

        private IQueryable<TicketRequest> GetTicketRequestsBaseData()
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

        public IQueryable<TicketRequestViewModel> GetTicketRequests()
        {
            try
            {
                IQueryable<TicketRequestViewModel> query = null;
                query = this.FindAllTicketRequests()
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<Ticket> GetTicketsBaseData()
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

        private IQueryable<TicketAttachment> GetTicketAttachmentsBaseData()
        {
            try
            {
                IQueryable<TicketAttachment> query = null;

                query = _dbCntxt.TicketAttachments
                                .Include(t => t.FileType)
                                .AsQueryable();
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        #endregion
    }
}
