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
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestTypeController : ControllerBase
    {
        private readonly IRequestType _requestType;
        public RequestTypeController(IRequestType requestType) 
        {
            _requestType = requestType;
        }

        [HttpPut("getRequestTypes")]
        public IActionResult GetRequestTypes([FromBody] PagingRequest paging, int status = 0, bool? requestCategoryPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<RequestTypeViewModel> query = null;

                if (status > 0)
                {
                    if (!GeneralHelper.IsValidRequestTypeStatus(status))
                        throw new CustomException("Invalid Request Type Status.", 400);
                }

                if (paging != null && paging.SearchCriteria != null)
                {
                    int requestTypestatus = Convert.ToInt32(paging.SearchCriteria.Id3 == null ? 0 : paging.SearchCriteria.Id3);

                    if (requestTypestatus > 0)
                        if (!GeneralHelper.IsValidRequestTypeStatus(requestTypestatus))
                            throw new CustomException("Invalid Request Type Status.", 400);

                }

                query = _requestType.GetRequestTypes(paging, status, requestCategoryPublished)
                                        .OrderBy(r => r.RequestCategory)
                                        .ThenBy(r => r.Name)
                                        .ThenBy(r => r.StatusDescForSorting)
                                        .ThenBy(r => (r.ModifiedDate.HasValue ? r.ModifiedDate : r.CreatedDate));
 
                var pagingResponse = _requestType.PagingFeature(query, paging);

                var data = new { REQUESTTYPELIST = pagingResponse };

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

        [HttpGet("{status:int}/{requestCategoryPublished:bool?}")]
        public IActionResult GetRequestTypes(int status = 0, bool? requestCategoryPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<RequestTypeViewModel> query = null;

                query = _requestType.GetRequestTypes(null, status, requestCategoryPublished);

                var data = new { REQUESTTYPELIST = query };

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

        [HttpGet("ForGrouping/{status:int}/{requestCategoryPublished:bool?}")]
        public async Task<IActionResult> GetRequestTypesForGrouping(int status = 0, bool? requestCategoryPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                var query = await _requestType.GetRequestTypesForGrouping(status, requestCategoryPublished);

                var data = new { REQUESTTYPELIST = query };

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

        [HttpGet("getRequestType")]
        public async Task<IActionResult> GetRequestType(int Id, int Version)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                SearchRequestType _search = new()
                {
                    Id = Id,
                    Version = Version
                };

                IQueryable<RequestTypeViewModel> query = null;
                query = _requestType.Find(_search);

                var data = new { REQUESTTYPE = await query.FirstOrDefaultAsync() };
                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRequestTypesByCategory")]
        public async Task<IActionResult> GetRequestTypesByCategory(int RequestCategoryId = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                List<RequestTypeBasicData> requesttypes = new List<RequestTypeBasicData>();
                 
                if (RequestCategoryId > 0)
                {
                    requesttypes = await _requestType.FindAll()
                                          .Where(r => r.RequestCategoryId == RequestCategoryId)
                                          .Select
                                          (
                                            r => new RequestTypeBasicData
                                            {
                                                Id = r.Id,
                                                Name = r.Name
                                            }
                                          )
                                          .OrderBy(r => r.Name)
                                          .ToListAsync();

                }
                else
                {
                    requesttypes = await _requestType.FindAll()
                                          .Select
                                          (
                                            r => new RequestTypeBasicData
                                            {
                                                Id = r.Id,
                                                Name = r.Name
                                            }
                                          )
                                          .OrderBy(r => r.Name)
                                          .ToListAsync();

                }

                var data = new { REQUESTTYPELIST = requesttypes };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRequestTypesStatus")]
        public IActionResult GetRequestTypesStatus()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var states = _requestType.GetRequestTypeStatuses()
                                         .OrderBy(s => s.Name)
                                         .ToList();

                var data = new { STATUSES = states };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getLockedRequestTypes")]
        public async Task<IActionResult> GetLockedRequestTypes()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                var query = await _requestType.GetLockedRequestTypes();

                var data = new { LockedRequestType = query};
                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getLockedRequestType")]
        public async Task<IActionResult> GetLockedRequestType(int RequestTypeId, int Version)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                var query = await _requestType.GetLockedRequestType(RequestTypeId, Version);

                var data = new { LockedRequestType = query };
                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveRequestType")]
        public async Task<IActionResult> SaveRequestType(RequestTypeViewModel model)
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
                    var requesttype = await _requestType.Add(model);
                    return Ok(requesttype);
                }
                else
                {
                    await _requestType.Update(model);
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

        [HttpPost("saveRequestForm")]
        public async Task<IActionResult> SaveRequestForm(RequestFormViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _requestType.SaveRequestForm(model);

                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details, customex.ListMessage);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveRequestTypeProcessOwners")]
        public async Task<IActionResult> SaveRequestTypeProcessOwners(RequestTypeProcessOwnersViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _requestType.SaveRequestTypeProcessOwners(model);

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

        [HttpPost("saveRequestStep")]
        public async Task<IActionResult> SaveRequestStep(SaveRequestStepViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _requestType.SaveRequestStep(model);

                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details, customex.ListMessage);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpDelete("deleteRequestType")]
        public async Task<IActionResult> DeleteRequestType(int Id, int Version, int ReasonId = 0, string Remarks = "")
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {

                RequestTypeDeletionParam deleteparam = new RequestTypeDeletionParam();
                deleteparam.Id = Id;
                deleteparam.Version = Version;
                deleteparam.ReasonId = ReasonId;
                deleteparam.Remarks = Remarks;

                await _requestType.Delete(deleteparam);

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

        [HttpDelete("deleteRequestForm")]
        public async Task<IActionResult> DeleteRequestForm(int RequestTypeId, int Version, int FieldId = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.DeleteRequestForm(RequestTypeId, Version, FieldId);

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

        [HttpDelete("deleteRequestTypeProcessOwners")]
        public async Task<IActionResult> DeleteRequestTypeProcessOwners(int Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.DeleteRequestTypeProcessOwner(Id);

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

        [HttpDelete("deleteRequestTypeWorkflow")]
        public async Task<IActionResult> DeleteRequestTypeWorkflow(int Id = 0, int Requesttypeid = 0, int Version = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.DeleteRequestTypeWorkFlow(Id, Requesttypeid, Version);

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

        [HttpDelete("deleteRequestStepAssignees")]
        public async Task<IActionResult> DeleteRequestStepAssignees(int Id, int DetailId = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.DeleteRequestStepAssignees(Id, DetailId);
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

        [HttpDelete("deleteRequestStepConditions")]
        public async Task<IActionResult> DeleteRequestStepConditions(int Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.DeleteRequestStepConditions(Id);
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

        [HttpDelete("deleteRequestStep")]
        public async Task<IActionResult> DeleteRequestStep(int RequestTypeId, int Version, int StepId)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.DeleteRequestStep(RequestTypeId, Version, StepId);

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

        [HttpPost("lockRequestType")]
        public async Task<IActionResult> LockRequestType(LockedRequestTypesViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _requestType.LockRequestType(model);

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

        [HttpDelete("deleteLockedRequestType")]
        public async Task<IActionResult> DeleteLockedRequestType(int RequestTypeId, int Version)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.DeleteLockedRequestType(RequestTypeId, Version);

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

        [HttpGet("getFields")]
        public async Task<IActionResult> GetFields(int requestTypeId = 0, int version = 0, bool? enabled = null, int? isRequired = null, bool? isLov = null, bool? isActiveLovOnly = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (requestTypeId > 0 || version > 0)
                {
                    if (requestTypeId <= 0 || version <= 0)
                    {
                        returnObject = GeneralHelper.SetReturnDetails(400, "Both Request type id and version is required.");
                        return StatusCode(returnObject.Code, returnObject);
                    }
                }

                var fields = await _requestType.GetFields(requestTypeId, version, isLov, isActiveLovOnly);

                if (enabled != null)
                    fields = fields.Where(f => f.Published == enabled);

                if (isRequired != null)
                    fields = fields.Where(f => f.IsRequired == isRequired);
             
                var data = new { FIELDLIST = fields };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getLOVFields")]
        public async Task<IActionResult> GetLOVFields(bool isLov = true, int requestTypeId = 0, int version = 0, bool? enabled = null, int? isRequired = null, bool? isActiveLovOnly = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (requestTypeId > 0 || version > 0)
                {
                    if (requestTypeId <= 0 || version <= 0)
                    {
                        returnObject = GeneralHelper.SetReturnDetails(400, "Both Request type and version is required.");
                        return StatusCode(returnObject.Code, returnObject);
                    }
                }

                var fields = await _requestType.GetFields(requestTypeId, version, isLov, isActiveLovOnly);

                if (enabled != null)
                    fields = fields.Where(f => f.Published == enabled);

                if (isRequired != null)
                    fields = fields.Where(f => f.IsRequired == isRequired);

                var data = new { FIELDLIST = fields };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRequestForms")]
        public async Task<IActionResult> GetRequestForms(int Id, int Version, bool? published = null, bool? activeLOVs = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var requestforms = await _requestType.GetRequestForms(Id, Version, published, activeLOVs);

                var data = new { CONTROLS = requestforms };

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

        [HttpPut("getRequestTypeProcessOwners")]
        public IActionResult GetRequestTypeProcessOwners([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                //process owner list 
                var query = _requestType.GetRequestProcessOwners(paging);

                var pagingResponse = _requestType.ProcessOwnersPagingFeatureProcessOwners(query, paging);

                var data = new { PROCESSOWNERS = pagingResponse };

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

        [HttpGet("getStepTypes")]
        public async Task<IActionResult> GetStepTypes(bool? Published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var steptypes = await _requestType.GetStepTypes();

                if (Published != null)
                    steptypes = steptypes.Where(s => s.Published == Published);

                var data = new { STEPTYPELIST = steptypes };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getApplicableComparisonOperators")]
        public async Task<IActionResult> GetApplicableComparisonOperators(int Fieldid)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var operators = await _requestType.GetApplicableComparisonOperators(Fieldid);

                var data = new { OPERATORSLIST = operators };

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

        [HttpGet("getStepsConditionSources")]
        public async Task<IActionResult> GetStepsConditionSources(bool? Published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                //var source1 = new { Id = 1, Name = "Field" };
                //var source2 = new { Id = 2, Name = "Step" };

                //var listsource = new[] { source1, source2 }.ToList();

                var sources = await _requestType.GetStepsConditionSources();

                if (Published != null)
                    sources = sources.Where(s => s.Published == Published);

                var data = new { SOURCES = sources };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRequestSteps")]
        public async Task<IActionResult> GetRequestSteps(int RequestTypeId, int Version, bool WithDetails = true)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var steptypes = await _requestType.GetRequestSteps(RequestTypeId, Version, WithDetails);

                var data = new { STEPTYPELIST = steptypes };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRequestStepsForCondition")]
        public async Task<IActionResult> GetRequestStepsForCondition(int requestTypeId, int version, int stepId = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var stepsList = await _requestType.GetRequestSteps(requestTypeId, version, false);

                var steps = stepsList.Where(s => s.StepId != stepId);

                var data = new { STEPTYPELIST = steps };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getApplicableWorkFlowNextStep")]
        public async Task<IActionResult> GetApplicableWorkFlowNextStep(int RequestTypeId, int Version, int StepId = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var steps = await _requestType.GetApplicableWorkFlowNextStep(RequestTypeId, Version, StepId);

                var data = new { STEPSLIST = steps };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getRequestTypeWorkFlows")]
        public async Task<IActionResult> GetRequestTypeWorkFlows(int RequestTypeId, int Version)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var workflows = await _requestType.GetRequestTypeProcessWorkFlows(RequestTypeId, Version);

                var data = new { WORKFLOWS = workflows };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveRequestTypeWorkflow")]
        public async Task<IActionResult> SaveRequestTypeWorkflow(SaveRequestTypeWorkFlowViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _requestType.SaveRequestTypeWorkflow(model);

                return Ok();
            }
            catch (CustomException customex)
            {
                returnObject = GeneralHelper.SetReturnDetails(customex.StatusCode, customex.Message, customex.Details, customex.ListMessage);
                return StatusCode(returnObject.Code, returnObject);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("switchFieldSequence")]
        public async Task<IActionResult> SwitchFieldSequence(FieldSequenceSwitchViewModel data)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _requestType.SwithFieldSequence(data);
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

        [HttpGet("validateListOfValuesInSteps")]
        public async Task<IActionResult> ValidateListOfValuesInSteps(int requestTypeId, int version, int fieldId, string name)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var isActivelyMapped = await _requestType.CheckLOVIsInvolvedInProcessAsync(requestTypeId, version, fieldId, name, true);

                var data = new { ISMAPPED = isActivelyMapped };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("ForTicketCreation/{userId}/{requestCategoryId:int}")]
        public async Task<IActionResult> GetRequestTypesForTicketCreation(string userId, int requestCategoryId = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var requestTypes = await _requestType.GetRequestTypesForTicketCreation(userId, requestCategoryId);

                var data = new { REQUESTTYPES = requestTypes };

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

        [HttpGet("{id:int}/{version:int}/BaseWorkFlow")]
        public async Task<IActionResult> GetRequestTypeBaseWorkFlow(int id, int version)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IEnumerable<RequestTypeBaseWorkFlowViewModel> query = null;

                var requestForms = new List<TicketRequestDetailViewModel>();

                //requestForms.Add(new TicketRequestDetailViewModel 
                //{
                //    Id = 0,
                //    TicketRequestId = 0,
                //    FieldId = 193,
                //    IsLov = true,
                //    ValueCode = null,
                //    Value = "IH",
                //    ValueCode2 = null,
                //    Value2 = null
                //});

                //requestForms.Add(new TicketRequestDetailViewModel
                //{
                //    Id = 0,
                //    TicketRequestId = 0,
                //    FieldId = 195,
                //    IsLov = true,
                //    ValueCode = null,
                //    Value = "Cash",
                //    ValueCode2 = null,
                //    Value2 = null
                //});

                //requestForms.Add(new TicketRequestDetailViewModel
                //{
                //    Id = 0,
                //    TicketRequestId = 0,
                //    FieldId = 208,
                //    IsLov = true,
                //    ValueCode = null,
                //    Value = "3",
                //    ValueCode2 = null,
                //    Value2 = null
                //});

                //requestForms.Add(new TicketRequestDetailViewModel
                //{
                //    Id = 0,
                //    TicketRequestId = 0,
                //    FieldId = 192,
                //    IsLov = false,
                //    ValueCode = null,
                //    Value = "Daryn",
                //    ValueCode2 = null,
                //    Value2 = null
                //});

                //requestForms.Add(new TicketRequestDetailViewModel
                //{
                //    Id = 0,
                //    TicketRequestId = 0,
                //    FieldId = 202,
                //    IsLov = false,
                //    ValueCode = null,
                //    Value = "900",
                //    ValueCode2 = null,
                //    Value2 = null
                //});

                //requestForms.Add(new TicketRequestDetailViewModel
                //{
                //    Id = 0,
                //    TicketRequestId = 0,
                //    FieldId = 218,
                //    IsLov = true,
                //    ValueCode = "passprt,sssid,tin",
                //    Value = "Passport,SSS,TIN",
                //    ValueCode2 = null,
                //    Value2 = null
                //});

                //requestForms.Add(new TicketRequestDetailViewModel
                //{
                //    Id = 0,
                //    TicketRequestId = 0,
                //    FieldId = 220,
                //    IsLov = true,
                //    ValueCode = "1,4,5",
                //    Value = "1,4,5",
                //    ValueCode2 = null,
                //    Value2 = null
                //});

                requestForms.Add(new TicketRequestDetailViewModel
                {
                    Id = 0,
                    TicketRequestId = 0,
                    FieldId = 221,
                    IsLov = true,
                    ValueCode = "drvlicense,sss,tin",
                    Value = "Driver's License,SSS,TIN",
                    ValueCode2 = null,
                    Value2 = null
                });

                requestForms.Add(new TicketRequestDetailViewModel
                {
                    Id = 0,
                    TicketRequestId = 0,
                    FieldId = 222,
                    IsLov = true,
                    ValueCode = "black,blue,green",
                    Value = "Black,Blue,Green",
                    ValueCode2 = null,
                    Value2 = null
                });

                query = await _requestType.GetRequestTypeBaseWorkFlows(id, version, requestForms, 1);

                var data = new { REQUESTTYPEBASEWORKFLOW = query };

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
