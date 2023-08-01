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
    public class FileTypeRepository : BaseApi, IFileType
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;

        public FileTypeRepository(
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

        public async Task<FileTypeViewModel> Add(FileTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            //if (String.IsNullOrWhiteSpace(model.MimeType))
            //    throw new CustomException("Mime Type is required.", 400);

            if (model.MimeTypes == null)
                throw new CustomException("Mime Types is required.", 400);

            if (model.MimeTypes.Count <= 0)
                throw new CustomException("Mime Types is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                if (await this.IfExists(model.Name))
                    throw new CustomException("File Type: " + model.Name + " is already exists.", 400);

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

                string mimeTypes = string.Join(",", model.MimeTypes.Select(x => x.ToString()).ToArray());

                //Insert File Types
                var fileType = new FileType()
                {
                    Name = model.Name,
                    MimeType = mimeTypes,
                    Published = model.Published,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.FileTypes.Add(fileType);
                await _dbCntxt.SaveChangesAsync();

                model.Id = fileType.Id;
                model.CreatedByPK = fileType.CreatedByPK;
                model.CreatedDate = fileType.CreatedDate;

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

                var fileType = await _dbCntxt.FileTypes.FindAsync(id);

                if (fileType == null)
                    throw new CustomException("File Type to delete is not found. ", 404);

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

                _dbCntxt.FileTypes.Remove(fileType);
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

        public async Task DeleteAll(FileTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<FileTypeViewModel> Find(object obj)
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<FileTypeViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(r => r.Id == id)
                            .Select
                            (
                                r => new FileTypeViewModel
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    MimeType = r.MimeType,
                                    MimeTypes = r.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
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

        public IQueryable<FileTypeViewModel> FindAll()
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<FileTypeViewModel> query = null;
                query = this.Get()
                           .Select
                           (
                               r => new FileTypeViewModel
                               {
                                   Id = r.Id,
                                   Name = r.Name,
                                   MimeType = r.MimeType,
                                   MimeTypes = r.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
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

        public async Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            bool isexists = false;
            int condition = 0;
            try
            {
                if (entityPrimaryKey == null)
                {
                    var fileTypeIndDb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch);

                    if (fileTypeIndDb != null)
                        isexists = true;
                }
                else
                {
                    condition = Convert.ToInt32(entityPrimaryKey);

                    var fileTypeIndDb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch && r.Id != condition);

                    if (fileTypeIndDb != null)
                        isexists = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return isexists;
        }

        public async Task Update(FileTypeViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Name is required.", 400);

            //if (String.IsNullOrWhiteSpace(model.MimeType))
            //    throw new CustomException("Mime Type is required.", 400);

            if (model.MimeTypes == null)
                throw new CustomException("Mime Types is required.", 400);

            if (model.MimeTypes.Count <= 0)
                throw new CustomException("Mime Types is required.", 400);

            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                if (await this.IfExists(model.Name, tableId))
                    throw new CustomException("File Type: " + model.Name + " is already exists.", 400);

                var fileType = await _dbCntxt.FileTypes.FirstOrDefaultAsync(a => a.Id == tableId);

                if (fileType != null)
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

                    string mimeTypes = string.Join(",", model.MimeTypes.Select(x => x.ToString()).ToArray());

                    fileType.Name = model.Name;
                    fileType.MimeType = mimeTypes;
                    fileType.Published = model.Published;
                    fileType.ModifiedByPK = cid;
                    fileType.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(fileType).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("File Type is not found", 404);
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

        private IQueryable<FileType> Get()
        {
            try
            {
                IQueryable<FileType> query = null;

                query = _dbCntxt.FileTypes
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public IQueryable<FileTypeViewModel> GetFileTypes(PagingRequest paging = null, bool? published = null)
        {
            try
            {
                IQueryable<FileTypeViewModel> query = null;
                query = this.FindAll()
                            .AsQueryable();

                if (published != null)
                    query = query.Where(o => o.Published == published);

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.Name.Contains(search) || p.MimeType.Contains(search));
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public PagingResponse<FileTypeViewModel> PagingFeature(IQueryable<FileTypeViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<FileTypeViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<FileTypeViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
