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
using Microsoft.EntityFrameworkCore.Storage;

namespace workflow.Services
{
    public class ReasonRepository : BaseApi, IReason
    {

        readonly IOption _option;

        public ReasonRepository(
            FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestCategoryRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IOption option) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _option = option;
        }

        public async Task<ReasonsViewModel> Add(ReasonsViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.ReasonTypeId == 0)
                throw new CustomException("Reason Type is required.", 400);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Reason is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var reasonindb = await this.FindAll().FirstOrDefaultAsync(c => c.Name == model.Name && c.ReasonTypeId == model.ReasonTypeId);

                if (reasonindb != null)
                    throw new CustomException("Reason is already existing in the selected reason type.", 400);

                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var reason = new Reason
                {
                    Name = model.Name,
                    Description = model.Description,
                    ReasonTypeId = model.ReasonTypeId,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cId
                };

                _dbCntxt.Reasons.Add(reason);
                await _dbCntxt.SaveChangesAsync();

                model.Id = reason.Id;
                model.CreatedByPK = reason.CreatedByPK;
                model.CreatedDate = reason.CreatedDate;

                dbContextTransaction.Commit();

                // Log Transaction
                // Code Here

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
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var reasonid = Convert.ToInt32(obj);

                var reason = await this.FindAll().Where(r => r.Id == reasonid).FirstOrDefaultAsync();

                if (reason != null)
                {
                    _dbCntxt.Reasons.RemoveRange(_dbCntxt.Reasons.Where(r => r.Id == reason.Id));
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                    throw new CustomException("Reason is not found.", 404);

                dbContextTransaction.Commit();

                // Log Transaction
                // Code Here
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

        public Task DeleteAll(ReasonsViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ReasonsViewModel> Find(object obj)
        {
            try
            {
                IQueryable<ReasonsViewModel> query = null;

                var condition = Convert.ToInt32(obj);

                query = this.Get()
                            .Where
                            (
                                r =>
                                r.Id == condition
                            )
                            .Select
                            (
                                r => new ReasonsViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = r.Description,
                                    ReasonTypeId = r.ReasonTypeId,
                                    ReasonTypeName = _dbCntxt.Options.FirstOrDefault(o => o.Id == r.ReasonTypeId).Name,
                                    ReasonTypePublished = _dbCntxt.Options.FirstOrDefault(o => o.Id == r.ReasonTypeId).Published,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
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

        public IQueryable<ReasonsViewModel> FindAll()
        {
            try
            {
                IQueryable<ReasonsViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                r => new ReasonsViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = r.Description,
                                    ReasonTypeId = r.ReasonTypeId,
                                    ReasonTypeName = _dbCntxt.Options.FirstOrDefault(o => o.Id == r.ReasonTypeId).Name,
                                    ReasonTypePublished = _dbCntxt.Options.FirstOrDefault(o => o.Id == r.ReasonTypeId).Published,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
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

        public IQueryable<ReasonsViewModel> GetReasons(PagingRequest paging)
        {
            try
            {
                IQueryable<ReasonsViewModel> query = null;
                query = this.FindAll()
                            .AsQueryable();

                var search = paging.Search.Value.ToLower();

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search) ||
                                            p.ReasonTypeName.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public Task<bool> IfExists(string tosearch, object entityprimarykey = null)
        {
            throw new NotImplementedException();
        }

        public PagingResponse<ReasonsViewModel> PagingFeature(IQueryable<ReasonsViewModel> query, PagingRequest paging, object Id = null)
        {
            try
            {
               
                // sort order call method from base class
                query = QueryHelper.SortOrder<ReasonsViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<ReasonsViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Update(ReasonsViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.ReasonTypeId == 0)
                throw new CustomException("Reason Type is required.", 400);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Reason is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {

                var reasonindb = await this.FindAll().FirstOrDefaultAsync(c => c.Name == model.Name && c.ReasonTypeId == model.ReasonTypeId && c.Id != model.Id);

                if (reasonindb != null)
                    throw new CustomException("Reason is already existing in the selected reason type.", 400);

                var reason = await _dbCntxt.Reasons.FirstOrDefaultAsync(r => r.Id == model.Id);

                if (reason != null)
                {
                    // get current user id
                    var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    reason.Name = model.Name;
                    reason.Description = model.Description;
                    reason.ReasonTypeId = model.ReasonTypeId;
                    reason.Published = model.Published;
                    reason.ModifiedByPK = cId;
                    reason.ModifiedDate = DateTime.Now;

                    _dbCntxt.Entry(reason).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    dbContextTransaction.Commit();

                    // Log Transaction
                    // Code Here
                }
                else
                    throw new CustomException("Reason is not found.", 404);
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
        private IQueryable<Reason> Get()
        {
            try
            {
                IQueryable<Reason> query = null;

                query = _dbCntxt.Reasons
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<ReasonTypesViewModel>> GetReasonTypes(bool published = true)
        {
            try
            {
                var reasontypes = await _option.FindAll()
                                                 .Where(r => r.OptionGroup == "Reason Types" && r.Published == published)
                                                 .Select
                                                 (
                                                     r => new ReasonTypesViewModel()
                                                     {
                                                         Id = r.Id,
                                                         Name = r.Name
                                                     }
                                                 )
                                                 .OrderBy(r => r.Name).ToListAsync();


                return reasontypes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<ReasonBasicData>> GetReasonByReasonType(string reasontype, bool published = true)
        {
            try
            {

                List<ReasonBasicData> reasons = new List<ReasonBasicData>();

                if (!String.IsNullOrWhiteSpace(reasontype))
                {
                    reasons = await this.FindAll()
                                        .Where
                                        (
                                            r => r.ReasonTypeName.Trim().ToUpper() == reasontype.Trim().ToUpper() &&
                                            r.Published == published &&
                                            r.ReasonTypePublished == true
                                        )
                                        .Select
                                        (
                                            r => new ReasonBasicData
                                            {
                                                Id = r.Id,
                                                Name = r.Name
                                            }
                                        )
                                        .OrderBy(r => r.Name)
                                        .ToListAsync();
                }
                else
                {
                    reasons = await this.FindAll()
                                        .Where
                                        (
                                            r => r.Published == published &&
                                            r.ReasonTypePublished == true
                                        )
                                        .Select
                                        (
                                            r => new ReasonBasicData
                                            {
                                                Id = r.Id,
                                                Name = r.Name
                                            }
                                        )
                                        .OrderBy(r => r.Name)
                                        .ToListAsync();

                }

                return reasons;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
