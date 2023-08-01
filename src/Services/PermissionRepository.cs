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
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Transactions;
using System.Text;

namespace workflow.Services
{
    public class PermissionRepository : BaseApi, IPermission
    {
        readonly IMenu _menu;
        public PermissionRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<UserRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor,
                IMenu menu, IEmailSender emailSender) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            this._menu = menu;
        }

        public async Task<Dictionary<string, ControlViewModel>> GetPermissionControl(string path)
        {
            var permissionCtrl = new Dictionary<string, ControlViewModel>();

            try
            {
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var vMenus = this._menu.GetMenuByUrl(path).SingleOrDefault();

                string vMenuId = "";

                if (vMenus == null)
                    throw new CustomException("Path: " + path + " is not found in the database. ", 404);

                vMenuId = vMenus.VMenuId;

                var permissionlist = await _dbCntxt.ControlViewModel
                                  .FromSqlRaw<ControlViewModel>("EXEC spPermissionControls {0}, {1}", cId, vMenuId).ToListAsync();

                var ctl = permissionlist.FirstOrDefault();

                //var ctl = await _dbCntxt.ControlViewModel
                //                .FromSqlInterpolated<ControlViewModel>($"EXEC [dbo].[spPermissionControls] @UserID = {cId}, @vMenuID={vMenuId}").FirstOrDefaultAsync();

                permissionCtrl.Add("ctl", ctl);

                var menuData = await _dbCntxt.AspNetUsersMenu.Where(x => x.VParentMenuId == vMenuId).ToListAsync();
                if (menuData.Count != 0)
                {
                    int i = 0;
                    foreach (var mn in menuData)
                    {
                        i++;
                        //var m = await _dbCntxt.ControlViewModel
                        //        .FromSqlInterpolated<ControlViewModel>($"EXEC [dbo].[spPermissionControls] @UserID = {cId}, @vMenuID={vMenuId}").FirstOrDefaultAsync();
                        var permissionlistmn = await _dbCntxt.ControlViewModel
                                  .FromSqlRaw<ControlViewModel>("EXEC spPermissionControls {0}, {1}", cId, mn.VMenuId).ToListAsync();

                        var m = permissionlistmn.FirstOrDefault();

                        permissionCtrl.Add("ctl" + i, m);
                    }
                }
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return permissionCtrl;
        }
    }
}
