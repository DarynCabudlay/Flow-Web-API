using workflow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using workflow.Helpers;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {

        private readonly ILog _log;

        public LogController(ILog log)
        {
            _log = log;
        }

        [HttpGet("getLogDashboard")]
        public IActionResult GetLogDashboard(int offset)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var obj = _log.GetLogDashboard(offset).Result;

                var data = new { TOTALUSER = obj.Object1, ACTIVEUSER = obj.Object2, TOTALLOGIN = obj.Object3, TOTALPAGEVISIT = obj.Object4, RWUDATA = obj.Object5, TRUDATA = obj.Object6, LHDATA = obj.Object7, TPVDATA = obj.Object8 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("pagevisit")]
        public async Task<IActionResult> PageVisit(string path)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _log.LogPageVisit(path);

                return Ok();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("pagevisitdata")]
        public IActionResult PageVisitHistoryData()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var obj = _log.GetPageVisitHistoryData().Result;

                var data = new { VISITEDLIST = obj.Object1, TOTALVISIT = obj.Object2, HIGHESTVISITEDPAGE = obj.Object3, HIGHESTVISITEDBY = obj.Object4 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("loginhistorydata")]
        public IActionResult LoginHistoryData()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var obj = _log.GetLoginHistoryData().Result;

                var data = new { LOGINLIST = obj.Object1, TOTALLOGIN = obj.Object2, HIGHESTLOGINBY = obj.Object3, HIGHESTLOGIN = obj.Object4 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }
    }
}
