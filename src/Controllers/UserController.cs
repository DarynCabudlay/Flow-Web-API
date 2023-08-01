using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.ManageViewModels;
using workflow.Services;
using Microsoft.AspNetCore.Http;
using workflow.Models.DataTableViewModels;
using workflow.Helpers;
using System.Collections.Generic;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IRequestType _requestType;
        private readonly IUserRequestTypesGrouping _userRequestTypesGrouping;
        private readonly IUserCommon _userCommon;
        private readonly IUserNotAllowedLinkType _userNotAllowedLinkType;

        public UserController(IUser user, IRequestType requestType, IUserRequestTypesGrouping userRequestTypesGrouping, IUserCommon userCommon, IUserNotAllowedLinkType userNotAllowedLinkType)
        {
            _user = user;
            _requestType = requestType;
            _userRequestTypesGrouping = userRequestTypesGrouping;
            _userCommon = userCommon;
            _userNotAllowedLinkType = userNotAllowedLinkType;
        }

        [HttpPut]
        public IActionResult GetUserListWithPaging([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var users = _user.GetUserList(paging);

                var pagingResponse = _user.PagingFeature(users, paging);

                var data = new { USERLIST = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var users = await _user.GetUserList()
                                       .ToListAsync();

                var data = new { USERLIST = users };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById(string Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                IQueryable<UserRegisterModel> query = null;
                query = _user.Find(Id);

                var data = new { USER = await query.FirstOrDefaultAsync() };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("getUserPageVisited")]
        public IActionResult GetUserPageVisited([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<PageVisitedViewModel> query = null;

                query = _user.GetPageVisited(null, paging);

                var pagingResponse = _user.PagingPageVisited(query, paging);

                var data = new { PAGEVISITEDLIST = pagingResponse };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("getUserLoginHistory")]
        public IActionResult GetUserLoginHistory([FromBody] PagingRequest paging)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                // initialize request type list 
                IQueryable<LoginHistoryViewModel> query = null;

                query = _user.GetLoginHistory(null, paging);

                var pagingResponse = _user.PagingLogHistory(query, paging);

                var data = new { LOGINHISTORYLIST = pagingResponse };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("saveUser")]
        public async Task<IActionResult> SaveUser(SaveUserModel model)
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
                    var newUser = await _user.Add(model);
                    return StatusCode(201, newUser); 
                }
                {
                    await _user.Update(model);
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

        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _user.Delete(Id);

                return NoContent();
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

        [HttpGet("getProfileData")]
        public async Task<IActionResult> GetProfileData()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var obj = await _user.GetProfileData();

                //Temp commented only
                //var data = new { USER = obj.Object1, ISTWOFACTORENABLED = obj.Object2, SETPASS = obj.Object3 };
                //return Ok(data);

                var data = new { USER = obj.Object1, ROLES = obj.Object2, LOGINLIST = obj.Object3, VISITEDLIST = obj.Object4, ISTWOFACTORENABLED = obj.Object5, SETPASS = obj.Object6 };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("updateprofile")]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _user.UpdateProfile(model);

                return Ok();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var result = await _user.ChangePassword(model);

                if (result.Succeeded)
                    return Ok();
                else
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, result.Errors.ToString());
                    return StatusCode(returnObject.Code, returnObject);
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

        [HttpPost("saveUserPreferences")]
        public async Task<IActionResult> SavePreferences(UserPreferencesViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }
                await _user.SavePreferences(model);

                return Ok();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getUserPreferences")]
        public async Task<IActionResult> GetPreferences()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var preferences = await _user.GetUserPreferences();

                var data = new { PREFERENCES = preferences };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("resendInitialPassword")]
        public async Task<IActionResult> ResendInitialPassword(string Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _user.ResendInitialPassword(Id);
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

        [HttpGet("getUsersForProcessOwners")]
        public async Task<IActionResult> GetUsersForProcessOwners(int RequestTypeId, int Version)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var processowners = await _requestType.GetRequestProcessOwners(null, RequestTypeId, Version).ToListAsync();

                var owners = processowners.Select(o => o.Owner).ToList();

                var users = await _user.FindAll()
                                       .Where(u => u.Status == true && !owners.Contains(u.Id))
                                       .Select
                                       (
                                          u => new
                                          {
                                              Id = u.Id,
                                              FirstName = u.FirstName,
                                              LastName = u.LastName,
                                              FullName = u.FullName,
                                              FullName2 = u.FullName2,
                                              Position = u.Position,
                                              Department = u.Department,
                                              Email = u.Email
                                          }
                                       )
                                       .OrderBy(u => u.FullName)
                                       .ToListAsync();


                var data = new { USERS = users };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers(bool? status = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var users = await _user.GetUsers();

                if (status != null)
                    users = users.Where(u => u.Status == status);

                var data = new { USERS = users };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id}/Roles")]
        public async Task<IActionResult> GetUserRole(string id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var userRoles = await _user.GetUserRole(id)
                                           .Select
                                           (    
                                                u =>
                                                new { Id = u.RoleId, Name = u.Role, IsChecked = false }
                                           )
                                           .ToListAsync();

                var data = new { USERROLES = userRoles };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("UserOrganizationalStructures")]
        public async Task<IActionResult> SaveUserOrganizationalStructures(SaveUserOrganizationalStructureWithDBValidationViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _user.SaveUserOrganizationalStructures(model);

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

        [HttpPost("Roles")]
        public async Task<IActionResult> SaveUserRoles(List<UserRoleViewModel> model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _user.SaveUserRoles(model);

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

        [HttpPut("{id}/UserOrganizationalStructuresWithPaging/{published:bool?}/{organizationalStructurePublished:bool?}/{organizationEntityPublished:bool?}")]
        public IActionResult GetUserOrganizationalStructures(string id, [FromBody] PagingRequest paging, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query =  _user.GetUserOrganizationalStructuresPerUser(id, paging, published, organizationalStructurePublished, organizationEntityPublished);

                var pagingResponse = _user.UserOrganizationalStructuresPagingFeature(query, paging);

                var data = new { USERORGANIZATIONALSTRUCTURES = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id}/UserOrganizationalStructures/{published:bool?}/{organizationalStructurePublished:bool?}/{organizationEntityPublished:bool?}")]
        public async Task<IActionResult> GetUserOrganizationalStructures(string id, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationEntityPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _user.GetUserOrganizationalStructuresPerUser(id, null, published, organizationalStructurePublished, organizationEntityPublished).ToListAsync();

                var data = new { USERORGANIZATIONALSTRUCTURES = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("UserOrganizationalStructure/{id:int}")]
        public async Task<IActionResult> GetUserOrganizationalStructure(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var orgStructure = await _user.GetUserOrganizationalStructures()
                                              .FirstOrDefaultAsync(u => u.Id == id);

                var data = new { USERORGANIZATIONALSTRUCTURE = orgStructure };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpDelete("UserOrganizationalStructure/{id:int}")]
        public async Task<IActionResult> DeleteUserOrganizationalStructure(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _user.DeleteUserOrganizationalStructure(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("SaveUserRequestTypesGrouping")]
        public async Task<IActionResult> SaveUserRequestTypesGrouping([FromBody] SaveUserRequestTypesGroupingWithCheckingDetailEntryViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _userRequestTypesGrouping.SaveUserRequestTypesGrouping(model);

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

        [HttpPost("UserRequestTypesGrouping")]
        public async Task<IActionResult> AddUserRequestTypesGrouping([FromBody] UserRequestTypesGroupingViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var userRequestTypeGrouping = await _userRequestTypesGrouping.Add(model);
                return StatusCode(201, userRequestTypeGrouping);
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

        [HttpPut("UserRequestTypesGrouping/{id:int}")]
        public async Task<IActionResult> UpdateUserRequestTypesGrouping(int id, [FromBody] UserRequestTypesGroupingViewModel model)
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

                await _userRequestTypesGrouping.Update(model);
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


        [HttpDelete("UserRequestTypesGrouping/{id:int}")]
        public async Task<IActionResult> DeleteUserRequestTypesGrouping(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _userRequestTypesGrouping.Delete(id);
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

        [HttpPut("{id}/UserRequestTypesGroupingWithPaging/{published:bool?}/{requestTypesGroupingPublished:bool?}")]
        public IActionResult GetUserRequestTypesGroupingPerUser(string id, [FromBody] PagingRequest paging, bool? published = null, bool? requestTypesGroupingPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _userCommon.GetUserRequestTypesGroupings(paging, published, requestTypesGroupingPublished)
                                       .Where(u => u.UserId == id);

                var pagingResponse = _userCommon.UserRequestTypeGroupingsPagingFeature(query, paging);

                var data = new { USERREQUESTTYPESGROUPING = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id}/UserRequestTypesGrouping/{published:bool?}/{requestTypesGroupingPublished:bool?}")]
        public async Task<IActionResult> GetUserRequestTypesGroupingPerUser(string id, bool? published = null, bool? requestTypesGroupingPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _userCommon.GetUserRequestTypesGroupings(null, published, requestTypesGroupingPublished)
                                       .Where(u => u.UserId == id)
                                       .ToListAsync();

                var data = new { USERREQUESTTYPESGROUPING = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("UserRequestTypesGroupingWithPaging/{published:bool?}/{requestTypesGroupingPublished:bool?}")]
        public IActionResult GetUserRequestTypesGrouping([FromBody] PagingRequest paging, bool? published = null, bool? requestTypesGroupingPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _userCommon.GetUserRequestTypesGroupings(paging, published, requestTypesGroupingPublished);

                var pagingResponse = _userCommon.UserRequestTypeGroupingsPagingFeature(query, paging);

                var data = new { USERREQUESTTYPESGROUPING = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("UserRequestTypesGrouping/{published:bool?}/{requestTypesGroupingPublished:bool?}")]
        public async Task<IActionResult> GetUserRequestTypesGrouping(bool? published = null, bool? requestTypesGroupingPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _userCommon.GetUserRequestTypesGroupings(null, published, requestTypesGroupingPublished)
                                             .ToListAsync();

                var data = new { USERREQUESTTYPESGROUPING = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }


        [HttpPost("SaveUserNotAllowedLinkType")]
        public async Task<IActionResult> SaveUserNotAllowedLinkType([FromBody] SaveUserNotAllowedLinkTypeWithCheckingDetailEntryViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _userNotAllowedLinkType.SaveUserNotAllowedLinkType(model);

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

        [HttpPost("UserNotAllowedLinkType")]
        public async Task<IActionResult> AddUserNotAllowedLinkType([FromBody] UserNotAllowedLinkTypeViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var userNotAllowedLinkType = await _userNotAllowedLinkType.Add(model);
                return StatusCode(201, userNotAllowedLinkType);
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

        [HttpPut("UserNotAllowedLinkType/{id:int}")]
        public async Task<IActionResult> UpdateUserNotAllowedLinkType(int id, [FromBody] UserNotAllowedLinkTypeViewModel model)
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

                await _userNotAllowedLinkType.Update(model);
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

        [HttpDelete("UserNotAllowedLinkType/{id:int}")]
        public async Task<IActionResult> DeleteUserNotAllowedLinkType(int id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                await _userNotAllowedLinkType.Delete(id);
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

        [HttpPut("{id}/UserNotAllowedLinkTypeWithPaging/{published:bool?}/{linkTypePublished:bool?}")]
        public IActionResult GetUserNotAllowedLinkTypePerUser(string id, [FromBody] PagingRequest paging, bool? published = null, bool? linkTypePublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _userCommon.GetUserNotAllowedLinkTypes(paging, published, linkTypePublished)
                                       .Where(u => u.UserId == id);

                var pagingResponse = _userCommon.UserNotAllowedLinkTypePagingFeature(query, paging);

                var data = new { USERNOTALLOWEDLINKTYPE = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("{id}/UserNotAllowedLinkType/{published:bool?}/{linkTypePublished:bool?}")]
        public async Task<IActionResult> GetUserNotAllowedLinkTypePerUser(string id, bool? published = null, bool? requestTypesGroupingPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _userCommon.GetUserNotAllowedLinkTypes(null, published, requestTypesGroupingPublished)
                                       .Where(u => u.UserId == id)
                                       .ToListAsync();

                var data = new { USERNOTALLOWEDLINKTYPE = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("UserNotAllowedLinkTypeWithPaging/{published:bool?}/{linkTypePublished:bool?}")]
        public IActionResult GetUserNotAllowedLinkTypes([FromBody] PagingRequest paging, bool? published = null, bool? linkTypePublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = _userCommon.GetUserNotAllowedLinkTypes(paging, published, linkTypePublished);

                var pagingResponse = _userCommon.UserNotAllowedLinkTypePagingFeature(query, paging);

                var data = new { USERNOTALLOWEDLINKTYPE = pagingResponse };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("UserNotAllowedLinkType/{published:bool?}/{linkTypePublished:bool?}")]
        public async Task<IActionResult> GetUserNotAllowedLinkTypes(bool? published = null, bool? linkTypePublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = await _userCommon.GetUserNotAllowedLinkTypes(null, published, linkTypePublished)
                                             .ToListAsync();

                var data = new { USERNOTALLOWEDLINKTYPE = query };

                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("UserOrganizationalStructure/{userOrganizationalStructureId:int}/Path/{published:bool?}/{organizationalStructurePublished:bool?}/{organizationEntityPublished:bool?}/{userPublished:bool?}/{approvalLevelPublished:bool?}")]
        public async Task<IActionResult> GetUserOrganizationalStructurePath(int userOrganizationalStructureId, bool? published = null, bool? organizationalStructurePublished = null, bool? organizationalEntityPublished = null, bool? userPublished = null, bool? approvalLevelPublished = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var query = (await _user.GetUserOrganizationalStructurePath(userOrganizationalStructureId, published, organizationalStructurePublished, organizationalEntityPublished, userPublished, approvalLevelPublished)).ToList();

                var data = new { USERORGANIZATIONALSTRUCTUREPATH = query };

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
