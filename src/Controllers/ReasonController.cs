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
    public class ReasonController : ControllerBase
    {
        private readonly IReason _reason;

        public ReasonController(IReason reason) 
        {
            _reason = reason;
        }

        [HttpPut("getReasons")]
        public IActionResult GetReasons([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<ReasonsViewModel> query = null;

                query = _reason.GetReasons(paging);
 
                var pagingResponse = _reason.PagingFeature(query, paging);

                var data = new { REASONLIST = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getReason")]
        public async Task<IActionResult> GetReason(int Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                IQueryable<ReasonsViewModel> query = null;
                query = _reason.Find(Id);

                var data = new { REASON = await query.FirstOrDefaultAsync() };
                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getReasonByReasonType")]
        public async Task<IActionResult> GetReasonByReasonType(string ReasonType, bool Published = true)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var reasons = await _reason.GetReasonByReasonType(ReasonType, Published);

                var data = new { REASONLIST = reasons };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getReasonTypes")]
        public async Task<IActionResult> GetReasonTypes(bool Published = true)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                var reasontypes = await _reason.GetReasonTypes(Published);

                var data = new { REASONTYPELIST = reasontypes };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveReason")]
        public async Task<IActionResult> SaveReason(ReasonsViewModel model)
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
                    var reason = await _reason.Add(model);
                    return Ok(reason);
                }
                else
                {
                    await _reason.Update(model);
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

        [HttpDelete("deleteReason")]
        public async Task<IActionResult> DeleteReason(int Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _reason.Delete(Id);

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
