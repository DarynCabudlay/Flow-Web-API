using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Services;
using Microsoft.AspNetCore.Http;
using workflow.Models.ManageViewModels;
using workflow.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IHoliday _holiday;

        public HolidayController(IHoliday holiday)
        {
            _holiday = holiday;
        }
            
        [HttpPut("getHolidays")]
        public IActionResult GetHolidays([FromBody] PagingRequest paging, bool? published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<HolidayViewModel> query = null;

                query =  _holiday.GetHolidays(paging, published)
                                 .OrderBy(r => r.Name);
                                       
                var pagingResponse = _holiday.PagingFeature(query, paging);

                var data = new { HOLIDAYLIST = pagingResponse };

                return Ok(data);
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getHolidaysAsSelection")]
        public IActionResult GetHolidaysAsSelection(bool? published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                IQueryable<HolidayViewModel> query = null;

                query = _holiday.GetHolidaysAsSelection(published)
                                .OrderBy(r => r.Name);

                if (published != null)
                    query = query.Where(h => h.Published == published);

                var data = new { HOLIDAYLIST = query };

                return Ok(data);
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("getHolidayCalendar")]
        public IActionResult GetHolidayCalendar([FromBody] PagingRequest paging, bool? published = null, bool? holidayPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<HolidayCalendarViewModel> query = null;

                query = _holiday.GetHolidayCalendar(paging, published, holidayPublished)
                                .OrderBy(r => r.Name);

                var pagingResponse = _holiday.PagingFeatureCalendar(query, paging);

                var data = new { HOLIDAYLIST = pagingResponse };

                return Ok(data);
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getHolidayCalendarById")]
        public IActionResult GetHolidayCalendarById(int id, bool? published = null, bool? holidayPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<HolidayCalendarViewModel> query = null;

                query = _holiday.GetHolidayCalendarById(id, published, holidayPublished);

                var data = new { HOLIDAY = query };

                return Ok(data);
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getHoliday")]
        public async Task<IActionResult> GetHoliday(int id, bool? published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                IQueryable<HolidayViewModel> query = null;
                query = _holiday.Find(id);

                if (published != null)
                    query = query.Where(h => h.Published == published);

                var data = new { HOLIDAY = await query.FirstOrDefaultAsync() };
                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getHolidayCalendarYears")]
        public async Task<IActionResult> GetHolidayCalendarYears(bool sortAscending = true)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _holiday.GetHolidayYears(sortAscending);

                var data = new { HOLIDAYYEARS = await query };
                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveHoliday")]
        public async Task<IActionResult> SaveHoliday(HolidayViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                if (model.Id == 0)
                {
                    var holiday = await _holiday.Add(model);
                    return Ok(holiday);
                }
                else
                {
                    await _holiday.Update(model);
                    return Ok();
                }
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpDelete("deleteHoliday")]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _holiday.Delete(id);

                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveHolidayCalendar")]
        public async Task<IActionResult> SaveHolidayCalendar(HolidayCalendarViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _holiday.SaveHolidayDates(model);

                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpDelete("deleteHolidayCalendar")]
        public async Task<IActionResult> DeleteHolidayCalendar(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _holiday.DeleteHolidayCalendar(id);

                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpDelete("deleteHolidayOffice")]
        public async Task<IActionResult> DeleteHolidayOffice(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _holiday.DeleteHolidayOffice(id);

                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("copyHolidayCalendar")]
        public async Task<IActionResult> CopyHolidayCalendar(CopyHolidayCalendarParam param)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _holiday.CopyHolidayCalendar(param.YearFrom, param.YearTo);
                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }
    }
}
