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
    public class RequestTypesGroupingController : ControllerBase
    {
        private readonly IRequestTypesGrouping _requestTypesGrouping;
        private readonly IRequestTypesGroupingDetail _requestTypesGroupingDetail;
        private readonly IRequestTypesGroupingCommon _requestTypesGroupingCommon;
        public RequestTypesGroupingController(IRequestTypesGrouping requestTypesGrouping, IRequestTypesGroupingDetail requestTypesGroupingDetail, IRequestTypesGroupingCommon requestTypesGroupingCommon) 
        {
            _requestTypesGrouping = requestTypesGrouping;
            _requestTypesGroupingDetail = requestTypesGroupingDetail;
            _requestTypesGroupingCommon = requestTypesGroupingCommon;
        }

        #region Display

        [HttpPut("WithPaging/{published:bool?}/{detailPublished:bool?}")]
        public IActionResult GetRequestTypesGroupings([FromBody] PagingRequest paging, bool? published = null, bool? detailPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _requestTypesGroupingCommon.GetRequestTypesGroupings(paging, published, detailPublished);

                var pagingResponse = _requestTypesGroupingCommon.RequestTypeGroupingsPagingFeature(query, paging);

                var data = new { REQUESTTYPESGROUPINGS = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{published:bool?}/{detailPublished:bool?}")]
        public async Task<IActionResult> GetRequestTypesGroupings(bool? published = null, bool? detailPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _requestTypesGroupingCommon.GetRequestTypesGroupings(null, published, detailPublished).ToListAsync();

                var data = new { REQUESTTYPESGROUPINGS = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("{id:int}/DetailsWithPaging/{published:bool?}/{detailPublished:bool?}")]
        public IActionResult GetRequestTypesGroupingDetailsPerRequestTypeGrouping(int id, [FromBody] PagingRequest paging, bool? published = null, bool? detailPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _requestTypesGroupingCommon
                            .GetRequestTypesGroupingDetails(paging, published, detailPublished)
                            .Where(r => r.RequestTypesGroupingId == id);

                var pagingResponse = _requestTypesGroupingCommon.RequestTypeGroupingDetailsPagingFeature(query, paging);

                var data = new { REQUESTTYPESGROUPINGDETAILS = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id:int}/Details/{published:bool?}/{detailPublished:bool?}")]
        public async Task<IActionResult> GetRequestTypesGroupingDetailsPerRequestTypeGrouping(int id, bool? published = null, bool? detailPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _requestTypesGroupingCommon
                           .GetRequestTypesGroupingDetails(null, published, detailPublished)
                           .Where(r => r.RequestTypesGroupingId == id).ToListAsync();

                var data = new { REQUESTTYPESGROUPINGDETAILS = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("DetailsWithPaging/{published:bool?}/{detailPublished:bool?}")]
        public IActionResult GetRequestTypesGroupingDetails([FromBody] PagingRequest paging, bool? published = null, bool? detailPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _requestTypesGroupingCommon
                            .GetRequestTypesGroupingDetails(paging, published, detailPublished);

                var pagingResponse = _requestTypesGroupingCommon.RequestTypeGroupingDetailsPagingFeature(query, paging);

                var data = new { REQUESTTYPESGROUPINGDETAILS = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Details/{published:bool?}/{detailPublished:bool?}")]
        public async Task<IActionResult> GetRequestTypesGroupingDetails(bool? published = null, bool? detailPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _requestTypesGroupingCommon
                           .GetRequestTypesGroupingDetails(null, published, detailPublished).ToListAsync();

                var data = new { REQUESTTYPESGROUPINGDETAILS = query };

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
        public async Task<IActionResult> AddRequestTypesGrouping([FromBody] RequestTypesGroupingViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var requestTypesGrouping = await _requestTypesGrouping.Add(model);
                return StatusCode(201, requestTypesGrouping);
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

        [HttpPost("Details")]
        public async Task<IActionResult> AddRequestTypesGroupingDetail([FromBody] RequestTypesGroupingDetailViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var requestTypesGroupingDetail = await _requestTypesGroupingDetail.Add(model);
                return StatusCode(201, requestTypesGroupingDetail);
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
        public async Task<IActionResult> UpdateRequestTypesGrouping(int id, [FromBody] RequestTypesGroupingViewModel model)
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

                await _requestTypesGrouping.Update(model);
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

        [HttpPut("Details/{id:int}")]
        public async Task<IActionResult> UpdateRequestTypesGroupingDetail(int id, [FromBody] RequestTypesGroupingDetailViewModel model)
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

                await _requestTypesGroupingDetail.Update(model, id);
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
        public async Task<IActionResult> DeleteRequestTypesGrouping(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestTypesGrouping.Delete(id);
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

        [HttpDelete("Details/{id:int}")]
        public async Task<IActionResult> DeleteRequestTypesGroupingDetail(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestTypesGroupingDetail.Delete(id);
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

        #region Save

        [HttpPost("SaveDetails")]
        public async Task<IActionResult> SaveRequestTypesGroupingDetails([FromBody] SaveRequestTypesGroupingDetailWithCheckingDetailEntryViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _requestTypesGroupingDetail.SaveRequestTypesGroupingDetails(model);
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
