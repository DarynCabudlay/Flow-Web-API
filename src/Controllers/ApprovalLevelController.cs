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
    public class ApprovalLevelController : ControllerBase
    {
        private readonly IApprovalLevel _approvalLevel;
        public ApprovalLevelController(IApprovalLevel approvalLevel) 
        {
            _approvalLevel = approvalLevel;
        }

        [HttpPost("Details")]
        public async Task<IActionResult> SaveApprovalLevelDetail([FromBody] ApprovalLevelDetailViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var approvalLevelDetail = await _approvalLevel.Add(model);
                return StatusCode(201, approvalLevelDetail);
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
        public async Task<IActionResult> UpdateApprovalLevelDetail(int id, [FromBody] ApprovalLevelDetailViewModel model)
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

                await _approvalLevel.Update(model);
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

        [HttpPut("Details/{published:bool?}/{organizationEntityPublished:bool?}")]
        public IActionResult GetApprovalLevelDetails([FromBody] PagingRequest paging, bool? published = null, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _approvalLevel.GetApprovalLevelDetails(paging, published, organizationEntityPublished);

                var pagingResponse = _approvalLevel.PagingFeature(query, paging);

                var data = new { APPROVALLEVELDETAILS = pagingResponse };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Details/{published:bool?}/{organizationEntityPublished:bool?}")]
        public IActionResult GetApprovalLevelDetails(bool? published = null, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _approvalLevel.GetApprovalLevelDetails(null, published, organizationEntityPublished)
                                          .OrderByDescending(a => a.OrganizationEntityHierarchy)
                                          .ThenBy(a => a.Sequence);

                var data = new { APPROVALLEVELDETAILS = query };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Details/{id:int}/{organizationEntityPublished:bool?}")]
        public IActionResult GetApprovalLevelDetail(int id, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _approvalLevel.GetApprovalLevelDetail(id, organizationEntityPublished);

                if (query == null)
                    return NotFound();

                var data = new { APPROVALLEVELDETAIL = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpDelete("Details/{id:int}")]
        public async Task<IActionResult> DeleteApprovalLevelDetail(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _approvalLevel.Delete(id);

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

        [HttpGet("{id:int}/Details/{published:bool?}/{organizationEntityPublished:bool?}")]
        public IActionResult GetDetailsPerApprovalLevel(int id, bool? published = null, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _approvalLevel.GetDetailsPerApprovalLevel(id, published, organizationEntityPublished);

                var data = new { APPROVALLEVELDETAILS = query };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id:int}/DetailsWithUsers/{organizationalStructureId:int}/{published:bool?}/{organizationEntityPublished:bool?}")]
        public async Task<IActionResult> GetDetailsWithUsersPerApprovalLevel(int id, int organizationalStructureId, bool? published = null, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _approvalLevel
                                  .GetDetailsWithUsersPerApprovalLevel(id, organizationalStructureId, published, organizationEntityPublished);

                var data = new { APPROVALLEVELDETAILS = query };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{organizationEntityPublished:bool?}")]
        public IActionResult GetApprovalLevels(bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _approvalLevel.GetApprovalLevels(organizationEntityPublished);

                var data = new { APPROVALLEVEL = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("PerOrganizationalStructure/{organizationalStructureId:int}/{organizationalStruturePublished:bool?}/{organizationalEntityPublished:bool?}")]
        public async Task<IActionResult> GetApprovalLevelsPerOrganizationalStructure(int organizationalStructureId, bool? organizationalStruturePublished = null, bool? organizationalEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _approvalLevel.GetApprovalLevels(organizationalStructureId, organizationalStruturePublished, organizationalEntityPublished);

                var data = new { APPROVALLEVEL = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id:int}/{organizationEntityPublished:bool?}")]
        public IActionResult GetApprovalLevels(int id, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _approvalLevel.GetApprovalLevels(organizationEntityPublished).Where(a => a.Id == id).FirstOrDefault();

                if (query == null)
                    return NotFound();

                var data = new { APPROVALLEVEL = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Detail/{approvalLevelDetailId:int}/ReportingTo/{organizationalStructureId:int}/{approvalLevelPublished:bool?}/{organizationalStructurePublished:bool?}/{organizationEntityPublished:bool?}/{userPublished:bool?}")]
        public async Task<IActionResult> GetApprovalLevelReportingTo(int approvalLevelDetailId, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _approvalLevel
                                  .GetApprovalLevelReportingTo(approvalLevelDetailId, organizationalStructureId, approvalLevelPublished, organizationalStructurePublished, organizationEntityPublished);

                var data = new { APPROVALLEVELREPORTINGTO = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Structures/{organizationalStructureId:int}/{userOrganizationalStructureId:int}/{published:bool?}/{organizationalStructurePublished:bool?}/{organizationEntityPublished:bool?}/{userPublished:bool?}")]
        public async Task<IActionResult> GetApprovalLevelStructures(int organizationalStructureId = 0, int userOrganizationalStructureId = 0, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _approvalLevel.GetApprovalLevelStructures(organizationalStructureId, userOrganizationalStructureId, published, organizationalStructurePublished, organizationEntityPublished, userPublished);

                var data = new { APPROVALLEVELSTRUCTURES = query };

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

        [HttpGet("Detail/{approvalLevelDetailId:int}/SpecificReportingTo/{reportingTo:int}/{organizationalStructureId:int}/{approvalLevelPublished:bool?}/{organizationalStructurePublished:bool?}/{organizationEntityPublished:bool?}/{userPublished:bool}")]
        public async Task<IActionResult> GetSpecificReportingTo(int approvalLevelDetailId, int reportingTo, int organizationalStructureId, bool? approvalLevelPublished = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null, bool? userPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _approvalLevel.GetSpecificApprovalLevelReportingToAsync(approvalLevelDetailId, reportingTo, organizationalStructureId, approvalLevelPublished, organizationalStructurePublished, organizationEntityPublished, userPublished);

                var data = new { APPROVALLEVELSTRUCTURES = query };

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
