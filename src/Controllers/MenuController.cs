using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using workflow.Models.ManageViewModels;
using workflow.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;
using workflow.Helpers;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenu _menu;
        private readonly IOption _option;

        public MenuController(IMenu menu, IOption option)
        {
            _menu = menu;
            _option = option;
        }

        [HttpGet("getSideMenu")]
        public IActionResult GetSideMenu()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var obj = _menu.GetSideMenu().Result;

                var data = new { MENU = obj.Object1 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getMenuList")]
        public IActionResult GetMenuList()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // get all Option Groups
                var optionLOV = _option.GetOptionsByGroup("Button Controls").Select(p => new { p.Id, p.Description }).ToList();

                var obj = _menu.GetMenuList();

                var data = new { MENULIST = obj.Object1, PARENT = obj.Object2, INDEX = obj.Object3, OPTIONLOV = optionLOV };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getMenuByUrl")]
        public async Task<IActionResult> GetMenuByUrl(string url)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var getData = await _menu.GetMenuByUrl(url).SingleOrDefaultAsync();
                var data = new { MENUINFO = getData };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveMenu")]
        public async Task<IActionResult> SaveMenu(MenuViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }
                    
                if (model.VMenuID == null)
                {
                    var menu = await _menu.Add(model);
                    return Ok(menu);
                }
                else
                {
                    await _menu.Update(model);
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

        [HttpDelete("deleteMenu")]
        public async Task<IActionResult> DeleteMenu(string Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _menu.Delete(Id);
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
