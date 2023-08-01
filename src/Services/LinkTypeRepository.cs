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
    public class LinkTypeRepository : BaseApi, ILinkType
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public LinkTypeRepository(
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

        public async Task<LinkTypeViewModel> Add(LinkTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            if (String.IsNullOrWhiteSpace(model.URL))
                throw new CustomException("URL is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                if (await this.IfExists(model.Name))
                    throw new CustomException("Link Type: " + model.Name + " is already exists.", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();

                if (saveIpPerSession)
                {
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                    await _userIpAddressPerSession.Save(userIp);
                }

                //Insert Link Types
                var linkType = new LinkType()
                {
                    Name = model.Name,
                    URL = model.URL,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.LinkTypes.Add(linkType);
                await _dbCntxt.SaveChangesAsync();

                model.Id = linkType.Id;
                model.CreatedByPK = linkType.CreatedByPK;
                model.CreatedDate = linkType.CreatedDate;

                if (saveIpPerSession)
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                if (transaction == null)
                    transactionToUse.Commit();

                return model;
            }
            catch (CustomException customex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (transaction == null)
                    transactionToUse.Dispose();
            }
        }

        public async Task Delete(object obj, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                int id = (int)obj;

                var linkType = await _dbCntxt.LinkTypes.FindAsync(id);

                if (linkType == null)
                    throw new CustomException("Link Type to delete is not found. ", 404);

                //Check if maintained in user not allowed link types
                var checkIfExisting = await _dbCntxt.UserNotAllowedLinkTypes.FirstOrDefaultAsync(u => u.LinkTypeId == id);

                if (checkIfExisting != null)
                    throw new CustomException("This link type is currently being used in user maintenance.", 400);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();

                if (saveIpPerSession)
                {
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);
                }

                _dbCntxt.LinkTypes.Remove(linkType);
                await _dbCntxt.SaveChangesAsync();

                if (saveIpPerSession)
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                if (transaction == null)
                    transactionToUse.Commit();
            }
            catch (CustomException customex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new CustomException(customex.Message, customex.StatusCode, customex.Details, customex.ListMessage);
            }
            catch (Exception ex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (transaction == null)
                    transactionToUse.Dispose();
            }
        }

        public async Task DeleteAll(LinkTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<LinkTypeViewModel> Find(object obj)
        {
            try
            {

                IQueryable<LinkTypeViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(l => l.Id == id)
                            .Select
                            (
                                l => new LinkTypeViewModel
                                {
                                    Id = l.Id,
                                    Name = l.Name,
                                    URL = l.URL,
                                    Published = l.Published,
                                    CreatedByPK = l.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == l.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == l.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : l.CreatedByPK,
                                    CreatedDate = l.CreatedDate,
                                    ModifiedByPK = l.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == l.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == l.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : l.ModifiedByPK,
                                    ModifiedDate = l.ModifiedDate
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

        public IQueryable<LinkTypeViewModel> FindAll()
        {
            try
            {
                IQueryable<LinkTypeViewModel> query = null;
                query = this.Get()
                           .Select
                           (
                                l => new LinkTypeViewModel
                                {
                                    Id = l.Id,
                                    Name = l.Name,
                                    URL = l.URL,
                                    Published = l.Published,
                                    CreatedByPK = l.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == l.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == l.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : l.CreatedByPK,
                                    CreatedDate = l.CreatedDate,
                                    ModifiedByPK = l.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == l.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == l.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : l.ModifiedByPK,
                                    ModifiedDate = l.ModifiedDate
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

        public async Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            bool isexists = false;
            int condition = 0;
            try
            {
                if (entityPrimaryKey == null)
                {
                    var linkTypeIndDb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch);

                    if (linkTypeIndDb != null)
                        isexists = true;
                }
                else
                {
                    condition = Convert.ToInt32(entityPrimaryKey);

                    var linkTypeIndDb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch && r.Id != condition);

                    if (linkTypeIndDb != null)
                        isexists = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return isexists;
        }

        public async Task Update(LinkTypeViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            if (String.IsNullOrWhiteSpace(model.URL))
                throw new CustomException("URL is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                if (await this.IfExists(model.Name, tableId))
                    throw new CustomException("File Type: " + model.Name + " is already exists.", 400);

                var linkType = await _dbCntxt.LinkTypes.FirstOrDefaultAsync(a => a.Id == tableId);

                if (linkType != null)
                {
                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();

                    if (saveIpPerSession)
                    {
                        // get IP address
                        string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        //add useripperssion
                        var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                        await _userIpAddressPerSession.Save(userIp);
                    }

                    linkType.Name = model.Name;
                    linkType.URL = model.URL;
                    linkType.Published = model.Published;
                    linkType.ModifiedByPK = cid;
                    linkType.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(linkType).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Link Type is not found", 404);
            }
            catch (CustomException customex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                if (transaction == null)
                    transactionToUse.Rollback();

                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (transaction == null)
                    transactionToUse.Dispose();
            }
        }

        private IQueryable<LinkType> Get()
        {
            try
            {
                IQueryable<LinkType> query = null;

                query = _dbCntxt.LinkTypes
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<LinkTypeViewModel> GetLinkTypes(PagingRequest paging = null, bool? published = null)
        {
            try
            {
                IQueryable<LinkTypeViewModel> query = null;
                query = this.FindAll()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(o => o.Published == published);

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.Name.Contains(search) || p.URL.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<LinkTypeViewModel> PagingFeature(IQueryable<LinkTypeViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<LinkTypeViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<LinkTypeViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
