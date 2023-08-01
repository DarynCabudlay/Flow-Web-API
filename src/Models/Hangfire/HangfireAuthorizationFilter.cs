using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using workflow.Services;
using System.Web;

namespace workflow.Models.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {       
        public bool Authorize([NotNull] DashboardContext context)
        {
            var _httpcontext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            //HttpContextAccessor _httpcontext = new HttpContextAccessor();
            //return _httpcontext.HttpContext.User.Identity.IsAuthenticated;

            return _httpcontext.User.Identity.IsAuthenticated;
        }
    }
}
