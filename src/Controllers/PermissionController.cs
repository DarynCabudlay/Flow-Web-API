using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using workflow.Services;
using Microsoft.AspNetCore.Http;
using workflow.Helpers;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermission _permission;
        public PermissionController(IPermission permission)
        {
            _permission = permission;
        }

        [HttpGet("getPermissionControl")]
        public async Task<IActionResult> GetPermissionControl(string path)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var controls = await _permission.GetPermissionControl(path);

                var data = new { CONTROLS = controls };

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
