using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;
using workflow.Models.ManageViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace workflow.Services
{
    public class CronJobRepository : BaseApi, ICronJobs
    {

        private readonly IUser _user;

        public CronJobRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<SettingRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor,
                IUser user) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _user = user;
        }

        public async Task RemoveLoginAttemptsAndTaggedAsDormant()
        {
            try
            {
                //Remove login attempts every midnight
                using (var dbContextTransaction = _dbCntxt.Database.BeginTransaction())
                {
                    try
                    {
                        var users = await _user.FindAll()
                                                .Where
                                                (
                                                    u =>
                                                    (u.State == UserAccountState.Active || u.State == UserAccountState.Soft_Locked) &&
                                                    u.LoginAttempts > 0 &&
                                                    u.Status == true
                                                )
                                                .ToListAsync();

                        var userstofind = users
                                            .Where
                                            (
                                                u =>
                                                (DateTime.Now.Date - Convert.ToDateTime(u.LoginAttemptsDate == null ? "1/1/1900" : u.LoginAttemptsDate).Date).TotalDays > 0
                                            )
                                            .Select(u => u.Id)
                                            .ToList();

                        if (userstofind.Count > 0)
                        {
                            var concatresult = string.Join(",", userstofind.Select(x => "'" + x.ToString() + "'").ToArray());
                            await _dbCntxt.Database.ExecuteSqlRawAsync("UPDATE dbo.AspNetUsers SET LoginAttempts = 0, LoginAttemptsDate = NULL, State = CASE WHEN State = 32 THEN 1 ELSE State END  WHERE Id in (" + concatresult + ")");
                        }

                        dbContextTransaction.Commit();

                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }


                var settings = await _dbCntxt.Setting.ToListAsync();

                var dormateffectivity = Convert.ToInt32(settings.FirstOrDefault(s => s.VSettingId == "_DORMANTSTATEEFFECTIVITY").VSettingOption);

                //Process dormat users
                using (var dbContextTransaction = _dbCntxt.Database.BeginTransaction())
                {    
                    try
                    {
                        var users = await _user.FindAll()
                                                .Where
                                                (
                                                    u =>
                                                    u.Status == true &&
                                                    u.State != UserAccountState.Dormant_User
                                                )
                                                .ToListAsync();

                        var userstofind = users
                                                .Where
                                                (
                                                    u =>
                                                    (DateTime.Now.Date - Convert.ToDateTime(u.LastLogin == null ? "12/31/9999" : u.LastLogin).Date).TotalDays > dormateffectivity
                                                )
                                                .Select(u => u.Id)
                                                .ToList();

                        if (userstofind.Count > 0)
                        {
                            var concatresult = string.Join(",", userstofind.Select(x => "'" + x.ToString() + "'").ToArray());
                            await _dbCntxt.Database.ExecuteSqlRawAsync("UPDATE dbo.AspNetUsers SET LoginAttempts = 0, LoginAttemptsDate = NULL, State = 16 WHERE Id in (" + concatresult + ")");
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }   
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task TagAsPasswordExpired()
        {
            try
            {

                var settings = await _dbCntxt.Setting.ToListAsync();

                //Process for initial password expiration
                using (var dbContextTransaction = _dbCntxt.Database.BeginTransaction())
                {
                    try
                    {
                        var users = await _user.FindAll()
                                                .Where
                                                (
                                                    u =>
                                                    (u.State == UserAccountState.Initial_Password) &&
                                                    u.Status == true
                                                )
                                                .ToListAsync();

                        var userstofind = users
                                            .Where
                                            (
                                                u =>
                                                DateTime.Now > u.InitialPasswordExpirationDate
                                            )
                                            .Select(u => u.Id)
                                            .ToList();

                        if (userstofind.Count > 0)
                        {
                            var concatresult = string.Join(",", userstofind.Select(x => "'" + x.ToString() + "'").ToArray());
                            await _dbCntxt.Database.ExecuteSqlRawAsync("UPDATE dbo.AspNetUsers SET State = 4 WHERE Id in (" + concatresult + ")");
                        }

                        dbContextTransaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }
                }

                //Process for password expiration
                using (var dbContextTransaction = _dbCntxt.Database.BeginTransaction())
                {
                    try
                    {
                        var users = await _user.FindAll()
                                                .Where
                                                (
                                                    u =>
                                                    u.IsPasswordNeverExpires == false &&
                                                    u.Status == true &&
                                                    (
                                                        u.State == UserAccountState.Active ||
                                                        u.State == UserAccountState.Soft_Locked ||
                                                        u.State == UserAccountState.Locked
                                                    )
                                                )
                                                .ToListAsync();

                        var userstofind = users
                                                .Where
                                                (
                                                    u =>
                                                    DateTime.Now > u.ExpirationDate
                                                )
                                                .Select(u => u.Id)
                                                .ToList();

                        if (userstofind.Count > 0)
                        {
                            var concatresult = string.Join(",", userstofind.Select(x => "'" + x.ToString() + "'").ToArray());
                            await _dbCntxt.Database.ExecuteSqlRawAsync("UPDATE dbo.AspNetUsers SET LoginAttempts = 0, LoginAttemptsDate = NULL, State = 8 WHERE Id in (" + concatresult + ")");
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
