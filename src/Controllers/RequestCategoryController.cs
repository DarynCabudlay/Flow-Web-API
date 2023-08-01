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
//For POC
//using System.Net.Http.Headers;
//using Microsoft.Net.Http.Headers;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestCategoryController : ControllerBase
    {
        private readonly IRequestCategory _requestCategory;

        //For POC
        //private readonly string authScheme = "";
        //private readonly string authParam = "";

        public RequestCategoryController(IRequestCategory requestCategory) 
        {
            _requestCategory = requestCategory;

            //For POC
            //var authorization = Request.Headers[HeaderNames.Authorization];

            //if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            //{
            //    // scheme will be "Bearer"
            //    // parmameter will be the token itself.
            //    authScheme = headerValue.Scheme;
            //    authParam = headerValue.Parameter;
            //}
        }

        [HttpPut("getRequestCategories")]
        public IActionResult GetRequestCategories([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<RequestCategoryViewModel> query = null;

                query = _requestCategory.GetRequestCategories(paging);

                var pagingResponse = _requestCategory.PagingFeature(query, paging);

                var data = new { REQUESTCATEGORYLIST = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRequestCategoriesAsSelection")]
        public IActionResult GetRequestCategoriesAsSelection(bool active = true)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var categories = _requestCategory.FindAll()
                                                .Where(r => r.Published == active)
                                                .Select
                                                (
                                                    r => new
                                                    {
                                                        Id = r.Id,
                                                        Category = r.Name
                                                    }
                                                )
                                                .OrderBy(r => r.Category)
                                                .ToList();


                var data = new { REQUESTCATEGORYLIST = categories};

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveRequestCategory")]
        public async Task<IActionResult> SaveRequestCategory(RequestCategoryViewModel model)
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
                    var category = await _requestCategory.Add(model);
                    return Ok(category);
                }
                else
                {
                    await _requestCategory.Update(model);
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

        [HttpDelete("deleteRequestCategory")]
        public async Task<IActionResult> DeleteRequestCategory(int Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestCategory.Delete(Id);

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

        [HttpDelete("deleteAllRequestCategory")]
        public async Task<IActionResult> DeleteAllRequestCategory(RequestCategoryViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _requestCategory.DeleteAll(model);
                return Ok();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("ForTicketCreation/{userId}")]
        public async Task<IActionResult> GetRequestCategoriesForTicketCreation(string userId)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var requestCategories = await _requestCategory.GetRequestCategoriesForTicketCreation(userId);

                var data = new { REQUESTCATEGORIES = requestCategories };

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
    }
}
