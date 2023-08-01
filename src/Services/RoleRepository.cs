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
using Microsoft.EntityFrameworkCore.Storage;

namespace workflow.Services
{
    public class RoleRepository : BaseApi, IRole
    {
        public RoleRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RoleRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<RoleViewModel> Find(object obj)
        {
            try
            {
                IQueryable<RoleViewModel> query = null;

                var condition = Convert.ToString(obj ?? 0);
                query = this.Get()
                            .Where(r => r.Id == condition)
                            .Select
                            (
                                r => new RoleViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    NormalizedName = r.NormalizedName,
                                    IndexPage = r.IndexPage,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    TotalUser = r.AspNetUserRoles.Count,
                                    IsChecked = false
                                }
                            ).AsQueryable();
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<RoleViewModel> FindAll()
        {
            try
            {
                IQueryable<RoleViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                r => new RoleViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    NormalizedName = r.NormalizedName,
                                    IndexPage = r.IndexPage,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    TotalUser = r.AspNetUserRoles.Count,
                                    IsChecked = false
                                }
                            ).AsQueryable();
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<RoleViewModel> GetRole(bool filterAdminRole = false)
        {
            try
            {
                IQueryable<RoleViewModel> query = null;

                query = this.FindAll()
                        .AsQueryable();

                if (filterAdminRole)
                {
                    var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var superAdmin =  _dbCntxt.AspNetUserRoles
                                            .FirstOrDefault
                                            (
                                                u =>
                                                u.UserId == cId &&
                                                u.RoleId == "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03"
                                            );
                    if (superAdmin == null)
                        query = query.Where(r => r.Id != "4594BBC7-831E-4BFE-B6C4-91DFA42DBB03").AsQueryable();

                }
                
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<AspNetUsersMenuPermission> GetMenuPer(object obj = null)
        {
            try
            {
                IQueryable<AspNetUsersMenuPermission> query = null;

                if (obj != null)
                {
                    var condition = Convert.ToString(obj ?? 0);
                    query = _dbCntxt.AspNetUsersMenuPermission
                                    .Where(r => r.RoleId == condition)
                                    .AsQueryable();
                }
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<RoleViewModel> Add(RoleViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {

                if (await this.IfExists(model.Name))
                    throw new CustomException("Role: " + model.Name + " is already exists.", 400);

                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var role = new AspNetRoles
                {
                    Id = GeneralHelper.GenerateGuidAsUniqueKey(),
                    Name = model.Name,
                    IndexPage = model.IndexPage,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cId
                };

                _dbCntxt.AspNetRoles.Add(role);
                await _dbCntxt.SaveChangesAsync();

                model.Id = role.Id;
                model.CreatedByPK = role.CreatedByPK;
                model.CreatedDate = role.CreatedDate;


                // reset role permission details
                await this.SaveMenuPermission(model.MenuPermission, role.Id);

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

        public async Task Update(RoleViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (await this.IfExists(model.Name, model.Id))
                    throw new CustomException("Role: " + model.Name + " is already exists.", 400);

                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var role = _dbCntxt.AspNetRoles.Find(model.Id);
                if (role != null)
                {
                    role.Name = model.Name;
                    role.IndexPage = model.IndexPage;
                    role.Published = model.Published;
                    role.ModifiedDate = DateTime.Now;
                    role.ModifiedByPK = cId;
                    _dbCntxt.Entry(role).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    // reset role permission details
                    await this.SaveMenuPermission(model.MenuPermission, model.Id);

                    dbContextTransaction.Commit();

                    // Log Transaction
                    // Code Here
                }
                else
                    throw new CustomException("Role is not found.", 404);
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

        private async Task SaveMenuPermission(List<MenuPermisionViewModel> permissions, string id)
        {
            _dbCntxt.AspNetUsersMenuPermissionControl.RemoveRange(_dbCntxt.AspNetUsersMenuPermissionControl.Where(x => x.MenuPermission.RoleId == id));
            _dbCntxt.AspNetUsersMenuPermission.RemoveRange(_dbCntxt.AspNetUsersMenuPermission.Where(x => x.RoleId == id));
            _dbCntxt.SaveChanges();

            foreach (var mn in permissions)
            {
                AspNetUsersMenuPermission aump = new();
                aump.VMenuPermissionId = Guid.NewGuid().ToString();
                aump.RoleId = id;
                aump.VMenuId = mn.VMenuId;
                _dbCntxt.AspNetUsersMenuPermission.Add(aump);
                await _dbCntxt.SaveChangesAsync();

                if (mn.SelectedControls != null)
                {
                    if (mn.SelectedControls.Count > 0)
                    {
                        foreach (var sc in mn.SelectedControls)
                        {
                            AspNetUsersMenuPermissionControl pc = new AspNetUsersMenuPermissionControl();
                            pc.Id = Guid.NewGuid().ToString();
                            pc.VMenuPermissionId = aump.VMenuPermissionId;
                            pc.MenuControlId = sc.id;
                            _dbCntxt.AspNetUsersMenuPermissionControl.Add(pc);
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        public async Task Delete(object obj, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var id = Convert.ToString(obj);

                var role = this.Find(id);

                if (role != null)
                {
                    var result = await _dbCntxt.AspNetUserRoles.Where(x => x.RoleId == id).CountAsync();
                    if (result > 0)
                        throw new CustomException("Unable to delete role with existing user(s)", 400);

                    var menupermissioncontrol = await _dbCntxt.AspNetUsersMenuPermission.Where(x => x.RoleId == id).ToListAsync();

                    if (menupermissioncontrol.Count > 0)
                    {
                        foreach (var permissioncontrol in menupermissioncontrol)
                        {
                            _dbCntxt.AspNetUsersMenuPermissionControl.RemoveRange(_dbCntxt.AspNetUsersMenuPermissionControl.Where(p => p.VMenuPermissionId == permissioncontrol.VMenuPermissionId));
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }

                    _dbCntxt.AspNetUsersMenuPermission.RemoveRange(_dbCntxt.AspNetUsersMenuPermission.Where(x => x.RoleId == id));
                    await _dbCntxt.SaveChangesAsync();

                    _dbCntxt.AspNetRoles.Remove(_dbCntxt.AspNetRoles.Where(x => x.Id == id).FirstOrDefault());
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                    throw new CustomException("Role is not found.", 404);

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

        public async Task DeleteAll(RoleViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var ids = model.DsList.Select(o => o.Id).ToArray();

                _dbCntxt.AspNetRoles.Remove(_dbCntxt.AspNetRoles.Where(x => x.Id == model.Id).FirstOrDefault());
                await _dbCntxt.SaveChangesAsync();

                foreach (var ds in model.DsList)
                {
                    var sql = "DELETE FROM AspNetRoles WHERE Id = {0}";
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

        public PagingResponse<RoleViewModel> PagingFeature(IQueryable<RoleViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<RoleViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<RoleViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<AspNetRoles> Get()
        {
            try
            {
                IQueryable<AspNetRoles> query = null;

                query = _dbCntxt.AspNetRoles
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
            string condition = "";
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
                    condition = entityprimarykey.ToString();

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
    }
}
