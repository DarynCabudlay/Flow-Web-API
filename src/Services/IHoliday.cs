using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IHoliday : IGenericMethod<HolidayViewModel>
    {
        IQueryable<HolidayViewModel> GetHolidays(PagingRequest paging = null, bool? published = null);
        IQueryable<HolidayViewModel> GetHolidaysAsSelection(bool? published = null);
        IQueryable<HolidayCalendarViewModel> GetHolidayCalendar(PagingRequest paging = null, bool? published = null, bool? holidayPublished = null);
        IQueryable<HolidayCalendarViewModel> GetHolidayCalendarById(int id, bool? published = null, bool? holidayPublished = null);
        PagingResponse<HolidayViewModel> PagingFeature(IQueryable<HolidayViewModel> query, PagingRequest paging, object id = null);
        PagingResponse<HolidayCalendarViewModel> PagingFeatureCalendar(IQueryable<HolidayCalendarViewModel> query, PagingRequest paging, object id = null);
        Task SaveHolidayDates(HolidayCalendarViewModel data);
        Task DeleteHolidayCalendar(int id);
        Task DeleteHolidayOffice(int id);
        Task CopyHolidayCalendar(int yearFrom, int yearTo);
        Task<IEnumerable<int>> GetHolidayYears(bool sortAscending = true);

    }
}
