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
using System.IO;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicket _ticket;
        private readonly ITicketRequest _ticketRequest;
        private readonly ITicketRequestDetail _ticketRequestDetail;
        private readonly ITicketCommon _ticketCommon;
        private readonly ITicketBaseWorkFlow _ticketBaseWorkFlow;
        private readonly ITicketAttachment _ticketAttachment;

        public TicketController(ITicket ticket, ITicketRequest ticketRequest, ITicketRequestDetail ticketRequestDetail, ITicketCommon ticketCommon, ITicketBaseWorkFlow ticketBaseWorkFlow, ITicketAttachment ticketAttachment) 
        {
            _ticket = ticket;
            _ticketRequest = ticketRequest;
            _ticketRequestDetail = ticketRequestDetail;
            _ticketCommon = ticketCommon;
            _ticketBaseWorkFlow = ticketBaseWorkFlow;
            _ticketAttachment = ticketAttachment;
        }

        #region Display

        [HttpPut("WithPaging/Status/{status:int}")]
        public IActionResult GetTickets([FromBody] PagingRequest paging, int status = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _ticketCommon.GetTickets(paging, status);

                var pagingResponse = _ticketCommon.TicketsPagingFeature(query, paging);

                var data = new { TICKETS = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Status/{status:int}")]
        public async Task<IActionResult> GetTickets(int status = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _ticketCommon.GetTickets(null, status).ToListAsync();

                var data = new { TICKETS = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _ticketCommon.GetTicket(id)
                                               .FirstOrDefaultAsync();

                var data = new { TICKET = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{ticketId:int}/BaseWorkFlowSteps")]
        public async Task<IActionResult> GetTicketBaseWorkFlowSteps(int ticketId)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = (await _ticketBaseWorkFlow.GetTicketBaseWorkFlowSteps(ticketId)).ToList();
                                                     
                var data = new { STEPTYPELIST = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{ticketId:int}/BaseStepsAndWorkFlows")]
        public async Task<IActionResult> GetTicketBaseStepsAndWorkFlows(int ticketId)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var workflows = await _ticketBaseWorkFlow.GetTicketBaseStepsAndWorkFlows(ticketId);

                var data = new { WORKFLOWS = workflows };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("Attachments/WithPaging/Category/{category:int}")]
        public IActionResult GetTicketAttachments([FromBody] PagingRequest paging, int category = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _ticketCommon.GetTicketAttachments(paging, category);

                var pagingResponse = _ticketCommon.TicketAttachmentsPagingFeature(query, paging);

                var data = new { TICKETATTACHMENTS = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Attachments/Category/{category:int}")]
        public async Task<IActionResult> GetTicketAttachments(int category = 0)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _ticketCommon.GetTicketAttachments(null, category).ToListAsync();

                var data = new { TICKETATTACHMENTS = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Attachments/{id:int}")]
        public async Task<IActionResult> GetTicketAttachment(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _ticketCommon.GetTicketAttachment(id)
                                               .FirstOrDefaultAsync();

                var data = new { TICKETATTACHMENT = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("Attachments/{id:int}/Download")]
        public async Task<IActionResult> DownloadTicketAttachment(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var file = await _ticketCommon.GetTicketAttachment(id)
                                              .FirstOrDefaultAsync();

                var bytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
                return File(bytes, "text/plain", file.OriginalFileName);
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
        public async Task<IActionResult> AddTicket([FromBody] TicketViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var ticket = await _ticket.Add(model);
                return StatusCode(201, ticket);
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

        [HttpPost("Request")]
        public async Task<IActionResult> AddTicketRequest([FromBody] TicketRequestViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var ticketRequest = await _ticketRequest.Add(model);
                return StatusCode(201, ticketRequest);
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

        [HttpPost("Attachment")]
        public async Task<IActionResult> AddTicketAttachment([FromBody] TicketAttachmentViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var ticketAttachment = await _ticketAttachment.Add(model);
                return StatusCode(201, ticketAttachment);
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
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketViewModel model)
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

                await _ticket.Update(model);
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

        [HttpPut("Request/{id:int}")]
        public async Task<IActionResult> UpdateTicketRequest(int id, [FromBody] TicketRequestViewModel model)
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

                await _ticketRequest.Update(model);
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

        [HttpPut("Attachment/{id:int}")]
        public async Task<IActionResult> UpdateTicketAttachment(int id, [FromBody] TicketAttachmentViewModel model)
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

                await _ticketAttachment.Update(model);
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
        public async Task<IActionResult> DeleteTicket(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _ticket.Delete(id);
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

        [HttpDelete("Request/{id:int}")]
        public async Task<IActionResult> DeleteTicketRequest(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _ticketRequest.Delete(id);
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

        [HttpDelete("Attachment/{id:int}")]
        public async Task<IActionResult> DeleteTicketAttachment(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _ticketAttachment.Delete(id);
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



        #endregion

        #region Cancel

        [HttpPut("Cancel")]
        public async Task<IActionResult> CancelTicket([FromBody] CancelTicketViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _ticket.Cancel(model);
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

        #region Submit

        [HttpPut("Submit/{id:int}")]
        public async Task<IActionResult> SubmitTicket(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _ticket.Submit(id);
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
