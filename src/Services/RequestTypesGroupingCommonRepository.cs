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
    public class RequestTypesGroupingCommonRepository : BaseApi, IRequestTypesGroupingCommon
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public RequestTypesGroupingCommonRepository(
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

        public IQueryable<RequestTypesGroupingViewModel> GetRequestTypesGroupings(PagingRequest paging = null, bool? published = null, bool? detailPublished = null)
        {
            try
            {
                IQueryable<RequestTypesGroupingViewModel> query = null;
                query = this.FindAllRequestTypesGroupings()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(o => o.Published == published);

                if (detailPublished != null)
                    query = query
                            .Select
                            (
                                r => new RequestTypesGroupingViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = r.Description,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = r.CreatedByName,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = r.ModifiedByName,
                                    ModifiedDate = r.ModifiedDate,
                                    RequestTypesGroupingDetails = r.RequestTypesGroupingDetails
                                                                  .Where(r => r.Published == detailPublished)
                                                                  .Select
                                                                  (
                                                                      r => new RequestTypesGroupingDetailViewModel()
                                                                      {
                                                                          Id = r.Id,
                                                                          RequestTypesGroupingId = r.RequestTypesGroupingId,
                                                                          RequestTypesGroupingName = r.RequestTypesGroupingName,
                                                                          RequestTypesGroupingDescription = r.RequestTypesGroupingDescription,
                                                                          RequestTypesGroupingPublished = r.RequestTypesGroupingPublished,
                                                                          RequestTypeId = r.RequestTypeId,
                                                                          RequestTypeName = r.RequestTypeName,
                                                                          Version = r.Version,
                                                                          Published = r.Published,
                                                                          CreatedDate = r.CreatedDate,
                                                                          ModifiedDate = r.ModifiedDate
                                                                      }
                                                                  ).ToList()
                                }
                            );

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<RequestTypesGroupingViewModel> RequestTypeGroupingsPagingFeature(IQueryable<RequestTypesGroupingViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<RequestTypesGroupingViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<RequestTypesGroupingViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<RequestTypesGroupingViewModel> FindRequestTypesGrouping(object obj)
        {
            try
            {
                IQueryable<RequestTypesGroupingViewModel> query = null;

                var id = (int)obj;

                query = this.GetRequestTypesGroupingsBaseData()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new RequestTypesGroupingViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = String.IsNullOrWhiteSpace(r.Description) ? "" : r.Description,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate,
                                    RequestTypesGroupingDetails = r.RequestTypesGroupingDetails
                                                                  .Select
                                                                  (
                                                                      r => new RequestTypesGroupingDetailViewModel()
                                                                      {
                                                                          Id = r.Id,
                                                                          RequestTypesGroupingId = r.RequestTypesGroupingId,
                                                                          RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                                                          RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                                                          RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                                                          RequestTypeId = r.RequestTypeId,
                                                                          RequestTypeName = r.RequestTypes.Name,
                                                                          Version = r.Version,
                                                                          Published = r.Published,
                                                                          CreatedDate = r.CreatedDate,
                                                                          ModifiedDate = r.ModifiedDate
                                                                      }
                                                                  ).ToList()

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

        public IQueryable<RequestTypesGroupingViewModel> FindAllRequestTypesGroupings()
        {
            try
            {
                IQueryable<RequestTypesGroupingViewModel> query = null;
                query = this.GetRequestTypesGroupingsBaseData()
                            .Select
                            (
                                r => new RequestTypesGroupingViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    Description = String.IsNullOrWhiteSpace(r.Description) ? "" : r.Description,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == r.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == r.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate,
                                    RequestTypesGroupingDetails = r.RequestTypesGroupingDetails
                                                                  .Select
                                                                  (
                                                                      r => new RequestTypesGroupingDetailViewModel()
                                                                      {
                                                                          Id = r.Id,
                                                                          RequestTypesGroupingId = r.RequestTypesGroupingId,
                                                                          RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                                                          RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                                                          RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                                                          RequestTypeId = r.RequestTypeId,
                                                                          RequestTypeName = r.RequestTypes.Name,
                                                                          Version = r.Version,
                                                                          Published = r.Published,
                                                                          CreatedDate = r.CreatedDate,
                                                                          ModifiedDate = r.ModifiedDate
                                                                      }
                                                                  ).ToList()

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

        public IQueryable<RequestTypesGroupingDetailViewModel> FindRequestTypesGroupingDetail(object obj)
        {
            try
            {
                IQueryable<RequestTypesGroupingDetailViewModel> query = null;

                var id = (int)obj;

                query = this.GetRequestTypesGroupingDetailsBaseData()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new RequestTypesGroupingDetailViewModel
                                {
                                    Id = r.Id,
                                    RequestTypesGroupingId = r.RequestTypesGroupingId,
                                    RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                    RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                    RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                    RequestTypeId = r.RequestTypeId,
                                    RequestTypeName = r.RequestTypes.Name,
                                    Version = r.Version,
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

        public IQueryable<RequestTypesGroupingDetailViewModel> FindAllRequestTypesGroupingDetails()
        {
            try
            {
                IQueryable<RequestTypesGroupingDetailViewModel> query = null;
                query = this.GetRequestTypesGroupingDetailsBaseData()
                            .Select
                            (
                                r => new RequestTypesGroupingDetailViewModel
                                {
                                    Id = r.Id,
                                    RequestTypesGroupingId = r.RequestTypesGroupingId,
                                    RequestTypesGroupingName = r.RequestTypesGrouping.Name,
                                    RequestTypesGroupingDescription = r.RequestTypesGrouping.Description,
                                    RequestTypesGroupingPublished = r.RequestTypesGrouping.Published,
                                    RequestTypeId = r.RequestTypeId,
                                    RequestTypeName = r.RequestTypes.Name,
                                    Version = r.Version,
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


        private IQueryable<RequestTypesGrouping> GetRequestTypesGroupingsBaseData()
        {
            try
            {
                IQueryable<RequestTypesGrouping> query = null;

                query = _dbCntxt.RequestTypesGroupings
                                .Include(r => r.RequestTypesGroupingDetails)
                                .ThenInclude(r => r.RequestTypes)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<RequestTypesGroupingDetail> GetRequestTypesGroupingDetailsBaseData()
        {
            try
            {
                IQueryable<RequestTypesGroupingDetail> query = null;

                query = _dbCntxt.RequestTypesGroupingDetails
                                .Include(r => r.RequestTypes)
                                .Include(r => r.RequestTypesGrouping)
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<RequestTypesGroupingDetailViewModel> GetRequestTypesGroupingDetails(PagingRequest paging = null, bool? published = null, bool? detailPublished = null)
        {
            try
            {
                IQueryable<RequestTypesGroupingDetailViewModel> query = null;
                query = this.FindAllRequestTypesGroupingDetails()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(o => o.Published == published);

                if (detailPublished != null)
                    query = query
                            .Select
                            (
                                r => new RequestTypesGroupingDetailViewModel
                                {
                                    Id = r.Id,
                                    RequestTypesGroupingId = r.RequestTypesGroupingId,
                                    RequestTypesGroupingName = r.RequestTypesGroupingName,
                                    RequestTypesGroupingDescription = r.RequestTypesGroupingDescription,
                                    RequestTypesGroupingPublished = r.RequestTypesGroupingPublished,
                                    RequestTypeId = r.RequestTypeId,
                                    RequestTypeName = r.RequestTypeName,
                                    Version = r.Version,
                                    Published = r.Published,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedByName = r.CreatedByName,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedByName = r.ModifiedByName,
                                    ModifiedDate = r.ModifiedDate
                                }
                            );

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.RequestTypeName.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<RequestTypesGroupingDetailViewModel> RequestTypeGroupingDetailsPagingFeature(IQueryable<RequestTypesGroupingDetailViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<RequestTypesGroupingDetailViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<RequestTypesGroupingDetailViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
