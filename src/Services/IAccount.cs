using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.AccountViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IAccount 
    {
        Task<Boolean> Register(RegisterViewModel model);
        Task ConfirmEmail(string userId, string code);
        Task<Object> Login(LoginViewModel model);
        Boolean IsAuthorized(string url);
        Boolean IsAllowed(string id);
        Task Logout(string ulhid);
        Task ForgotPassword(ForgotPasswordViewModel model);
        Task ResetPassword(ResetPasswordViewModel model);
        Task<String> GenerateQRCodeUri();
        Task<ObjectReturnModel> GetCurrentUser();
        Task VerifyAuthApp(TwoFactorAuthModel TFM);
        Task Disable2fa();
        AuthenticationProperties ExternalLogin(string provider, string redirectUrl = null);
        Task<String> Callback(string returnUrl = null, string remoteError = null);
        Task<Object> Associate(AssociateViewModel associate);
        Task<IActionResult> Providers();
        Task<IdentityResult> SetPassword(SetPasswordViewModel model);
        Task UpdateUserState(string userid, UserAccountState state);
        Task<List<UserAccountState>> GetApplicableUserState(string userid, UserAccountState fromstate);
        Task<Object> InitialPassword(InitialPasswordViewModel model);
        Task<Object> ChangePasswordExpired(ChangePasswordExpiredViewModel model);
        Task<Object> GetCurrentUserState(string userid);
    }
}
