using Microsoft.AspNetCore.Mvc;
using workflow.Models.AccountViewModels;
using workflow.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using workflow.Services;
using System;
using Microsoft.AspNetCore.Http;
using workflow.Helpers;

namespace workflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CronJobController : ControllerBase
    {
        //IEnumerable<string> cookies = new List<string>();
        //CookieContainer cookieJar = new CookieContainer();

        private readonly ICronJobs _cronJobs;

        public CronJobController(ICronJobs cronJobs)
        {
            _cronJobs = cronJobs;
        }

        //Temporarily Removed
        //[HttpPut("removeLoginAttemptsAndTaggedAsDormant")]
        //public async Task<IActionResult> RemoveLoginAttemptsAndTaggedAsDormant()
        //{
        //    APIReturnObject returnObject = new APIReturnObject();
        //    try
        //    {
        //        await _cronJobs.RemoveLoginAttemptsAndTaggedAsDormant();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
        //        return StatusCode(returnObject.Code, returnObject);
        //    }
        //}

        //Temporarily Removed
        //[HttpPut("taggedPasswordExpired")]
        //public async Task<IActionResult> TagAsPasswordExpired()
        //{
        //    APIReturnObject returnObject = new APIReturnObject();
        //    try
        //    {
        //        await _cronJobs.TagAsPasswordExpired();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
        //        return StatusCode(returnObject.Code, returnObject);
        //    }
        //}
    }
}
