using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using workflow.Data;
using workflow.Models;

namespace workflow.Services
{
    public abstract class BaseApi : ControllerBase
    {
        // class that we use in our application code to interact with the underlying database and user management
        //protected readonly ApplicationDbContext _applicationDbContext;
        // class that we use in our application code to interact with the underlying database
        protected readonly FliDbContext _dbCntxt;
        // ASP.NET Core Identity is a fully featured membership system for creating and maintaining user logins. 
        // concrete class that manages the user.
        protected readonly UserManager<ApplicationUser> _userManager;
        // provides the APIs for managing roles in a persistence store
        protected readonly RoleManager<IdentityRole> _roleManager;
        // concrete class which handles the user sign in from the application.
        protected readonly SignInManager<ApplicationUser> _signInManager;
        // provides logging API
        protected readonly ILogger _logger;
        // this is really just meant for configuration
        protected readonly IConfiguration _config;
        // provides information about the web hosting environment an application is running in
        protected IWebHostEnvironment _env;

        protected IHttpContextAccessor _httpContextAccessor;

        public BaseApi(
                //ApplicationDbContext applicationDbContext,
                FliDbContext dbCntxt, 
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<BaseApi> logger, 
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor
        )
        {
            //_applicationDbContext = applicationDbContext;
            _dbCntxt = dbCntxt;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _config = config;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }
        
        // Desconstructor
        ~BaseApi()
        {
            this._dbCntxt.Dispose();
            //this._applicationDbContext.Dispose();
        }

        public void TransactionLogging(ChangeLog model)
        {             
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Code Here
                
            }
            catch (Exception)
            {
                dbContextTransaction.Rollback();
            }
        }
        public void ErrorLogging(ChangeLog model)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {   
                // get current user id
                var cId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Code Here

            }
            catch (Exception)
            {
            }
        }

    }
}
