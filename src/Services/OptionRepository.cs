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

namespace workflow.Services
{
    public class OptionRepository : BaseApi, IOption
    {
        public OptionRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<OptionRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<OptionViewModel> Find(object data = null)
        {
            try
            {
                IQueryable<OptionViewModel> query = null;

                if (data != null)
                {
                    var condition = Convert.ToInt32(data ?? 0);
                    query = _dbCntxt.Options
                                    .Where(o => o.Id == condition)
                                    .Select
                                    (
                                        o => new OptionViewModel
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Description = o.Description,
                                            OptionGroup = o.OptionGroup,
                                            Published = o.Published,
                                            CreatedByPK = o.CreatedByPK,
                                            CreatedDate = o.CreatedDate,
                                            ModifiedByPK = o.ModifiedByPK,
                                            ModifiedDate = o.ModifiedDate
                                        }
                                    )
                                    .AsQueryable();
                }
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<OptionViewModel> FindAll()
        {
            try
            {
                IQueryable<OptionViewModel> query = null;

                query = _dbCntxt.Options
                                .Select
                                (
                                    o => new OptionViewModel
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        Description = o.Description,
                                        OptionGroup = o.OptionGroup,
                                        Published = o.Published,
                                        CreatedByPK = o.CreatedByPK,
                                        CreatedDate = o.CreatedDate,
                                        ModifiedByPK = o.ModifiedByPK,
                                        ModifiedDate = o.ModifiedDate
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

        public IQueryable<OptionViewModel> GetOptions(PagingRequest paging)
        {
            try
            {
                IQueryable<OptionViewModel> query = null;

                query = this.FindAll()
                            .AsQueryable();

                // custom search 
                if (!paging.SearchCriteria.IsPageLoad && !string.IsNullOrEmpty(paging.SearchCriteria.Filter))
                {
                    query = query.Where(p => p.Name.Contains(paging.SearchCriteria.Filter));
                }

                // default search 
                var search = paging.Search.Value.ToLower();

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(p => p.Id.ToString().Contains(search) || p.Name.Contains(search) ||
                                           p.Description.Contains(search) || p.OptionGroup.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IEnumerable<object> GetDistinctOptionGroup()
        {
            try
            {
                return _dbCntxt.Options.Select(p => new { p.OptionGroup }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<OptionViewModel> GetOptionsByGroup(object obj = null)
        {
            try
            {
                IQueryable<OptionViewModel> query = null;

                if (obj != null)
                {
                    var condition = Convert.ToString(obj ?? "");
                    query = _dbCntxt.Options
                                    .Where(o => o.OptionGroup == condition)
                                    .Select
                                    (
                                        o => new OptionViewModel
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Description = o.Description,
                                            OptionGroup = o.OptionGroup,
                                            Published = o.Published,
                                            CreatedByPK = o.CreatedByPK,
                                            CreatedDate = o.CreatedDate,
                                            ModifiedByPK = o.ModifiedByPK,
                                            ModifiedDate = o.ModifiedDate
                                        }
                                    )
                                    .AsQueryable();
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<OptionViewModel> Add(OptionViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var option = new Options
                {
                    Name = model.Name,
                    Description = model.Description,
                    OptionGroup = model.OptionGroup,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cId
                };

                _dbCntxt.Options.Add(option);
                await _dbCntxt.SaveChangesAsync();

                model.Id = option.Id;
                model.CreatedByPK = option.CreatedByPK;
                model.CreatedDate = option.CreatedDate;

                dbContextTransaction.Commit();

                // Log Transaction
                // Code Here

                return model;
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Update(OptionViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier); 

                var option = _dbCntxt.Options.Find(model.Id);
                if (option != null)
                {
                    option.Name = model.Name;
                    option.Description = model.Description;
                    option.OptionGroup = model.OptionGroup;
                    option.Published = model.Published;
                    option.ModifiedDate = DateTime.Now;
                    option.ModifiedByPK = cId;
                    _dbCntxt.Entry(option).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    dbContextTransaction.Commit();

                    // Log Transaction
                    // Code Here
                }
                else
                    throw new CustomException("Option is not found.", 404);
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
                var id = Convert.ToInt16(obj);

                var requestType = _dbCntxt.Options.Find(id);

                if (requestType != null)
                {
                    _dbCntxt.Options.Remove(_dbCntxt.Options.Where(x => x.Id == id).FirstOrDefault());
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                    throw new CustomException("Option is not found.", 404);

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

        public async Task DeleteAll(OptionViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var ids = model.DsList.Select(o => o.Id).ToArray();                

                foreach (var ds in model.DsList)
                {
                    var sql = "DELETE FROM Options WHERE Id = {0}";
                    await _dbCntxt.Database.ExecuteSqlRawAsync(sql, ds.Id);
                }

                dbContextTransaction.Commit();

                // Log Transaction
                // Code Here
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<OptionViewModel> PagingFeature(IQueryable<OptionViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<OptionViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<OptionViewModel>(query, paging);
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
    }
}
