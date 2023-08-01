//New Forked Solution (5/22/2023)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using workflow.Data;
using workflow.Models;
using workflow.Services;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using workflow.Models.Hangfire;
using workflow.Controllers;

namespace workflow
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "workflow", Version = "v1" });
            });

            var domain = Configuration.GetSection("AppSettings")["Domain"];
            bool isMSSQL = Convert.ToBoolean(Configuration.GetSection("AppSettings")["MsSql"]);
            if (isMSSQL)
            {
                var connString = Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<FliDbContext>(options =>
                    options.UseSqlServer(connString));

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connString));
            }
            else
            {
                // var connString = Configuration.GetConnectionString("MySqlConnection");
                // services.AddDbContext<FliDbContext>(options =>
                //     options.UseMySQL(connString));

                // services.AddDbContext<ApplicationDbContext>(options =>
                //     options.UseMySQL(connString));
            }
            services.AddMvc();

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            //services.ConfigureExternalProviders(Configuration);

            //Initialize JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = domain,
                    ValidAudience = domain,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings")["Token"]))
                };
            });


            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(o => { });


            // Add application services.
            //TransientsCollections(ref services);

            services = TransientsCollections(services);

            services.AddControllers();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "workflow v1"));
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            //context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }


            var clientSubscriber = Configuration.GetSection("AppSettings")["AngularPath"];

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //Temporary commented due to issue if with authorization
            //app.UseHangfireDashboard("/JobDashboard", new DashboardOptions()
            //{
            //    Authorization = new IDashboardAuthorizationFilter[] { new HangfireAuthorizationFilter() }
            //});

            app.UseHangfireDashboard("/JobDashboard");
            RunCronJobs(backgroundJobs); //run jobs

            app.UseRouting();
            //app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCors(x => x.WithOrigins(clientSubscriber).AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "app-fallback",
                    pattern: "{controller=Fallback}/{action=Index}");
                endpoints.MapFallbackToController("Index", "Fallback");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }

        private IServiceCollection TransientsCollections(IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IRequestCategory, RequestCategoryRepository>();
            services.AddTransient<IRequestType, RequestTypeRepository>();
            services.AddTransient<IOption, OptionRepository>();
            services.AddTransient<IRole, RoleRepository>();
            services.AddTransient<IMenu, MenuRepository>();
            services.AddTransient<IAccount, AccountRepository>();
            services.AddTransient<ILog, LogRepository>();
            services.AddTransient<IUser, UserRepository>();
            services.AddTransient<IPermission, PermissionRepository>();
            services.AddTransient<ISetting, SettingRepository>();
            services.AddTransient<ICronJobs, CronJobRepository>();
            services.AddTransient<IReason, ReasonRepository>();
            services.AddTransient<IUserIPAddressPerSession, UserIPAddressPerSessionRepository>();
            services.AddTransient<IHoliday, HolidayRepository>();
            services.AddTransient<IOrganizationalStructure, OrganizationalStructureRepository>();
            services.AddTransient<IApprovalLevel, ApprovalLevelRepository>();
            services.AddTransient<IRequestTypesGrouping, RequestTypesGroupingRepository>();
            services.AddTransient<IRequestTypesGroupingDetail, RequestTypesGroupingDetailRepository>();
            services.AddTransient<IRequestTypesGroupingCommon, RequestTypesGroupingCommonRepository>();
            services.AddTransient<IFileType, FileTypeRepository>();
            services.AddTransient<ILinkType, LinkTypeRepository>();
            services.AddTransient<IUserRequestTypesGrouping, UserRequestTypesGroupingRepository>();
            services.AddTransient<IUserCommon, UserCommonRepository>();
            services.AddTransient<IUserNotAllowedLinkType, UserNotAllowedLinkTypeRepository>();
            services.AddTransient<ITicket, TicketRepository>();
            services.AddTransient<ITicketRequest, TicketRequestRepository>();
            services.AddTransient<ITicketRequestDetail, TicketRequestDetailRepository>();
            services.AddTransient<ITicketBaseWorkFlow, TicketBaseWorkFlowRepository>();
            services.AddTransient<ITicketBaseWorkFlowAssignee, TicketBaseWorkFlowAssigneeRepository>();
            services.AddTransient<ITicketCommon, TicketCommonRepository>();
            services.AddTransient<ITicketAttachment, TicketAttachmentRepository>();

            return services;
        }

        private void RunCronJobs(IBackgroundJobClient backgroundJobs)
        {
            //backgroundJobs.Enqueue(() => crn.ProcessUserAccountStates());
            //backgroundJobs.Enqueue<ICronJobs>((x => x.ProcessUserAccountStates()));
            //RecurringJob.AddOrUpdate("Update User Account State : Runs once a day", () => crn.ProcessUserAccountStates(), Cron.Daily);
            //RecurringJob.AddOrUpdate("Update User Account State : Runs once a day", () => crn.ProcessUserAccountStates(), "*/1 * * * *");
            RecurringJob.AddOrUpdate<ICronJobs>("Remove Login Attempts And Tagged As Dormant : Runs once a day", (x => x.RemoveLoginAttemptsAndTaggedAsDormant()), Cron.Daily);
            RecurringJob.AddOrUpdate<ICronJobs>("Tag As Password Expired : Runs every minute", (x => x.TagAsPasswordExpired()), Cron.Minutely);

        }
    }
}
