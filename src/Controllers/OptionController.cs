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

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OptionController : ControllerBase
    {
        private readonly IOption _option;

        public OptionController(IOption option)
        {
            _option = option;
        }

        [HttpPut("getOptionList")]
        public IActionResult GetOptionList([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // get all Option Groups
                var optionGroup = _option.GetDistinctOptionGroup();

                // initialize option list 
                IQueryable<OptionViewModel> query = null;

                query = _option.GetOptions(paging);

                var pagingResponse = _option.PagingFeature(query, paging);

                var data = new { OPTIONLIST = pagingResponse, OPTIONGROUP = optionGroup };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getOptionGroup")]
        public IActionResult GetOptionGroup()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // get all Option Groups
                var optionGroup = _option.GetDistinctOptionGroup();

                var data = new { OPTIONGROUP = optionGroup };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getOptionsByGroup")]
        public IActionResult GetOptionsByGroup(string group)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var getData = _option.GetOptionsByGroup(group);

                var data = new { OPTIONLIST = getData };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveOption")]
        public async Task<IActionResult> SaveOption(OptionViewModel model)
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
                    var option = await _option.Add(model);
                    return Ok(option);
                }
                else
                {
                    await _option.Update(model);
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

        [HttpDelete("deleteOption")]
        public async Task<IActionResult> DeleteOption(int Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _option.Delete(Id);

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
