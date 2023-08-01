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
using workflow.Helpers;

namespace workflow.Services
{
    public class LogRepository : BaseApi, ILog
    {
        readonly IUser _user;
        public LogRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<LogRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor,
                IUser user) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _user = user;
        }
        public async Task<ObjectReturnModel> GetLogDashboard(int offset)
        {
            try
            {
                var userlist = await (_dbCntxt.AspNetUsers).ToListAsync();
                var totalUser = userlist.Count;
                var activeUser = userlist.Where(x => x.EmailConfirmed == true).Count();
                var totalLogin = await (_dbCntxt.AspNetUsersLoginHistory).CountAsync();
                var roles = await (_dbCntxt.AspNetRoles).ToListAsync();
                var pages = await (from anupv in _dbCntxt.AspNetUsersPageVisited
                                   select new
                                   {
                                       anupv.VPageVisitedId,
                                       anupv.UserId,
                                       anupv.DDateVisited,
                                       anupv.NvPageName,
                                       anupv.NvIpaddress
                                   }).ToListAsync();
                var pagess = (pages.GroupBy(x => x.NvPageName).OrderByDescending(x => x.Count())).ToList();
                var totalPageVisit = pages.Count;

                var RWUDataList = new List<dynamic>();
                var rwuLabels = new List<string>();
                var rwuData = new List<int>();
                var rwuBackgroundColor = new List<string>();
                var random = new Random();
                foreach (var role in roles)
                {
                    rwuLabels.Add(role.Name);
                    var userCount = _dbCntxt.AspNetUserRoles.Where(x => x.RoleId == role.Id).Count();
                    rwuData.Add(userCount);
                    var color = string.Format("#{0:X6}", random.Next(0x100000));
                    rwuBackgroundColor.Add(color);
                }
                RWUDataList.Add(rwuLabels);
                RWUDataList.Add(rwuData);
                RWUDataList.Add(rwuBackgroundColor);

                var TRUDataList = new List<dynamic>();
                var truLabels = new List<string>();
                var truData = new List<int>();
                var lhData = new List<List<int>>();
                DateTime today = DateTime.UtcNow;
                today = today.AddMinutes(offset);
                for (var i = 5; i >= 0; i--)
                {
                    var predate = today.AddMonths(-i);
                    var month = predate.ToString("MMM");
                    var userCount = (from ur in userlist
                                     where ur.Date.AddMinutes(offset).Month == predate.Month && ur.Date.AddMinutes(offset).Year == predate.Year
                                     select ur).Count();

                    var countList = new List<int>();
                    var days = DateTime.DaysInMonth(predate.Year, predate.Month);
                    for (var j = 1; j <= days; j++)
                    {
                        var loginCount = (from lh in _dbCntxt.AspNetUsersLoginHistory
                                          where lh.DLogIn.AddMinutes(offset).Month == predate.Month && lh.DLogIn.AddMinutes(offset).Year == predate.Year && lh.DLogIn.AddMinutes(offset).Day == j
                                          select lh).Count();
                        countList.Add(loginCount);
                        if (i == 0 && today.Day == j)
                            break;
                    }

                    lhData.Add(countList);
                    truLabels.Add(month);
                    truData.Add(userCount);
                }
                TRUDataList.Add(truLabels);
                TRUDataList.Add(truData);

                var TPVDataList = new List<dynamic>();
                var tpvLabels = new List<string>();
                var tpvData = new List<int>();
                foreach (var p in pagess)
                {
                    tpvLabels.Add(p.Key[1..]);
                    tpvData.Add(p.Count());
                }
                TPVDataList.Add(tpvLabels);
                TPVDataList.Add(tpvData);             

                var data = new ObjectReturnModel { Object1 = totalUser, Object2 = activeUser, Object3 = totalLogin, Object4 = totalPageVisit, Object5 = RWUDataList, Object6 = TRUDataList, Object7 = lhData, Object8 = TPVDataList };

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task LogPageVisit(string path)
        {
            try 
            {
                using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
                try
                {
                    // get current user id
                    var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                    AspNetUsersPageVisited anupv = new();
                    anupv.VPageVisitedId = GeneralHelper.GenerateGuidAsUniqueKey();
                    anupv.UserId = cId;
                    anupv.NvPageName = path;
                    anupv.DDateVisited = DateTime.UtcNow;
                    anupv.NvIpaddress = ip;
                    _dbCntxt.AspNetUsersPageVisited.Add(anupv);
                    await _dbCntxt.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<ObjectReturnModel> GetPageVisitHistoryData()
        {
            try 
            {
                var visitedList = await _user.GetPageVisited()
                                   .Select(x =>
                                        new
                                        {
                                            x.UserId,
                                            x.Name,
                                            x.Email,
                                            x.PageName,
                                            x.VisitedDate,
                                            x.IP
                                        })
                                   .ToListAsync();

                var totalVisit = visitedList.Count;
                var highestVisit = (from vl in visitedList
                                    group vl.PageName by vl.PageName into l
                                    select new
                                    {
                                        PageName = l.Key,
                                        l.ToList().Count
                                    }).OrderByDescending(x => x.Count).FirstOrDefault();
                var highestVisitedBy = (from vl in visitedList
                                        group vl.UserId by vl.UserId into l
                                        select new
                                        {
                                            Id = l.Key,
                                            l.ToList().Count,
                                            visitedList.Where(x => x.UserId == l.Key).FirstOrDefault().Name
                                        }).OrderByDescending(x => x.Count).FirstOrDefault();

                var data = new ObjectReturnModel { Object1 = visitedList, Object2 = totalVisit, Object3 = highestVisit.PageName, Object4 = highestVisitedBy.Name };

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<ObjectReturnModel> GetLoginHistoryData()
        {
            try 
            {
                var loginList = await _user.GetLoginHistory()
                                   .Select(x => 
                                        new 
                                        { 
                                            x.UserId,
                                            x.Name,
                                            x.Email,
                                            x.Login, 
                                            x.Logout,
                                            x.IP 
                                        })
                                   .ToListAsync();

                var totalLogin = loginList.Count;
                var highestLogin = (from ll in loginList
                                    group ll.UserId by ll.UserId into l
                                    select new
                                    {
                                        Id = l.Key,
                                        l.ToList().Count,
                                        loginList.Where(x => x.UserId == l.Key).FirstOrDefault().Name
                                    }).OrderByDescending(x => x.Count).FirstOrDefault();

                var data = new ObjectReturnModel { Object1 = loginList, Object2 = totalLogin, Object3 = highestLogin.Name, Object4 = highestLogin.Count };

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
