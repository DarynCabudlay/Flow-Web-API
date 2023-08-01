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
using Microsoft.EntityFrameworkCore.Storage;
using workflow.Data;

namespace workflow.Services
{
    public class ApprovalLevelRepository : BaseApi, IApprovalLevel
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IOrganizationalStructure _organizationalStructure;
        public ApprovalLevelRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IOrganizationalStructure organizationalStructure) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _organizationalStructure = organizationalStructure;
        }

        public async Task<ApprovalLevelDetailViewModel> Add(ApprovalLevelDetailViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.ApprovalLevelId < 0)
                throw new CustomException("Approval Level is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var approvalLevel = await _dbCntxt.ApprovalLevels.FirstOrDefaultAsync(a => a.Id == model.ApprovalLevelId);

                if (approvalLevel == null)
                    throw new CustomException("Approval Level is not found.", 404);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                await _userIpAddressPerSession.Save(userIp);

                //get maximum sequence per approval level
                int sequence = 0;
                var approvalLevelDetails = await _dbCntxt.ApprovalLevelDetails
                                                         .Where(a => a.ApprovalLevelId == model.ApprovalLevelId)
                                                         .ToListAsync();

                if (approvalLevelDetails.Count > 0)
                    sequence = approvalLevelDetails.Max(a => a.Sequence) + 1;
                else
                    sequence = 1;

                //Insert approval level details
                var approvalLevelDetail = new ApprovalLevelDetail()
                {
                    ApprovalLevelId = model.ApprovalLevelId,
                    Sequence = sequence,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.ApprovalLevelDetails.Add(approvalLevelDetail);
                await _dbCntxt.SaveChangesAsync();

                model.Id = approvalLevelDetail.Id;
                model.Sequence = approvalLevelDetail.Sequence;
                model.CreatedByPK = approvalLevelDetail.CreatedByPK;
                model.CreatedDate = approvalLevelDetail.CreatedDate;
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();

                return model;
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Delete(object obj, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                int id = (int)obj;

                var approvalLevelDetail = await _dbCntxt.ApprovalLevelDetails.FindAsync(id);

                if (approvalLevelDetail == null)
                    throw new CustomException("Approval Level Detail to delete is not found. ", 404);

                //Check if used as user approval level and reporting to
                var userApprovalLevel = await _dbCntxt.UserOrganizationalStructures
                                                      .FirstOrDefaultAsync(u => u.ApprovalLevelDetailId == id || u.ReportingTo == id);

                if (userApprovalLevel != null)
                    throw new CustomException("This approval level detail is currently being used in user maintenance.", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                _dbCntxt.ApprovalLevelDetails.Remove(approvalLevelDetail);
                await _dbCntxt.SaveChangesAsync();


                var approvalLevelDetails = await _dbCntxt.ApprovalLevelDetails
                                                          .Where(a => a.ApprovalLevelId == approvalLevelDetail.ApprovalLevelId)
                                                          .OrderBy(a => a.Sequence)
                                                          .ToListAsync();

                if (approvalLevelDetails.Count > 0)
                {
                    int ctr = 1;
                    foreach (var approvalDetail in approvalLevelDetails)
                    {
                        approvalDetail.Sequence = ctr;

                        _dbCntxt.Entry(approvalDetail).State = EntityState.Modified;
                        await _dbCntxt.SaveChangesAsync();

                        ctr++;
                    }
                }

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details, customex.ListMessage);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task DeleteAll(ApprovalLevelDetailViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ApprovalLevelDetailViewModel> Find(object obj)
        {
            try
            {
                IQueryable<ApprovalLevelDetailViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where
                            (
                                a =>
                                a.Id == id
                            )
                            .Select
                            (
                                a => new ApprovalLevelDetailViewModel
                                {
                                    Id = a.Id,
                                    ApprovalLevelId = a.ApprovalLevelId,
                                    ApprovalLevel = a.ApprovelLevel.Name,
                                    OrganizationEntityId = a.ApprovelLevel.OrganizationEntityId,
                                    OrganizationEntityName = a.ApprovelLevel.OrganizationEntity.Name,
                                    OrganizationEntityHierarchy = a.ApprovelLevel.OrganizationEntity.Hierarchy,
                                    OrganizationEntityPublished = a.ApprovelLevel.OrganizationEntity.Published,
                                    Sequence = a.Sequence,
                                    ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString(),
                                    Published = a.Published,
                                    CreatedByPK = a.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == a.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == a.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : a.CreatedByPK,
                                    CreatedDate = a.CreatedDate,
                                    ModifiedByPK = a.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == a.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == a.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : a.ModifiedByPK,
                                    ModifiedDate = a.ModifiedDate
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

        public IQueryable<ApprovalLevelDetailViewModel> FindAll()
        {
            try
            {
                IQueryable<ApprovalLevelDetailViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                a => new ApprovalLevelDetailViewModel
                                {
                                    Id = a.Id,
                                    ApprovalLevelId = a.ApprovalLevelId,
                                    ApprovalLevel = a.ApprovelLevel.Name,
                                    OrganizationEntityId = a.ApprovelLevel.OrganizationEntityId,
                                    OrganizationEntityName = a.ApprovelLevel.OrganizationEntity.Name,
                                    OrganizationEntityHierarchy = a.ApprovelLevel.OrganizationEntity.Hierarchy,
                                    OrganizationEntityPublished = a.ApprovelLevel.OrganizationEntity.Published,
                                    Sequence = a.Sequence,
                                    ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString(),
                                    Published = a.Published,
                                    CreatedByPK = a.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == a.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == a.CreatedByPK).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault().ToString() : a.CreatedByPK,
                                    CreatedDate = a.CreatedDate,
                                    ModifiedByPK = a.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == a.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == a.ModifiedByPK).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault().ToString() : a.ModifiedByPK,
                                    ModifiedDate = a.ModifiedDate
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

        public IQueryable<ApprovalLevelDetailViewModel> GetApprovalLevelDetail(int id, bool? organizationEntityPublished = null)
        {
            try
            {
                IQueryable<ApprovalLevelDetailViewModel> query = null;
                query = this.Find(id)
                            .AsQueryable();

                if (organizationEntityPublished != null)
                    query = query.Where(a => a.OrganizationEntityPublished == organizationEntityPublished);

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<ApprovalLevelDetailViewModel> GetApprovalLevelDetails(PagingRequest paging = null, bool? published = null, bool? organizationEntityPublished = null)
        {
            try
            {
                IQueryable<ApprovalLevelDetailViewModel> query = null;
                query = this.FindAll()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(o => o.Published == published);

                if (organizationEntityPublished != null)
                    query = query.Where(o => o.OrganizationEntityPublished == published);

                if (paging != null)
                {
                    var organizationEntityId = paging.SearchCriteria.Id1;

                    if (organizationEntityId != null)
                    {
                        if (Convert.ToInt32(organizationEntityId) > 0)
                            query = query.Where(o => o.OrganizationEntityId == organizationEntityId);
                    }

                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.ApprovalLevel.Contains(search) || p.OrganizationEntityName.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<ApprovalLevelViewModel> GetApprovalLevels(bool? organizationEntityPublished = null)
        {
            try
            {
                IQueryable<ApprovalLevelViewModel> query = null;
                query = _dbCntxt.ApprovalLevels
                                .Include(a => a.OrganizationEntity)
                                .Include(a => a.ApprovalLevelDetails)
                                .Select
                                (
                                    a => new ApprovalLevelViewModel
                                    {
                                        Id = a.Id,
                                        Name = a.Name,
                                        OrganizationEntityId = a.OrganizationEntityId,
                                        OrganizationEntityName = a.OrganizationEntity.Name,
                                        OrganizationEntityHierarchy = a.OrganizationEntity.Hierarchy,
                                        OrganizationEntityPublished = a.OrganizationEntity.Published
                                    }
                                )
                                .OrderBy(a => a.OrganizationEntityHierarchy)
                                .AsQueryable();

                if (organizationEntityPublished != null)
                    query = query.Where(a => a.OrganizationEntityPublished == organizationEntityPublished);

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<ApprovalLevelViewModel>> GetApprovalLevels(int organizationalStructureId, bool? organizationalStruturePublished = null, bool? organizationalEntityPublished = null)
        {
            try
            {
                var organizationalStructure = await _organizationalStructure
                                              .GetOrganizationalStructures(null, organizationalStruturePublished, organizationalEntityPublished)
                                              .FirstOrDefaultAsync(o => o.Id == organizationalStructureId);

                int organizationEntityId = organizationalStructure == null ? 0 : organizationalStructure.OrganizationEntityId;

                var organizationEntity = await _organizationalStructure
                                              .GetOrgranizationEntities(null, organizationalEntityPublished)
                                              .FirstOrDefaultAsync(o => o.Id == organizationEntityId);

                int organizationEntityHierarchy = organizationEntity == null ? 0 : organizationEntity.Hierarchy;

                var query = await this
                                .GetApprovalLevels(organizationalEntityPublished)
                                .Where
                                (
                                    a =>
                                    a.OrganizationEntityHierarchy == organizationEntityHierarchy
                                )
                                .ToListAsync();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<ApprovalLevelDetailViewModel> GetDetailsPerApprovalLevel(int id, bool? published = null, bool? organizationEntityPublished = null)
        {
            try
            {
                IQueryable<ApprovalLevelDetailViewModel> query = null;
                query = this.GetApprovalLevelDetails(null, published, organizationEntityPublished)
                            .Where
                            (
                                a => a.ApprovalLevelId == id
                            )
                            .OrderBy(a => a.Sequence)
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<ApprovalLevelDetailWithUserViewModel>> GetDetailsWithUsersPerApprovalLevel(int id, int organizationalStructureId, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            List<ApprovalLevelDetailWithUserViewModel> detailsWithUsers = new List<ApprovalLevelDetailWithUserViewModel>();
            try
            {
                var approvallevelDetails = await this.GetApprovalLevelDetails(null, published, organizationEntityPublished)
                                                    .Where
                                                    (
                                                        a => a.ApprovalLevelId == id
                                                    )
                                                    .OrderBy(a => a.Sequence)
                                                    .ToListAsync();

                if (approvallevelDetails.Count > 0)
                {
                    detailsWithUsers = approvallevelDetails
                                           .Select
                                           (
                                               a => new ApprovalLevelDetailWithUserViewModel()
                                               {
                                                   Id = a.Id,
                                                   ApprovalLevelId = a.ApprovalLevelId,
                                                   ApprovalLevel = a.ApprovalLevel,
                                                   OrganizationEntityId = a.OrganizationEntityId,
                                                   OrganizationEntityName = a.OrganizationEntityName,
                                                   OrganizationEntityHierarchy = a.OrganizationEntityHierarchy,
                                                   Sequence = a.Sequence,
                                                   ApprovalLevelAndSequence = a.ApprovalLevelAndSequence,
                                                   Published = a.Published,
                                                   User = null
                                               }

                                           ).ToList();


                    var orgWithUsers = (await this.GetOrganizationalStructurePathWithUser
                                           (organizationalStructureId, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                           .ToList();

                    if (approvallevelDetails.Count > 0)
                    {
                        foreach (var detail in approvallevelDetails)
                        {
                            var orgWithUser = orgWithUsers.FirstOrDefault(o => o.ApprovalLevelDetailId == detail.Id);

                            if (orgWithUser != null)
                            {
                                var detailsWithUser = detailsWithUsers.FirstOrDefault(d => d.Id == detail.Id);
                                detailsWithUser.User = new UserBasicInfoModel()
                                {
                                    Id = orgWithUser.UserId,
                                    FirstName = orgWithUser.FirstName,
                                    MiddleName = orgWithUser.MiddleName,
                                    LastName = orgWithUser.LastName,
                                    FullName = orgWithUser.FullName
                                };
                            }
                        }
                    }
                }

                return detailsWithUsers;
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

        public PagingResponse<ApprovalLevelDetailViewModel> PagingFeature(IQueryable<ApprovalLevelDetailViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<ApprovalLevelDetailViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<ApprovalLevelDetailViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Update(ApprovalLevelDetailViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.ApprovalLevelId < 0)
                throw new CustomException("Approval Level is required.", 400);

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var approvalLevel = await _dbCntxt.ApprovalLevels.FirstOrDefaultAsync(a => a.Id == model.ApprovalLevelId);

                if (approvalLevel == null)
                    throw new CustomException("Approval Level is not found.", 404);

                var approvalLevelDetail = await _dbCntxt.ApprovalLevelDetails.FirstOrDefaultAsync(a => a.Id == tableId);

                if (approvalLevelDetail != null)
                {
                    //if (!model.Published)
                    //{
                    //    //Check if used as user approval level
                    //    var userApprovalLevel = await _dbCntxt.UserOrganizationalStructures
                    //                                          .FirstOrDefaultAsync(u => u.ApprovalLevelDetailId == model.Id || u.ReportingTo == model.Id);

                    //    if (userApprovalLevel != null)
                    //        throw new CustomException("Unable to deactivate this approval level detail since this is currently being used by user as approval level or reporting to.", 400);
                    //}

                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);

                    approvalLevelDetail.ApprovalLevelId = model.ApprovalLevelId;
                    approvalLevelDetail.Published = model.Published;
                    //approvalLevelDetail.Sequence = model.Sequence;
                    approvalLevelDetail.ModifiedByPK = cid;
                    approvalLevelDetail.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(approvalLevelDetail).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                    dbContextTransaction.Commit();
                }
                else
                    throw new CustomException("Approval Level Detail is not found", 404);
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<ApprovalLevelDetail> Get()
        {
            try
            {
                IQueryable<ApprovalLevelDetail> query = null;

                query = _dbCntxt.ApprovalLevelDetails
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<List<ApprovalLevelDetailViewModel>> GetApprovalLevelDetailsInSequentialMannerAsync(bool? published = null, bool? organizationEntityPublished = null)
        {
            try
            {
                List<ApprovalLevelDetailViewModel> query = null;
                query = await this.FindAll()
                                  .ToListAsync();

                if (published != null)
                    query = query.Where(a => a.Published == published).ToList();

                if (organizationEntityPublished != null)
                    query = query.Where(a => a.OrganizationEntityPublished == organizationEntityPublished).ToList();

                query = query.OrderByDescending(a => a.OrganizationEntityHierarchy)
                             .ThenByDescending(a => a.Sequence).ToList();

                int ctr = 1;
                foreach (var detail in query)
                {
                    detail.RowNumber = ctr;
                    ctr++;
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task<IEnumerable<UserOrganizationalStructureViewModel>> GetUsersOrganizationalStructures(bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null)
        {
            try
            {
                List<UserOrganizationalStructureViewModel> query = null;

                query = await _dbCntxt.UserOrganizationalStructures
                                      .Include(u => u.User)
                                      .Include(u => u.User.AspNetUsersProfile)
                                      .Include(u => u.OrganizationalStructure)
                                      .Include(u => u.OrganizationalStructure.OrganizationEntity)
                                      .Select
                                        (
                                            u => new UserOrganizationalStructureViewModel
                                            {
                                                Id = u.Id,
                                                UserId = u.UserId,
                                                OrganizationalStructureId = u.OrganizationalStructureId,
                                                OrganizationalStructure = u.OrganizationalStructure.Name,
                                                OrganizationalStructurePublished = u.OrganizationalStructure.Published,
                                                OrganizationEntityId = u.OrganizationalStructure.OrganizationEntityId,
                                                OrganizationEntityName = u.OrganizationalStructure.OrganizationEntity.Name,
                                                OrganizationEntityPublished = u.OrganizationalStructure.OrganizationEntity.Published,
                                                User = new UserRegisterModel
                                                {
                                                    Id = u.User.Id,
                                                    UserName = u.User.UserName,
                                                    Email = u.User.Email,
                                                    FirstName = u.User.AspNetUsersProfile.FirstName,
                                                    LastName = u.User.AspNetUsersProfile.LastName,
                                                    Status = u.User.Status
                                                },
                                                ApprovalLevelDetailId = u.ApprovalLevelDetailId,
                                                ApprovalLevel = (u.ApprovalLevelDetailId > 0 ? _dbCntxt.ApprovalLevelDetails
                                                                .Include(a => a.ApprovelLevel)
                                                                .Where
                                                                (
                                                                    a =>
                                                                    a.Id == u.ApprovalLevelDetailId
                                                                )
                                                                .Select
                                                                (
                                                                    a => new ApprovalLevelDetailViewModel
                                                                    {
                                                                        Id = a.Id,
                                                                        ApprovalLevelId = a.ApprovalLevelId,
                                                                        ApprovalLevel = a.ApprovelLevel.Name,
                                                                        Sequence = a.Sequence,
                                                                        ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                                    }
                                                                )
                                                                .FirstOrDefault() : new ApprovalLevelDetailViewModel
                                                                {
                                                                    Id = 0,
                                                                    ApprovalLevelId = 0,
                                                                    ApprovalLevel = "Staff",
                                                                    Sequence = 0,
                                                                    ApprovalLevelAndSequence = "Staff"
                                                                }),
                                                ReportingTo = u.ReportingTo,
                                                ReportingToApprovalLevel = (u.ReportingTo == 9999 ? new ApprovalLevelDetailViewModel
                                                {
                                                    Id = 0,
                                                    ApprovalLevelId = 0,
                                                    ApprovalLevel = "Next in Line",
                                                    Sequence = 0,
                                                    ApprovalLevelAndSequence = "Next in Line"

                                                } : _dbCntxt.ApprovalLevelDetails
                                                                        .Include(a => a.ApprovelLevel)
                                                                        .Where
                                                                        (
                                                                            a =>
                                                                            a.Id == u.ReportingTo
                                                                        )
                                                                        .Select
                                                                        (
                                                                            a => new ApprovalLevelDetailViewModel
                                                                            {
                                                                                Id = a.Id,
                                                                                ApprovalLevelId = a.ApprovalLevelId,
                                                                                ApprovalLevel = a.ApprovelLevel.Name,
                                                                                Sequence = a.Sequence,
                                                                                ApprovalLevelAndSequence = a.ApprovelLevel.Name + " - " + a.Sequence.ToString()
                                                                            }
                                                                        )
                                                                        .FirstOrDefault()),
                                                Published = u.Published
                                            }
                                        ).ToListAsync();

                if (published != null)
                    query = query.Where(u => u.Published == published).ToList();

                if (organizationalStructurePublished != null)
                    query = query.Where(u => u.OrganizationalStructurePublished == organizationalStructurePublished).ToList();

                if (organizationEntityPublished != null)
                    query = query.Where(u => u.OrganizationEntityPublished == organizationEntityPublished).ToList();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<ApprovalLevelStructureViewModel>> GetApprovalLevelStructures(int organizationalStructureId = 0, int userOrganizationalStructureId = 0, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            List<ApprovalLevelStructureViewModel> approvalLevelStructures = new List<ApprovalLevelStructureViewModel>();
            try
            {
                var approvalLevelDetails = (await this.GetApprovalLevelDetailsInSequentialMannerAsync(published, organizationEntityPublished))
                                            .ToList();

                var organizationalStructures = await _organizationalStructure
                                                    .GetOrganizationalStructures(null, organizationalStructurePublished, organizationEntityPublished, userPublished)
                                                    .ToListAsync();

                List<int> orgStructureIdsParam = new List<int>();
                if (organizationalStructureId > 0)
                {
                    var orgStructure = await _dbCntxt.OrganizationalStructures
                                             .FirstOrDefaultAsync(o => o.Id == Convert.ToInt32(organizationalStructureId));

                    if (orgStructure == null)
                        throw new CustomException("Organizational Structure is not found.", 404);

                    if (orgStructure.Published)
                    {
                        orgStructureIdsParam = (await _organizationalStructure
                                               .GetOrganizationalStructureChildren(Convert.ToInt32(organizationalStructureId), null, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                               .Select(o => o.Id)
                                               .ToList();

                        orgStructureIdsParam.Add(Convert.ToInt32(organizationalStructureId));
                    }
                    else
                    {
                        //If org structure is inactive then empty the list
                        organizationalStructures = new List<OrganizationalStructureViewModel>();
                    }
                }
                else
                {
                    var parentOrgs = organizationalStructures
                                    .Where(o => o.ParentId == 0).ToList();

                    if (parentOrgs.Count > 0)
                    {
                        List<int> allOrgStructureIds = new List<int>();
                        foreach (var parentOrg in parentOrgs)
                        {
                            allOrgStructureIds.AddRange(
                                                (await _organizationalStructure
                                                        .GetOrganizationalStructureChildren(parentOrg.Id, null, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                                        .Select(o => o.Id)
                                                        .ToList()
                                                );

                            allOrgStructureIds.Add(parentOrg.Id);
                        }

                        organizationalStructures = organizationalStructures
                                                   .Where(o => allOrgStructureIds.Contains(o.Id)).ToList();

                    }
                }

                List<int> usersChildrenAndParentsToDisplay = new List<int>();
                if (userOrganizationalStructureId > 0)
                {

                    var userOrg = await _dbCntxt.UserOrganizationalStructures
                                                .FirstOrDefaultAsync(o => o.Id == Convert.ToInt32(userOrganizationalStructureId));
                    if (userOrg == null)
                        throw new CustomException("User Organizational Structure is not found.", 404);

                    usersChildrenAndParentsToDisplay.Add(userOrganizationalStructureId);

                    var userOrgParents = (await _organizationalStructure
                                          .GetOrganizationalStructureParents(userOrg.OrganizationalStructureId, null, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                          .ToList();

                    if (userOrgParents.Count > 0)
                    {
                        foreach (var userOrgParent in userOrgParents)
                        {
                            usersChildrenAndParentsToDisplay.AddRange(userOrgParent.UserOrganizationalStructures
                                         .Where(u => u.ApprovalLevelDetailId > 0)
                                         .Select(u => u.Id).ToList());
                        }
                    }

                    var userOrgPaths = (await _organizationalStructure
                                          .GetOrganizationalStructurePathAsync(userOrg.OrganizationalStructureId, null, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                          .Where
                                              (
                                                  u =>
                                                  u.Id == userOrg.OrganizationalStructureId
                                              )
                                          .ToList();

                    if (userOrgPaths.Count > 0)
                    {
                        foreach (var userOrgPath in userOrgPaths)
                        {
                            usersChildrenAndParentsToDisplay
                                         .AddRange
                                         (
                                            userOrgPath
                                            .UserOrganizationalStructures
                                            .Select(u => u.Id).ToList()
                                         );
                        }
                    }

                    var userOrgChildren = (await _organizationalStructure
                                          .GetOrganizationalStructureChildren(userOrg.OrganizationalStructureId, null, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                          //.Where(o => o.OrganizationEntityPublished == true)
                                          .ToList();

                    if (userOrgChildren.Count > 0)
                    {
                        foreach (var userOrgChild in userOrgChildren)
                        {
                            usersChildrenAndParentsToDisplay.AddRange(userOrgChild.UserOrganizationalStructures
                                         .Select(u => u.Id).ToList());
                        }
                    }

                    if (orgStructureIdsParam.Count > 0)
                    {
                        var isUserOrgStructureExistsInChildren = orgStructureIdsParam.Contains(userOrg.OrganizationalStructureId);

                        if (!isUserOrgStructureExistsInChildren)
                            throw new CustomException("User Organizational Structure is not found in Top Level Children.", 404);
                    }
                }

                List<int> orgStructureIdsToDisplay = new List<int>();
                if (orgStructureIdsParam.Count > 0)
                    organizationalStructures = organizationalStructures
                                               .Where(o => orgStructureIdsParam.Contains(o.Id))
                                               .ToList();
                else
                    organizationalStructures = organizationalStructures
                                               .ToList();

                orgStructureIdsToDisplay = organizationalStructures.Select(o => o.Id).ToList();

                var userOrganizationalStructures = (await this.GetUsersOrganizationalStructures(userPublished, organizationalStructurePublished, organizationEntityPublished))
                                                              .Where
                                                              (
                                                                  u => orgStructureIdsToDisplay.Contains(u.OrganizationalStructureId)
                                                              )
                                                              .ToList();

                if (orgStructureIdsParam.Count > 0)
                {
                    if (userOrganizationalStructureId > 0)
                    {
                        userOrganizationalStructures = userOrganizationalStructures
                                                       .Where
                                                       (
                                                            o =>
                                                            usersChildrenAndParentsToDisplay.Contains(o.Id)
                                                       )
                                                       .ToList();
                    }
                }
                else
                {
                    if (userOrganizationalStructureId > 0)
                    {
                        userOrganizationalStructures = userOrganizationalStructures
                                                       .Where
                                                       (
                                                            o =>
                                                            usersChildrenAndParentsToDisplay.Contains(o.Id)
                                                       )
                                                       .ToList();
                    }
                }

                var userOrganizationalStructuresIds = userOrganizationalStructures
                                                     .Select(u => u.Id).ToList();

                if (userOrganizationalStructures.Count > 0)
                {
                    List<UserOrganizationalStructureViewModel> structureEntryPoint = new List<UserOrganizationalStructureViewModel>();

                    //Check If Users Approval Level used as reportingTo to exclude from the entry point of approval level structures
                    bool isUserApprovalLevelDetailIdIsUsed = false;
                    foreach (var userOrganizationalStructure in userOrganizationalStructures)
                    {
                        if (userOrganizationalStructure.ApprovalLevelDetailId > 0)
                        {
                            //Get User Org Approval Level Detail Row Number
                            var userOrgApprovalLevelRowNumber = this.GetApprovalDetailRowNumber(userOrganizationalStructure.ApprovalLevelDetailId, approvalLevelDetails);

                            var userOrgPath = (await _organizationalStructure.GetOrganizationalStructurePathAsync(userOrganizationalStructure.OrganizationalStructureId, organizationalStructures, organizationalStructurePublished, organizationEntityPublished, userPublished)
                                              )
                                              .Where
                                              (
                                                  u =>
                                                  u.Id == userOrganizationalStructure.OrganizationalStructureId
                                              )
                                              .FirstOrDefault();

                            List<OrganizationalStructureWithUserViewModel> orgWithUsers = new List<OrganizationalStructureWithUserViewModel>();
                            if (userOrgPath != null)
                            {
                                var staffs = userOrgPath.UserOrganizationalStructures
                                                        .Where
                                                        (
                                                            u => u.ApprovalLevelDetailId == 0 &&
                                                            userOrganizationalStructuresIds.Contains(u.Id)
                                                        ).ToList();

                                var approvers = userOrgPath
                                               .UserOrganizationalStructures
                                               .Where
                                               (
                                                    u => u.ApprovalLevelDetailId > 0 &&
                                                    userOrganizationalStructuresIds.Contains(u.Id)
                                               )
                                               .OrderBy
                                               (
                                                   u =>
                                                   this.GetApprovalDetailRowNumber(u.ApprovalLevelDetailId, approvalLevelDetails)
                                               )
                                               .ToList();

                                var orgUsers = new List<UserOrganizationalStructureViewModel>();

                                orgUsers.AddRange(staffs);
                                orgUsers.AddRange(approvers);

                                orgWithUsers = this.AddUsersInOrgStructure(userOrgPath.Id, userOrgPath.Name, userOrgPath.OrganizationEntityId, userOrgPath.OrganizationEntityName, userOrgPath.OrganizationEntityHierarchy, orgUsers, approvalLevelDetails);

                                var userOnTheSameStructures = orgWithUsers
                                                        .Where
                                                        (
                                                            u =>
                                                            u.ApprovalLevelDetailId == 0 ||
                                                            (
                                                                u.ApprovalLevelDetailRowNumber < userOrgApprovalLevelRowNumber && u.ApprovalLevelDetailId > 0
                                                            )
                                                        )
                                                        .ToList()
                                                        .GroupBy(u => new { u.ApprovalLevelDetailId, u.ReportingTo });

                                isUserApprovalLevelDetailIdIsUsed = false;
                                if (userOnTheSameStructures.Count() > 0)
                                {
                                    foreach (var userGroup in userOnTheSameStructures)
                                    {
                                        foreach (var user in userGroup)
                                        {
                                            if (user.ReportingTo == userOrganizationalStructure.ApprovalLevelDetailId)
                                            {
                                                isUserApprovalLevelDetailIdIsUsed = true;
                                                break;
                                            }

                                            if (user.ReportingTo > 0)
                                            {
                                                var nextUsersInPath = orgWithUsers
                                                                     .Where
                                                                     (
                                                                         u =>
                                                                         u.ApprovalLevelDetailRowNumber > user.ApprovalLevelDetailRowNumber
                                                                     )
                                                                     .OrderBy(u => u.ApprovalLevelDetailRowNumber)
                                                                     .ToList();


                                                var nextUserInPath = new OrganizationalStructureWithUserViewModel();

                                                if (user.ReportingTo == 9999)
                                                {
                                                    nextUserInPath = nextUsersInPath.FirstOrDefault();

                                                    if (nextUserInPath != null)
                                                    {
                                                        if (nextUserInPath.ApprovalLevelDetailId == userOrganizationalStructure.ApprovalLevelDetailId)
                                                        {
                                                            isUserApprovalLevelDetailIdIsUsed = true;
                                                            break;
                                                        }

                                                        if (nextUserInPath.ReportingTo == userOrganizationalStructure.ApprovalLevelDetailId)
                                                        {
                                                            isUserApprovalLevelDetailIdIsUsed = true;
                                                            break;
                                                        }

                                                        if (nextUserInPath.ReportingToRowNumber > userOrgApprovalLevelRowNumber)
                                                            continue;

                                                        isUserApprovalLevelDetailIdIsUsed = this.CheckApprovalLevelUsedAsReportingTo(orgWithUsers, nextUserInPath, userOrganizationalStructure.ApprovalLevelDetailId, userOrgApprovalLevelRowNumber);

                                                        if (isUserApprovalLevelDetailIdIsUsed)
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    nextUserInPath = nextUsersInPath
                                                                         .Where
                                                                         (
                                                                            u =>
                                                                            u.ApprovalLevelDetailId == user.ReportingTo
                                                                         )
                                                                         .FirstOrDefault();

                                                    if (nextUserInPath == null)
                                                    {
                                                        nextUserInPath = nextUsersInPath
                                                                         .Where
                                                                         (
                                                                            u =>
                                                                            u.ApprovalLevelDetailRowNumber > user.ReportingToRowNumber
                                                                         )
                                                                         .FirstOrDefault();
                                                    }

                                                    if (nextUserInPath != null)
                                                    {
                                                        if (nextUserInPath.ApprovalLevelDetailId == userOrganizationalStructure.ApprovalLevelDetailId)
                                                        {
                                                            isUserApprovalLevelDetailIdIsUsed = true;
                                                            break;
                                                        }

                                                        if (nextUserInPath.ReportingTo == userOrganizationalStructure.ApprovalLevelDetailId)
                                                        {
                                                            isUserApprovalLevelDetailIdIsUsed = true;
                                                            break;
                                                        }

                                                        if (nextUserInPath.ReportingToRowNumber > userOrgApprovalLevelRowNumber)
                                                            continue;

                                                        isUserApprovalLevelDetailIdIsUsed = this.CheckApprovalLevelUsedAsReportingTo(orgWithUsers, nextUserInPath, userOrganizationalStructure.ApprovalLevelDetailId, userOrgApprovalLevelRowNumber);

                                                        if (isUserApprovalLevelDetailIdIsUsed)
                                                            break;
                                                    }
                                                }
                                            }
                                        }

                                        if (isUserApprovalLevelDetailIdIsUsed)
                                            break;
                                    }

                                }

                                if (isUserApprovalLevelDetailIdIsUsed)
                                    continue;
                            }

                            //Get children
                            var children = (await _organizationalStructure.GetOrganizationalStructureChildren(userOrganizationalStructure.OrganizationalStructureId, organizationalStructures, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                //.Where(o => o.OrganizationEntityPublished == true)
                                .ToList();

                            if (children.Count > 0)
                            {
                                foreach (var child in children)
                                {
                                    orgWithUsers = new List<OrganizationalStructureWithUserViewModel>();

                                    var staffs = child.UserOrganizationalStructures.Where(u => u.ApprovalLevelDetailId == 0).ToList();

                                    var approvers = child
                                                    .UserOrganizationalStructures
                                                    .Where
                                                    (
                                                        u => u.ApprovalLevelDetailId > 0
                                                    )
                                                    .OrderBy
                                                    (
                                                        u =>
                                                        this.GetApprovalDetailRowNumber(u.ApprovalLevelDetailId, approvalLevelDetails)
                                                    )
                                                    .ToList();

                                    var orgUsers = new List<UserOrganizationalStructureViewModel>();

                                    orgUsers.AddRange(staffs);
                                    orgUsers.AddRange(approvers);

                                    orgWithUsers = this.AddUsersInOrgStructure(child.Id, child.Name, child.OrganizationEntityId, child.OrganizationEntityName, child.OrganizationEntityHierarchy, orgUsers, approvalLevelDetails);

                                    var userOnChild = orgWithUsers
                                                    .Where
                                                    (
                                                        u =>
                                                        u.ApprovalLevelDetailId == 0 ||
                                                        (
                                                            u.ApprovalLevelDetailRowNumber < userOrgApprovalLevelRowNumber && u.ApprovalLevelDetailId > 0
                                                        )
                                                    )
                                                    .ToList()
                                                    .GroupBy(u => new { u.ApprovalLevelDetailId, u.ReportingTo });

                                    if (userOnChild.Count() > 0)
                                    {
                                        isUserApprovalLevelDetailIdIsUsed = false;
                                        foreach (var userGroup in userOnChild)
                                        {
                                            foreach (var user in userGroup)
                                            {
                                                if (user.ReportingTo == userOrganizationalStructure.ApprovalLevelDetailId)
                                                {
                                                    isUserApprovalLevelDetailIdIsUsed = true;
                                                    break;
                                                }

                                                if (user.ReportingTo > 0)
                                                {
                                                    var nextUsersInPath = orgWithUsers
                                                                         .Where
                                                                         (
                                                                             u =>
                                                                             u.ApprovalLevelDetailRowNumber > user.ApprovalLevelDetailRowNumber
                                                                         )
                                                                         .OrderBy(u => u.ApprovalLevelDetailRowNumber)
                                                                         .ToList();


                                                    var nextUserInPath = new OrganizationalStructureWithUserViewModel();

                                                    if (user.ReportingTo == 9999)
                                                    {
                                                        nextUserInPath = nextUsersInPath.FirstOrDefault();

                                                        if (nextUserInPath != null)
                                                        {
                                                            if (nextUserInPath.ApprovalLevelDetailId == userOrganizationalStructure.ApprovalLevelDetailId)
                                                            {
                                                                isUserApprovalLevelDetailIdIsUsed = true;
                                                                break;
                                                            }

                                                            if (nextUserInPath.ReportingTo == userOrganizationalStructure.ApprovalLevelDetailId)
                                                            {
                                                                isUserApprovalLevelDetailIdIsUsed = true;
                                                                break;
                                                            }

                                                            if (nextUserInPath.ReportingToRowNumber > userOrgApprovalLevelRowNumber)
                                                                continue;

                                                            isUserApprovalLevelDetailIdIsUsed = this.CheckApprovalLevelUsedAsReportingTo(orgWithUsers, nextUserInPath, userOrganizationalStructure.ApprovalLevelDetailId, userOrgApprovalLevelRowNumber);

                                                            if (isUserApprovalLevelDetailIdIsUsed)
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        nextUserInPath = nextUsersInPath
                                                                             .Where
                                                                             (
                                                                                u =>
                                                                                u.ApprovalLevelDetailId == user.ReportingTo
                                                                             )
                                                                             .FirstOrDefault();

                                                        if (nextUserInPath == null)
                                                        {
                                                            nextUserInPath = nextUsersInPath
                                                                             .Where
                                                                             (
                                                                                u =>
                                                                                u.ApprovalLevelDetailRowNumber > user.ReportingToRowNumber
                                                                             )
                                                                             .FirstOrDefault();
                                                        }

                                                        if (nextUserInPath != null)
                                                        {
                                                            if (nextUserInPath.ApprovalLevelDetailId == userOrganizationalStructure.ApprovalLevelDetailId)
                                                            {
                                                                isUserApprovalLevelDetailIdIsUsed = true;
                                                                break;
                                                            }

                                                            if (nextUserInPath.ReportingTo == userOrganizationalStructure.ApprovalLevelDetailId)
                                                            {
                                                                isUserApprovalLevelDetailIdIsUsed = true;
                                                                break;
                                                            }

                                                            if (nextUserInPath.ReportingToRowNumber > userOrgApprovalLevelRowNumber)
                                                                continue;

                                                            isUserApprovalLevelDetailIdIsUsed = this.CheckApprovalLevelUsedAsReportingTo(orgWithUsers, nextUserInPath, userOrganizationalStructure.ApprovalLevelDetailId, userOrgApprovalLevelRowNumber);

                                                            if (isUserApprovalLevelDetailIdIsUsed)
                                                                break;
                                                        }
                                                    }
                                                }
                                            }

                                            if (isUserApprovalLevelDetailIdIsUsed)
                                                break;
                                        }

                                    }

                                    if (isUserApprovalLevelDetailIdIsUsed)
                                        break;
                                }

                                if (isUserApprovalLevelDetailIdIsUsed)
                                    continue;
                            }

                            structureEntryPoint.Add(userOrganizationalStructure);
                        }
                        else
                            structureEntryPoint.Add(userOrganizationalStructure);

                    }

                    var consolidatedUsers = new List<UserWithReportingToOrganizationalStructureViewModel>();

                    //Get users parent
                    foreach (var entryPoint in structureEntryPoint)
                    {
                        List<OrganizationalStructureWithUserViewModel> orgWithUsers = new List<OrganizationalStructureWithUserViewModel>();

                        var paths = (await _organizationalStructure.GetOrganizationalStructurePathAsync(entryPoint.OrganizationalStructureId, organizationalStructures, organizationalStructurePublished, organizationEntityPublished, userPublished))
                            //.Where(o => o.OrganizationEntityPublished == true)
                            .ToList();

                        if (paths.Count > 0)
                        {
                            foreach (var path in paths)
                            {
                                var staffs = path
                                             .UserOrganizationalStructures
                                             .Where
                                             (
                                                u => u.ApprovalLevelDetailId == 0 &&
                                                userOrganizationalStructuresIds.Contains(u.Id)
                                             ).ToList();

                                var approvers = path
                                                .UserOrganizationalStructures
                                                .Where
                                                (
                                                    u => u.ApprovalLevelDetailId > 0 &&
                                                    userOrganizationalStructuresIds.Contains(u.Id)
                                                )
                                                .OrderBy
                                                (
                                                    u =>
                                                    this.GetApprovalDetailRowNumber(u.ApprovalLevelDetailId, approvalLevelDetails)
                                                )
                                                .ToList();

                                var orgUsers = new List<UserOrganizationalStructureViewModel>();

                                orgUsers.AddRange(staffs);
                                orgUsers.AddRange(approvers);

                                orgWithUsers.AddRange(this.AddUsersInOrgStructure(path.Id, path.Name, path.OrganizationEntityId, path.OrganizationEntityName, path.OrganizationEntityHierarchy, orgUsers, approvalLevelDetails));
                            }

                            int entryPointApprovalLevelRowNumber = this.GetApprovalDetailRowNumber(entryPoint.ApprovalLevelDetailId, approvalLevelDetails);

                            int entryPointReportingToRowNumber = this.GetApprovalDetailRowNumber(entryPoint.ReportingTo, approvalLevelDetails);

                            var usersInPath = orgWithUsers
                                            .Where
                                            (
                                                u =>
                                                u.ApprovalLevelDetailRowNumber > entryPointApprovalLevelRowNumber
                                            )
                                            .OrderBy(u => u.ApprovalLevelDetailRowNumber)
                                            .ToList();

                            if (usersInPath.Count > 0)
                            {
                                if (entryPoint.ReportingTo > 0)
                                {
                                    var nextUserInPath = new OrganizationalStructureWithUserViewModel();

                                    if (entryPoint.ReportingTo == 9999)
                                    {
                                        nextUserInPath = usersInPath
                                                        .FirstOrDefault();

                                        if (nextUserInPath != null)
                                        {
                                            consolidatedUsers.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                            {
                                                Id = entryPoint.Id,
                                                UserId = entryPoint.UserId,
                                                FirstName = entryPoint.User.FirstName,
                                                LastName = entryPoint.User.LastName,
                                                FullName = entryPoint.User.FirstName + " " + entryPoint.User.LastName,
                                                UserOrganizationalStructureId = entryPoint.OrganizationalStructureId,
                                                UserOrganizationalStructure = entryPoint.OrganizationalStructure,
                                                UserOrganizationEntityId = entryPoint.OrganizationEntityId,
                                                UserOrganizationEntityName = entryPoint.OrganizationEntityName,
                                                UserApprovalLevelDetailId = entryPoint.ApprovalLevelDetailId,
                                                UserApprovalLevel = entryPoint.ApprovalLevel,
                                                ReportingToId = nextUserInPath.UserOrgStructureId,
                                                ReportingToUserId = nextUserInPath.UserId,
                                                ReportingToFirstName = nextUserInPath.FirstName,
                                                ReportingToLastName = nextUserInPath.LastName,
                                                ReportingToFullName = nextUserInPath.FullName,
                                                ReportingToOrganizationalStructureId = nextUserInPath.Id,
                                                ReportingToOrganizationalStructure = nextUserInPath.Name,
                                                ReportingToOrganizationEntityId = nextUserInPath.OrganizationEntityId,
                                                ReportingToOrganizationEntityName = nextUserInPath.OrganizationEntityName,
                                                ReportingToApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                                ReportingToApprovalLevel = nextUserInPath.ApprovalLevel,
                                                Published = entryPoint.Published
                                            });

                                            consolidatedUsers.AddRange(this.AddUsersInPath(nextUserInPath, usersInPath));
                                        }
                                        else
                                        {
                                            //With reporting to but not assign to a user till the end of the path
                                            consolidatedUsers.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                            {
                                                Id = entryPoint.Id,
                                                UserId = entryPoint.UserId,
                                                FirstName = entryPoint.User.FirstName,
                                                LastName = entryPoint.User.LastName,
                                                FullName = entryPoint.User.FirstName + " " + entryPoint.User.LastName,
                                                UserOrganizationalStructureId = entryPoint.OrganizationalStructureId,
                                                UserOrganizationalStructure = entryPoint.OrganizationalStructure,
                                                UserOrganizationEntityId = entryPoint.OrganizationEntityId,
                                                UserOrganizationEntityName = entryPoint.OrganizationEntityName,
                                                UserApprovalLevelDetailId = entryPoint.ApprovalLevelDetailId,
                                                UserApprovalLevel = entryPoint.ApprovalLevel,
                                                ReportingToId = 0,
                                                ReportingToUserId = "",
                                                ReportingToFirstName = "",
                                                ReportingToLastName = "",
                                                ReportingToFullName = "",
                                                ReportingToOrganizationalStructureId = 0,
                                                ReportingToOrganizationalStructure = "",
                                                ReportingToOrganizationEntityId = 0,
                                                ReportingToOrganizationEntityName = "",
                                                ReportingToApprovalLevelDetailId = 0,
                                                ReportingToApprovalLevel = null,
                                                Published = entryPoint.Published
                                            });
                                        }
                                    }
                                    else
                                    {
                                        nextUserInPath = usersInPath
                                                         .Where
                                                         (
                                                            u =>
                                                            u.ApprovalLevelDetailId == entryPoint.ReportingTo
                                                         )
                                                         .FirstOrDefault();

                                        if (nextUserInPath != null)
                                        {
                                            consolidatedUsers.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                            {
                                                Id = entryPoint.Id,
                                                UserId = entryPoint.UserId,
                                                FirstName = entryPoint.User.FirstName,
                                                LastName = entryPoint.User.LastName,
                                                FullName = entryPoint.User.FirstName + " " + entryPoint.User.LastName,
                                                UserOrganizationalStructureId = entryPoint.OrganizationalStructureId,
                                                UserOrganizationalStructure = entryPoint.OrganizationalStructure,
                                                UserOrganizationEntityId = entryPoint.OrganizationEntityId,
                                                UserOrganizationEntityName = entryPoint.OrganizationEntityName,
                                                UserApprovalLevelDetailId = entryPoint.ApprovalLevelDetailId,
                                                UserApprovalLevel = entryPoint.ApprovalLevel,
                                                ReportingToId = nextUserInPath.UserOrgStructureId,
                                                ReportingToUserId = nextUserInPath.UserId,
                                                ReportingToFirstName = nextUserInPath.FirstName,
                                                ReportingToLastName = nextUserInPath.LastName,
                                                ReportingToFullName = nextUserInPath.FullName,
                                                ReportingToOrganizationalStructureId = nextUserInPath.Id,
                                                ReportingToOrganizationalStructure = nextUserInPath.Name,
                                                ReportingToOrganizationEntityId = nextUserInPath.OrganizationEntityId,
                                                ReportingToOrganizationEntityName = nextUserInPath.OrganizationEntityName,
                                                ReportingToApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                                ReportingToApprovalLevel = nextUserInPath.ApprovalLevel,
                                                Published = entryPoint.Published
                                            });

                                            consolidatedUsers.AddRange(this.AddUsersInPath(nextUserInPath, usersInPath));
                                        }
                                        else
                                        {
                                            nextUserInPath = usersInPath
                                                            .Where
                                                            (
                                                               u =>
                                                               u.ApprovalLevelDetailRowNumber > entryPointReportingToRowNumber
                                                            )
                                                            .FirstOrDefault();

                                            if (nextUserInPath != null)
                                            {
                                                consolidatedUsers.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                                {
                                                    Id = entryPoint.Id,
                                                    UserId = entryPoint.UserId,
                                                    FirstName = entryPoint.User.FirstName,
                                                    LastName = entryPoint.User.LastName,
                                                    FullName = entryPoint.User.FirstName + " " + entryPoint.User.LastName,
                                                    UserOrganizationalStructureId = entryPoint.OrganizationalStructureId,
                                                    UserOrganizationalStructure = entryPoint.OrganizationalStructure,
                                                    UserOrganizationEntityId = entryPoint.OrganizationEntityId,
                                                    UserOrganizationEntityName = entryPoint.OrganizationEntityName,
                                                    UserApprovalLevelDetailId = entryPoint.ApprovalLevelDetailId,
                                                    UserApprovalLevel = entryPoint.ApprovalLevel,
                                                    ReportingToId = nextUserInPath.UserOrgStructureId,
                                                    ReportingToUserId = nextUserInPath.UserId,
                                                    ReportingToFirstName = nextUserInPath.FirstName,
                                                    ReportingToLastName = nextUserInPath.LastName,
                                                    ReportingToFullName = nextUserInPath.FullName,
                                                    ReportingToOrganizationalStructureId = nextUserInPath.Id,
                                                    ReportingToOrganizationalStructure = nextUserInPath.Name,
                                                    ReportingToOrganizationEntityId = nextUserInPath.OrganizationEntityId,
                                                    ReportingToOrganizationEntityName = nextUserInPath.OrganizationEntityName,
                                                    ReportingToApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                                    ReportingToApprovalLevel = nextUserInPath.ApprovalLevel,
                                                    Published = entryPoint.Published
                                                });

                                                consolidatedUsers.AddRange(this.AddUsersInPath(nextUserInPath, usersInPath));
                                            }
                                            else
                                            {
                                                //With reporting to but not assign to a user till the end of the path
                                                consolidatedUsers.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                                {
                                                    Id = entryPoint.Id,
                                                    UserId = entryPoint.UserId,
                                                    FirstName = entryPoint.User.FirstName,
                                                    LastName = entryPoint.User.LastName,
                                                    FullName = entryPoint.User.FirstName + " " + entryPoint.User.LastName,
                                                    UserOrganizationalStructureId = entryPoint.OrganizationalStructureId,
                                                    UserOrganizationalStructure = entryPoint.OrganizationalStructure,
                                                    UserOrganizationEntityId = entryPoint.OrganizationEntityId,
                                                    UserOrganizationEntityName = entryPoint.OrganizationEntityName,
                                                    UserApprovalLevelDetailId = entryPoint.ApprovalLevelDetailId,
                                                    UserApprovalLevel = entryPoint.ApprovalLevel,
                                                    ReportingToId = 0,
                                                    ReportingToUserId = "",
                                                    ReportingToFirstName = "",
                                                    ReportingToLastName = "",
                                                    ReportingToFullName = "",
                                                    ReportingToOrganizationalStructureId = 0,
                                                    ReportingToOrganizationalStructure = "",
                                                    ReportingToOrganizationEntityId = 0,
                                                    ReportingToOrganizationEntityName = "",
                                                    ReportingToApprovalLevelDetailId = 0,
                                                    ReportingToApprovalLevel = null,
                                                    Published = entryPoint.Published
                                                });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    consolidatedUsers.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                    {
                                        Id = entryPoint.Id,
                                        UserId = entryPoint.UserId,
                                        FirstName = entryPoint.User.FirstName,
                                        LastName = entryPoint.User.LastName,
                                        FullName = entryPoint.User.FirstName + " " + entryPoint.User.LastName,
                                        UserOrganizationalStructureId = entryPoint.OrganizationalStructureId,
                                        UserOrganizationalStructure = entryPoint.OrganizationalStructure,
                                        UserOrganizationEntityId = entryPoint.OrganizationEntityId,
                                        UserOrganizationEntityName = entryPoint.OrganizationEntityName,
                                        UserApprovalLevelDetailId = entryPoint.ApprovalLevelDetailId,
                                        UserApprovalLevel = entryPoint.ApprovalLevel,
                                        ReportingToId = 0,
                                        ReportingToUserId = "",
                                        ReportingToFirstName = "",
                                        ReportingToLastName = "",
                                        ReportingToFullName = "",
                                        ReportingToOrganizationalStructureId = 0,
                                        ReportingToOrganizationalStructure = "",
                                        ReportingToOrganizationEntityId = 0,
                                        ReportingToOrganizationEntityName = "",
                                        ReportingToApprovalLevelDetailId = 0,
                                        ReportingToApprovalLevel = null,
                                        Published = entryPoint.Published
                                    });
                                }
                            }
                            else
                            {
                                consolidatedUsers.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                {
                                    Id = entryPoint.Id,
                                    UserId = entryPoint.UserId,
                                    FirstName = entryPoint.User.FirstName,
                                    LastName = entryPoint.User.LastName,
                                    FullName = entryPoint.User.FirstName + " " + entryPoint.User.LastName,
                                    UserOrganizationalStructureId = entryPoint.OrganizationalStructureId,
                                    UserOrganizationalStructure = entryPoint.OrganizationalStructure,
                                    UserOrganizationEntityId = entryPoint.OrganizationEntityId,
                                    UserOrganizationEntityName = entryPoint.OrganizationEntityName,
                                    UserApprovalLevelDetailId = entryPoint.ApprovalLevelDetailId,
                                    UserApprovalLevel = entryPoint.ApprovalLevel,
                                    ReportingToId = 0,
                                    ReportingToUserId = "",
                                    ReportingToFirstName = "",
                                    ReportingToLastName = "",
                                    ReportingToFullName = "",
                                    ReportingToOrganizationalStructureId = 0,
                                    ReportingToOrganizationalStructure = "",
                                    ReportingToOrganizationEntityId = 0,
                                    ReportingToOrganizationEntityName = "",
                                    ReportingToApprovalLevelDetailId = 0,
                                    ReportingToApprovalLevel = null,
                                    Published = entryPoint.Published
                                });
                            }
                        }
                    }

                    var uniqueConsolidatedUsers = consolidatedUsers
                                                  .GroupBy
                                                  (
                                                      c => new
                                                      {
                                                          c.Id,
                                                          c.UserId,
                                                          c.FirstName,
                                                          c.LastName,
                                                          c.FullName,
                                                          c.UserOrganizationalStructureId,
                                                          c.UserOrganizationalStructure,
                                                          c.UserOrganizationEntityId,
                                                          c.UserOrganizationEntityName,
                                                          c.UserApprovalLevelDetailId,
                                                          c.ReportingToId,
                                                          c.ReportingToUserId,
                                                          c.ReportingToFirstName,
                                                          c.ReportingToLastName,
                                                          c.ReportingToFullName,
                                                          c.ReportingToOrganizationalStructureId,
                                                          c.ReportingToOrganizationalStructure,
                                                          c.ReportingToOrganizationEntityId,
                                                          c.ReportingToOrganizationEntityName,
                                                          c.ReportingToApprovalLevelDetailId,
                                                          c.Published
                                                      }
                                                  ).Select
                                                  (
                                                      c => c.First()
                                                  ).ToList();

                    var approvalLevelTopLevels = new List<UserWithReportingToOrganizationalStructureViewModel>();

                    if (userOrganizationalStructureId > 0)
                    {
                        var userConsolidated = new List<UserWithReportingToOrganizationalStructureViewModel>();

                        userConsolidated.Add(uniqueConsolidatedUsers.FirstOrDefault(u => u.Id == userOrganizationalStructureId));
                        //Get Child
                        userConsolidated.AddRange(this.GetUserChild(userOrganizationalStructureId, uniqueConsolidatedUsers));

                        var userReportingToId = uniqueConsolidatedUsers.FirstOrDefault(u => u.Id == userOrganizationalStructureId).ReportingToId;
                        //Get Parent
                        userConsolidated.AddRange(this.GetUserReportingTo(userReportingToId, uniqueConsolidatedUsers));

                        uniqueConsolidatedUsers = userConsolidated;
                    }
                    
                    approvalLevelTopLevels = uniqueConsolidatedUsers
                                        .Where(c => c.ReportingToApprovalLevelDetailId == 0)
                                        .ToList();

                    foreach (var approvalLevelTopLevel in approvalLevelTopLevels)
                    {
                        ApprovalLevelStructureViewModel approvalLevelStructure = new ApprovalLevelStructureViewModel();

                        string webClass = "";

                        var organizationEntity = await _dbCntxt.OrganizationEntities.FirstOrDefaultAsync(o => o.Id == approvalLevelTopLevel.UserOrganizationEntityId);

                        if (organizationEntity != null)
                            webClass = organizationEntity.WebClass;

                        approvalLevelStructure.StyleClass = webClass + " text-white fs-nano";
                        approvalLevelStructure.Type = "person";

                        string imagePath = "";

                        var userProfile = await _dbCntxt.AspNetUsersProfile.FirstOrDefaultAsync(u => u.Id == approvalLevelTopLevel.UserId);

                        if (userProfile != null)
                            imagePath = userProfile.Photo;

                        approvalLevelStructure.Data = new ApprovalLevelStructureDataViewModel
                        {
                            OrganizationalStructureId = approvalLevelTopLevel.UserOrganizationalStructureId,
                            UserOrganizationalStructureId = approvalLevelTopLevel.Id,
                            Image = imagePath,
                            Name = approvalLevelTopLevel.FullName,
                            Title = approvalLevelTopLevel.UserApprovalLevel.ApprovalLevelAndSequence,
                            OrganizationalStructure = approvalLevelTopLevel.UserOrganizationalStructure,
                            Entity = approvalLevelTopLevel.UserOrganizationEntityName
                        };

                        int parentId = approvalLevelTopLevel.Id;

                        approvalLevelStructure.Children = await this.GetApprovalLevelStructuresChildren(parentId, uniqueConsolidatedUsers);

                        if (approvalLevelStructure.Children.Count > 0)
                            approvalLevelStructure.Expanded = true;

                        approvalLevelStructures.Add(approvalLevelStructure);
                    }
                }

                return approvalLevelStructures;
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

        public async Task<List<ApprovalLevelStructureViewModel>> GetApprovalLevelStructuresChildren(int parentId, List<UserWithReportingToOrganizationalStructureViewModel> consolidatedUsers)
        {
            List<ApprovalLevelStructureViewModel> approvalLevelStructures = new List<ApprovalLevelStructureViewModel>();

            var approvalLevelStructureChildren = consolidatedUsers
                                                 .Where(o => o.ReportingToId == parentId);

            if (approvalLevelStructureChildren.Count() > 0)
            {
                foreach (var child in approvalLevelStructureChildren)
                {
                    ApprovalLevelStructureViewModel approvalLevelStructure = new ApprovalLevelStructureViewModel();

                    string webClass = "";

                    var organizationEntity = await _dbCntxt.OrganizationEntities.FirstOrDefaultAsync(o => o.Id == child.UserOrganizationEntityId);

                    if (organizationEntity != null)
                        webClass = organizationEntity.WebClass;

                    approvalLevelStructure.StyleClass = webClass + " text-white fs-nano";
                    approvalLevelStructure.Type = "person";

                    string imagePath = "";

                    var userProfile = await _dbCntxt.AspNetUsersProfile.FirstOrDefaultAsync(u => u.Id == child.UserId);

                    if (userProfile != null)
                        imagePath = userProfile.Photo;

                    approvalLevelStructure.Data = new ApprovalLevelStructureDataViewModel
                    {
                        OrganizationalStructureId = child.UserOrganizationalStructureId,
                        UserOrganizationalStructureId = child.Id,
                        Image = imagePath,
                        Name = child.FullName,
                        Title = child.UserApprovalLevel.ApprovalLevelAndSequence,
                        OrganizationalStructure = child.UserOrganizationalStructure,
                        Entity = child.UserOrganizationEntityName
                    };

                    int childId = child.Id;

                    approvalLevelStructure.Children = await this.GetApprovalLevelStructuresChildren(childId, consolidatedUsers);

                    if (approvalLevelStructure.Children.Count > 0)
                        approvalLevelStructure.Expanded = true;

                    approvalLevelStructures.Add(approvalLevelStructure);
                }
            }

            return approvalLevelStructures;
        }

        public int GetApprovalDetailRowNumber(int id, List<ApprovalLevelDetailViewModel> details)
        {
            int rowNumber = 0;
            try
            {
                var approvalLevelDetail = details
                                        .FirstOrDefault
                                        (
                                            a => a.Id == id
                                        );

                if (approvalLevelDetail != null)
                    rowNumber = approvalLevelDetail.RowNumber;

                return rowNumber;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private bool CheckApprovalLevelUsedAsReportingTo(List<OrganizationalStructureWithUserViewModel> orgWithUsers, OrganizationalStructureWithUserViewModel user, int mainApprovalLevelDetailId, int mainApprovalLevelDetailRowNumber)
        {
            bool returnValue = false;
            bool isUserApprovalLevelDetailIdIsUsed = false;

            if (user != null)
            {
                if (user.ReportingTo > 0)
                {
                    var nextUsersInPath = orgWithUsers
                                    .Where
                                    (
                                        u =>
                                        u.ApprovalLevelDetailRowNumber > user.ApprovalLevelDetailRowNumber
                                    )
                                    .OrderBy(u => u.ApprovalLevelDetailRowNumber)
                                    .ToList();

                    if (user.ReportingTo == 9999)
                    {
                        var nextUserInPath = nextUsersInPath.FirstOrDefault();

                        if (nextUserInPath != null)
                        {
                            if (nextUserInPath.ApprovalLevelDetailId == mainApprovalLevelDetailId)
                                return true;

                            if (nextUserInPath.ReportingTo == mainApprovalLevelDetailId)
                                return true;

                            if (nextUserInPath.ReportingToRowNumber > mainApprovalLevelDetailRowNumber)
                                return false;

                            isUserApprovalLevelDetailIdIsUsed = this.CheckApprovalLevelUsedAsReportingTo(orgWithUsers, nextUserInPath, mainApprovalLevelDetailId, mainApprovalLevelDetailRowNumber);
                        }
                    }
                    else
                    {
                        var nextUserInPath = nextUsersInPath
                                            .Where
                                            (
                                                u =>
                                                u.ApprovalLevelDetailId == user.ReportingTo
                                            )
                                            .FirstOrDefault();

                        if (nextUserInPath != null)
                        {
                            if (nextUserInPath.ApprovalLevelDetailId == mainApprovalLevelDetailId)
                                return true;

                            if (nextUserInPath.ReportingTo == mainApprovalLevelDetailId)
                                return true;

                            if (nextUserInPath.ReportingToRowNumber > mainApprovalLevelDetailRowNumber)
                                return false;

                            isUserApprovalLevelDetailIdIsUsed = this.CheckApprovalLevelUsedAsReportingTo(orgWithUsers, nextUserInPath, mainApprovalLevelDetailId, mainApprovalLevelDetailRowNumber);
                        }
                        else
                        {
                            nextUserInPath = nextUsersInPath
                                             .Where
                                             (
                                                u =>
                                                u.ApprovalLevelDetailRowNumber > user.ReportingToRowNumber
                                             )
                                             .FirstOrDefault();

                            if (nextUserInPath != null)
                            {
                                if (nextUserInPath.ApprovalLevelDetailId == mainApprovalLevelDetailId)
                                    return true;

                                if (nextUserInPath.ReportingTo == mainApprovalLevelDetailId)
                                    return true;

                                if (nextUserInPath.ReportingToRowNumber > mainApprovalLevelDetailRowNumber)
                                    return false;

                                isUserApprovalLevelDetailIdIsUsed = this.CheckApprovalLevelUsedAsReportingTo(orgWithUsers, nextUserInPath, mainApprovalLevelDetailId, mainApprovalLevelDetailRowNumber);
                            }
                        }
                    }
                }
            }

            returnValue = isUserApprovalLevelDetailIdIsUsed;

            return returnValue;
        }

        private List<OrganizationalStructureWithUserViewModel> AddUsersInOrgStructure(int orgStructureId, string orgStructure, int orgEntityId, string orgEntity, int orgEntityHierarchy, List<UserOrganizationalStructureViewModel> usersOrg, List<ApprovalLevelDetailViewModel> approvalLevelDetails)
        {
            List<OrganizationalStructureWithUserViewModel> users = new List<OrganizationalStructureWithUserViewModel>();

            if (usersOrg.Count > 0)
            {
                foreach (var orgUser in usersOrg)
                {
                    var orgWithUser = new OrganizationalStructureWithUserViewModel();
                    orgWithUser.Id = orgStructureId;
                    orgWithUser.Name = orgStructure;
                    orgWithUser.OrganizationEntityId = orgEntityId;
                    orgWithUser.OrganizationEntityName = orgEntity;
                    orgWithUser.OrganizationEntityHierarchy = orgEntityHierarchy;
                    orgWithUser.UserOrgStructureId = orgUser.Id;
                    orgWithUser.UserId = orgUser.UserId;
                    orgWithUser.FirstName = orgUser.User.FirstName;
                    orgWithUser.MiddleName = orgUser.User.MiddleName;
                    orgWithUser.LastName = orgUser.User.LastName;
                    orgWithUser.FullName = orgUser.User.FirstName + " " + orgUser.User.LastName;
                    orgWithUser.ApprovalLevelDetailId = orgUser.ApprovalLevelDetailId;
                    orgWithUser.ApprovalLevel = orgUser.ApprovalLevel;
                    orgWithUser.ApprovalLevelDetailRowNumber = this.GetApprovalDetailRowNumber(orgWithUser.ApprovalLevelDetailId, approvalLevelDetails);
                    orgWithUser.ReportingTo = orgUser.ReportingTo;
                    orgWithUser.ReportingToApprovalLevel = orgUser.ReportingToApprovalLevel;
                    orgWithUser.ReportingToRowNumber = this.GetApprovalDetailRowNumber(orgWithUser.ReportingTo, approvalLevelDetails);
                    orgWithUser.Published = orgUser.Published;

                    users.Add(orgWithUser);
                }
            }

            return users;
        }

        private List<UserWithReportingToOrganizationalStructureViewModel> AddUsersInPath(OrganizationalStructureWithUserViewModel userInPath, List<OrganizationalStructureWithUserViewModel> usersInPath)
        {
            List<UserWithReportingToOrganizationalStructureViewModel> usersToReturn = new List<UserWithReportingToOrganizationalStructureViewModel>();

            if (userInPath != null)
            {
                if (userInPath.ReportingTo > 0)
                {
                    var nextUsersInPath = usersInPath
                                   .Where(u => u.ApprovalLevelDetailRowNumber > userInPath.ApprovalLevelDetailRowNumber)
                                   .OrderBy(u => u.ApprovalLevelDetailRowNumber)
                                   .ToList();

                    if (userInPath.ReportingTo == 9999)
                    {
                        var nextUserInPath = nextUsersInPath.FirstOrDefault();

                        if (nextUserInPath != null)
                        {
                            usersToReturn.Add(new UserWithReportingToOrganizationalStructureViewModel()
                            {
                                Id = userInPath.UserOrgStructureId,
                                UserId = userInPath.UserId,
                                FirstName = userInPath.FirstName,
                                LastName = userInPath.LastName,
                                FullName = userInPath.FullName,
                                UserOrganizationalStructureId = userInPath.Id,
                                UserOrganizationalStructure = userInPath.Name,
                                UserOrganizationEntityId = userInPath.OrganizationEntityId,
                                UserOrganizationEntityName = userInPath.OrganizationEntityName,
                                UserApprovalLevelDetailId = userInPath.ApprovalLevelDetailId,
                                UserApprovalLevel = userInPath.ApprovalLevel,
                                ReportingToId = nextUserInPath.UserOrgStructureId,
                                ReportingToUserId = nextUserInPath.UserId,
                                ReportingToFirstName = nextUserInPath.FirstName,
                                ReportingToLastName = nextUserInPath.LastName,
                                ReportingToFullName = nextUserInPath.FullName,
                                ReportingToOrganizationalStructureId = nextUserInPath.Id,
                                ReportingToOrganizationalStructure = nextUserInPath.Name,
                                ReportingToOrganizationEntityId = nextUserInPath.OrganizationEntityId,
                                ReportingToOrganizationEntityName = nextUserInPath.OrganizationEntityName,
                                ReportingToApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                ReportingToApprovalLevel = nextUserInPath.ApprovalLevel,
                                Published = userInPath.Published
                            });

                            usersToReturn.AddRange(this.AddUsersInPath(nextUserInPath, usersInPath));
                        }
                        else
                        {
                            //With reporting to but not assign to a user till the end of the path
                            usersToReturn.Add(new UserWithReportingToOrganizationalStructureViewModel()
                            {
                                Id = userInPath.UserOrgStructureId,
                                UserId = userInPath.UserId,
                                FirstName = userInPath.FirstName,
                                LastName = userInPath.LastName,
                                FullName = userInPath.FullName,
                                UserOrganizationalStructureId = userInPath.Id,
                                UserOrganizationalStructure = userInPath.Name,
                                UserOrganizationEntityId = userInPath.OrganizationEntityId,
                                UserOrganizationEntityName = userInPath.OrganizationEntityName,
                                UserApprovalLevelDetailId = userInPath.ApprovalLevelDetailId,
                                UserApprovalLevel = userInPath.ApprovalLevel,
                                ReportingToId = 0,
                                ReportingToUserId = "",
                                ReportingToFirstName = "",
                                ReportingToLastName = "",
                                ReportingToFullName = "",
                                ReportingToOrganizationalStructureId = 0,
                                ReportingToOrganizationalStructure = "",
                                ReportingToOrganizationEntityId = 0,
                                ReportingToOrganizationEntityName = "",
                                ReportingToApprovalLevelDetailId = 0,
                                ReportingToApprovalLevel = null,
                                Published = userInPath.Published
                            });
                        }
                    }
                    else
                    {
                        var nextUserInPath = nextUsersInPath
                                             .Where
                                             (
                                                u =>
                                                u.ApprovalLevelDetailId == userInPath.ReportingTo
                                             )
                                             .FirstOrDefault();

                        if (nextUserInPath != null)
                        {
                            usersToReturn.Add(new UserWithReportingToOrganizationalStructureViewModel()
                            {
                                Id = userInPath.UserOrgStructureId,
                                UserId = userInPath.UserId,
                                FirstName = userInPath.FirstName,
                                LastName = userInPath.LastName,
                                FullName = userInPath.FullName,
                                UserOrganizationalStructureId = userInPath.Id,
                                UserOrganizationalStructure = userInPath.Name,
                                UserOrganizationEntityId = userInPath.OrganizationEntityId,
                                UserOrganizationEntityName = userInPath.OrganizationEntityName,
                                UserApprovalLevelDetailId = userInPath.ApprovalLevelDetailId,
                                UserApprovalLevel = userInPath.ApprovalLevel,
                                ReportingToId = nextUserInPath.UserOrgStructureId,
                                ReportingToUserId = nextUserInPath.UserId,
                                ReportingToFirstName = nextUserInPath.FirstName,
                                ReportingToLastName = nextUserInPath.LastName,
                                ReportingToFullName = nextUserInPath.FullName,
                                ReportingToOrganizationalStructureId = nextUserInPath.Id,
                                ReportingToOrganizationalStructure = nextUserInPath.Name,
                                ReportingToOrganizationEntityId = nextUserInPath.OrganizationEntityId,
                                ReportingToOrganizationEntityName = nextUserInPath.OrganizationEntityName,
                                ReportingToApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                ReportingToApprovalLevel = nextUserInPath.ApprovalLevel,
                                Published = userInPath.Published
                            });

                            usersToReturn.AddRange(this.AddUsersInPath(nextUserInPath, usersInPath));
                        }
                        else
                        {
                            nextUserInPath = nextUsersInPath
                                             .Where
                                             (
                                                u =>
                                                u.ApprovalLevelDetailRowNumber > userInPath.ReportingToRowNumber
                                             )
                                             .FirstOrDefault();

                            if (nextUserInPath != null)
                            {
                                usersToReturn.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                {
                                    Id = userInPath.UserOrgStructureId,
                                    UserId = userInPath.UserId,
                                    FirstName = userInPath.FirstName,
                                    LastName = userInPath.LastName,
                                    FullName = userInPath.FullName,
                                    UserOrganizationalStructureId = userInPath.Id,
                                    UserOrganizationalStructure = userInPath.Name,
                                    UserOrganizationEntityId = userInPath.OrganizationEntityId,
                                    UserOrganizationEntityName = userInPath.OrganizationEntityName,
                                    UserApprovalLevelDetailId = userInPath.ApprovalLevelDetailId,
                                    UserApprovalLevel = userInPath.ApprovalLevel,
                                    ReportingToId = nextUserInPath.UserOrgStructureId,
                                    ReportingToUserId = nextUserInPath.UserId,
                                    ReportingToFirstName = nextUserInPath.FirstName,
                                    ReportingToLastName = nextUserInPath.LastName,
                                    ReportingToFullName = nextUserInPath.FullName,
                                    ReportingToOrganizationalStructureId = nextUserInPath.Id,
                                    ReportingToOrganizationalStructure = nextUserInPath.Name,
                                    ReportingToOrganizationEntityId = nextUserInPath.OrganizationEntityId,
                                    ReportingToOrganizationEntityName = nextUserInPath.OrganizationEntityName,
                                    ReportingToApprovalLevelDetailId = nextUserInPath.ApprovalLevelDetailId,
                                    ReportingToApprovalLevel = nextUserInPath.ApprovalLevel,
                                    Published = userInPath.Published
                                });

                                usersToReturn.AddRange(this.AddUsersInPath(nextUserInPath, usersInPath));
                            }
                            else
                            {
                                //With reporting to but not assign to a user till the end of the path
                                usersToReturn.Add(new UserWithReportingToOrganizationalStructureViewModel()
                                {
                                    Id = userInPath.UserOrgStructureId,
                                    UserId = userInPath.UserId,
                                    FirstName = userInPath.FirstName,
                                    LastName = userInPath.LastName,
                                    FullName = userInPath.FullName,
                                    UserOrganizationalStructureId = userInPath.Id,
                                    UserOrganizationalStructure = userInPath.Name,
                                    UserOrganizationEntityId = userInPath.OrganizationEntityId,
                                    UserOrganizationEntityName = userInPath.OrganizationEntityName,
                                    UserApprovalLevelDetailId = userInPath.ApprovalLevelDetailId,
                                    UserApprovalLevel = userInPath.ApprovalLevel,
                                    ReportingToId = 0,
                                    ReportingToUserId = "",
                                    ReportingToFirstName = "",
                                    ReportingToLastName = "",
                                    ReportingToFullName = "",
                                    ReportingToOrganizationalStructureId = 0,
                                    ReportingToOrganizationalStructure = "",
                                    ReportingToOrganizationEntityId = 0,
                                    ReportingToOrganizationEntityName = "",
                                    ReportingToApprovalLevelDetailId = 0,
                                    ReportingToApprovalLevel = null,
                                    Published = userInPath.Published
                                });
                            }
                        }
                    }
                }
                else
                {
                    usersToReturn.Add(new UserWithReportingToOrganizationalStructureViewModel()
                    {
                        Id = userInPath.UserOrgStructureId,
                        UserId = userInPath.UserId,
                        FirstName = userInPath.FirstName,
                        LastName = userInPath.LastName,
                        FullName = userInPath.FullName,
                        UserOrganizationalStructureId = userInPath.Id,
                        UserOrganizationalStructure = userInPath.Name,
                        UserOrganizationEntityId = userInPath.OrganizationEntityId,
                        UserOrganizationEntityName = userInPath.OrganizationEntityName,
                        UserApprovalLevelDetailId = userInPath.ApprovalLevelDetailId,
                        UserApprovalLevel = userInPath.ApprovalLevel,
                        ReportingToId = 0,
                        ReportingToUserId = "",
                        ReportingToFirstName = "",
                        ReportingToLastName = "",
                        ReportingToFullName = "",
                        ReportingToOrganizationalStructureId = 0,
                        ReportingToOrganizationalStructure = "",
                        ReportingToOrganizationEntityId = 0,
                        ReportingToOrganizationEntityName = "",
                        ReportingToApprovalLevelDetailId = 0,
                        ReportingToApprovalLevel = null,
                        Published = userInPath.Published
                    });
                }
            }

            return usersToReturn;
        }


        private List<UserWithReportingToOrganizationalStructureViewModel> GetUserReportingTo(int reportingToId, List<UserWithReportingToOrganizationalStructureViewModel> users)
        {
            var returnUsers = new List<UserWithReportingToOrganizationalStructureViewModel>();
            var reportingTo = users.FirstOrDefault(u => u.Id == reportingToId);

            if (reportingTo != null)
            {
                returnUsers.Add(reportingTo);
                returnUsers.AddRange(GetUserReportingTo(reportingTo.ReportingToId, users));
            }

            return returnUsers;
        }

        private List<UserWithReportingToOrganizationalStructureViewModel> GetUserChild(int childId, List<UserWithReportingToOrganizationalStructureViewModel> users)
        {
            var returnUsers = new List<UserWithReportingToOrganizationalStructureViewModel>();
            var children = users.Where(u => u.ReportingToId == childId).ToList();

            if (children.Count > 0)
            {
                foreach (var child in children)
                {
                    returnUsers.Add(child);

                    returnUsers.AddRange(GetUserChild(child.Id, users));
                }
            }

            return returnUsers;
        }

        public async Task<IEnumerable<OrganizationalStructureWithUserViewModel>> GetOrganizationalStructurePathWithUser(int organizationalStructureId, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null, bool? approvalLevelPublished = null)
        {
            List<OrganizationalStructureWithUserViewModel> orgWithUsers = new List<OrganizationalStructureWithUserViewModel>();
            try
            {
                var approvalLevelDetails = (await this.GetApprovalLevelDetailsInSequentialMannerAsync(approvalLevelPublished, organizationEntityPublished))
                                            .ToList();

                var orgStructurePaths = (await _organizationalStructure
                                                .GetOrganizationalStructurePathAsync(organizationalStructureId, null, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                                .ToList();

                if (orgStructurePaths.Count > 0)
                {
                    foreach (var orgStructurePath in orgStructurePaths)
                    {
                        var staffs = orgStructurePath.UserOrganizationalStructures
                                            .Where
                                            (
                                                u => u.ApprovalLevelDetailId == 0
                                            ).ToList();

                        var approvers = orgStructurePath
                                        .UserOrganizationalStructures
                                        .Where
                                        (
                                            u => u.ApprovalLevelDetailId > 0
                                        )
                                        .OrderBy
                                        (
                                            u =>
                                            this.GetApprovalDetailRowNumber(u.ApprovalLevelDetailId, approvalLevelDetails)
                                        )
                                        .ToList();

                        var orgUsers = new List<UserOrganizationalStructureViewModel>();

                        orgUsers.AddRange(staffs);
                        orgUsers.AddRange(approvers);

                        orgWithUsers.AddRange(this.AddUsersInOrgStructure(orgStructurePath.Id, orgStructurePath.Name, orgStructurePath.OrganizationEntityId, orgStructurePath.OrganizationEntityName, orgStructurePath.OrganizationEntityHierarchy, orgUsers, approvalLevelDetails));
                    }
                }

                return orgWithUsers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<ApprovalLevelReportingToViewModel>> GetApprovalLevelReportingTo(int approvalLevelDetailId, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            List<ApprovalLevelReportingToViewModel> reportingTo = new List<ApprovalLevelReportingToViewModel>();
            try
            {
                var approvalLevelDetails = (await this.GetApprovalLevelDetailsInSequentialMannerAsync
                                            (approvalLevelPublished, organizationEntityPublished))
                                            .ToList();

                reportingTo.Add(new ApprovalLevelReportingToViewModel
                {
                    Id = 0,
                    Name = "Not Applicable",
                    OrganizationEntityId = 0,
                    OrganizationEntityName = "",
                    OrganizationEntityHierarchy = 0,
                    User = null
                });

                if (approvalLevelDetailId > 0)
                {
                    var approvalLevelDetail = approvalLevelDetails
                                          .FirstOrDefault(a => a.Id == approvalLevelDetailId);

                    if (approvalLevelDetail != null)
                    {
                        var approvalLevelDetailRowNumber = approvalLevelDetail.RowNumber;

                        var approvalLevelGroupings = approvalLevelDetails
                                                 .Where
                                                 (
                                                    a =>
                                                    a.RowNumber > approvalLevelDetailRowNumber
                                                 )
                                                 .GroupBy
                                                 (
                                                    a => new 
                                                    {
                                                        a.ApprovalLevelId,
                                                        a.ApprovalLevel,
                                                        a.OrganizationEntityId,
                                                        a.OrganizationEntityName,
                                                        a.OrganizationEntityHierarchy
                                                    }
                                                 )
                                                 .Select(a => a.First())
                                                 .ToList();

                        reportingTo.AddRange(approvalLevelGroupings
                                      .Select
                                      (
                                        a => new ApprovalLevelReportingToViewModel()
                                        {
                                            Id = a.ApprovalLevelId,
                                            Name = a.ApprovalLevel,
                                            OrganizationEntityId = a.OrganizationEntityId,
                                            OrganizationEntityName = a.OrganizationEntityName,
                                            OrganizationEntityHierarchy = a.OrganizationEntityHierarchy,
                                            User = null,
                                            UserApprovalLevelDetail = null
                                        }
                                      ).ToList());

                        if (approvalLevelGroupings.Count > 0)
                        {
                            var reportingToNextInLine = await this.GetSpecificApprovalLevelReportingToAsync(approvalLevelDetailId, 9999, organizationalStructureId, approvalLevelPublished, organizationalStructurePublished, organizationEntityPublished, userPublished);

                            //Add 9999
                            reportingTo.Add(new ApprovalLevelReportingToViewModel
                            {
                                Id = 9999,
                                Name = "Next In Line",
                                OrganizationEntityId = 0,
                                OrganizationEntityName = "",
                                OrganizationEntityHierarchy = 0,
                                User = reportingToNextInLine != null ?
                                       new UserBasicInfoModel()
                                       {
                                           Id = reportingToNextInLine.UserId,
                                           FirstName = reportingToNextInLine.User.FirstName,
                                           LastName = reportingToNextInLine.User.LastName,
                                           MiddleName = reportingToNextInLine.User.MiddleName,
                                           FullName = reportingToNextInLine.User.FullName
                                       }
                                       : null,
                                UserApprovalLevelDetail = reportingToNextInLine != null ?
                                       reportingToNextInLine.ApprovalLevel
                                       : null
                            });

                        }
                    }
                }
                else
                {
                    var organizationalStructure = await _organizationalStructure
                                              .GetOrganizationalStructures(null, organizationalStructurePublished, organizationEntityPublished)
                                              .FirstOrDefaultAsync(o => o.Id == organizationalStructureId);

                    int organizationEntityId = organizationalStructure == null ? 0 : organizationalStructure.OrganizationEntityId;

                    var organizationEntity = await _organizationalStructure
                                                  .GetOrgranizationEntities(null, organizationEntityPublished)
                                                  .FirstOrDefaultAsync(o => o.Id == organizationEntityId);

                    int organizationEntityHierarchy = organizationEntity == null ? 0 : organizationEntity.Hierarchy;

                    var approvalLevels = await this
                                    .GetApprovalLevels(organizationEntityPublished)
                                    .Where
                                    (
                                        a =>
                                        a.OrganizationEntityHierarchy <= organizationEntityHierarchy
                                    )
                                    .Select
                                    (
                                        a => new ApprovalLevelReportingToViewModel()
                                        {
                                            Id = a.Id,
                                            Name = a.Name,
                                            OrganizationEntityId = a.OrganizationEntityId,
                                            OrganizationEntityName = a.OrganizationEntityName,
                                            OrganizationEntityHierarchy = a.OrganizationEntityHierarchy,
                                            User = null,
                                            UserApprovalLevelDetail = null
                                        }
                                    ).ToListAsync();

                    reportingTo.AddRange(approvalLevels);

                    if (approvalLevels.Count > 0)
                    {
                        var reportingToNextInLine = await this.GetSpecificApprovalLevelReportingToAsync(approvalLevelDetailId, 9999, organizationalStructureId, approvalLevelPublished, organizationalStructurePublished, organizationEntityPublished, userPublished);

                        //Add 9999
                        reportingTo.Add(new ApprovalLevelReportingToViewModel
                        {
                            Id = 9999,
                            Name = "Next In Line",
                            OrganizationEntityId = 0,
                            OrganizationEntityName = "",
                            OrganizationEntityHierarchy = 0,
                            User = reportingToNextInLine != null ?
                                       new UserBasicInfoModel()
                                       {
                                           Id = reportingToNextInLine.UserId,
                                           FirstName = reportingToNextInLine.User.FirstName,
                                           LastName = reportingToNextInLine.User.LastName,
                                           MiddleName = reportingToNextInLine.User.MiddleName,
                                           FullName = reportingToNextInLine.User.FullName
                                       }
                                       : null,
                            UserApprovalLevelDetail = reportingToNextInLine != null ?
                                       reportingToNextInLine.ApprovalLevel
                                       : null
                        });
                    }
                }

                return reportingTo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<UserOrganizationalStructureViewModel> GetSpecificApprovalLevelReportingToAsync(int approvalLevelDetailId, int reportingTo, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            UserOrganizationalStructureViewModel userOrg = new UserOrganizationalStructureViewModel();
            try
            {
                var approvalLevelDetails = (await this.GetApprovalLevelDetailsInSequentialMannerAsync(approvalLevelPublished, organizationEntityPublished))
                                            .ToList();

                var approvalLevelDetail = approvalLevelDetails.FirstOrDefault(a => a.Id == approvalLevelDetailId);

                int approvalLevelDetailRowNumber = approvalLevelDetail != null ? approvalLevelDetail.RowNumber : 0;

                var orgStructurePaths = (await _organizationalStructure
                                                .GetOrganizationalStructurePathAsync(organizationalStructureId, null, organizationalStructurePublished, organizationEntityPublished, userPublished))
                                                .ToList();
                List<OrganizationalStructureWithUserViewModel> orgWithUsers = new List<OrganizationalStructureWithUserViewModel>();
                if (orgStructurePaths.Count > 0)
                {
                    foreach (var orgStructurePath in orgStructurePaths)
                    {
                        var staffs = orgStructurePath.UserOrganizationalStructures
                                            .Where
                                            (
                                                u => u.ApprovalLevelDetailId == 0
                                            ).ToList();

                        var approvers = orgStructurePath
                                        .UserOrganizationalStructures
                                        .Where
                                        (
                                            u => u.ApprovalLevelDetailId > 0
                                        )
                                        .OrderBy
                                        (
                                            u =>
                                            this.GetApprovalDetailRowNumber(u.ApprovalLevelDetailId, approvalLevelDetails)
                                        )
                                        .ToList();

                        var orgUsers = new List<UserOrganizationalStructureViewModel>();

                        orgUsers.AddRange(staffs);
                        orgUsers.AddRange(approvers);

                        orgWithUsers.AddRange(this.AddUsersInOrgStructure(orgStructurePath.Id, orgStructurePath.Name, orgStructurePath.OrganizationEntityId, orgStructurePath.OrganizationEntityName, orgStructurePath.OrganizationEntityHierarchy, orgUsers, approvalLevelDetails));
                    }


                    if (reportingTo > 0)
                    {
                        if (reportingTo == 9999)
                        {
                            userOrg = orgWithUsers
                                        .Where
                                        (
                                            o => o.ApprovalLevelDetailRowNumber > approvalLevelDetailRowNumber &&
                                            approvalLevelDetails.Select(a => a.Id).Contains(o.ApprovalLevelDetailId)
                                        )
                                        .Select
                                        (
                                            o => new UserOrganizationalStructureViewModel()
                                            {
                                                Id = o.UserOrgStructureId,
                                                UserId = o.UserId,
                                                User = new UserRegisterModel()
                                                {
                                                    Id = o.UserId,
                                                    FirstName = o.FirstName,
                                                    LastName = o.LastName,
                                                    MiddleName = o.MiddleName,
                                                    FullName = o.FullName
                                                },
                                                OrganizationalStructureId = o.Id,
                                                OrganizationalStructure = o.Name,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                ApprovalLevelDetailId = o.ApprovalLevelDetailId,
                                                ApprovalLevel = o.ApprovalLevel,
                                                ReportingTo = o.ReportingTo,
                                                ReportingToApprovalLevel = o.ReportingToApprovalLevel
                                            }
                                        )
                                        .FirstOrDefault();
                        }
                        else
                        {
                            userOrg = orgWithUsers
                                        .Where
                                        (
                                            o => o.ApprovalLevelDetailId == reportingTo &&
                                            approvalLevelDetails.Select(a => a.Id).Contains(o.ApprovalLevelDetailId)
                                        )
                                        .Select
                                        (
                                            o => new UserOrganizationalStructureViewModel()
                                            {
                                                Id = o.UserOrgStructureId,
                                                UserId = o.UserId,
                                                User = new UserRegisterModel()
                                                {
                                                    Id = o.UserId,
                                                    FirstName = o.FirstName,
                                                    LastName = o.LastName,
                                                    MiddleName = o.MiddleName,
                                                    FullName = o.FullName
                                                },
                                                OrganizationalStructureId = o.Id,
                                                OrganizationalStructure = o.Name,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                ApprovalLevelDetailId = o.ApprovalLevelDetailId,
                                                ApprovalLevel = o.ApprovalLevel,
                                                ReportingTo = o.ReportingTo,
                                                ReportingToApprovalLevel = o.ReportingToApprovalLevel
                                            }
                                        )
                                        .FirstOrDefault();
                        }
                    }
                    else
                        userOrg = null;
                }

                return userOrg;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public UserOrganizationalStructureViewModel GetSpecificApprovalLevelReportingTo(int approvalLevelDetailId, int reportingTo, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            UserOrganizationalStructureViewModel userOrg = new UserOrganizationalStructureViewModel();
            try
            {
                var approvalLevelDetails = this.GetApprovalLevelDetailsInSequentialManner(approvalLevelPublished, organizationEntityPublished)
                                               .ToList();

                var approvalLevelDetail = approvalLevelDetails.FirstOrDefault(a => a.Id == approvalLevelDetailId);

                int approvalLevelDetailRowNumber = approvalLevelDetail != null ? approvalLevelDetail.RowNumber : 0;

                var orgStructurePaths = _organizationalStructure
                                                .GetOrganizationalStructurePath(organizationalStructureId, null, organizationalStructurePublished, organizationEntityPublished, userPublished)
                                                .ToList();

                List<OrganizationalStructureWithUserViewModel> orgWithUsers = new List<OrganizationalStructureWithUserViewModel>();
                if (orgStructurePaths.Count > 0)
                {
                    foreach (var orgStructurePath in orgStructurePaths)
                    {
                        var staffs = orgStructurePath.UserOrganizationalStructures
                                            .Where
                                            (
                                                u => u.ApprovalLevelDetailId == 0
                                            ).ToList();

                        var approvers = orgStructurePath
                                        .UserOrganizationalStructures
                                        .Where
                                        (
                                            u => u.ApprovalLevelDetailId > 0
                                        )
                                        .OrderBy
                                        (
                                            u =>
                                            this.GetApprovalDetailRowNumber(u.ApprovalLevelDetailId, approvalLevelDetails)
                                        )
                                        .ToList();

                        var orgUsers = new List<UserOrganizationalStructureViewModel>();

                        orgUsers.AddRange(staffs);
                        orgUsers.AddRange(approvers);

                        orgWithUsers.AddRange(this.AddUsersInOrgStructure(orgStructurePath.Id, orgStructurePath.Name, orgStructurePath.OrganizationEntityId, orgStructurePath.OrganizationEntityName, orgStructurePath.OrganizationEntityHierarchy, orgUsers, approvalLevelDetails));
                    }


                    if (reportingTo > 0)
                    {
                        if (reportingTo == 9999)
                        {
                            userOrg = orgWithUsers
                                        .Where
                                        (
                                            o => o.ApprovalLevelDetailRowNumber > approvalLevelDetailRowNumber &&
                                            approvalLevelDetails.Select(a => a.Id).Contains(o.ApprovalLevelDetailId)
                                        )
                                        .Select
                                        (
                                            o => new UserOrganizationalStructureViewModel()
                                            {
                                                Id = o.UserOrgStructureId,
                                                UserId = o.UserId,
                                                User = new UserRegisterModel()
                                                {
                                                    Id = o.UserId,
                                                    FirstName = o.FirstName,
                                                    LastName = o.LastName,
                                                    MiddleName = o.MiddleName,
                                                    FullName = o.FullName
                                                },
                                                OrganizationalStructureId = o.Id,
                                                OrganizationalStructure = o.Name,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                ApprovalLevelDetailId = o.ApprovalLevelDetailId,
                                                ApprovalLevel = o.ApprovalLevel,
                                                ReportingTo = o.ReportingTo,
                                                ReportingToApprovalLevel = o.ReportingToApprovalLevel
                                            }
                                        )
                                        .FirstOrDefault();
                        }
                        else
                        {
                            userOrg = orgWithUsers
                                        .Where
                                        (
                                            o => o.ApprovalLevelDetailId == reportingTo &&
                                            approvalLevelDetails.Select(a => a.Id).Contains(o.ApprovalLevelDetailId)
                                        )
                                        .Select
                                        (
                                            o => new UserOrganizationalStructureViewModel()
                                            {
                                                Id = o.UserOrgStructureId,
                                                UserId = o.UserId,
                                                User = new UserRegisterModel()
                                                {
                                                    Id = o.UserId,
                                                    FirstName = o.FirstName,
                                                    LastName = o.LastName,
                                                    MiddleName = o.MiddleName,
                                                    FullName = o.FullName
                                                },
                                                OrganizationalStructureId = o.Id,
                                                OrganizationalStructure = o.Name,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                ApprovalLevelDetailId = o.ApprovalLevelDetailId,
                                                ApprovalLevel = o.ApprovalLevel,
                                                ReportingTo = o.ReportingTo,
                                                ReportingToApprovalLevel = o.ReportingToApprovalLevel
                                            }
                                        )
                                        .FirstOrDefault();
                        }
                    }
                    else
                        userOrg = null;
                }

                return userOrg;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public List<ApprovalLevelDetailViewModel> GetApprovalLevelDetailsInSequentialManner(bool? published = null, bool? organizationEntityPublished = null)
        {
            try
            {
                List<ApprovalLevelDetailViewModel> query = null;
                query =  this.FindAll()
                             .ToList();

                if (published != null)
                    query = query.Where(a => a.Published == published).ToList();

                if (organizationEntityPublished != null)
                    query = query.Where(a => a.Published == organizationEntityPublished).ToList();

                query = query.OrderByDescending(a => a.OrganizationEntityHierarchy)
                             .ThenByDescending(a => a.Sequence).ToList();

                int ctr = 1;
                foreach (var detail in query)
                {
                    detail.RowNumber = ctr;
                    ctr++;
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}