using Microsoft.AspNetCore.Mvc;
using workflow.Models.AccountViewModels;
using workflow.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using workflow.Services;
using System;
using Microsoft.AspNetCore.Http;
using workflow.Helpers;

namespace workflow.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _account;

        public AccountController(IAccount account)
        {
            _account = account;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _account.Register(model);
                    return Ok(result);
                }
                returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                return StatusCode(400, returnObject);
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

        [HttpGet("confirmemail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _account.ConfirmEmail(userId, code);
                return StatusCode(201);
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (ModelState.IsValid)
                {
                    var ip = Request.Host.Value;
                    var result = await _account.Login(model);

                    return Ok(result);
                }
                returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                return BadRequest(returnObject);
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

        [HttpGet("isLoggedIn")]
        public IActionResult IsLoggedIn()
        {
            return Ok(true);
        }

        [HttpGet("isAuthorized")]
        public IActionResult IsAuthorized(string url)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var isAuthorized = _account.IsAuthorized(url);

                return Ok(isAuthorized);

                //if (isAuthorized)
                //    return Ok(isAuthorized);
                //else
                //    return StatusCode(403);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("isAllowed")]
        [AllowAnonymous]
        public IActionResult IsAllowed(string id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var isAllowed = _account.IsAllowed(id);
                return Ok(isAllowed);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string ulhid)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _account.Logout(ulhid);
                return Ok();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (ModelState.IsValid)
                {
                    await _account.ForgotPassword(model);
                    return Ok();
                }
                returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                return StatusCode(400, returnObject);
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

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(400, returnObject);
                }

                await _account.ResetPassword(model);
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

        [HttpGet("getCurrentUser")]
        public IActionResult GetCurrentUser()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var obj = _account.GetCurrentUser().Result;

                var data = new { CURRENTUSER = obj.Object1 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("generateQRCodeUri")]
        public IActionResult GenerateQRCodeUri()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                return Ok(new { _account.GenerateQRCodeUri().Result });
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost("verifyAuthApp")]
        public async Task<IActionResult> VerifyAuthApp(TwoFactorAuthModel TFM)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (ModelState.IsValid)
                {
                    await _account.VerifyAuthApp(TFM);
                    return Ok();
                }
                return StatusCode(201);
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

        [HttpGet("disable2fa")]
        public async Task<IActionResult> Disable2fa()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                await _account.Disable2fa();
                return Ok();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpGet("externalLogin")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider) //, string returnUrl = null
        {
            var redirectUrl = Url.Action("Callback", "Account");
            var properties = _account.ExternalLogin(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> Callback(string returnUrl = null, string remoteError = null)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var result = await _account.Callback(returnUrl, remoteError);
                return Redirect(result);
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }

        }

        [HttpGet("providers")]
        [AllowAnonymous]
        public async Task<IActionResult> Providers()
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                return await _account.Providers();
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPost]
        [Route("associate")]
        [AllowAnonymous]
        public async Task<IActionResult> Associate([FromBody] AssociateViewModel associate)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var result = await _account.Associate(associate);

                return Ok(result);
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

        [HttpPost("setpassword")]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                }

                var result = await _account.SetPassword(model);

                if (result.Succeeded)
                    return Ok();
                else
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, result.Errors.ToString());
                    return StatusCode(400, returnObject);
                }
            }
            catch (Exception ex)
            {
                returnObject = GeneralHelper.SetReturnDetails(500, (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return StatusCode(returnObject.Code, returnObject);
            }
        }

        [HttpPut("updateUserState")]
        public async Task<IActionResult> UpdateUserState(UserStateParam param)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                    return StatusCode(returnObject.Code, returnObject);
                };

                await _account.UpdateUserState(param.Id, param.State);
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

        [HttpGet("getApplicableUserState")]
        public async Task<IActionResult> GetApplicableUserState(string Id, UserAccountState fromstate)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var states = await _account.GetApplicableUserState(Id, fromstate);
                return Ok(states);
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

        [HttpPut("initialPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> InitialPassword(InitialPasswordViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _account.InitialPassword(model);
                    return Ok(result);
                }
                returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                return StatusCode(returnObject.Code, returnObject);
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

        [HttpPut("changePasswordExpired")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePasswordExpired(ChangePasswordExpiredViewModel model)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _account.ChangePasswordExpired(model);
                    return Ok(result);
                }
                returnObject = GeneralHelper.SetReturnDetails(400, "Invalid Model");
                return StatusCode(returnObject.Code, returnObject);
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

        [HttpGet("getCurrentUserState")]
        public async Task<IActionResult> GetCurrentUserState(string Id)
        {
            APIReturnObject returnObject = new APIReturnObject();
            try
            {
                var userinfo = await _account.GetCurrentUserState(Id);
                return Ok(userinfo);
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