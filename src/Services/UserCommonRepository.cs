using api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using workflow.Models;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using workflow.Helpers;
using System.Text.Json;
using System.Data.Entity.Core.Objects;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace workflow.Services
{
    public class UserCommonRepository : BaseApi, IUserCommon
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public UserCommonRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
        }

        private IQueryable<UserRequestTypesGrouping> GetUserRequestTypesGroupingBaseData()
        {
            try
            {
                IQueryable<UserRequestTypesGrouping> query = null;

                query = _dbCntxt.UserRequestTypesGroupings
                                .Include(u => u.User)
                                .Include(u => u.User.AspNetUsersProfile)
                                .Include(r => r.RequestTypesGrouping)
                                .Include(r => r.RequestTypesGrouping.RequestTypesGroupingDetails)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserRequestTypesGroupingViewModel> FindUserRequestTypesGrouping(object obj)
        {
            try
            {

                IQueryable<UserRequestTypesGroupingViewModel> query = null;

                var id = (int)obj;

                query = this.GetUserRequestTypesGroupingBaseData()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new UserRequestTypesGroupingViewModel
                                {
                                    Id = r.Id,
                                    UserId = r.UserId,
                                    User = new UserBasicInfoModel()
                                    {
                                        Id = r.User.Id,
                                        FirstName = r.User.AspNetUsersProfile.FirstName,
                                        LastName = r.User.AspNetUsersProfile.LastName,
                                        MiddleName = r.User.AspNetUsersProfile.MiddleName,
                                        FullName = r.User.AspNetUsersProfile.FirstName + " " + r.User.AspNetUsersProfile.LastName,
                                        Status = r.User.Status
                                    },
                                    RequestTypesGroupId = r.RequestTypesGroupId,
                                    RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                    RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                    RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                    RequestTypeGroupingDetailsCount = r.RequestTypesGrouping.RequestTypesGroupingDetails.Count,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate
                                }
                            )
                            .AsQueryable();

                return query;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserRequestTypesGroupingViewModel> FindAllUserRequestTypesGrouping()
        {
            try
            {
                IQueryable<UserRequestTypesGroupingViewModel> query = null;
                query = this.GetUserRequestTypesGroupingBaseData()
                           .Select
                            (
                                r => new UserRequestTypesGroupingViewModel
                                {
                                    Id = r.Id,
                                    UserId = r.UserId,
                                    User = new UserBasicInfoModel()
                                    {
                                        Id = r.User.Id,
                                        FirstName = r.User.AspNetUsersProfile.FirstName,
                                        LastName = r.User.AspNetUsersProfile.LastName,
                                        MiddleName = r.User.AspNetUsersProfile.MiddleName,
                                        FullName = r.User.AspNetUsersProfile.FirstName + " " + r.User.AspNetUsersProfile.LastName,
                                        Status = r.User.Status
                                    },
                                    RequestTypesGroupId = r.RequestTypesGroupId,
                                    RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                    RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                    RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                    RequestTypeGroupingDetailsCount = r.RequestTypesGrouping.RequestTypesGroupingDetails.Count,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate
                                }
                            )
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserRequestTypesGroupingViewModel> GetUserRequestTypesGroupings(PagingRequest paging = null, bool? published = null, bool? requestTypesGroupingPublished = null)
        {
            try
            {
                IQueryable<UserRequestTypesGroupingViewModel> query = null;
                query = this.FindAllUserRequestTypesGrouping()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(u => u.Published == published);

                if (requestTypesGroupingPublished != null)
                    query = query.Where(u => u.RequestTypesGroupingPublished == requestTypesGroupingPublished);
                            
                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.RequestTypesGroupingName.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<UserRequestTypesGroupingViewModel> UserRequestTypeGroupingsPagingFeature(IQueryable<UserRequestTypesGroupingViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<UserRequestTypesGroupingViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<UserRequestTypesGroupingViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<UserNotAllowedLinkType> GetUserNotAllowedLinkTypeBaseData()
        {
            try
            {
                IQueryable<UserNotAllowedLinkType> query = null;

                query = _dbCntxt.UserNotAllowedLinkTypes
                                .Include(u => u.User)
                                .Include(u => u.User.AspNetUsersProfile)
                                .Include(r => r.LinkType)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserNotAllowedLinkTypeViewModel> FindUserNotAllowedLinkType(object obj)
        {
            try
            {

                IQueryable<UserNotAllowedLinkTypeViewModel> query = null;

                var id = (int)obj;

                query = this.GetUserNotAllowedLinkTypeBaseData()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new UserNotAllowedLinkTypeViewModel
                                {
                                    Id = r.Id,
                                    UserId = r.UserId,
                                    User = new UserBasicInfoModel()
                                    {
                                        Id = r.User.Id,
                                        FirstName = r.User.AspNetUsersProfile.FirstName,
                                        LastName = r.User.AspNetUsersProfile.LastName,
                                        MiddleName = r.User.AspNetUsersProfile.MiddleName,
                                        FullName = r.User.AspNetUsersProfile.FirstName + " " + r.User.AspNetUsersProfile.LastName,
                                        Status = r.User.Status
                                    },
                                    LinkTypeId = r.LinkTypeId,
                                    LinkTypeName = r.LinkType.Name,
                                    LinkTypeURL = r.LinkType.URL,
                                    LinkTypePublished = r.LinkType.Published,
                                    Category = (UserNotAllowedLinkTypeCategory)r.Category,
                                    CategoryDescription = r.Category == 1 ? "Request" : "WorkFlow",
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate
                                }
                            )
                            .AsQueryable();

                return query;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserNotAllowedLinkTypeViewModel> FindAllUserNotAllowedLinkType()
        {
            try
            {
                IQueryable<UserNotAllowedLinkTypeViewModel> query = null;
                query = this.GetUserNotAllowedLinkTypeBaseData()
                           .Select
                            (
                                r => new UserNotAllowedLinkTypeViewModel
                                {
                                    Id = r.Id,
                                    UserId = r.UserId,
                                    User = new UserBasicInfoModel()
                                    {
                                        Id = r.User.Id,
                                        FirstName = r.User.AspNetUsersProfile.FirstName,
                                        LastName = r.User.AspNetUsersProfile.LastName,
                                        MiddleName = r.User.AspNetUsersProfile.MiddleName,
                                        FullName = r.User.AspNetUsersProfile.FirstName + " " + r.User.AspNetUsersProfile.LastName,
                                        Status = r.User.Status
                                    },
                                    LinkTypeId = r.LinkTypeId,
                                    LinkTypeName = r.LinkType.Name,
                                    LinkTypeURL = r.LinkType.URL,
                                    LinkTypePublished = r.LinkType.Published,
                                    Category = (UserNotAllowedLinkTypeCategory)r.Category,
                                    CategoryDescription = r.Category == 1 ? "Request" : "WorkFlow",
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate
                                }
                            )
                            .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<UserNotAllowedLinkTypeViewModel> GetUserNotAllowedLinkTypes(PagingRequest paging = null, bool? published = null, bool? linkTypePublished = null)
        {
            try
            {
                IQueryable<UserNotAllowedLinkTypeViewModel> query = null;
                query = this.FindAllUserNotAllowedLinkType()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(u => u.Published == published);

                if (linkTypePublished != null)
                    query = query.Where(u => u.LinkTypePublished == linkTypePublished);

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.LinkTypeName.Contains(search) || p.LinkTypeURL.Contains(search) || p.CategoryDescription.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<UserNotAllowedLinkTypeViewModel> UserNotAllowedLinkTypePagingFeature(IQueryable<UserNotAllowedLinkTypeViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<UserNotAllowedLinkTypeViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<UserNotAllowedLinkTypeViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
