using workflow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using workflow.Models.ManageViewModels;
using workflow.Helpers;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISetting _setting;
        private readonly IRole _role;
        private readonly IUser _user;

        public SettingController(IRole role, 
                                 ISetting setting, 
                                 IUser user) 
        {
            _role = role;
            _setting = setting;
            _user = user;
        }

        [HttpGet("getSettingData")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSettingData()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var roles = await _role.FindAll()
                                       .Select(x => new { x.Id, x.Name, IsChecked = false })
                                       .OrderBy(x => x.Name)                                  
                                       .ToListAsync();

                var settings = _setting.GetSettingData();

                 var preferences = await _user.GetUserPreferences();

                var data = new { ROLES = roles, SETTINGS = settings, PREFERENCES = preferences  };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("updateSetting")]
        public async Task<IActionResult> UpdateSetting(SettingViewModel setting)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _setting.Update(setting);

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
