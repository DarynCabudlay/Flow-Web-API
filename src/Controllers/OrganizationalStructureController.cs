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
    public class OrganizationalStructureController : ControllerBase
    {
        private readonly IOrganizationalStructure _organizationalStructure;
        public OrganizationalStructureController(IOrganizationalStructure organizationalStructure) 
        {
            _organizationalStructure = organizationalStructure;
        }

        [HttpPost("saveOrganizationEntity")]
        public async Task<IActionResult> SaveOrganizationEntity(OrganizationEntityViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _organizationalStructure.SaveOrganizationEntity(model);

                if (model.Id <= 0)
                    return StatusCode(201);
                else
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

        [HttpPut("getOrganizationEntities")]
        public IActionResult GetOrganizationEntities([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _organizationalStructure.GetOrgranizationEntities(paging);

                var pagingResponse = _organizationalStructure.OrganizationEntitiesPagingFeature(query, paging);

                var data = new { ORGANIZATIONENTITIES = pagingResponse };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getOrganizationEntities")]
        public IActionResult GetOrganizationEntities(bool? published = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _organizationalStructure.GetOrgranizationEntities(null, published);

                var data = new { ORGANIZATIONENTITIES = query };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpDelete("deleteOrganizationEntity")]
        public async Task<IActionResult> DeleteOrganizationEntity(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _organizationalStructure.DeleteOrganizationEntity(id);

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

        [HttpPut("getOrganizationalStructures")]
        public IActionResult GetOrganizationalStructures([FromBody] PagingRequest paging, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _organizationalStructure.GetOrganizationalStructures(paging, published, organizationEntityPublished, usersPublished);

                var pagingResponse = _organizationalStructure.PagingFeature(query, paging);

                var data = new { ORGANIZATIONALSTRUCTURES = pagingResponse };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("TopLevels/{published:bool?}/{organizationEntityPublished:bool?}")]
        public IActionResult GetOrganizationalStructuresTopLevels(bool? published = null, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _organizationalStructure
                            .GetOrganizationalStructures(null, published, organizationEntityPublished)
                            .Where(o => o.ParentId == 0)
                            .Select
                            (
                                o => new
                                {
                                    o.Id,
                                    o.Name,
                                    o.OrganizationEntityId,
                                    o.OrganizationEntityName,
                                    o.Published
                                }
                            );

                var data = new { ORGANIZATIONALSTRUCTURES = query };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getOrganizationalStructures")]
        public IActionResult GetOrganizationalStructures(bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _organizationalStructure.GetOrganizationalStructures(null, published, organizationEntityPublished, usersPublished);

                var data = new { ORGANIZATIONALSTRUCTURES = query };

                return Ok(data);

            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveOrganizationalStructure")]
        public async Task<IActionResult> SaveOrganizationalStructure(OrganizationalStructureViewModel model)
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
                    var organizationalStructure = await _organizationalStructure.Add(model);
                    return StatusCode(201, organizationalStructure);
                }
                else
                {
                    await _organizationalStructure.Update(model);
                    return NoContent();
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

        [HttpDelete("deleteOrganizationalStructure")]
        public async Task<IActionResult> DeleteOrganizationalStructure(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _organizationalStructure.Delete(id);

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

        [HttpGet("getWholeOrganizationalStructures")]
        public async Task<IActionResult> GetWholeOrganizationalStructures(bool structuresOnly = false, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _organizationalStructure.GetWholeOrganizationalStructures(structuresOnly, published, organizationEntityPublished, usersPublished);

                var data = new { ORGANIZATIONALSTRUCTURES = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("TabularViewWithPaging/{published:bool?}/{organizationEntityPublished:bool?}/{usersPublished:bool?}")]
        public async Task<IActionResult> GetWholeOrganizationalStructuresInTabularViewWithPaging([FromBody] PagingRequest paging, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _organizationalStructure.GetWholeOrganizationalStructuresInTabularView(paging, published, organizationEntityPublished, usersPublished);

                var pagingResponse = _organizationalStructure.DataTablePagingFeature(query.Details, paging);

                pagingResponse.ColumnHeaders = query.ColumnHeaders;

                var data = new { ORGANIZATIONALSTRUCTURES = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("TabularView/{published:bool?}/{organizationEntityPublished:bool?}/{usersPublished:bool?}")]
        public async Task<IActionResult> GetWholeOrganizationalStructuresInTabularView(bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var queryModel = (await _organizationalStructure.GetWholeOrganizationalStructuresInTabularView(null, published, organizationEntityPublished, usersPublished));

                var data = new { ORGANIZATIONALSTRUCTURES = queryModel };

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
