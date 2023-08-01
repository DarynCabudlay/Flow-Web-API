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
using workflow.Data;
using System.IO;

namespace workflow.Services
{
    public class TicketAttachmentRepository : BaseApi, ITicketAttachment
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IRequestType _requestType;

        public TicketAttachmentRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IRequestType requestType) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _requestType = requestType;
        }

        public async Task<TicketAttachmentViewModel> Add(TicketAttachmentViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            if (model.ReferenceId <= 0)
                throw new CustomException("Reference Id is required", 400);

            if (!(GeneralHelper.IsValidTicketAttachmentCategory(Convert.ToInt32(model.Category))))
                throw new CustomException("Ticket Attachment Category is required.", 400);

            if (model.FileTypeId <= 0)
                throw new CustomException("File Type is required", 400);

            if (String.IsNullOrWhiteSpace(model.FilePath))
                throw new CustomException("File Name and Path is required", 400);

            string filePath = "";

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var attachmentCategory = (TicketAttachmentCategory)model.Category;

                string requestCategoryName = "";
                string requestTypeName = "";

                if (attachmentCategory == TicketAttachmentCategory.Ticket)
                {
                    var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == model.ReferenceId);

                    if (ticket == null)
                        throw new CustomException("Ticket as reference is not found", 404);

                    var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == ticket.RequestTypeId && r.Version == ticket.Version);

                    if (requestType != null)
                        requestTypeName = requestType.Name.ToUpper();

                    var requestCategory = await _dbCntxt.RequestCategories.FirstOrDefaultAsync(r => r.Id == requestType.RequestCategoryId);

                    if (requestCategory != null)
                        requestCategoryName = requestCategory.Name.ToUpper();

                }
                else if (attachmentCategory == TicketAttachmentCategory.TicketRequest)
                {
                    var ticketRequest = await _dbCntxt.TicketRequests.FirstOrDefaultAsync(t => t.Id == model.ReferenceId);

                    if (ticketRequest == null)
                        throw new CustomException("Ticket Detail as reference is not found", 404);

                    var ticket = await _dbCntxt.Tickets.FirstOrDefaultAsync(t => t.Id == ticketRequest.TicketId);

                    if (ticket != null)
                    {
                        var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == ticket.RequestTypeId && r.Version == ticket.Version);

                        if (requestType != null)
                            requestTypeName = requestType.Name.ToUpper();

                        var requestCategory = await _dbCntxt.RequestCategories.FirstOrDefaultAsync(r => r.Id == requestType.RequestCategoryId);

                        if (requestCategory != null)
                            requestCategoryName = requestCategory.Name.ToUpper();
                    }
                }
                else
                {
                    //for workflows
                    //to follow
                }

                var fileType = await _dbCntxt.FileTypes.FirstOrDefaultAsync(r => r.Id == model.FileTypeId);

                if (fileType == null)
                    throw new CustomException("File Type is not found.", 404);

                model.OriginalFileName = Path.GetFileName(model.FilePath);

                FileInfo fileInfo = new FileInfo(model.OriginalFileName);

                var originalFileExtension = fileInfo.Extension;

                var splittedFileTypes = fileType.MimeType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (!splittedFileTypes.Contains(originalFileExtension.Replace(".", "")))
                    throw new CustomException("Invalid file extension: '" + originalFileExtension + "' based from specified file type: '" + fileType.Name + "'", 404);

                if (await this.ValidateFileAttachmentsSize(model.FilePath, model.ReferenceId, model.Category))
                    throw new CustomException("You have reach the maximum limit of file attachment.", 404);

                var directoryForDraft = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_DRAFTATTACHMENTDIRECTORY");

                if (directoryForDraft == null)
                    throw new CustomException("No specified file path for draft attachment(s).", 404);

                if (String.IsNullOrWhiteSpace(directoryForDraft.VSettingOption))
                    throw new CustomException("No specified file path for draft attachment(s).", 404);

                var attachmentInDb = await _dbCntxt.TicketAttachments
                                         .FirstOrDefaultAsync
                                         (
                                            a =>
                                            a.OriginalFileName == model.OriginalFileName &&
                                            a.ReferenceId == model.ReferenceId
                                         );

                if (attachmentInDb != null)
                    throw new CustomException("Duplicate filename is found.", 404);

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

                filePath = directoryForDraft.VSettingOption;

                if (!filePath.EndsWith(@"\"))
                    filePath = filePath + @"\";

                filePath += DateTime.Now.Date.ToShortDateString().Replace("/", "-") + @"\" + requestCategoryName + @"\" + requestTypeName;

                if (model.Category == TicketAttachmentCategory.Ticket)
                    //Ticket
                    filePath = Path.Combine(filePath, model.ReferenceId.ToString());  
                else if (model.Category == TicketAttachmentCategory.TicketRequest)
                {
                    //Ticket Request
                    var ticketRequest = await _dbCntxt.TicketRequests.FirstOrDefaultAsync(t => t.Id == model.ReferenceId);

                    filePath = Path.Combine(filePath, Path.Combine(ticketRequest.TicketId + @"\Details\", model.ReferenceId.ToString()));
                }
                else
                {
                    //Workflows
                }

                var newFilePath = await this.SaveFileAsync(filePath, model.FilePath);

                //Insert Ticket
                var newTicketAttachment = new TicketAttachment()
                {
                    ReferenceId = model.ReferenceId,
                    Category = (int) model.Category,
                    FileTypeId = model.FileTypeId,
                    OriginalFileName = model.OriginalFileName,
                    FilePath = newFilePath,
                    Notes = model.Notes,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.TicketAttachments.Add(newTicketAttachment);
                await _dbCntxt.SaveChangesAsync();

                model.Id = newTicketAttachment.Id;
                model.CreatedByPK = newTicketAttachment.CreatedByPK;
                model.CreatedDate = newTicketAttachment.CreatedDate;

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

                //remove created file if there's an error upon saving
                this.DeleteFile(filePath);

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

                var ticketAttachment = await _dbCntxt.TicketAttachments.FindAsync(id);

                if (ticketAttachment == null)
                    throw new CustomException("Ticket Attachment to delete is not found. ", 404);

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

                _dbCntxt.TicketAttachments.Remove(ticketAttachment);
                await _dbCntxt.SaveChangesAsync();

                this.DeleteFile(ticketAttachment.FilePath);

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

        public async Task DeleteAll(TicketAttachmentViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketAttachmentViewModel> Find(object obj)
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<TicketAttachmentViewModel> query = null;

                var id = (int)obj;

                query = this.Get()
                            .Where(t => t.Id == id)
                            .Select
                            (
                                t => new TicketAttachmentViewModel
                                {
                                    Id = t.Id,
                                    ReferenceId = t.ReferenceId,
                                    Category = (TicketAttachmentCategory) t.Category,
                                    CategoryDesc = t.Category == 1 ? "Ticket" : t.Category == 2 ? "Ticket Request" : "Workflow",
                                    FileTypeId = t.FileTypeId,
                                    Notes = t.Notes,
                                    OriginalFileName = t.OriginalFileName,
                                    MaskedFileName = t.MaskedFileName,
                                    FilePath = t.FilePath,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    FileType = new FileTypeViewModel()
                                    {
                                        Id = t.FileType.Id,
                                        Name = t.FileType.Name,
                                        MimeType = t.FileType.MimeType,
                                        MimeTypes = t.FileType.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
                                        Published = t.FileType.Published
                                    }
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

        public IQueryable<TicketAttachmentViewModel> FindAll()
        {
            try
            {
                char[] delimeterChars = { ',' };

                IQueryable<TicketAttachmentViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                t => new TicketAttachmentViewModel
                                {
                                    Id = t.Id,
                                    ReferenceId = t.ReferenceId,
                                    Category = (TicketAttachmentCategory)t.Category,
                                    CategoryDesc = t.Category == 1 ? "Ticket" : t.Category == 2 ? "Ticket Request" : "Workflow",
                                    FileTypeId = t.FileTypeId,
                                    Notes = t.Notes,
                                    OriginalFileName = t.OriginalFileName,
                                    MaskedFileName = t.MaskedFileName,
                                    FilePath = t.FilePath,
                                    CreatedByPK = t.CreatedByPK,
                                    CreatedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.CreatedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.CreatedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.CreatedByPK,
                                    CreatedDate = t.CreatedDate,
                                    ModifiedByPK = t.ModifiedByPK,
                                    ModifiedByName = _dbCntxt.AspNetUsersProfile.FirstOrDefault(u => u.Id == t.ModifiedByPK) != null ? _dbCntxt.AspNetUsersProfile.Where(u => u.Id == t.ModifiedByPK).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault().ToString() : t.ModifiedByPK,
                                    ModifiedDate = t.ModifiedDate,
                                    FileType = new FileTypeViewModel()
                                    {
                                        Id = t.FileType.Id,
                                        Name = t.FileType.Name,
                                        MimeType = t.FileType.MimeType,
                                        MimeTypes = t.FileType.MimeType.Split(delimeterChars, StringSplitOptions.None).ToList(),
                                        Published = t.FileType.Published
                                    }
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

        public Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            throw new NotImplementedException();
        }

        public async Task Update(TicketAttachmentViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            IDbContextTransaction transactionToUse = null;

            int tableId = 0;

            if (id == null)
                tableId = model.Id;
            else
                tableId = Convert.ToInt32(id);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            //using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (transaction == null)
                    transactionToUse = _dbCntxt.Database.BeginTransaction();
                else
                    transactionToUse = transaction;

                var ticketAttachment = await _dbCntxt.TicketAttachments.FirstOrDefaultAsync(a => a.Id == tableId);

                if (ticketAttachment != null)
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

                    ticketAttachment.Notes = model.Notes;
                    ticketAttachment.ModifiedDate = DateTime.Now;
                    ticketAttachment.ModifiedByPK = cid;

                    _dbCntxt.Entry(ticketAttachment).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();

                    if (saveIpPerSession)
                        //remove useripperssion
                        await _userIpAddressPerSession.Remove(spId);

                    if (transaction == null)
                        transactionToUse.Commit();
                }
                else
                    throw new CustomException("Ticket Attachment is not found", 404);
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

        private async Task<string> SaveFileAsync(string filePath, string fromFilePath)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                string newFilePath = Path.Combine(filePath, Path.GetFileName(fromFilePath));

                using (Stream inputStream = System.IO.File.Open(fromFilePath, FileMode.Open))

                using (FileStream outputFileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await inputStream.CopyToAsync(outputFileStream);
                }

                if (!(System.IO.File.Exists(newFilePath)))
                    throw new CustomException("File is not created", 500);

                return newFilePath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private string SaveFile(string filePath, string fromFilePath)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                string newFilePath = Path.Combine(filePath, Path.GetFileName(fromFilePath));

                using (Stream inputStream = System.IO.File.Open(fromFilePath, FileMode.Open))

                using (FileStream outputFileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    inputStream.CopyTo(outputFileStream);
                }


                if (!(System.IO.File.Exists(newFilePath)))
                    throw new CustomException("File is not created", 500);

                return newFilePath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private void DeleteFile(string fileFullPath)
        {
            try
            {
                //delete file and it's folder once deleted file is the one only left inside the folder, otherwise delete file only
                if (System.IO.File.Exists(fileFullPath))
                {
                    string fileName = Path.GetFileName(fileFullPath);
                    string filePath = Path.GetDirectoryName(fileFullPath);

                    var files = Directory.GetFiles(filePath);
                    var folders = Directory.GetDirectories(filePath);
                    
                    if (files.Count() == 1 && folders.Count() == 0)
                    {
                        //delete file
                        System.IO.File.Delete(fileFullPath);

                        //delete directory folder
                        Directory.Delete(filePath);
                    }
                    else
                        System.IO.File.Delete(fileFullPath);
                }
                    
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private long GetFileSize(string FilePath)
        {   
            try
            {
                long fileSize = 0;

                if (System.IO.File.Exists(FilePath))
                    fileSize = new FileInfo(FilePath).Length;

                return fileSize;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task<bool> ValidateFileAttachmentsSize(string fromFilePath, int referenceId, TicketAttachmentCategory category)
        {
            bool fileSizeExceeded = false;
            try
            {
                var setting = await _dbCntxt.Setting.FirstOrDefaultAsync(s => s.VSettingId == "_ATTACHMENTMAXIMUMLENGTH");

                int maximumFileSize = 0; //in MB
                if (setting != null)
                    maximumFileSize = Convert.ToInt32(setting.VSettingOption);

                int ticketAttachmentCategory = (int) TicketAttachmentCategory.Ticket;
                int ticketRequestAttachmentCategory = (int)TicketAttachmentCategory.TicketRequest;

                var ticketAttachments = new List<string>();

                if (category == TicketAttachmentCategory.Ticket)
                {
                    //Get all ticket request details
                    var ticketRequests = await _dbCntxt.TicketRequests
                                                   .Where(t => t.TicketId == referenceId)
                                                   .Select(t => t.Id)
                                                   .Distinct()
                                                   .ToListAsync();

                    //Get attachment for ticket and it's dtails
                    ticketAttachments = await _dbCntxt.TicketAttachments
                                                        .Where
                                                        (
                                                            t =>
                                                            (
                                                                t.ReferenceId == referenceId && 
                                                                t.Category == ticketAttachmentCategory
                                                            ) ||
                                                            (
                                                                ticketRequests.Contains(t.ReferenceId) &&
                                                                t.Category == ticketRequestAttachmentCategory
                                                            )
                                                        )
                                                        .Select(t => t.FilePath)
                                                        .ToListAsync();

                }
                else if (category == TicketAttachmentCategory.TicketRequest)
                {
                    //Get specific ticket request
                    var ticketRequest = await _dbCntxt.TicketRequests
                                                      .FirstOrDefaultAsync(t => t.Id == referenceId);

                    int ticketId = ticketRequest.TicketId;

                    var ticketRequests = await _dbCntxt.TicketRequests
                                                   .Where(t => t.TicketId == ticketId)
                                                   .Select(t => t.Id)
                                                   .Distinct()
                                                   .ToListAsync();

                    ticketAttachments = await _dbCntxt.TicketAttachments
                                                    .Where
                                                    (
                                                        t =>
                                                        (
                                                            ticketRequests.Contains(t.ReferenceId) &&
                                                            t.Category == ticketRequestAttachmentCategory
                                                        ) ||
                                                        (
                                                            t.ReferenceId == ticketId &&
                                                            t.Category == ticketAttachmentCategory
                                                        )
                                                    )
                                                    .Select(t => t.FilePath)
                                                    .ToListAsync();

                }
                else
                {
                    //for workflows
                }

                //Sum all the sizes
                long allSizes = 0;
                foreach(var attachment in ticketAttachments)
                {
                    allSizes += this.GetFileSize(attachment);
                }

                long attachmentSizeForSaving = this.GetFileSize(fromFilePath);

                //add all ticket sizes plus attachment to be saved to get the total size to compare with maximum file size limit
                allSizes = allSizes + attachmentSizeForSaving;

                long sizeInMB = allSizes / (1024 * 1024);

                if (sizeInMB > (long)maximumFileSize)
                    fileSizeExceeded = true;

                return fileSizeExceeded;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private IQueryable<TicketAttachment> Get()
        {
            try
            {
                IQueryable<TicketAttachment> query = null;

                query = _dbCntxt.TicketAttachments
                                .Include(t => t.FileType)
                                .AsQueryable();
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
