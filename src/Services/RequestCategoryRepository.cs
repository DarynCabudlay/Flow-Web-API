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
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace workflow.Services
{
    public class RequestCategoryRepository : BaseApi, IRequestCategory
    {

        private readonly IUserIPAddressPerSession _useripaddresspersession;

        public RequestCategoryRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestCategoryRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession useripaddresspersession) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _useripaddresspersession = useripaddresspersession;
        }

        public IQueryable<RequestCategoryViewModel> Find(object obj)
        {
            try
            {
                IQueryable<RequestCategoryViewModel> query = null;

                var condition = Convert.ToInt32(obj ?? 0);
                query = this.Get()
                            .Where(r => r.Id == condition)
                            .Select
                            (
                                r => new RequestCategoryViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = r.Description,
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
        public IQueryable<RequestCategoryViewModel> FindAll()
        {
            try
            {
                IQueryable<RequestCategoryViewModel> query = null;

                query = this.Get()
                            .Select
                            (
                                r => new RequestCategoryViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = r.Description,
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

        public IQueryable<RequestCategoryViewModel> GetRequestCategories(PagingRequest paging)
        {
            try
            {
                IQueryable<RequestCategoryViewModel> query = null;

                query = this.FindAll()
                            .AsQueryable();

                var search = paging.Search.Value.ToLower();

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(p => p.Id.ToString().Contains(search) || p.Name.Contains(search) ||
                                            p.Description.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<RequestCategoryViewModel> Add(RequestCategoryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Request Category is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (await this.IfExists(model.Name))
                    throw new CustomException("Request Category: " + model.Name + " is already exists.", 400);


                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spid = await _useripaddresspersession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                //add useripperssion
                var userip = _useripaddresspersession.AddData(spid, cId, ip);
                await _useripaddresspersession.Save(userip);

                var requestCategory = new RequestCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cId
                };

                _dbCntxt.RequestCategories.Add(requestCategory);
                await _dbCntxt.SaveChangesAsync();

                model.Id = requestCategory.Id;
                model.CreatedByPK = requestCategory.CreatedByPK;
                model.CreatedDate = requestCategory.CreatedDate;

                //remove useripperssion
                await _useripaddresspersession.Remove(spid);

                dbContextTransaction.Commit();

                return model;
            }
            catch(CustomException customex)
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

        public async Task Update(RequestCategoryViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Request Category is required.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {

                if (await this.IfExists(model.Name, model.Id))
                    throw new CustomException("Request Category: " + model.Name + " is already exists.", 400);

                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spid = await _useripaddresspersession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                //add useripperssion
                var userip = _useripaddresspersession.AddData(spid, cId, ip);
                await _useripaddresspersession.Save(userip);

                var requestCategory = _dbCntxt.RequestCategories.Find(model.Id);
                if (requestCategory != null)
                {
                    requestCategory.Name = model.Name;
                    requestCategory.Description = model.Description;
                    requestCategory.Published = model.Published;
                    requestCategory.ModifiedDate = DateTime.Now;
                    requestCategory.ModifiedByPK = cId;
                    _dbCntxt.Entry(requestCategory).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    //remove useripperssion
                    await _useripaddresspersession.Remove(spid);

                    dbContextTransaction.Commit();
                }
                else
                    throw new CustomException("Request Category is not found", 404);
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

                var requestCategory = this.Find(id);

                if (requestCategory != null)
                {

                    var requestTypeInCategory = _dbCntxt.RequestTypes
                                                        .FirstOrDefaultAsync(r => r.RequestCategoryId == id);

                    if (requestTypeInCategory != null)
                        throw new CustomException("Cannot delete this Request Category since this was used in request types maintenance.", 400);

                    // get current user id
                    var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    // get current session id
                    int spid = await _useripaddresspersession.GetSPID();
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                    //add useripperssion
                    var userip = _useripaddresspersession.AddData(spid, cId, ip);
                    await _useripaddresspersession.Save(userip);

                    _dbCntxt.RequestCategories.RemoveRange(_dbCntxt.RequestCategories.Where(x => x.Id == id));
                    await _dbCntxt.SaveChangesAsync();

                    //remove useripperssion
                    await _useripaddresspersession.Remove(spid);

                    dbContextTransaction.Commit();

                }
                else
                    throw new CustomException("Request Category is not found.", 404);

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

        public async Task DeleteAll(RequestCategoryViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var ids = model.DsList.Select(o => o.Id).ToArray();                

                foreach (var ds in model.DsList)
                {
                    var sql = "DELETE FROM RequestCategories WHERE Id = {0}";
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

        public PagingResponse<RequestCategoryViewModel> PagingFeature(IQueryable<RequestCategoryViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<RequestCategoryViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<RequestCategoryViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<RequestCategory> Get()
        {
            try
            {
                IQueryable<RequestCategory> query = null;

                query = _dbCntxt.RequestCategories
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> IfExists(string tosearch, object entityprimarykey = null)
        {
            bool isexists = false;
            int condition = 0;
            try
            {


                if (entityprimarykey == null)
                {
                    var requesttypesindb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == tosearch);

                    if (requesttypesindb != null)
                        isexists = true;
                }
                else
                {
                    condition = Convert.ToInt32(entityprimarykey);

                    var requesttypesindb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == tosearch && r.Id != condition);

                    if (requesttypesindb != null)
                        isexists = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return isexists;
        }

        public async Task<IEnumerable<RequestCategoryViewModel>> GetRequestCategoriesForTicketCreation(string userId)
        {
            List<RequestCategoryViewModel> requestCategories = new List<RequestCategoryViewModel>();
            try
            {
                var userRequestTypesGroupings = await _dbCntxt.UserRequestTypesGroupings
                                                              .Include(u => u.RequestTypesGrouping)
                                                              .Include(u => u.RequestTypesGrouping.RequestTypesGroupingDetails)
                                                              .ThenInclude(r => r.RequestTypes)
                                                              .ThenInclude(r => r.RequestCategory)
                                                            .Where
                                                            (
                                                                u => u.UserId == userId &&
                                                                u.Published == true
                                                            )
                                                            .ToListAsync();

                foreach (var userRequestTypesGrouping in userRequestTypesGroupings)
                {
                    var requestTypesGrouping = userRequestTypesGrouping.RequestTypesGrouping;

                    foreach(var requestType in requestTypesGrouping.RequestTypesGroupingDetails)

                    requestCategories.AddRange(requestTypesGrouping.RequestTypesGroupingDetails
                                                        .Where(r => r.RequestTypes.RequestCategory.Published == true)
                                                        .Select
                                                        (
                                                            r => new RequestCategoryViewModel
                                                            {
                                                                Id = r.RequestTypes.RequestCategoryId,
                                                                Name = r.RequestTypes.RequestCategory.Name
                                                            }
                                                        )
                                                        .ToList());

                }

                if (requestCategories.Count > 0)
                {
                    //order by name
                    requestCategories = requestCategories
                                        .OrderBy(r => r.Name).ToList();

                    //group by tp unique
                    requestCategories = requestCategories
                                       .GroupBy
                                       (
                                            r => new
                                            {
                                                r.Id,
                                                r.Name
                                            }
                                       )
                                       .Select(r => r.First())
                                       .ToList();
                }

                return requestCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
