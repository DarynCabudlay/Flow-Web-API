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
    public class MenuRepository : BaseApi, IMenu
    {
        private List<MenuViewModel> TMenu { get; set; }
        private List<MenuViewModel> MenuListBySerial { get; set; }

        readonly IOption _option;

        public MenuRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<MenuRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<MenuViewModel> Find(object data = null)
        {
            try
            {
                return (IQueryable<MenuViewModel>)Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<MenuViewModel> FindAll()
        {
            try
            {
                IQueryable<MenuViewModel> query = null;

                query = _dbCntxt.AspNetUsersMenu
                                .Select
                                (
                                    mn => new MenuViewModel
                                    {
                                        VMenuID = mn.VMenuId,
                                        VParentMenuID = mn.VParentMenuId,
                                        ISerialNo = mn.ISerialNo,
                                        NvMenuName = mn.NvMenuName,
                                        NvPageUrl = mn.NvPageUrl,
                                        NvFabIcon = mn.NvFabIcon,
                                        PrefixCode = mn.PrefixCode,
                                        Published = mn.Published,
                                        CreatedDate = mn.CreatedDate,
                                        CreatedByPK = mn.CreatedByPK,
                                        ButtonControls = _dbCntxt.AspNetUsersMenuControl.Where(x => x.VMenuId == mn.VMenuId)
                                                                .Select(x => new UserMenuControlViewModel { Id = x.OptionId, MenuControlId = x.MenuControlId, Name = x.Option.Name, Description = x.Option.Description })
                                                                .ToList(),
                                        IsMenu = mn.IsMenu
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

        public ObjectReturnModel GetMenuList(object obj = null)
        {
            try
            {
                using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
                try
                {
                    var menuData = this.FindAll();

                    List<MenuViewModel> menuList = new();
                    foreach (var mn in menuData)
                    {
                        MenuViewModel menu = new();
                        menu.VMenuID = mn.VMenuID;
                        menu.VParentMenuID = mn.VParentMenuID;
                        menu.ISerialNo = mn.ISerialNo;
                        menu.NvMenuName = mn.NvMenuName;
                        menu.NvPageUrl = mn.NvPageUrl;
                        menu.NvFabIcon = mn.NvFabIcon;
                        menu.PrefixCode = mn.PrefixCode;
                        menu.Published = mn.Published;
                        menu.CreatedByPK = mn.CreatedByPK;
                        menu.CreatedDate = mn.CreatedDate;
                        menu.ButtonControls = mn.ButtonControls;
                        menu.IsMenu = mn.IsMenu;
                        menu.Child = new List<MenuViewModel>();
                        menuList.Add(menu);
                    }

                    var pMenu = menuList.Where(x => x.VParentMenuID == null).OrderBy(x => x.ISerialNo).ToList();
                    TMenu = menuList.Where(x => x.VParentMenuID != null).OrderBy(x => x.ISerialNo).ToList();

                    List<MenuViewModel> Menu = new();
                    MenuListBySerial = new List<MenuViewModel>();
                    foreach (var pm in pMenu)
                    {
                        pm.NameWithParent = pm.NvMenuName;
                        MenuListBySerial.Add(pm);
                        MenuViewModel mn = new();
                        GetAllMenu(pm, TMenu.ToList());
                        Menu.Add(mn);
                    }
                    var parent = MenuListBySerial.Where(x => x.NvPageUrl == "#").ToList();
                    var Index = MenuListBySerial.Where(x => x.NvPageUrl != "#").ToList();

                    var data = new ObjectReturnModel { Object1 = MenuListBySerial, Object2 = parent, Object3 = Index };

                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private void GetAllMenu(MenuViewModel pMenu, List<MenuViewModel> menuList)
        {
            List<MenuViewModel> childList = new();
            List<MenuViewModel> tempList = new();
            foreach (var ml in menuList)
            {
                if (pMenu.VMenuID == ml.VParentMenuID)
                {
                    ml.NameWithParent = (pMenu.NameWithParent ?? pMenu.NvMenuName) + " >> " + ml.NvMenuName;
                    childList.Add(ml);
                    TMenu.Remove(ml);
                }
                else
                    tempList.Add(ml);
            }
            foreach (var cl in childList)
            {
                MenuListBySerial.Add(cl);
                GetAllMenu(cl, tempList);
            }
        }

        public async Task<ObjectReturnModel> GetSideMenu()
        {
            try
            {
                using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
                try
                {
                    // get current user id
                    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var roleIds = await _dbCntxt.AspNetUserRoles.Where(x => x.UserId == userId)
                                                .Select(r => r.RoleId).ToListAsync();

                    var menuData = await (from mn in _dbCntxt.AspNetUsersMenu
                                          join anump in _dbCntxt.AspNetUsersMenuPermission on mn.VMenuId equals anump.VMenuId
                                          where roleIds.Contains(anump.RoleId) && mn.Published == true && mn.IsMenu == true
                                          select new
                                          {
                                              mn.VMenuId,
                                              mn.VParentMenuId,
                                              mn.ISerialNo,
                                              mn.NvMenuName,
                                              mn.NvPageUrl,
                                              mn.NvFabIcon,
                                          }).OrderBy(x => x.ISerialNo).Distinct().ToListAsync();

                    List<MenuViewModel> menuList = new();
                    foreach (var mn in menuData)
                    {
                        MenuViewModel menu = new();
                        menu.VMenuID = mn.VMenuId;
                        menu.VParentMenuID = mn.VParentMenuId;
                        menu.ISerialNo = mn.ISerialNo;
                        menu.NvMenuName = mn.NvMenuName;
                        menu.NvPageUrl = mn.NvPageUrl;
                        menu.NvFabIcon = mn.NvFabIcon;
                        menu.Child = new List<MenuViewModel>();
                        menuList.Add(menu);
                    }

                    var pMenu = menuList.Where(x => x.VParentMenuID == null).OrderBy(x => x.ISerialNo).ToList();
                    TMenu = menuList.Where(x => x.VParentMenuID != null).OrderBy(x => x.ISerialNo).ToList();

                    List<MenuViewModel> Menu = new();
                    foreach (var pm in pMenu)
                    {
                        MenuViewModel mn = new();
                        mn = GetMenuHierarchy(pm, TMenu.ToList());
                        Menu.Add(mn);
                    }

                    var data = new ObjectReturnModel { Object1 = Menu };

                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private MenuViewModel GetMenuHierarchy(MenuViewModel pMenu, List<MenuViewModel> menuList)
        {
            List<MenuViewModel> childList = new();
            List<MenuViewModel> tempList = new();
            foreach (var ml in menuList)
            {
                if (pMenu.VMenuID == ml.VParentMenuID)
                {
                    childList.Add(ml);
                    TMenu.Remove(ml);
                }
                else
                    tempList.Add(ml);
            }
            foreach (var cl in childList)
            {
                _ = new MenuViewModel();
                MenuViewModel child = GetMenuHierarchy(cl, tempList);
                pMenu.Child.Add(child);
            }

            return pMenu;
        }

        public IQueryable<AspNetUsersMenu> GetMenuByUrl(string url)
        {
            try
            {
                IQueryable<AspNetUsersMenu> query = null;

                query = _dbCntxt.AspNetUsersMenu.Where(x => x.NvPageUrl == url).AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<MenuViewModel> Add(MenuViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                bool isSerialExists = _dbCntxt.AspNetUsersMenu.Where(x => x.ISerialNo == model.ISerialNo && x.VParentMenuId == model.VParentMenuID && x.VMenuId != model.VMenuID).Any();
                //if (isSerialExists)
                //    return BadRequest("Serial No. Exists");

                var menu = new AspNetUsersMenu
                {
                    VMenuId = GeneralHelper.GenerateGuidAsUniqueKey(),
                    NvMenuName = model.NvMenuName,
                    ISerialNo = model.ISerialNo,
                    NvFabIcon = model.NvFabIcon,
                    Published = model.Published,
                    VParentMenuId = model.VParentMenuID,
                    NvPageUrl = model.NvPageUrl,
                    PrefixCode = model.PrefixCode,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cId,
                    IsMenu = model.IsMenu
                };

                _dbCntxt.AspNetUsersMenu.Add(menu);
                await _dbCntxt.SaveChangesAsync();

                model.VMenuID = menu.VMenuId;
                model.CreatedByPK = menu.CreatedByPK;
                model.CreatedDate = menu.CreatedDate;

                // section for inserting controls
                if (model.SelectedControls != null && model.SelectedControls.Count > 0)
                {
                    _dbCntxt.AspNetUsersMenuPermissionControl.RemoveRange(_dbCntxt.AspNetUsersMenuPermissionControl.Where(x => x.MenuControl.VMenuId == menu.VMenuId));
                    _dbCntxt.AspNetUsersMenuControl.RemoveRange(_dbCntxt.AspNetUsersMenuControl.Where(x => x.VMenuId == menu.VMenuId));
                    foreach (var opt in model.SelectedControls)
                    {
                        AspNetUsersMenuControl aumc = new();
                        aumc.MenuControlId = GeneralHelper.GenerateGuidAsUniqueKey();
                        aumc.VMenuId = menu.VMenuId;
                        aumc.OptionId = opt.id;
                        _dbCntxt.AspNetUsersMenuControl.Add(aumc);
                    }
                    await _dbCntxt.SaveChangesAsync();
                }

                dbContextTransaction.Commit();

                return model;

                // Log Transaction
                // Code Here
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Update(MenuViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                bool isSerialExists = _dbCntxt.AspNetUsersMenu.Where(x => x.ISerialNo == model.ISerialNo && x.VParentMenuId == model.VParentMenuID && x.VMenuId != model.VMenuID).Any();
                //if (isSerialExists)
                //    return BadRequest("Serial No. Exists");

                var menu = _dbCntxt.AspNetUsersMenu.Find(model.VMenuID);
                if (menu != null)
                {
                    menu.NvMenuName = model.NvMenuName;
                    menu.ISerialNo = model.ISerialNo;
                    menu.NvFabIcon = model.NvFabIcon;
                    menu.Published = model.Published;
                    menu.VParentMenuId = model.VParentMenuID;
                    menu.NvPageUrl = model.NvPageUrl;
                    menu.PrefixCode = model.PrefixCode;
                    menu.ModifiedDate = DateTime.Now;
                    menu.ModifiedByPK = cId;
                    menu.IsMenu = model.IsMenu;
                    _dbCntxt.Entry(menu).State = EntityState.Modified;
                    //await _dbCntxt.SaveChangesAsync();

                    // section for inserting controls
                    _dbCntxt.AspNetUsersMenuPermissionControl.RemoveRange(_dbCntxt.AspNetUsersMenuPermissionControl.Where(x => x.MenuControl.VMenuId == menu.VMenuId));
                    _dbCntxt.AspNetUsersMenuControl.RemoveRange(_dbCntxt.AspNetUsersMenuControl.Where(x => x.VMenuId == menu.VMenuId));
                    if (model.SelectedControls != null && model.SelectedControls.Count > 0)
                    {
                        foreach (var opt in model.SelectedControls)
                        {
                            AspNetUsersMenuControl aumc = new();
                            aumc.MenuControlId = GeneralHelper.GenerateGuidAsUniqueKey();
                            aumc.VMenuId = menu.VMenuId;
                            aumc.OptionId = opt.id;
                            _dbCntxt.AspNetUsersMenuControl.Add(aumc);
                        }   
                    }

                    await _dbCntxt.SaveChangesAsync();

                    dbContextTransaction.Commit();

                    // Log Transaction
                    // Code Here
                }
                else
                    throw new CustomException("Menu is not found.", 404);
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
                var id = Convert.ToString(obj);

                var menu = _dbCntxt.AspNetUsersMenu.Find(id);

                if (menu != null)
                {
                    _dbCntxt.AspNetUsersMenuPermissionControl.RemoveRange(_dbCntxt.AspNetUsersMenuPermissionControl.Where(x => x.MenuControl.VMenuId == menu.VMenuId));
                    _dbCntxt.AspNetUsersMenuControl.RemoveRange(_dbCntxt.AspNetUsersMenuControl.Where(x => x.VMenuId == menu.VMenuId));
                    _dbCntxt.AspNetUsersMenu.RemoveRange(_dbCntxt.AspNetUsersMenu.Where(x => x.VMenuId == id));
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                    throw new CustomException("Menu is not found.", 404);

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

        public async Task DeleteAll(MenuViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var ids = model.DsList.Select(o => o.Id).ToArray();

                _dbCntxt.AspNetRoles.Remove(_dbCntxt.AspNetRoles.Where(x => x.Id == model.VMenuID).FirstOrDefault());
                await _dbCntxt.SaveChangesAsync();

                foreach (var ds in model.DsList)
                {
                    var sql = "DELETE FROM AspNetUsersMenu WHERE VMenuID = {0}";
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

        public PagingResponse<MenuViewModel> PagingFeature(IQueryable<MenuViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<MenuViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<MenuViewModel>(query, paging);
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

        public ObjectReturnModel GetMenuPerRole(string roleId)
        {
            try
            {
                IEnumerable<MenuViewModel> source = null;
                source = (from mn in _dbCntxt.AspNetUsersMenu
                          join pm in _dbCntxt.AspNetUsersMenuPermission on mn.VMenuId equals pm.VMenuId
                          into UserPermission
                          from pm in UserPermission.DefaultIfEmpty()
                          where mn.Published == true && pm.RoleId == roleId
                          select new MenuViewModel
                          {                              
                              VMenuID = mn.VMenuId,
                              RoleId = pm.RoleId,
                              VParentMenuID = mn.VParentMenuId,
                              ISerialNo = mn.ISerialNo,
                              NvMenuName = mn.NvMenuName,
                              NvPageUrl = mn.NvPageUrl,
                              NvFabIcon = mn.NvFabIcon,
                              PrefixCode = mn.PrefixCode,
                              Published = mn.Published,
                              ButtonControls = (from w in _dbCntxt.AspNetUsersMenuControl
                                                where w.VMenuId == mn.VMenuId
                                                select new UserMenuControlViewModel { Id = w.OptionId, MenuControlId = w.MenuControlId, Name = w.Option.Name, Description = w.Option.Description }).ToList(),
                              SelectedRoleControls = (from w in _dbCntxt.AspNetUsersMenuPermissionControl
                                                  where w.MenuPermission.VMenuId == mn.VMenuId && w.MenuPermission.RoleId == pm.RoleId
                                                  select new IdListStr { id = w.MenuControlId, description = w.MenuControl.Option.Description }).ToList(),
                              IsChecked = pm.VMenuId != null,
                              ModifiedByPK = mn.ModifiedByPK,
                              ModifiedDate = mn.ModifiedDate,
                              CreatedByPK = mn.CreatedByPK,
                              CreatedDate = mn.CreatedDate
                          }).OrderBy(x => x.ISerialNo).ToList();

                // parent menu list
                var pMenu = source.Where(x => x.VParentMenuID == null).OrderBy(x => x.ISerialNo).ToList();
                TMenu = source.Where(x => x.VParentMenuID != null).OrderBy(x => x.ISerialNo).ToList();

                List<MenuViewModel> Menu = new();
                MenuListBySerial = new List<MenuViewModel>();
                foreach (var pm in pMenu)
                {
                    pm.NameWithParent = pm.NvMenuName;
                    MenuListBySerial.Add(pm);
                    MenuViewModel mn = new();
                    GetAllMenu(pm, TMenu.ToList());
                    Menu.Add(mn);
                }
                var parent = MenuListBySerial.Where(x => x.NvPageUrl == "#").ToList();
                var Index = MenuListBySerial.Where(x => x.NvPageUrl != "#").ToList();

                var data = new ObjectReturnModel { Object1 = MenuListBySerial, Object2 = parent, Object3 = Index };

                return data;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
