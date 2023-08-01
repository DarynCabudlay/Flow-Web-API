using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.ManageViewModels;
using workflow.Services;
using Microsoft.AspNetCore.Http;
using workflow.Helpers;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRole _role;
        private readonly IMenu _menu;

        public RoleController(IRole role, IMenu menu)
        {
            _role = role;
            _menu = menu;
        }

        [HttpGet("getRoleList")]
        public IActionResult GetRoleList(bool filterAdminRole = false)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize role list
                IQueryable<RoleViewModel> query = null;

                query = _role.GetRole(filterAdminRole);

                var data = new { ROLES = query };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRoleById")]
        public IActionResult GetRoleById(string Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
               // IQueryable<RoleViewModel> query = null;
                var role = _role.Find(Id);

                //var menuPer = await _role.GetMenuPer(Id).Select(x => new { x.VMenuId }).ToListAsync();

                var menupermissions = _menu.GetMenuPerRole(Id);

                var data = new { ROLE = role, MENUPERMISSION = menupermissions.Object1, PARENT = menupermissions.Object2, INDEX = menupermissions.Object3 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveRole")]
        public async Task<IActionResult> SaveRole(RoleViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                if (model.Id == null)
                {
                    var role = await _role.Add(model);
                    return Ok(role);
                }
                else
                {
                    await _role.Update(model);
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

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _role.Delete(Id);
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

        [HttpGet("deleteAllRole")]
        public async Task<IActionResult> DeleteAllRole(RoleViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _role.DeleteAll(model);
                return Ok();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }
    }
}
