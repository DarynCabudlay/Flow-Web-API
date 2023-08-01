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
    public class HolidayRepository : BaseApi, IHoliday
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public HolidayRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<HolidayRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
        }

        public async Task<HolidayViewModel> Add(HolidayViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Holiday Name is required.", 400);

            if (model.WithFixedSchedule)
            {
                if (model.FixedScheduleMonth == null || model.FixedScheduleMonth <= 0)
                    throw new CustomException("Fixed Schedule by month is required.", 400);

                if (model.FixedScheduleDay == null || model.FixedScheduleDay <= 0)
                    throw new CustomException("Fixed Schedule by day is required.", 400);
            }
                
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (await this.IfExists(model.Name))
                    throw new CustomException("Holiday Name is already existing.", 400);
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                await _userIpAddressPerSession.Save(userIp);
                //Insert Holiday
                var holiday = new Holiday
                {
                    Name = model.Name,
                    WithFixedSchedule = model.WithFixedSchedule,
                    FixedScheduleMonth = model.FixedScheduleMonth,
                    FixedScheduleDay = model.FixedScheduleDay,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.Holidays.Add(holiday);
                await _dbCntxt.SaveChangesAsync();

                model.Id = holiday.Id;
                model.CreatedByPK = holiday.CreatedByPK;
                model.CreatedDate = holiday.CreatedDate;
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
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                int id = (int)obj;
                var holiday = await this.FindAll().Where(h => h.Id == id).FirstOrDefaultAsync();

                if (holiday != null)
                {
                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);                    
                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);
                    //remove holiday
                    _dbCntxt.Holidays.RemoveRange(_dbCntxt.Holidays.Where(h => h.Id == id));
                    await _dbCntxt.SaveChangesAsync();
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);
                }
                else
                    throw new CustomException("Holiday is not found.", 404);

                dbContextTransaction.Commit();
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

        public async Task DeleteAll(HolidayViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<HolidayViewModel> Find(object obj)
        {
            try
            {

                IQueryable<HolidayViewModel> query = null;

                var condition = (int)obj;

                query = this.Get()
                            .Where
                            (
                                h =>
                                h.Id == condition
                            )
                            .Select
                            (
                                h => new HolidayViewModel
                                {
                                    Id = h.Id,
                                    Name = h.Name,
                                    WithFixedSchedule = h.WithFixedSchedule,
                                    WithFixedScheduleText = h.WithFixedSchedule ? "Yes" : "No",
                                    FixedScheduleMonth = h.FixedScheduleMonth,
                                    FixedScheduleDay = h.FixedScheduleDay,
                                    FixedScheduleInText = (h.FixedScheduleMonth == 1 ? "January " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 2 ? "February " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 3 ? "March " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 4 ? "April " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 5 ? "May " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 6 ? "June " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 7 ? "July " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 8 ? "August " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 9 ? "September " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 10 ? "October " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 11 ? "November " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 12 ? "December " + h.FixedScheduleDay.ToString() : "")))))))))))),
                                    Published = h.Published,
                                    CreatedByPK = h.CreatedByPK,
                                    CreatedBy = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == h.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == h.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : h.CreatedByPK,
                                    CreatedDate = h.CreatedDate,
                                    ModifiedByPK = h.ModifiedByPK,
                                    ModifiedBy = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == h.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == h.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : h.ModifiedByPK,
                                    ModifiedDate = h.ModifiedDate
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

        public IQueryable<HolidayViewModel> FindAll()
        {
            try
            {
                IQueryable<HolidayViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                h => new HolidayViewModel
                                {
                                    Id = h.Id,
                                    Name = h.Name,
                                    WithFixedSchedule = h.WithFixedSchedule,
                                    WithFixedScheduleText = h.WithFixedSchedule ? "Yes" : "No",
                                    FixedScheduleMonth = h.FixedScheduleMonth,
                                    FixedScheduleDay = h.FixedScheduleDay,
                                    FixedScheduleInText = (h.FixedScheduleMonth == 1 ? "January " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 2 ? "February " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 3 ? "March " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 4 ? "April " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 5 ? "May " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 6 ? "June " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 7 ? "July " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 8 ? "August " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 9 ? "September " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 10 ? "October " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 11 ? "November " + h.FixedScheduleDay.ToString() : (h.FixedScheduleMonth == 12 ? "December " + h.FixedScheduleDay.ToString() : "")))))))))))),
                                    Published = h.Published,
                                    CreatedByPK = h.CreatedByPK,
                                    CreatedBy = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == h.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == h.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : h.CreatedByPK,
                                    CreatedDate = h.CreatedDate,
                                    ModifiedByPK = h.ModifiedByPK,
                                    ModifiedBy = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == h.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == h.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : h.ModifiedByPK,
                                    ModifiedDate = h.ModifiedDate
                                }
                            )
                            .AsQueryable();

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

        public IQueryable<HolidayViewModel> GetHolidays(PagingRequest paging = null, bool? published = null)
        {
            try
            {
                IQueryable<HolidayViewModel> query = null;
                query = this.FindAll()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(h => h.Published == published);

                if (paging != null)
                {
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.Name.Contains(search) || p.WithFixedScheduleText.Contains(search) || p.WithFixedScheduleText.Contains(search));
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

        public IQueryable<HolidayCalendarViewModel> GetHolidayCalendar(PagingRequest paging = null, bool? published = null, bool? holidayPublished = null)
        {
            try
            {
                IQueryable<HolidayCalendarViewModel> query = null;
                query = this.GetHolidayDates()
                            .Select
                            (
                                h => new HolidayCalendarViewModel
                                {
                                    Id = h.Id,
                                    HolidayId = h.HolidayId,
                                    Name = h.Holiday.Name,
                                    Date = h.Date,
                                    HolidayTypeId = h.HolidayTypeId,
                                    HolidayType = _dbCntxt.Options.FirstOrDefault(o => o.OptionGroup == "Holiday Type" && o.Id == h.HolidayTypeId).Name,
                                    HolidayPublished = h.Holiday.Published,
                                    Published = h.Published,
                                    CreatedByPK = h.CreatedByPK,
                                    CreatedBy = Convert.ToString(_dbCntxt.AspNetUsersProfile
                                                        .Where(u => u.Id == h.CreatedByPK)
                                                        .Select
                                                        (
                                                            u => new
                                                            {
                                                                Name = u.LastName + ", " + u.FirstName
                                                            }
                                                        ).FirstOrDefault().Name),
                                    CreatedDate = h.CreatedDate,
                                    ModifiedByPK = h.ModifiedByPK,
                                    ModifiedBy = Convert.ToString(_dbCntxt.AspNetUsersProfile
                                                        .Where(u => u.Id == h.ModifiedByPK)
                                                        .Select
                                                        (
                                                            u => new
                                                            {
                                                                Name = u.LastName + ", " + u.FirstName
                                                            }
                                                        ).FirstOrDefault().Name),
                                    ModifiedDate = h.ModifiedDate,
                                    HolidayOffices = _dbCntxt.HolidayAffectedOffices
                                                             .Where(ho => ho.HolidayDateId == h.Id)
                                                             .Select
                                                             (
                                                                ho => new HolidayOfficeViewModel()
                                                                {
                                                                    HolidayOfficeId = ho.Id,
                                                                    HolidayDateId = ho.HolidayDateId,
                                                                    Id = ho.OfficeId,
                                                                    OfficeId = ho.OfficeId,
                                                                    Office = ho.Office.Name,
                                                                    Name = ho.Office.Name,
                                                                    CreatedByPK = ho.CreatedByPK,
                                                                    CreatedBy = Convert.ToString(_dbCntxt.AspNetUsersProfile
                                                                                                .Where(u => u.Id == ho.CreatedByPK)
                                                                                                .Select
                                                                                                (
                                                                                                    u => new
                                                                                                    {
                                                                                                        Name = u.LastName + ", " + u.FirstName
                                                                                                    }
                                                                                                ).FirstOrDefault().Name),
                                                                    CreatedDate = h.CreatedDate,
                                                                }
                                                             ).ToList()
                                }
                            )
                            .AsQueryable();

                if (published != null)
                    query = query.Where(h => h.Published == published);

                if (holidayPublished != null)
                    query = query.Where(h => h.HolidayPublished == holidayPublished);

                if (paging != null)
                {
                    if (paging.SearchCriteria != null)
                    {
                        int calendarYear = Convert.ToInt32((paging.SearchCriteria.Id1 == null ? 0 : paging.SearchCriteria.Id1));

                        if (calendarYear > 0)
                            query = query.Where(p => p.Date.Year == calendarYear);
                    }

                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.Name.Contains(search) || p.HolidayType.Contains(search));
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

        public async Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            bool isExists = false;
            int condition = 0;

            try
            {
                if (entityPrimaryKey == null)
                {
                    var holidayInDb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch);

                    if (holidayInDb != null)
                        isExists = true;
                }
                else
                {
                    condition = Convert.ToInt32(entityPrimaryKey);
                    var requesttypesindb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch && r.Id != condition);

                    if (requesttypesindb != null)
                        isExists = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return isExists;
        }

        public PagingResponse<HolidayViewModel> PagingFeature(IQueryable<HolidayViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<HolidayViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<HolidayViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<HolidayCalendarViewModel> PagingFeatureCalendar(IQueryable<HolidayCalendarViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<HolidayCalendarViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<HolidayCalendarViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task SaveHolidayDates(HolidayCalendarViewModel model)
        {
            if (model == null)
                throw new CustomException("Kindly provide atleast one holiday calendar to proceed.", 400);

            if (model.HolidayId <= 0)
                throw new CustomException("Invalid entry for Holiday.", 400);

            if (model.HolidayTypeId <= 0)
                throw new CustomException("Holiday Type is required.", 400);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                //Validate Holiday every year
                int holidayYear = model.Date.Year;
                if (model.Id <= 0)
                {
                    var checkHoliday = await _dbCntxt.HolidayDates.FirstOrDefaultAsync(h => h.HolidayId == model.HolidayId && h.Date.Year == holidayYear);

                    if (checkHoliday != null)
                        throw new CustomException("Duplicate Holiday in Year " + holidayYear.ToString(), 400);

                    var checkDuplicateDate = await _dbCntxt.HolidayDates
                                                           .Include(h => h.Holiday)
                                                           .FirstOrDefaultAsync(h => h.Date.Date == model.Date.Date);

                    if (checkDuplicateDate != null)
                        throw new CustomException("Date: " + model.Date.ToShortDateString() + " was already assigned to Holiday: " + checkDuplicateDate.Holiday.Name, 400);
                }
                else
                {
                    var checkHoliday = await _dbCntxt.HolidayDates.FirstOrDefaultAsync(h => h.HolidayId == model.HolidayId && h.Date.Year == holidayYear && h.Id != model.Id);

                    if (checkHoliday != null)
                        throw new CustomException("Duplicate Holiday in Year " + holidayYear.ToString(), 400);

                    var checkDuplicateDate = await _dbCntxt.HolidayDates
                                                           .Include(h => h.Holiday)
                                                           .FirstOrDefaultAsync(h => h.Date.Date == model.Date.Date && h.Id != model.Id);

                    if (checkDuplicateDate != null)
                        throw new CustomException("Date: " + model.Date.ToShortDateString() + " was already assigned to Holiday: " + checkDuplicateDate.Holiday.Name, 400);
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

                //validate holiday offices model
                if (model.HolidayOffices != null)
                {
                    if (model.HolidayOffices.Count > 0)
                    {
                        var holidayOfficeIds = model.HolidayOffices.Select(c => c.HolidayOfficeId);

                        //Remove GUI deleted holiday office in DB
                        _dbCntxt.HolidayAffectedOffices.RemoveRange(_dbCntxt.HolidayAffectedOffices.Where(r => r.HolidayDateId == model.Id && !holidayOfficeIds.Contains(r.Id)));
                        await _dbCntxt.SaveChangesAsync();

                        foreach (var office in model.HolidayOffices)
                        {
                            if (office.OfficeId <= 0)
                                throw new CustomException("Invalid entry for office.", 400);

                            var checkIfOffice = await _dbCntxt.Options.FirstOrDefaultAsync(o => o.Id == office.OfficeId && o.OptionGroup == "Site Locations");

                            if (checkIfOffice == null)
                                throw new CustomException("Office entry not found.", 404);

                            if (this.CheckIfDuplicateOfficeInModel(office, model.HolidayOffices))
                                throw new CustomException("Duplicate Office Entry in Model.", 404);

                            if (this.CheckIfDuplicateOfficeInDatabase(office))
                                throw new CustomException("Duplicate Office Entry in Database.", 404);
                        }
                    }
                }

                HolidayDate holidayDate = new HolidayDate();

                if (model.Id <= 0)
                {
                    holidayDate.HolidayId = model.HolidayId;
                    holidayDate.HolidayTypeId = model.HolidayTypeId;
                    holidayDate.Date = model.Date.Date;
                    holidayDate.Published = model.Published;
                    holidayDate.CreatedByPK = cid;
                    holidayDate.CreatedDate = DateTime.Now;

                    _dbCntxt.HolidayDates.Add(holidayDate);
                }
                else
                {
                    holidayDate = await _dbCntxt.HolidayDates.FirstOrDefaultAsync(h => h.Id == model.Id);

                    if (holidayDate != null)
                    {
                        holidayDate.HolidayId = model.HolidayId;
                        holidayDate.HolidayTypeId = model.HolidayTypeId;
                        holidayDate.Date = model.Date.Date;
                        holidayDate.Published = model.Published;
                        holidayDate.ModifiedByPK = cid;
                        holidayDate.ModifiedDate = DateTime.Now;

                        _dbCntxt.Entry(holidayDate).State = EntityState.Modified;
                    }
                }

                await _dbCntxt.SaveChangesAsync();

                bool withOffices = false;
                if (model.HolidayOffices != null)
                {
                    if (model.HolidayOffices.Count > 0)
                    {
                        withOffices = true;

                        model.HolidayOffices
                             .ForEach(h => h.HolidayDateId = holidayDate.Id);

                        var holidayOfficesIds = model.HolidayOffices.Select(c => c.HolidayOfficeId);
                        var concatHolidayOfficesIds = string.Join(",", holidayOfficesIds.Select(x => x.ToString()).ToArray());
                        //delete all holiday offices from DB that are not included in current model
                        string sqlQuery = @"Declare @HolidayDateId Int
                                        Set @HolidayDateId = " + holidayDate.Id + @"

                                        Select Id into #TempHolidayDates from dbo.HolidayAffectedOffices 
                                        Where
	                                        HolidayDateId = @HolidayDateId And
                                            Id Not in (" + concatHolidayOfficesIds + @")

                                        Delete from dbo.HolidayAffectedOffices Where Id in (Select Id from #TempHolidayDates)

                                        Drop Table #TempHolidayDates";

                        _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                        await _dbCntxt.SaveChangesAsync();
                        //save holiday office
                        foreach (var officeInfo in model.HolidayOffices)
                        {
                            //new holiday office
                            var holidayOffice = new HolidayAffectedOffice();

                            if (officeInfo.HolidayOfficeId <= 0)
                            {
                                holidayOffice.HolidayDateId = holidayDate.Id;
                                holidayOffice.OfficeId = officeInfo.OfficeId;
                                holidayOffice.CreatedByPK = cid;
                                holidayOffice.CreatedDate = DateTime.Now;

                                _dbCntxt.HolidayAffectedOffices.Add(holidayOffice);
                            }
                            else
                            {
                                holidayOffice = await _dbCntxt.HolidayAffectedOffices.Where(o => o.Id == officeInfo.HolidayOfficeId).FirstOrDefaultAsync();

                                if (holidayOffice != null)
                                {
                                    if (holidayOffice.OfficeId != officeInfo.OfficeId)
                                    {
                                        holidayOffice.OfficeId = officeInfo.OfficeId;
                                        holidayOffice.CreatedByPK = cid;
                                        holidayOffice.CreatedDate = DateTime.Now;
                                        _dbCntxt.Entry(holidayOffice).State = EntityState.Modified;
                                    }
                                }
                            }

                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }

                if (!withOffices)
                {
                    //delete all holiday offices from DB that once current model is empty
                    string sqlQuery = @"Declare @HolidayDateId Int
                                    Set @HolidayDateId = " + holidayDate.Id + @"

                                    Select Id into #TempHolidayDates from dbo.HolidayAffectedOffices 
                                    Where
	                                    HolidayDateId = @HolidayDateId

                                    Delete from dbo.HolidayAffectedOffices Where Id in (Select Id from #TempHolidayDates)

                                    Drop Table #TempHolidayDates";

                    _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                    await _dbCntxt.SaveChangesAsync();
                }

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
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

        public async Task Update(HolidayViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Holiday Name is required.", 400);

            if (model.WithFixedSchedule)
            {
                if (model.FixedScheduleMonth == null || model.FixedScheduleMonth <= 0)
                    throw new CustomException("Fixed Schedule by month is required.", 400);

                if (model.FixedScheduleDay == null || model.FixedScheduleDay <= 0)
                    throw new CustomException("Fixed Schedule by day is required.", 400);
            }

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (await this.IfExists(model.Name, model.Id))
                    throw new CustomException("Holiday Name is already existing.", 400);

                var holiday = await _dbCntxt.Holidays.FirstOrDefaultAsync(h => h.Id == model.Id);

                if (holiday != null)
                {
                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);

                    holiday.Name = model.Name;
                    holiday.WithFixedSchedule = model.WithFixedSchedule;
                    holiday.FixedScheduleMonth = model.FixedScheduleMonth;
                    holiday.FixedScheduleDay = model.FixedScheduleDay;
                    holiday.Published = model.Published;
                    holiday.ModifiedByPK = cid;
                    holiday.ModifiedDate = DateTime.Now;

                    _dbCntxt.Entry(holiday).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                    dbContextTransaction.Commit();
                }
                else
                    throw new CustomException("Holiday is not found", 404);
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

        private IQueryable<Holiday> Get()
        {
            try
            {
                IQueryable<Holiday> query = null;

                query = _dbCntxt.Holidays
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<HolidayDate> GetHolidayDates()
        {
            try
            {
                IQueryable<HolidayDate> query = null;

                query = _dbCntxt.HolidayDates
                                .Include(h => h.Holiday)
                                .Include(h => h.HolidayAffectedOffices)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private bool CheckIfDuplicateOfficeInModel(HolidayOfficeViewModel model, List<HolidayOfficeViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                c => c.HolidayDateId == model.HolidayDateId &&
                                                c.OfficeId == model.OfficeId 
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }

        private bool CheckIfDuplicateOfficeInDatabase(HolidayOfficeViewModel model)
        {
            bool returnValue = false;

            List<HolidayAffectedOffice> checkDuplicate = new();

            if (model.HolidayOfficeId > 0)
            {
                checkDuplicate = _dbCntxt.HolidayAffectedOffices
                                         .Where
                                         (
                                             c => c.HolidayDateId == model.HolidayDateId &&
                                             c.OfficeId == model.OfficeId &&
                                             c.Id != model.HolidayOfficeId
                                         ).ToList();
            }
            else
            {
                checkDuplicate = _dbCntxt.HolidayAffectedOffices
                                         .Where
                                         (
                                             c => c.HolidayDateId == model.HolidayDateId &&
                                             c.OfficeId == model.OfficeId
                                         ).ToList();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }

        public async Task DeleteHolidayCalendar(int id)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var holidaydate = await this.GetHolidayDates().FirstOrDefaultAsync(h => h.Id == id);

                if (holidaydate != null)
                {
                    //All child table under same foreign key will be auto deleted
                    _dbCntxt.HolidayDates.RemoveRange(_dbCntxt.HolidayDates.Where(h => h.Id == holidaydate.Id));
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                    throw new CustomException("Holiday Calendar is not found.", 404);

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
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

        public async Task DeleteHolidayOffice(int id)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var holidayoffice = await _dbCntxt.HolidayAffectedOffices.FirstOrDefaultAsync(h => h.Id == id);

                if (holidayoffice != null)
                {
                    _dbCntxt.HolidayAffectedOffices.RemoveRange(_dbCntxt.HolidayAffectedOffices.Where(h => h.Id == holidayoffice.Id));
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                    throw new CustomException("Holiday Office is not found.", 404);

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
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

        public IQueryable<HolidayCalendarViewModel> GetHolidayCalendarById(int id, bool? published = null, bool? holidayPublished = null)
        {
            try
            {
                IQueryable<HolidayCalendarViewModel> query = null;
                query = this.GetHolidayDates()
                            .Where
                            (
                                h =>
                                h.Id == id
                            )
                            .Select
                            (
                                h => new HolidayCalendarViewModel
                                {
                                    Id = h.Id,
                                    HolidayId = h.HolidayId,
                                    Name = h.Holiday.Name,
                                    Date = h.Date,
                                    HolidayTypeId = h.HolidayTypeId,
                                    HolidayType = _dbCntxt.Options.FirstOrDefault(o => o.OptionGroup == "Holiday Type" && o.Id == h.HolidayTypeId).Name,
                                    HolidayPublished = h.Holiday.Published,
                                    Published = h.Published,
                                    CreatedByPK = h.CreatedByPK,
                                    CreatedBy = Convert.ToString(_dbCntxt.AspNetUsersProfile
                                                        .Where(u => u.Id == h.CreatedByPK)
                                                        .Select
                                                        (
                                                            u => new
                                                            {
                                                                Name = u.LastName + ", " + u.FirstName
                                                            }
                                                        ).FirstOrDefault().Name),
                                    CreatedDate = h.CreatedDate,
                                    ModifiedByPK = h.ModifiedByPK,
                                    ModifiedBy = Convert.ToString(_dbCntxt.AspNetUsersProfile
                                                        .Where(u => u.Id == h.ModifiedByPK)
                                                        .Select
                                                        (
                                                            u => new
                                                            {
                                                                Name = u.LastName + ", " + u.FirstName
                                                            }
                                                        ).FirstOrDefault().Name),
                                    ModifiedDate = h.ModifiedDate,
                                    HolidayOffices = _dbCntxt.HolidayAffectedOffices
                                                             .Where(ho => ho.HolidayDateId == h.Id)
                                                             .Select
                                                             (
                                                                ho => new HolidayOfficeViewModel()
                                                                {
                                                                    HolidayOfficeId = ho.Id,
                                                                    HolidayDateId = ho.HolidayDateId,
                                                                    Id = ho.OfficeId,
                                                                    OfficeId = ho.OfficeId,
                                                                    Office = ho.Office.Name,
                                                                    Name = ho.Office.Name,
                                                                    CreatedByPK = ho.CreatedByPK,
                                                                    CreatedBy = Convert.ToString(_dbCntxt.AspNetUsersProfile
                                                                                                .Where(u => u.Id == ho.CreatedByPK)
                                                                                                .Select
                                                                                                (
                                                                                                    u => new
                                                                                                    {
                                                                                                        Name = u.LastName + ", " + u.FirstName
                                                                                                    }
                                                                                                ).FirstOrDefault().Name),
                                                                    CreatedDate = h.CreatedDate,
                                                                }
                                                             ).ToList()
                                }
                            )
                            .AsQueryable();

                if (published != null)
                    query = query.Where(h => h.Published == published);

                if (holidayPublished != null)
                    query = query.Where(h => h.HolidayPublished == holidayPublished);

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

        public IQueryable<HolidayViewModel> GetHolidaysAsSelection(bool? published = null)
        {
            try
            {
                IQueryable<HolidayViewModel> query = null;
                query = this.FindAll()
                            .Select
                            (
                                h => new HolidayViewModel()
                                { 
                                    Id = h.Id,
                                    Name = h.Name,
                                    WithFixedSchedule = h.WithFixedSchedule,
                                    WithFixedScheduleText = h.WithFixedScheduleText,
                                    FixedScheduleMonth = h.FixedScheduleMonth,
                                    FixedScheduleDay = h.FixedScheduleDay,
                                    FixedScheduleInText = h.FixedScheduleInText,
                                    Published = h.Published
                                }
                            )
                            .AsQueryable();

                if (published != null)
                    query = query.Where(h => h.Published == published);

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

        public async Task CopyHolidayCalendar(int yearFrom, int yearTo)
        {

            if (yearFrom <= 0 || yearTo <= 0)
                throw new CustomException("Invalid Year from or Year To", 400);

            if (yearFrom <= 1900)
                throw new CustomException("Year from is out of range", 400);

            if (yearTo <= 1900)
                throw new CustomException("Year to is out of range", 400);

            if (yearFrom >= yearTo)
                throw new CustomException("Invalid year range, Year from must be earlier than Year to", 400);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var holidayCalendar = GetHolidayDates().ToList();

                var yearFromCalendar = holidayCalendar
                                        .Where
                                        (
                                            h => h.Date.Year == yearFrom && 
                                            h.Published == true && 
                                            h.Holiday.Published == true
                                        ).ToList();

                if (yearFromCalendar.Count <= 0)
                    throw new CustomException("Year from: " + yearFrom.ToString() + " has no maintained holiday calendar.", 404);

                var yearToCalendar = holidayCalendar.Where(h => h.Date.Year == yearTo).ToList();

                if (yearToCalendar.Count > 0)
                {
                    //delete info on year to
                    //Due to cascase delete, offices will also be removed
                    _dbCntxt.HolidayDates.RemoveRange(_dbCntxt.HolidayDates.Where(h => h.Date.Year == yearTo));
                    await _dbCntxt.SaveChangesAsync();
                }
                //insert calendar

                int yearDifference = yearTo - yearFrom;

                foreach(var calendar in yearFromCalendar)
                {
                    var newHolidayDate = new HolidayDate();
                    newHolidayDate.HolidayId = calendar.HolidayId;
                    newHolidayDate.Date = calendar.Date.AddYears(yearDifference);
                    newHolidayDate.HolidayTypeId = calendar.HolidayTypeId;
                    newHolidayDate.Published = calendar.Published;
                    newHolidayDate.CreatedByPK = cid;
                    newHolidayDate.CreatedDate = DateTime.Now;

                    _dbCntxt.HolidayDates.Add(newHolidayDate);
                    await _dbCntxt.SaveChangesAsync();

                    int newHolidayDateId = newHolidayDate.Id;

                    if (calendar.HolidayAffectedOffices.Count > 0)
                    {
                        foreach (var office in calendar.HolidayAffectedOffices)
                        {
                            var newHolidayOffice = new HolidayAffectedOffice();
                            newHolidayOffice.HolidayDateId = newHolidayDateId;
                            newHolidayOffice.OfficeId = office.OfficeId;
                            newHolidayOffice.CreatedByPK = cid;
                            newHolidayOffice.CreatedDate = DateTime.Now;

                            _dbCntxt.HolidayAffectedOffices.Add(newHolidayOffice);
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
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

        public async Task<IEnumerable<int>> GetHolidayYears(bool sortAscending = true)
        {
            try
            {
                List<int> query = null;
                if (sortAscending)
                {
                    query = await this.GetHolidayDates()
                                    .Select
                                    (
                                        h =>
                                        h.Date.Year
                                    )
                                    .Distinct()
                                    .OrderBy(h => h)
                                    .ToListAsync();
                }
                else
                {
                    query = await this.GetHolidayDates()
                                    .Select
                                    (
                                        h =>
                                        h.Date.Year
                                    )
                                    .Distinct()
                                    .OrderByDescending(h => h)
                                    .ToListAsync();
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
    }
}
