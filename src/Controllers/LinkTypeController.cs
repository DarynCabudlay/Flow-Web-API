using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
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
    public class LinkTypeController : ControllerBase
    {
        private readonly ILinkType _linkType;
        public LinkTypeController(ILinkType linkType) 
        {
            _linkType = linkType;
        }

        #region Display

        [HttpPut("WithPaging/{published:bool?}")]
        public IActionResult GetLinkTypes([FromBody] PagingRequest paging, bool? published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _linkType.GetLinkTypes(paging, published);

                var pagingResponse = _linkType.PagingFeature(query, paging);

                var data = new { LINKTYPES = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{published:bool?}")]
        public async Task<IActionResult> GetLinkTypes(bool? published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _linkType.GetLinkTypes(null, published).ToListAsync();

                var data = new { LINKTYPES = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        #endregion

        #region Add

        [HttpPost]
        public async Task<IActionResult> AddLinkType([FromBody] LinkTypeViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var fileType = await _linkType.Add(model);
                return StatusCode(201, fileType);
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

        #endregion

        #region Update

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateLinkType(int id, [FromBody] LinkTypeViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (id != model.Id)
                    throw new CustomException("Paramenter id is not equal to Id in the model.", 400);

                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _linkType.Update(model);
                return NoContent();

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

        #endregion

        #region Delete     

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLinkType(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _linkType.Delete(id);
                return NoContent();
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

        #endregion
    }
}
