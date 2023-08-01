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
    public class OrganizationalStructureRepository : BaseApi, IOrganizationalStructure
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public OrganizationalStructureRepository(
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

        public async Task<OrganizationalStructureViewModel> Add(OrganizationalStructureViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.ParentId < 0)
                throw new CustomException("Parent Entity is required.", 400);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            if (model.OrganizationEntityId <= 0)
                throw new CustomException("Organization Entity is required.", 400);
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var organizationalEntity = await _dbCntxt.OrganizationEntities
                                                   .FirstOrDefaultAsync(o => o.Id == model.OrganizationEntityId);

                if (organizationalEntity == null)
                    throw new CustomException("Entry for organization entity is not found.", 404);

                //Check if child has higher or equal to parent hierarchy
                if (model.ParentId > 0)
                {
                    var parentOrg = await this.GetOrganizationalStructures(null)
                                              .FirstOrDefaultAsync(o => o.Id == model.ParentId);

                    if (parentOrg == null)
                        throw new CustomException("Specified parent is not found", 404);

                    var childEntity = await this.GetOrgranizationEntities(null)
                                                .FirstOrDefaultAsync(o => o.Id == model.OrganizationEntityId);

                    if (parentOrg.OrganizationEntityHierarchy >= childEntity.Hierarchy)
                        throw new CustomException("Entity: " + childEntity.Name + " cannot be added as child of " + parentOrg.Name + " due to hierarchical sequence.", 400);
                }

                //Check duplicate in Parent
                var checkDuplicateNameInParent = await _dbCntxt.OrganizationalStructures
                                                 .Where
                                                 (
                                                    o =>
                                                    o.ParentId == model.ParentId &&
                                                    o.OrganizationEntityId == model.OrganizationEntityId &&
                                                    o.Name == model.Name
                                                 ).FirstOrDefaultAsync();

                if (checkDuplicateNameInParent != null)
                {
                    var organizationEntity = await this.GetOrgranizationEntities()
                                             .FirstOrDefaultAsync(o => o.Id == model.OrganizationEntityId);

                    throw new CustomException("Name: '" + model.Name + "' as " + organizationEntity.Name + " is already existing within desired parent.", 400);
                }
                    

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                await _userIpAddressPerSession.Save(userIp);

                //Insert organizational structures
                var organizationalStructures = new OrganizationalStructure()
                {
                    ParentId = model.ParentId,
                    OrganizationEntityId = model.OrganizationEntityId,
                    Name = model.Name,
                    Description = model.Description,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.OrganizationalStructures.Add(organizationalStructures);
                await _dbCntxt.SaveChangesAsync();

                model.Id = organizationalStructures.Id;
                model.CreatedByPK = organizationalStructures.CreatedByPK;
                model.CreatedDate = organizationalStructures.CreatedDate;
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

                var organizationalStructure = await _dbCntxt.OrganizationalStructures.FindAsync(id);

                if (organizationalStructure == null)
                    throw new CustomException("Organization Structure to delete is not found. ", 404);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                //Check if used in organization structures
                var organizationalStructures = await _dbCntxt.OrganizationalStructures.Where(o => o.ParentId == id).ToListAsync();

                if (organizationalStructures.Count > 0)
                    throw new CustomException("Cannot delete this organization structure due to connectivity within organizational structures. ", 400);

                _dbCntxt.OrganizationalStructures.Remove(organizationalStructure);
                await _dbCntxt.SaveChangesAsync();

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

        public async Task DeleteAll(OrganizationalStructureViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteOrganizationEntity(int id)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {

                var organizationEntity = await _dbCntxt.OrganizationEntities.FindAsync(id);

                if (organizationEntity == null)
                    throw new CustomException("Organization Entity to delete is not found. ", 404);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                //Check if used in organization structures
                var organizationalStructures = await _dbCntxt.OrganizationalStructures.Where(o => o.OrganizationEntityId == id).ToListAsync();

                if (organizationalStructures.Count > 0)
                    throw new CustomException("Cannot delete this organization entity due to connectivity within organizational structures. ", 400);

                _dbCntxt.OrganizationEntities.Remove(organizationEntity);
                await _dbCntxt.SaveChangesAsync();

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

        public IQueryable<OrganizationalStructureViewModel> Find(object obj)
        {
            try
            {
                var orgStructure = this.Get().AsQueryable();

                IQueryable<OrganizationalStructureViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where
                            (
                                o =>
                                o.Id == id
                            )
                            .Select
                            (
                                o => new OrganizationalStructureViewModel
                                {
                                    Id = o.Id,
                                    Name = o.Name,
                                    Description = String.IsNullOrWhiteSpace(o.Description) ? "" : o.Description,
                                    OrganizationEntityId = o.OrganizationEntityId,
                                    OrganizationEntityName = o.OrganizationEntity.Name,
                                    OrganizationEntityHierarchy = o.OrganizationEntity.Hierarchy,
                                    OrganizationEntityPublished = o.OrganizationEntity.Published,
                                    ParentId = o.ParentId,
                                    Parent = o.ParentId > 0 ? orgStructure.Where(or => or.Id == o.ParentId).Select(or => or.Name).FirstOrDefault() : "",
                                    ParentOrganizationEntityName = o.ParentId > 0 ? orgStructure.Where(or => or.Id == o.ParentId).Select(or => or.OrganizationEntity.Name).FirstOrDefault() : "",
                                    ParentOrganizationEntityHierarchy = o.ParentId > 0 ? orgStructure.Where(or => or.Id == o.ParentId).Select(or => or.OrganizationEntity.Hierarchy).FirstOrDefault() : 0,
                                    Published = o.Published,
                                    CreatedByPK = o.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == o.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == o.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : o.CreatedByPK,
                                    CreatedDate = o.CreatedDate,
                                    ModifiedByPK = o.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == o.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == o.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : o.ModifiedByPK,
                                    ModifiedDate = o.ModifiedDate,
                                    UserOrganizationalStructures = o.UserOrganizationalStructures
                                                                    .Select
                                                                    (
                                                                        u => new UserOrganizationalStructureViewModel
                                                                        {
                                                                            Id = u.Id,
                                                                            UserId = u.UserId,
                                                                            OrganizationEntityId = u.OrganizationalStructure.OrganizationEntityId,
                                                                            OrganizationEntityName = u.OrganizationalStructure.OrganizationEntity.Name,
                                                                            OrganizationalStructureId = u.OrganizationalStructureId,
                                                                            OrganizationalStructure = u.OrganizationalStructure.Name,
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

        public IQueryable<OrganizationalStructureViewModel> FindAll()
        {
            try
            {
                var orgStructure = this.Get().AsQueryable();

                IQueryable<OrganizationalStructureViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                o => new OrganizationalStructureViewModel
                                {
                                    Id = o.Id,
                                    Name = o.Name,
                                    Description = String.IsNullOrWhiteSpace(o.Description) ? "" : o.Description,
                                    OrganizationEntityId = o.OrganizationEntityId,
                                    OrganizationEntityName = o.OrganizationEntity.Name,
                                    OrganizationEntityHierarchy = o.OrganizationEntity.Hierarchy,
                                    OrganizationEntityPublished = o.OrganizationEntity.Published,
                                    ParentId = o.ParentId,
                                    Parent = o.ParentId > 0 ? orgStructure.Where(or => or.Id == o.ParentId).Select(or => or.Name).FirstOrDefault() : "",
                                    ParentOrganizationEntityName = o.ParentId > 0 ? orgStructure.Where(or => or.Id == o.ParentId).Select(or => or.OrganizationEntity.Name).FirstOrDefault() : "",
                                    ParentOrganizationEntityHierarchy = o.ParentId > 0 ? orgStructure.Where(or => or.Id == o.ParentId).Select(or => or.OrganizationEntity.Hierarchy).FirstOrDefault() : 0,
                                    Published = o.Published,
                                    CreatedByPK = o.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == o.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == o.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : o.CreatedByPK,
                                    CreatedDate = o.CreatedDate,
                                    ModifiedByPK = o.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == o.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == o.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : o.ModifiedByPK,
                                    ModifiedDate = o.ModifiedDate,
                                    UserOrganizationalStructures = o.UserOrganizationalStructures
                                                                    .Select
                                                                    (
                                                                        u => new UserOrganizationalStructureViewModel
                                                                        {
                                                                            Id = u.Id,
                                                                            UserId = u.UserId,
                                                                            OrganizationEntityId = u.OrganizationalStructure.OrganizationEntityId,
                                                                            OrganizationEntityName = u.OrganizationalStructure.OrganizationEntity.Name,
                                                                            OrganizationalStructureId = u.OrganizationalStructureId,
                                                                            OrganizationalStructure = u.OrganizationalStructure.Name,
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

        public IQueryable<OrganizationEntityViewModel> GetOrgranizationEntities(PagingRequest paging = null, bool? published = null)
        {
            try
            {
                var organizationEntities = _dbCntxt.OrganizationEntities      
                                                    .Select
                                                    (
                                                        o => new OrganizationEntityViewModel()
                                                        {
                                                            Id = o.Id,
                                                            Name = o.Name,
                                                            Hierarchy = o.Hierarchy,
                                                            CreatedByPK = o.CreatedByPK,
                                                            CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == o.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == o.CreatedByPK).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault().ToString() : o.CreatedByPK,
                                                            CreatedDate = o.CreatedDate,
                                                            ModifiedByPK = o.ModifiedByPK,
                                                            ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == o.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == o.ModifiedByPK).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault().ToString() : o.ModifiedByPK,
                                                            ModifiedDate = o.ModifiedDate,
                                                            Published = o.Published,
                                                            WebClass = o.WebClass,
                                                            Color = o.Color
                                                        }
                                                    )
                                                    .AsQueryable();

                if (published != null)
                    organizationEntities = organizationEntities.Where(o => o.Published == published);

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        organizationEntities = organizationEntities.Where(p => p.Name.Contains(search));
                }

                return organizationEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            bool isExists = false;
            int condition = 0;

            try
            {
                if (entityPrimaryKey == null)
                {
                    var organizationalStructuresInDb = await this.FindAll().FirstOrDefaultAsync(o => o.Name == toSearch);

                    if (organizationalStructuresInDb != null)
                        isExists = true;
                }
                else
                {
                    condition = Convert.ToInt32(entityPrimaryKey);
                    var organizationalStructuresInDb = await this.FindAll().FirstOrDefaultAsync(o => o.Name == toSearch && o.Id != condition);

                    if (organizationalStructuresInDb != null)
                        isExists = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return isExists;
        }

        public PagingResponse<OrganizationEntityViewModel> OrganizationEntitiesPagingFeature(IQueryable<OrganizationEntityViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<OrganizationEntityViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<OrganizationEntityViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task SaveOrganizationEntity(OrganizationEntityViewModel model)
        {
            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required", 400);

            if (model.Hierarchy <= 0)
                throw new CustomException("Hierarchy is required", 400);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var orgEntity = new OrganizationEntity();

                if (model.Id <= 0)
                    orgEntity = await _dbCntxt.OrganizationEntities.Where(o => o.Name == model.Name).FirstOrDefaultAsync();
                else
                    orgEntity = await _dbCntxt.OrganizationEntities.Where(o => o.Name == model.Name && o.Id != model.Id).FirstOrDefaultAsync();

                if (orgEntity != null)
                    throw new CustomException("Organization Entity: '" + model.Name + "' already exists.", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                OrganizationEntity organizationEntity = new OrganizationEntity();

                if (model.Id <= 0)
                {
                    organizationEntity.Name = model.Name;
                    organizationEntity.Hierarchy = model.Hierarchy;
                    organizationEntity.Published = model.Published;
                    organizationEntity.WebClass = model.WebClass;
                    organizationEntity.Color = model.Color;
                    organizationEntity.CreatedByPK = cid;
                    organizationEntity.CreatedDate = DateTime.Now;
                    _dbCntxt.OrganizationEntities.Add(organizationEntity);
                }
                else
                {
                    organizationEntity = await _dbCntxt.OrganizationEntities.FirstOrDefaultAsync(o => o.Id == model.Id);

                    if (organizationEntity != null)
                    {
                        organizationEntity.Name = model.Name;
                        organizationEntity.Hierarchy = model.Hierarchy;
                        organizationEntity.Published = model.Published;
                        organizationEntity.WebClass = model.WebClass;
                        organizationEntity.Color = model.Color;
                        organizationEntity.ModifiedByPK = cid;
                        organizationEntity.ModifiedDate = DateTime.Now;
                        _dbCntxt.Entry(organizationEntity).State = EntityState.Modified;
                    }
                }

                await _dbCntxt.SaveChangesAsync();

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

        public async Task Update(OrganizationalStructureViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.ParentId < 0)
                throw new CustomException("Parent Entity is required.", 400);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            if (model.OrganizationEntityId <= 0)
                throw new CustomException("Organization Entity is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var organizationalEntity = await _dbCntxt.OrganizationEntities
                                                   .FirstOrDefaultAsync(o => o.Id == model.OrganizationEntityId);

                if (organizationalEntity == null)
                    throw new CustomException("Entry for organization entity is not found.", 404);

                var organizationalStructure = await _dbCntxt.OrganizationalStructures.FirstOrDefaultAsync(r => r.Id == model.Id);

                if (organizationalStructure != null)
                {
                    //Check if child has higher or equal to parent hierarchy
                    if (model.ParentId > 0)
                    {
                        var parentOrg = await this.GetOrganizationalStructures(null)
                                            .FirstOrDefaultAsync(o => o.Id == model.ParentId);

                        if (parentOrg == null)
                            throw new CustomException("Specified parent is not found", 404);

                        var childEntity = await this.GetOrgranizationEntities(null)
                                              .FirstOrDefaultAsync(o => o.Id == model.OrganizationEntityId);

                        if (parentOrg.OrganizationEntityHierarchy >= childEntity.Hierarchy)
                            throw new CustomException("Entity: " + childEntity.Name + " cannot be added as child of " + parentOrg.Name + " due to hierarchical sequence.", 400);
                    }

                    var checkDuplicateName = await _dbCntxt.OrganizationalStructures
                                                     .Where
                                                     (
                                                        o =>
                                                        o.ParentId == model.ParentId &&
                                                        o.OrganizationEntityId == model.OrganizationEntityId &&
                                                        o.Name == model.Name &&
                                                        o.Id != model.Id
                                                     ).FirstOrDefaultAsync();

                    if (checkDuplicateName != null)
                    {
                        var organizationEntity = await this.GetOrgranizationEntities()
                                                 .FirstOrDefaultAsync(o => o.Id == model.OrganizationEntityId);

                        throw new CustomException("Name: '" + model.Name + "' as " + organizationEntity.Name + " is already existing within desired parent.", 400);
                    }
                    
                    
                    if (model.OrganizationEntityId != organizationalStructure.OrganizationEntityId)
                    {
                        //Check if with maintained in user organizational structures
                        var userOrg = await _dbCntxt.UserOrganizationalStructures.FirstOrDefaultAsync(u => u.OrganizationalStructureId == model.Id);

                        if (userOrg != null)
                            throw new CustomException("Organizational Entity can no longer be changed, users were already maintained under this organizational structure.", 400);
                    }

                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);

                    organizationalStructure.ParentId = model.ParentId;
                    organizationalStructure.OrganizationEntityId = model.OrganizationEntityId;
                    organizationalStructure.Name = model.Name;
                    organizationalStructure.Description = model.Description;
                    organizationalStructure.Published = model.Published;
                    organizationalStructure.ModifiedByPK = cid;
                    organizationalStructure.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(organizationalStructure).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                    dbContextTransaction.Commit();
                }
                else
                    throw new CustomException("Organizational Structure is not found", 404);
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

        private IQueryable<OrganizationalStructure> Get()
        {
            try
            {
                IQueryable<OrganizationalStructure> query = null;

                query = _dbCntxt.OrganizationalStructures
                                .Include(o => o.OrganizationEntity)
                                .Include(u => u.UserOrganizationalStructures)
                                .ThenInclude(u => u.User)
                                .ThenInclude(u => u.AspNetUsersProfile)
                                .Include(u => u.UserOrganizationalStructures)
                                .ThenInclude(u => u.OrganizationalStructure)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<OrganizationalStructureViewModel> GetOrganizationalStructures(PagingRequest paging = null, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            try
            {
                IQueryable<OrganizationalStructureViewModel> query = null;
                query = this.FindAll()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(o => o.Published == published);

                if (organizationEntityPublished != null)
                    query = query.Where(o => o.OrganizationEntityPublished == organizationEntityPublished);

                if (usersPublished != null)
                    query = query
                            .Select
                            (
                                o => new OrganizationalStructureViewModel
                                {
                                    Id = o.Id,
                                    Name = o.Name,
                                    Description = o.Description,
                                    OrganizationEntityId = o.OrganizationEntityId,
                                    OrganizationEntityName = o.OrganizationEntityName,
                                    OrganizationEntityHierarchy = o.OrganizationEntityHierarchy,
                                    OrganizationEntityPublished = o.OrganizationEntityPublished,
                                    ParentId = o.ParentId,
                                    Parent = o.Parent,
                                    ParentOrganizationEntityName = o.ParentOrganizationEntityName,
                                    ParentOrganizationEntityHierarchy = o.ParentOrganizationEntityHierarchy,
                                    Published = o.Published,
                                    CreatedByPK = o.CreatedByPK,
                                    CreatedByName = o.CreatedByName,
                                    CreatedDate = o.CreatedDate,
                                    ModifiedByPK = o.ModifiedByPK,
                                    ModifiedByName = o.ModifiedByPK,
                                    ModifiedDate = o.ModifiedDate,
                                    UserOrganizationalStructures = o.UserOrganizationalStructures
                                                                    .Where(u => u.Published == usersPublished).ToList()
                                }
                            );

                if (paging != null)
                {
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search) ||
                                                p.Parent.Contains(search));
                }

                return query;
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

        public PagingResponse<OrganizationalStructureViewModel> PagingFeature(IQueryable<OrganizationalStructureViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<OrganizationalStructureViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<OrganizationalStructureViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructuresChild(int parentId, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            List<OrganizationalStructureViewModel> orgStructureChildren = new List<OrganizationalStructureViewModel>();

            try
            {
                var orgStructures = await this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished).ToListAsync();

                orgStructureChildren = orgStructures
                                        .Where
                                        (
                                            o =>
                                            o.ParentId == parentId
                                        )
                                        .Select
                                        (
                                            o => new OrganizationalStructureViewModel()
                                            {
                                                Id = o.Id,
                                                ParentId = o.ParentId,
                                                Parent = o.Parent,
                                                ParentOrganizationEntityName = o.ParentOrganizationEntityName,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                OrganizationEntityHierarchy = o.OrganizationEntityHierarchy,
                                                OrganizationEntityPublished = o.OrganizationEntityPublished,
                                                Name = o.Name,
                                                Description = o.Description,
                                                Published = o.Published
                                            }
                                        )
                                        .ToList();

                if (published != null)
                    orgStructureChildren = orgStructureChildren.Where(o => o.Published == published).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return orgStructureChildren;
        }

        private async Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructuresParent(int childId, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            List<OrganizationalStructureViewModel> orgStructureChildren = new List<OrganizationalStructureViewModel>();

            try
            {
                var orgStructures = await this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished).ToListAsync();

                orgStructureChildren = orgStructures
                                        .Where
                                        (
                                            o =>
                                            o.Id == childId
                                        )
                                        .Select
                                        (
                                            o => new OrganizationalStructureViewModel()
                                            {
                                                Id = o.Id,
                                                ParentId = o.ParentId,
                                                Parent = o.Parent,
                                                ParentOrganizationEntityName = o.ParentOrganizationEntityName,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                OrganizationEntityHierarchy = o.OrganizationEntityHierarchy,
                                                OrganizationEntityPublished = o.OrganizationEntityPublished,
                                                Name = o.Name,
                                                Description = o.Description,
                                                Published = o.Published
                                            }
                                        )
                                        .ToList();

                if (published != null)
                    orgStructureChildren = orgStructureChildren.Where(o => o.Published == published).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return orgStructureChildren;
        }

        public async Task<IEnumerable<WholeOrganizationalStructuresViewModel>> GetWholeOrganizationalStructures(bool structuresOnly = true, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            try
            {
                List<WholeOrganizationalStructuresViewModel> wholeOrganizationStructures = new List<WholeOrganizationalStructuresViewModel>();

                var organizationalStructures = await this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished)
                                                         .ToListAsync();

                var orgStructuresEntryPoint = organizationalStructures
                                            .Where
                                            (
                                                o =>
                                                o.ParentId == 0
                                            )
                                            .ToList();

                int nodeNumber = 1;

                foreach (var entryPoint in orgStructuresEntryPoint)
                {
                    WholeOrganizationalStructuresViewModel orgStructure = new WholeOrganizationalStructuresViewModel();

                    orgStructure.Id = entryPoint.Id;
                    orgStructure.ParentId = 0;
                    orgStructure.ParentName = "";
                    orgStructure.UserId = "";
                    orgStructure.Name = entryPoint.Name;
                    orgStructure.Description = entryPoint.Description;
                    orgStructure.OrganizationEntityId = entryPoint.OrganizationEntityId;
                    orgStructure.OrganizationEntity = entryPoint.OrganizationEntityName;
                    orgStructure.Published = entryPoint.Published;
                    orgStructure.NodeNumber = nodeNumber;
                    orgStructure.Type = "Entity";

                    int childrenParentId = entryPoint.Id;

                    orgStructure.Children = (await this.GetWholeOrganizationalStructureChildren(organizationalStructures, childrenParentId, nodeNumber, entryPoint.UserOrganizationalStructures, structuresOnly, published)).ToList();

                    wholeOrganizationStructures.Add(orgStructure);
                }

                return wholeOrganizationStructures;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<WholeOrganizationalStructuresViewModel>> GetWholeOrganizationalStructureChildren(List<OrganizationalStructureViewModel> organizationalStructuresData, int parentId, int nodeNumber, List<UserOrganizationalStructureViewModel> users, bool structuresOnly = true, bool? published = null)
        {
            List<WholeOrganizationalStructuresViewModel> organizationalStructures = new List<WholeOrganizationalStructuresViewModel>();

            string parentName = (organizationalStructuresData.Where(o => o.Id == parentId).FirstOrDefault()).Name;

            if (structuresOnly == false)
            {
                if (users.Count > 0)
                {
                    var userOrg = new WholeOrganizationalStructuresViewModel();

                    var staffUsers = users.Where(u => u.ApprovalLevelDetailId == 0)
                                          .OrderBy(u => u.User.FirstName)
                                          .ToList();
                    var approverUsers = users.Where(u => u.ApprovalLevelDetailId > 0)
                                             .OrderBy(u => u.ApprovalLevel.Sequence)
                                             .ToList();

                    users = new List<UserOrganizationalStructureViewModel>();

                    users.AddRange(approverUsers);
                    users.AddRange(staffUsers);

                    foreach (var user in users)
                    {
                        userOrg = new WholeOrganizationalStructuresViewModel();
                        userOrg.Id = 0;
                        userOrg.ParentId = parentId;
                        userOrg.ParentName = parentName;
                        userOrg.UserId = user.UserId;
                        userOrg.Name = user.User.FirstName + " " + user.User.LastName + " (" + user.ApprovalLevel.ApprovalLevelAndSequence + ")";
                        userOrg.Description = user.User.FirstName + " " + user.User.LastName + " (" + user.ApprovalLevel.ApprovalLevelAndSequence + ")";
                        userOrg.Published = user.Published;
                        userOrg.NodeNumber = nodeNumber;
                        userOrg.Children = null;
                        userOrg.Type = "User";

                        organizationalStructures.Add(userOrg);
                    }
                }
            }

            var orgStructureChildren = organizationalStructuresData.Where(o => o.ParentId == parentId);

            if (orgStructureChildren.Count() > 0)
            {
                nodeNumber += 1;

                var orgStructure = new WholeOrganizationalStructuresViewModel();

                foreach (var children in orgStructureChildren)
                {
                    orgStructure = new WholeOrganizationalStructuresViewModel();

                    orgStructure.Id = children.Id;
                    orgStructure.ParentId = parentId;
                    orgStructure.ParentName = parentName;
                    orgStructure.UserId = "";
                    orgStructure.Name = children.Name;
                    orgStructure.Description = children.Description;
                    orgStructure.OrganizationEntityId = children.OrganizationEntityId;
                    orgStructure.OrganizationEntity = children.OrganizationEntityName;
                    orgStructure.Published = children.Published;
                    orgStructure.NodeNumber = nodeNumber;
                    orgStructure.Type = "Entity";

                    int childrenParentId = children.Id;

                    orgStructure.Children =  (await this.GetWholeOrganizationalStructureChildren(organizationalStructuresData, childrenParentId, nodeNumber, children.UserOrganizationalStructures, structuresOnly, published)).ToList();

                    organizationalStructures.Add(orgStructure);
                }
            }

            return organizationalStructures;
        }

        public async Task<OrganizationalStructureTabularViewModel> GetWholeOrganizationalStructuresInTabularView(PagingRequest paging = null, bool ? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {

            OrganizationalStructureTabularViewModel returnModel = new OrganizationalStructureTabularViewModel();

            returnModel.ColumnHeaders = new List<string>();
            returnModel.Details = new DataTable();

            IQueryable<DataRow> query = null;

            try
            {
                var organizationStructures = await this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished)
                                                       .ToListAsync();

                var orgnaizationEntities = await this.GetOrgranizationEntities(null, organizationEntityPublished)
                                                       .OrderBy(o => o.Hierarchy)
                                                       .Select
                                                       (
                                                          o => new
                                                          {
                                                              o.Id,
                                                              o.Name,
                                                              o.Hierarchy
                                                          }
                                                       )
                                                       .ToListAsync();


                DataTable patternOrganizationalStructure = new DataTable();

                var dtOrganizationEntities = GeneralHelper.ToDataTable(orgnaizationEntities);

                if (dtOrganizationEntities.Rows.Count > 0)
                {
                    DataRow drOrg = null;
                    drOrg = dtOrganizationEntities.NewRow();
                    drOrg["Id"] = dtOrganizationEntities.Rows.Count + 1;
                    drOrg["Name"] = "User";
                    drOrg["Hierarchy"] = dtOrganizationEntities.Rows.Count + 1;
                    dtOrganizationEntities.Rows.Add(drOrg);
                    dtOrganizationEntities.AcceptChanges();
                }

                foreach (DataRow drOrganizationEntity in dtOrganizationEntities.Rows)
                {
                    returnModel.ColumnHeaders.Add(drOrganizationEntity["Name"].ToString());
                    patternOrganizationalStructure.Columns.Add(drOrganizationEntity["Name"].ToString(), typeof(string));
                }

                patternOrganizationalStructure.AcceptChanges();

                var parentOrganizationalStructures = organizationStructures
                                     .Where(o => o.ParentId == 0)
                                     .ToList();


                var dtOrganizationalStructuresInDb = patternOrganizationalStructure.Copy();

                foreach (var orgStructure in parentOrganizationalStructures)
                {
                    int childrenParentId = orgStructure.Id;

                    DataTable dtChilOrg = new DataTable();

                    dtChilOrg = await this.GetWholeOrganizationalStructuresChildrenInTabularView(organizationStructures, dtOrganizationEntities, patternOrganizationalStructure, childrenParentId, published);

                    if (dtChilOrg.Rows.Count > 0)
                    {
                        if (orgStructure.UserOrganizationalStructures != null)
                        {
                            if (orgStructure.UserOrganizationalStructures.Count > 0)
                            {
                                foreach (var user in orgStructure.UserOrganizationalStructures)
                                {
                                    DataRow drOrgStructure;
                                    drOrgStructure = dtOrganizationalStructuresInDb.NewRow();
                                    drOrgStructure[orgStructure.OrganizationEntityName] = orgStructure.Name;
                                    drOrgStructure["User"] = user.User.FirstName + " " + user.User.LastName;

                                    dtOrganizationalStructuresInDb.Rows.Add(drOrgStructure);
                                }
                            }
                        }
                        
                        foreach (DataRow drOrg in dtChilOrg.Rows)
                        {
                            drOrg[orgStructure.OrganizationEntityName] = orgStructure.Name;
                            dtOrganizationalStructuresInDb.ImportRow(drOrg);
                        }
                    }
                    else
                    {
                        //DataRow drOrgStructure;

                        //drOrgStructure = dtOrganizationalStructuresInDb.NewRow();
                        //drOrgStructure[orgStructure.OrganizationEntityName] = orgStructure.Name;

                        //dtOrganizationalStructuresInDb.Rows.Add(drOrgStructure);

                        bool withUsers = false;
                        if (orgStructure.UserOrganizationalStructures != null)
                        {
                            if (orgStructure.UserOrganizationalStructures.Count > 0)
                                withUsers = true;      
                        }

                        if (withUsers)
                        {
                            foreach (var user in orgStructure.UserOrganizationalStructures)
                            {
                                DataRow drOrgStructure;
                                drOrgStructure = dtOrganizationalStructuresInDb.NewRow();
                                drOrgStructure[orgStructure.OrganizationEntityName] = orgStructure.Name;
                                drOrgStructure["User"] = user.User.FirstName + " " + user.User.LastName;

                                dtOrganizationalStructuresInDb.Rows.Add(drOrgStructure);
                            }
                        }
                        else
                        {
                            DataRow drOrgStructure;
                            drOrgStructure = dtOrganizationalStructuresInDb.NewRow();
                            drOrgStructure[orgStructure.OrganizationEntityName] = orgStructure.Name;

                            dtOrganizationalStructuresInDb.Rows.Add(drOrgStructure);
                        }
                    }
                }

                int columnCount = dtOrganizationalStructuresInDb.Columns.Count;

                foreach (DataRow dr in dtOrganizationalStructuresInDb.Rows)
                {
                    for(int counter = 0; counter < columnCount; counter++)
                    {
                        if (dr[counter] == null || dr.IsNull(counter))
                            dr[counter] = "";
                    }
                }

                dtOrganizationalStructuresInDb.AcceptChanges();
                query = dtOrganizationalStructuresInDb
                            .AsEnumerable()
                            .AsQueryable();

                if (paging != null)
                {
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        List<DataRow> query2 = new List<DataRow>();

                        int rowIndex = 0;
                        foreach (DataRow drRow in dtOrganizationEntities.Rows)
                        {
                            var query1 = query
                                        .Where
                                        (
                                            p => p.Field<string>(rowIndex).ToString().ToUpper().Contains(search.ToUpper())
                                        );

                            if (query1.Count() > 0)
                                query2.AddRange(query1.AsEnumerable());
                                
                            rowIndex++;
                        }

                        query = query2.Distinct().AsQueryable();
                    }
                }

                if (query.Count() > 0)
                    returnModel.Details = query.CopyToDataTable();
                else
                {
                    //Assign structure only
                    returnModel.Details = patternOrganizationalStructure;
                }

                return returnModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<DataTable> GetWholeOrganizationalStructuresChildrenInTabularView(List<OrganizationalStructureViewModel> organizationalStructuresData, DataTable dtOrgEntities, DataTable patternOrgStructureTable, int parentId, bool? published = null)
        {
            DataTable dtOrganizationalStructures = new DataTable();

            DataTable dtOrganizationStructureInDb = new DataTable();
            dtOrganizationStructureInDb = patternOrgStructureTable.Copy();

            var orgStructureChildren = organizationalStructuresData.Where(o => o.ParentId == parentId);

            if (orgStructureChildren.Count() > 0)
            {
                foreach (var children in orgStructureChildren)
                {
                    int childrenParentId = children.Id;

                    DataTable dtChilOrg = new DataTable();

                    dtChilOrg = await this.GetWholeOrganizationalStructuresChildrenInTabularView(organizationalStructuresData, dtOrgEntities, patternOrgStructureTable, childrenParentId,  published);

                    if (dtChilOrg.Rows.Count > 0)
                    {
                        if (children.UserOrganizationalStructures.Count > 0)
                        {
                            foreach (var user in children.UserOrganizationalStructures)
                            {
                                DataRow drOrgStructure;
                                drOrgStructure = dtOrganizationStructureInDb.NewRow();
                                drOrgStructure[children.OrganizationEntityName] = children.Name;
                                drOrgStructure["User"] = user.User.FirstName + " " + user.User.LastName;

                                dtOrganizationStructureInDb.Rows.Add(drOrgStructure);
                            }
                        }

                        foreach (DataRow drOrg in dtChilOrg.Rows)
                        {
                            drOrg[children.OrganizationEntityName] = children.Name;
                            dtOrganizationStructureInDb.ImportRow(drOrg);
                        }
                    }
                    else
                    {
                        //DataRow drOrgStructure;

                        //drOrgStructure = dtOrganizationStructureInDb.NewRow();
                        //drOrgStructure[children.OrganizationEntityName] = children.Name;

                        //dtOrganizationStructureInDb.Rows.Add(drOrgStructure);

                        bool withUsers = false;

                        if (children.UserOrganizationalStructures != null)
                        {
                            if (children.UserOrganizationalStructures.Count > 0)
                                withUsers = true;
                        }

                        if (withUsers)
                        {
                            foreach (var user in children.UserOrganizationalStructures)
                            {
                                DataRow drOrgStructure;
                                drOrgStructure = dtOrganizationStructureInDb.NewRow();
                                drOrgStructure[children.OrganizationEntityName] = children.Name;
                                drOrgStructure["User"] = user.User.FirstName + " " + user.User.LastName;

                                dtOrganizationStructureInDb.Rows.Add(drOrgStructure);
                            }
                        }
                        else
                        {
                            DataRow drOrgStructure;
                            drOrgStructure = dtOrganizationStructureInDb.NewRow();
                            drOrgStructure[children.OrganizationEntityName] = children.Name;

                            dtOrganizationStructureInDb.Rows.Add(drOrgStructure);
                        }
                    }
                }
            }

            dtOrganizationalStructures = dtOrganizationStructureInDb;

            return dtOrganizationalStructures;
        }

        public DataTablePagingResponse DataTablePagingFeature(DataTable query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.DataTableSortOrder(query, paging);
                // paginate call method from base class
                return QueryHelper.DataTablePaginate(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructureChildren(int parentId, List<OrganizationalStructureViewModel> organizationalStructures = null, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            List<OrganizationalStructureViewModel> returnOrgStructures = new List<OrganizationalStructureViewModel>();
            try
            {
                List<OrganizationalStructureViewModel> orgStructures = new List<OrganizationalStructureViewModel>();
                bool paramOrgStructureHasValue = false;

                if (organizationalStructures != null)
                {
                    if (organizationalStructures.Count > 0)
                        paramOrgStructureHasValue = true;
                }

                if (paramOrgStructureHasValue)
                    orgStructures = organizationalStructures;
                else
                    orgStructures = await this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished).ToListAsync();

                var orgStructureChildren = orgStructures
                                        .Where
                                        (
                                            o =>
                                            o.ParentId == parentId
                                        )
                                        .Select
                                        (
                                            o => new OrganizationalStructureViewModel()
                                            {
                                                Id = o.Id,
                                                ParentId = o.ParentId,
                                                Parent = o.Parent,
                                                ParentOrganizationEntityName = o.ParentOrganizationEntityName,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                OrganizationEntityHierarchy = o.OrganizationEntityHierarchy,
                                                OrganizationEntityPublished = o.OrganizationEntityPublished,
                                                Name = o.Name,
                                                Description = o.Description,
                                                Published = o.Published,
                                                UserOrganizationalStructures = o.UserOrganizationalStructures
                                            }
                                        )
                                        .ToList();

                if (orgStructureChildren.Count > 0)
                {
                    foreach(var child in orgStructureChildren)
                    {
                        OrganizationalStructureViewModel returnOrg = new OrganizationalStructureViewModel();

                        returnOrg.Id = child.Id;
                        returnOrg.ParentId = child.ParentId;
                        returnOrg.Parent = child.Parent;
                        returnOrg.ParentOrganizationEntityName = child.ParentOrganizationEntityName;
                        returnOrg.OrganizationEntityId = child.OrganizationEntityId;
                        returnOrg.OrganizationEntityName = child.OrganizationEntityName;
                        returnOrg.OrganizationEntityHierarchy = child.OrganizationEntityHierarchy;
                        returnOrg.OrganizationEntityPublished = child.OrganizationEntityPublished;
                        returnOrg.Name = child.Name;
                        returnOrg.Description = child.Description;
                        returnOrg.Published = child.Published;
                        returnOrg.UserOrganizationalStructures = child.UserOrganizationalStructures;

                        returnOrgStructures.Add(returnOrg);

                        int childparentId = child.Id;

                        returnOrgStructures.AddRange(await this.GetOrganizationalStructureChildren(childparentId, orgStructures, published, organizationEntityPublished, usersPublished));
                    }
                }

                return returnOrgStructures;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructureParents(int childId, List<OrganizationalStructureViewModel> organizationalStructures, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            List<OrganizationalStructureViewModel> returnOrgStructures = new List<OrganizationalStructureViewModel>();
            try
            {
                List<OrganizationalStructureViewModel> orgStructures = new List<OrganizationalStructureViewModel>();
                bool paramOrgStructureHasValue = false;

                if (organizationalStructures != null)
                {
                    if (organizationalStructures.Count > 0)
                        paramOrgStructureHasValue = true;
                }

                if (paramOrgStructureHasValue)
                    orgStructures = organizationalStructures;
                else
                    orgStructures = await this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished).ToListAsync();

                returnOrgStructures = (await this.GetOrganizationalStructurePathAsync(childId, orgStructures, published, organizationEntityPublished, usersPublished))
                                                 .Where(o => o.Id != childId)
                                                 .ToList();

                return returnOrgStructures;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructurePathAsync(int childId, List<OrganizationalStructureViewModel> organizationalStructures, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            List<OrganizationalStructureViewModel> returnOrgStructures = new List<OrganizationalStructureViewModel>();
            try
            {
                List<OrganizationalStructureViewModel> orgStructures = new List<OrganizationalStructureViewModel>();

                bool paramOrgStructureHasValue = false;

                if (organizationalStructures != null)
                {
                    if (organizationalStructures.Count > 0)
                        paramOrgStructureHasValue = true;
                }

                if (paramOrgStructureHasValue)
                    orgStructures = organizationalStructures;
                else
                    orgStructures = await this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished).ToListAsync();

                var orgStructurePath = orgStructures
                                        .Where
                                        (
                                            o =>
                                            o.Id == childId
                                        )
                                        .Select
                                        (
                                            o => new OrganizationalStructureViewModel()
                                            {
                                                Id = o.Id,
                                                ParentId = o.ParentId,
                                                Parent = o.Parent,
                                                ParentOrganizationEntityName = o.ParentOrganizationEntityName,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                OrganizationEntityHierarchy = o.OrganizationEntityHierarchy,
                                                OrganizationEntityPublished = o.OrganizationEntityPublished,
                                                Name = o.Name,
                                                Description = o.Description,
                                                Published = o.Published,
                                                UserOrganizationalStructures = o.UserOrganizationalStructures
                                            }
                                        )
                                        .ToList();

                if (orgStructurePath.Count > 0)
                {
                    foreach (var path in orgStructurePath)
                    {
                        OrganizationalStructureViewModel returnOrg = new OrganizationalStructureViewModel();

                        returnOrg.Id = path.Id;
                        returnOrg.ParentId = path.ParentId;
                        returnOrg.Parent = path.Parent;
                        returnOrg.ParentOrganizationEntityName = path.ParentOrganizationEntityName;
                        returnOrg.OrganizationEntityId = path.OrganizationEntityId;
                        returnOrg.OrganizationEntityName = path.OrganizationEntityName;
                        returnOrg.OrganizationEntityHierarchy = path.OrganizationEntityHierarchy;
                        returnOrg.OrganizationEntityPublished = path.OrganizationEntityPublished;
                        returnOrg.Name = path.Name;
                        returnOrg.Description = path.Description;
                        returnOrg.Published = path.Published;
                        returnOrg.UserOrganizationalStructures = path.UserOrganizationalStructures;

                        returnOrgStructures.Add(returnOrg);

                        int pathParentId = path.ParentId;

                        returnOrgStructures.AddRange(await this.GetOrganizationalStructurePathAsync(pathParentId, orgStructures, published, organizationEntityPublished, usersPublished));
                    }
                }

                return returnOrgStructures;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IEnumerable<OrganizationalStructureViewModel> GetOrganizationalStructurePath(int childId, List<OrganizationalStructureViewModel> organizationalStructures, bool? published, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            List<OrganizationalStructureViewModel> returnOrgStructures = new List<OrganizationalStructureViewModel>();
            try
            {
                List<OrganizationalStructureViewModel> orgStructures = new List<OrganizationalStructureViewModel>();

                bool paramOrgStructureHasValue = false;

                if (organizationalStructures != null)
                {
                    if (organizationalStructures.Count > 0)
                        paramOrgStructureHasValue = true;
                }

                if (paramOrgStructureHasValue)
                    orgStructures = organizationalStructures;
                else
                    orgStructures = this.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished).ToList();

                var orgStructurePath = orgStructures
                                        .Where
                                        (
                                            o =>
                                            o.Id == childId
                                        )
                                        .Select
                                        (
                                            o => new OrganizationalStructureViewModel()
                                            {
                                                Id = o.Id,
                                                ParentId = o.ParentId,
                                                Parent = o.Parent,
                                                ParentOrganizationEntityName = o.ParentOrganizationEntityName,
                                                OrganizationEntityId = o.OrganizationEntityId,
                                                OrganizationEntityName = o.OrganizationEntityName,
                                                OrganizationEntityHierarchy = o.OrganizationEntityHierarchy,
                                                OrganizationEntityPublished = o.OrganizationEntityPublished,
                                                Name = o.Name,
                                                Description = o.Description,
                                                Published = o.Published,
                                                UserOrganizationalStructures = o.UserOrganizationalStructures
                                            }
                                        )
                                        .ToList();

                if (orgStructurePath.Count > 0)
                {
                    foreach (var path in orgStructurePath)
                    {
                        OrganizationalStructureViewModel returnOrg = new OrganizationalStructureViewModel();

                        returnOrg.Id = path.Id;
                        returnOrg.ParentId = path.ParentId;
                        returnOrg.Parent = path.Parent;
                        returnOrg.ParentOrganizationEntityName = path.ParentOrganizationEntityName;
                        returnOrg.OrganizationEntityId = path.OrganizationEntityId;
                        returnOrg.OrganizationEntityName = path.OrganizationEntityName;
                        returnOrg.OrganizationEntityHierarchy = path.OrganizationEntityHierarchy;
                        returnOrg.OrganizationEntityPublished = path.OrganizationEntityPublished;
                        returnOrg.Name = path.Name;
                        returnOrg.Description = path.Description;
                        returnOrg.Published = path.Published;
                        returnOrg.UserOrganizationalStructures = path.UserOrganizationalStructures;

                        returnOrgStructures.Add(returnOrg);

                        int pathParentId = path.ParentId;

                        returnOrgStructures.AddRange(this.GetOrganizationalStructurePath(pathParentId, orgStructures, published, organizationEntityPublished, usersPublished));
                    }
                }

                return returnOrgStructures;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

    }
}
