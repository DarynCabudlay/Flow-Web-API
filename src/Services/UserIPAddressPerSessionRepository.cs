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
using workflow.Data;

namespace workflow.Services
{
    public class UserIPAddressPerSessionRepository : BaseApi, IUserIPAddressPerSession
    {
        public UserIPAddressPerSessionRepository(FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestCategoryRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Remove(int spid)
        {
            try
            {
                _dbCntxt.UserIPAddressPerSessions.RemoveRange(_dbCntxt.UserIPAddressPerSessions.Where(u => u.SpId == spid).ToList());
                await _dbCntxt.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task Save(UserIPAddressPerSessionsViewModel model)
        {
            try
            {

                UserIPAddressPerSession userip = new UserIPAddressPerSession();
                userip.SpId = model.SpId;
                userip.UserId = model.UserId;
                userip.IPAddress = model.IPAddress;
                _dbCntxt.UserIPAddressPerSessions.Add(userip);
                await _dbCntxt.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public UserIPAddressPerSessionsViewModel AddData(int spid, string userid, string ipaddress)
        {
            UserIPAddressPerSessionsViewModel _userip = new UserIPAddressPerSessionsViewModel()
            {
                SpId = spid,
                UserId = userid,
                IPAddress = ipaddress
            };

            return _userip;
        }

        public async Task<int> GetSPID()
        {
           try
           {
                int spid = await _dbCntxt.SqlSessions
                                        .FromSqlRaw("Select Convert(Int, @@SPID) as SpId")
                                        .Select(c => c.SpId)
                                        .FirstOrDefaultAsync();
                return spid;
           }
           catch (Exception ex)
           {
               throw new Exception(ex.Message, ex.InnerException);
           }

        }
    }
}
