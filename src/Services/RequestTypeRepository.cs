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
    public class RequestTypeRepository : BaseApi, IRequestType
    {
        private readonly IUserIPAddressPerSession _userIpAddressPerSession;
        private readonly IUser _user;

        public RequestTypeRepository(
                FliDbContext dbCntxt,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RequestTypeRepository> logger,
                IConfiguration config,
                IWebHostEnvironment env,
                IHttpContextAccessor httpContextAccessor, IUserIPAddressPerSession userIpAddressPerSession, IUser user) : base(dbCntxt, userManager, roleManager, signInManager, logger, config, env, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIpAddressPerSession = userIpAddressPerSession;
            _user = user;
        }

        public async Task<RequestTypeViewModel> Add(RequestTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.RequestCategoryId <= 0)
                throw new CustomException("Request Category is required.", 400);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Request Type is required.", 400);
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (await this.IfExists(model.Name))
                    throw new CustomException("Request Name provided for the category is already existing.", 400);
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);

                await _userIpAddressPerSession.Save(userIp);

                int maxRequestType = 0;

                if (_dbCntxt.RequestTypes.Count() > 0)
                    maxRequestType = _dbCntxt.RequestTypes.Max(c => c.Id);

                int tableId = maxRequestType + 1;
                //Manipulate to insert unique id in table request type
                var deletedRequestTypeTrails = await _dbCntxt.AuditTrails
                                        .Where(c => c.TableName == "RequestTypes" && c.Action == "Delete").ToListAsync();

                AuditTrail exists = new AuditTrail();
                do
                {
                    exists = deletedRequestTypeTrails
                            .FirstOrDefault
                            (
                                c =>
                                c.Key == tableId.ToString()
                            );

                    if (exists != null)
                        tableId += 1;

                } while (exists != null);

                //Insert request type
                var requestType = new RequestType
                {
                    Id = tableId,
                    Name = model.Name,
                    RequestCategoryId = model.RequestCategoryId,
                    Description = model.Description,
                    Status = (int)RequestTypeStatus.Draft,
                    Version = 1,
                    Version2 = 0,
                    CreatedDate = DateTime.Now,
                    CreatedByPK = cid
                };

                _dbCntxt.RequestTypes.Add(requestType);
                await _dbCntxt.SaveChangesAsync();

                model.Id = requestType.Id;
                model.Status = (RequestTypeStatus)requestType.Status;
                model.Version = requestType.Version;
                model.Version2 = requestType.Version2;
                model.CreatedByPK = requestType.CreatedByPK;
                model.CreatedDate = requestType.CreatedDate;
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();

                return model;
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task SaveRequestForm(RequestFormViewModel model)
        {
            if (model == null)
                throw new CustomException("Kindly provide atleast one field to proceed.", 400);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestType == null)
                    throw new CustomException("Request Type ID and Version is not found.", 404);

                //Check properties to be changed if mapped in process flow
                if (model.OldFieldId > 0)
                {
                    //this should check whether mapped as active/deactivated to ensure that field structure is intact incase there's a deactivated mapping which will be tagged as active
                    bool isfieldIsMappedToProcess = await this.CheckFieldIsInvolvedInProcessAsync(model.RequestTypeId, model.Version, model.OldFieldId);

                    if (isfieldIsMappedToProcess)
                    {
                        var fieldInfo = await this.GetField(model.RequestTypeId, model.Version, model.OldFieldId);

                        //validate field type
                        if (model.FieldType != fieldInfo.FieldType)
                            throw new CustomException("Field Type must not be changed.", 404);

                        if (model.FieldDataType != fieldInfo.FieldDataType)
                            throw new CustomException("Field Data Type must not be changed.", 404);

                        if (model.FieldDecimalDigit != fieldInfo.FieldDecimalDigit)
                            throw new CustomException("Decimal Digit must not be changed.", 404);

                        if (model.FieldCurrencySymbol != fieldInfo.FieldCurrencySymbol)
                            throw new CustomException("Currency Symbol must not be changed.", 404);

                        if ((model.FieldMaxLength != fieldInfo.FieldMaxLength) || (model.FieldMaxNumber != fieldInfo.FieldMaxNumber))
                            throw new CustomException("Maximum Length must not be changed.", 404);

                        if ((model.FieldMinLength != fieldInfo.FieldMinLength) || (model.FieldMinNumber != fieldInfo.FieldMinNumber))
                            throw new CustomException("Minimum Length must not be changed.", 404);

                        if (model.Published != fieldInfo.Published)
                            throw new CustomException("Enable state must not be changed.", 404);

                        if (model.Sequence != fieldInfo.Sequence)
                            throw new CustomException("Sequence must not be changed.", 404);

                        if (model.Editable != fieldInfo.Editable)
                            throw new CustomException("Editable state not be changed.", 404);

                        if (model.DefaultValue != fieldInfo.DefaultValue)
                            throw new CustomException("Default Value not be changed.", 404);

                        if (model.DefaultValue2 != fieldInfo.DefaultValue2)
                            throw new CustomException("Default Value2 not be changed.", 404);
                    }
                }

                //For Request Form
                #region Field Validations

                //Field Name
                if (String.IsNullOrWhiteSpace(model.FieldName))
                    throw new CustomException("Field Name is required.", 400);

                if (model.FieldName.Contains("_"))
                    throw new CustomException("Underscore is not acceptable for Field Name.", 400);

                var existingFields = await this.GetFields(model.RequestTypeId, model.Version);

                RequestFormViewModel checkfield = new RequestFormViewModel();

                if (model.OldFieldId <= 0)
                    checkfield = existingFields.Where(f => f.FieldName == model.FieldName).FirstOrDefault();
                else
                    checkfield = existingFields.Where(f => f.FieldName == model.FieldName && f.OldFieldId != model.OldFieldId).FirstOrDefault();

                if (checkfield != null)
                    throw new CustomException("There are dubplicate Field Name within this workflow.", 400);

                //Field Type
                if (!(GeneralHelper.IsValidFieldType(Convert.ToInt32(model.FieldType))))
                    throw new CustomException("Field type is required.", 400);

                //Check if numeric/alphabet/special characters
                bool isDataTypeHasValues = (model.FieldDataType.Length > 0 ? true : false);

                //Data types
                if (model.FieldType == FieldType.TextBox || model.FieldType == FieldType.Dropdown || model.FieldType == FieldType.CheckboxList || model.FieldType == FieldType.RadioButton || model.FieldType == FieldType.TextArea)
                {
                    if (!isDataTypeHasValues)
                        throw new CustomException("Data type is required.", 400);
                }
                else
                {
                    if (isDataTypeHasValues)
                        throw new CustomException("Data type is not required", 400);
                }

                bool isNumeric = false;
                bool isAlphabet = false;
                bool isCurrency = false;
                bool isAlphaNumeric = false;

                if (isDataTypeHasValues)
                {
                    string dataType = model.FieldDataType;

                    if (dataType == "Numeric")
                        isNumeric = true;
                    else if (dataType == "Alphabet")
                        isAlphabet = true;
                    else if (dataType == "Currency")
                        isCurrency = true;
                    else
                        isAlphaNumeric = true;
                }

                //For Text area
                if (model.FieldType == FieldType.TextArea)
                {
                    if (isNumeric || isCurrency)
                        throw new CustomException("Invalid Data type for Text Area.", 400);
                }

                //Decimal Digit
                bool decimalDigitNotRequired = false;
                if (isDataTypeHasValues)
                {
                    if (isNumeric || isCurrency)
                    {
                        if (model.FieldDecimalDigit == null)
                            throw new CustomException("Decimal Digit is required.", 400);
                    }
                    else
                    {
                        if (model.FieldDecimalDigit != null)
                            decimalDigitNotRequired = true;
                    }
                }
                else
                {
                    if (model.FieldDecimalDigit != null)
                        decimalDigitNotRequired = true;
                }

                if (decimalDigitNotRequired)
                    throw new CustomException("Decimal Digit is not required.", 400);

                //If not date range
                if (model.FieldType != FieldType.DateRange)
                {
                    //Value2 is not required
                    if (!(String.IsNullOrWhiteSpace(model.DefaultValue2)))
                        throw new CustomException("Default Value2 is not required", 400);
                }

                if (model.FieldType == FieldType.DateRange)
                {
                    //If field type is date range, if has specified value then require the two values
                    if ((!String.IsNullOrWhiteSpace(model.DefaultValue) && String.IsNullOrWhiteSpace(model.DefaultValue2)) || (String.IsNullOrWhiteSpace(model.DefaultValue) && !String.IsNullOrWhiteSpace(model.DefaultValue2)))
                        throw new CustomException("Date Range Value is required", 400);
                }

                //Default Value
                if (!(String.IsNullOrWhiteSpace(model.DefaultValue)))
                {
                    if (model.FieldType == FieldType.Date || model.FieldType == FieldType.DateRange)
                    {
                        if (!(GeneralHelper.IsValidDate(model.DefaultValue)))
                            throw new CustomException("Default Value is not a valid date.", 400);
                    }
                    else
                    {
                        if (model.FieldType == FieldType.Email)
                        {
                            //Check if email is valid
                            if (!(GeneralHelper.IsValidEmail(model.DefaultValue)))
                                throw new CustomException("Default Value is not a valid email format.", 400);
                        }

                        if (model.FieldType == FieldType.Url)
                        {
                            //Check if url is valid
                        }

                        if (isDataTypeHasValues)
                        {
                            int decimalDigit = Convert.ToInt32(model.FieldDecimalDigit == null ? 0 : model.FieldDecimalDigit);
                            //If value is a valid numeric values
                            if (isNumeric)
                            {
                                if (!(GeneralHelper.IsNumeric(model.DefaultValue, decimalDigit, false)))
                                    throw new CustomException("Default Value is not a numeric format.", 400);

                                if (decimalDigit > 0)
                                {
                                    bool withPercentage = false;
                                    if (model.DefaultValue.Contains("%"))
                                    {
                                        withPercentage = true;
                                        //remove percentage first (if existing) before rounding off
                                        model.DefaultValue = model.DefaultValue.Replace("%", "");
                                    }

                                    model.DefaultValue = GeneralHelper.ReconstructValue(model.DefaultValue, decimalDigit);

                                    int decimalPointIndex = model.DefaultValue.IndexOf(".");

                                    decimalPointIndex++;

                                    string decimalPlaces = model.DefaultValue.Substring(decimalPointIndex, (model.DefaultValue.Length - decimalPointIndex));

                                    int countDecimalPlacesInDefaultValue = decimalPlaces.Length;

                                    if (withPercentage)
                                    {
                                        if (countDecimalPlacesInDefaultValue > decimalDigit)
                                            //round to the nearest decimal digit
                                            model.DefaultValue = Math.Round(Convert.ToDouble(model.DefaultValue), decimalDigit).ToString();

                                        //append percentage
                                        model.DefaultValue += "%";
                                    }
                                    else
                                    {
                                        if (countDecimalPlacesInDefaultValue > decimalDigit)
                                            model.DefaultValue = Math.Round(Convert.ToDouble(model.DefaultValue), decimalDigit).ToString();
                                    }
                                }
                            }

                            //If value is a valid currency values
                            if (isCurrency)
                            {
                                if (!(GeneralHelper.IsNumeric(model.DefaultValue, decimalDigit, true)))
                                    throw new CustomException("Default Value is not a currency format.", 400);

                                if (decimalDigit > 0)
                                {
                                    model.DefaultValue = GeneralHelper.ReconstructValue(model.DefaultValue, decimalDigit);
                                    int decimalPointIndex = model.DefaultValue.IndexOf(".");
                                    decimalPointIndex++;
                                    string decimalPlaces = model.DefaultValue.Substring(decimalPointIndex, (model.DefaultValue.Length - decimalPointIndex));
                                    int countDecimalPlacesInDefaultValue = decimalPlaces.Length;
                                    if (countDecimalPlacesInDefaultValue > decimalDigit)
                                        model.DefaultValue = Math.Round(Convert.ToDouble(model.DefaultValue), decimalDigit).ToString();
                                }
                            }

                            //If value is alphabet 
                            if (isAlphabet)
                            {
                                if (!(GeneralHelper.IsAlpha(model.DefaultValue)))
                                    throw new CustomException("Default Value is not an alphabet.", 400);
                            }
                            //If value is alphanumeric
                            if (isAlphaNumeric)
                            {

                            }
                        }
                    }
                }

                //Currency symbol
                if (isCurrency)
                {
                    if (String.IsNullOrWhiteSpace(model.FieldCurrencySymbol))
                        throw new CustomException("Currency symbol is required.", 400);
                }
                else
                {
                    if (!(String.IsNullOrWhiteSpace(model.FieldCurrencySymbol)))
                        throw new CustomException("Currency symbol is not required.", 400);
                }

                //Editable
                if (String.IsNullOrWhiteSpace(model.DefaultValue))
                {
                    if (!(String.IsNullOrWhiteSpace(model.Editable)))
                        throw new CustomException("Editable is not required.", 400);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(model.Editable))
                        throw new CustomException("Editable is required.", 400);
                }

                if (!(model.FieldType == FieldType.Email || model.FieldType == FieldType.Url || model.FieldType == FieldType.Date || model.FieldType == FieldType.DateRange || model.FieldType == FieldType.Dropdown || model.FieldType == FieldType.CheckboxList || model.FieldType == FieldType.RadioButton))
                {
                    //Field Lengths are required
                    if (isNumeric || isCurrency)
                    {
                        if (model.FieldMaxLength != null || model.FieldMinLength != null)
                            throw new CustomException("Minlength or MaxLength is not required", 400);

                        if (model.FieldMaxNumber == null || model.FieldMinNumber == null)
                            throw new CustomException("length(s) are required.", 400);

                        if (model.FieldMaxNumber < model.FieldMinNumber)
                            throw new CustomException("Maximum length should be larger than minimum length.", 400);
                    }
                    else
                    {
                        if (model.FieldMinNumber != null || model.FieldMaxNumber != null)
                            throw new CustomException("MinNumber or MaxNumber is not required", 400);

                        if (model.FieldMaxLength == null || model.FieldMinLength == null)
                            throw new CustomException("length(s) are required.", 400);

                        if (model.FieldMaxLength < model.FieldMinLength)
                            throw new CustomException("Maximum length should be larger than minimum length.", 400);
                    }

                    if (!(String.IsNullOrWhiteSpace(model.DefaultValue)))
                    {
                        //Valid Lengths entries 
                        if (isNumeric || isCurrency)
                        {
                            if (model.FieldDecimalDigit > 0)
                            {
                                if (model.FieldMinNumber > Convert.ToDouble(model.DefaultValue))
                                    throw new CustomException("" + model.FieldMinNumber + " is minimum", 400);

                                if (model.FieldMaxNumber < Convert.ToDouble(model.DefaultValue))
                                    throw new CustomException("" + model.FieldMaxNumber + " is maximum", 400);
                            }
                            else
                            {
                                if (model.FieldMinNumber > Convert.ToInt32(model.DefaultValue))
                                    throw new CustomException("" + model.FieldMinNumber + " is minimum", 400);

                                if (model.FieldMaxNumber < Convert.ToInt32(model.DefaultValue))
                                    throw new CustomException("" + model.FieldMaxNumber + " is maximum", 400);
                            }
                        }
                        else
                        {
                            if (model.FieldMinLength > model.DefaultValue.Length)
                                throw new CustomException("Alteast " + model.FieldMinLength + "character is required", 400);

                            if (model.FieldMaxLength < model.DefaultValue.Length)
                                throw new CustomException("Specified value exceeds the maximum.", 400);
                        }
                    }
                }
                else
                {
                    if (model.FieldMinLength != null || model.FieldMaxLength != null || model.FieldMinNumber != null || model.FieldMaxNumber != null)
                        throw new CustomException("Lengths are not required on this field type.", 400);
                }

                //Default Value 2 only for date range
                if (!(String.IsNullOrWhiteSpace(model.DefaultValue2)))
                {
                    //assuming the defaul value has value
                    //check if valid date time
                    if (!(GeneralHelper.IsValidDate(model.DefaultValue2)))
                        throw new CustomException("Default Value2 is not a valid date.", 400);

                    var date1 = Convert.ToDateTime(model.DefaultValue);
                    var date2 = Convert.ToDateTime(model.DefaultValue2);

                    if (date2.Date < date1.Date)
                        throw new CustomException("Invalid date range.", 400);
                }

                if (model.Sequence <= 0)
                    throw new CustomException("Sequence is required.", 400);

                //LOVs
                if (model.FieldType == FieldType.Dropdown || model.FieldType == FieldType.CheckboxList || model.FieldType == FieldType.RadioButton)
                {
                    if (model.FieldLOVs.Length <= 0)
                        throw new CustomException("List of values is required.", 400);

                    var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(model.FieldLOVs);

                    if (lovs.Count > 0)
                    {
                        //Atleast 1 active LOV to continue
                        var hasActiveLov = lovs.FirstOrDefault(v => v.IsActive == true) != null ? true : false;

                        if (!hasActiveLov)
                            throw new CustomException("Atleast one active LOV to continue.", 400);

                        //Check if with LOVs in DB that map to process but then no longer existing/technically was deleted in GUI or Tagged as deactivated in the current list

                        //Check first if field is mapped to process, otherwise no need to check LOV
                        bool isfieldActivelyMappedToProcess = await this.CheckFieldIsInvolvedInProcessAsync(model.RequestTypeId, model.Version, model.OldFieldId, true);
                        if (isfieldActivelyMappedToProcess)
                        {
                            var fieldInfo = await this.GetField(model.RequestTypeId, model.Version, model.OldFieldId);

                            if (fieldInfo != null)
                            {
                                List<string> affectedLOVs = new List<string>();

                                //Tagged as Deactived LOV in UI
                                foreach (var lov in lovs)
                                {
                                    if (!lov.IsActive)
                                    {
                                        var isMappedToProcess = await this.CheckLOVIsInvolvedInProcessAsync(model.RequestTypeId, model.Version, model.OldFieldId, lov.OldName, true);

                                        if (isMappedToProcess)
                                            affectedLOVs.Add(lov.OldName);
                                    }
                                }

                                if (affectedLOVs.Count > 0)
                                    throw new CustomException("There is/are LOVs listed below that is/are currently mapped to processed but tagged as deactivated in UI.", 400, null, affectedLOVs);

                                var lovsInDB = JsonSerializer.Deserialize<List<LOVViewModel>>(fieldInfo.FieldLOVs);

                                //Deleted LOV in UI
                                foreach (var lovInDB in lovsInDB)
                                {
                                    var isMappedToProcess = await this.CheckLOVIsInvolvedInProcessAsync(model.RequestTypeId, model.Version, model.OldFieldId, lovInDB.Name, true);

                                    if (isMappedToProcess)
                                    {
                                        //Check in list of LOV from UI if existing, otherwise return error message
                                        var isInUILOV = lovs.Where(f => f.OldName == lovInDB.Name).ToList().Count > 0;

                                        if (!isInUILOV)
                                            affectedLOVs.Add(lovInDB.Name);
                                    }
                                }

                                if (affectedLOVs.Count > 0)
                                    throw new CustomException("There is/are deleted LOVs listed below that is/are currently mapped to processed.", 400, null, affectedLOVs);
                            }
                        }

                        //Check if has duplicate value
                        var distinctValues = lovs.Select(l => l.Value).Distinct().ToList();

                        if (distinctValues.Count != lovs.Count)
                            throw new CustomException("There are duplicate LOV Value(s) in the list.", 400);

                        //Check if has duplicate name
                        var distinctNames = lovs.Select(l => l.Name).Distinct().ToList();

                        if (distinctNames.Count != lovs.Count)
                            throw new CustomException("There are duplicate LOV Name(s) in the list.", 400);

                        //bool withdefaultvalue = false;
                        foreach (var lov in lovs)
                        {
                            if (String.IsNullOrWhiteSpace(lov.Value) || String.IsNullOrWhiteSpace(lov.Name))
                                throw new CustomException("LOV Value/Name is required.", 400);

                            if (isDataTypeHasValues)
                            {
                                int decimalDigit = Convert.ToInt32(model.FieldDecimalDigit == null ? 0 : model.FieldDecimalDigit);
                                //If value is a valid numeric values
                                if (isNumeric)
                                {
                                    if (!(GeneralHelper.IsNumeric(lov.Name, decimalDigit, false)))
                                        throw new CustomException("LOV Name is not a numeric format.", 400);

                                    if (decimalDigit > 0)
                                    {
                                        bool withPercentage = false;
                                        if (lov.Name.Contains("%"))
                                        {
                                            withPercentage = true;
                                            //remove percentage first (if existing) before rounding off
                                            lov.Name = lov.Name.Replace("%", "");

                                        }

                                        lov.Name = GeneralHelper.ReconstructValue(lov.Name, decimalDigit);
                                        int decimalPointIndex = lov.Name.IndexOf(".");
                                        decimalPointIndex++;
                                        string decimalPlaces = lov.Name.Substring(decimalPointIndex, (lov.Name.Length - decimalPointIndex));
                                        int countdecimalPlacesinlov = decimalPlaces.Length;

                                        if (withPercentage)
                                        {
                                            if (countdecimalPlacesinlov > decimalDigit)
                                                //round to the nearest decimal digit
                                                lov.Name = Math.Round(Convert.ToDouble(lov.Name), decimalDigit).ToString();

                                            //append percentage
                                            lov.Name += "%";
                                        }
                                        else
                                        {
                                            if (countdecimalPlacesinlov > decimalDigit)
                                                lov.Name = Math.Round(Convert.ToDouble(lov.Name), decimalDigit).ToString();
                                        }
                                    }
                                }

                                //If value is a valid currency values
                                if (isCurrency)
                                {
                                    if (!(GeneralHelper.IsNumeric(lov.Name, decimalDigit, true)))
                                        throw new CustomException("LOV Value is not a currency format.", 400);

                                    if (decimalDigit > 0)
                                    {
                                        lov.Name = GeneralHelper.ReconstructValue(lov.Name, decimalDigit);
                                        int decimalPointIndex = lov.Name.IndexOf(".");
                                        decimalPointIndex++;
                                        string decimalPlaces = lov.Name.Substring(decimalPointIndex, (lov.Name.Length - decimalPointIndex));
                                        int countdecimalPlacesinlov = decimalPlaces.Length;

                                        if (countdecimalPlacesinlov > decimalDigit)
                                            lov.Name = Math.Round(Convert.ToDouble(lov.Name), decimalDigit).ToString();
                                    }
                                }

                                //If value is alphabet 
                                if (isAlphabet)
                                {
                                    if (!(GeneralHelper.IsAlpha(lov.Name)))
                                        throw new CustomException("LOV Value is not an alphabet", 400);
                                }
                                //If value is alphanumeric
                                if (isAlphaNumeric)
                                {

                                }
                            }
                        }
                    }
                    else
                        throw new CustomException("List of values is required.", 400);
                }
                else
                {
                    if (model.FieldLOVs.Length > 0)
                        throw new CustomException("List of values is not required.", 400);
                }
                #endregion

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                if (model.OldFieldId <= 0)
                {
                    if (model.FieldLOVs.Length > 0)
                    {
                        //Parse LOV to be saved
                        var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(model.FieldLOVs);

                        foreach (var lov in lovs)
                        {
                            lov.OldName = "";
                            lov.OldValue = "";
                        }

                        model.FieldLOVs = JsonSerializer.Serialize(lovs);
                    }

                    //rearrange sequence
                    //check if has fields alreadfy
                    var requestFormInDb = await _dbCntxt.RequestForms
                                                    .Where
                                                    (
                                                        r => r.RequestTypeId == model.RequestTypeId &&
                                                        r.Version == model.Version
                                                    ).ToListAsync();

                    if (requestFormInDb.Count <= 0)
                    {
                        if (model.Sequence != 1)
                            throw new CustomException("Sequence is out of range.", 400);
                    }
                    else
                    {
                        if (model.Sequence != (requestFormInDb.Max(f => f.Sequence) + 1))
                        {
                            if (!(await this.CheckValidSequence(model.RequestTypeId, model.Version, model.Sequence)))
                                throw new CustomException("Sequence is out of range.", 400);
                        }

                        await ReArrangeSequence(model.RequestTypeId, model.Version, model.OldFieldId, model.Sequence, (model.Sequence + 1));
                    }

                    RequestForm form = new RequestForm();
                    form.RequestTypeId = model.RequestTypeId;
                    form.Version = model.Version;

                    if (model.FieldId == 0)
                    {
                        if (model.FieldLOVs.Length > 0)
                        {
                            //Parse LOV to be saved
                            var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(model.FieldLOVs);

                            foreach (var lov in lovs)
                            {
                                lov.OldName = "";
                                lov.OldValue = "";
                            }

                            model.FieldLOVs = JsonSerializer.Serialize(lovs);
                        }

                        GeneralField field = new GeneralField();
                        string controlShortText = GeneralHelper.GetFieldTypeShorText(model.FieldType);
                        string concatDateTime = DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "") + DateTime.Now.ToString("hh:mm:ss:fff").Replace(":", "");
                        string zeros = "00000";
                        int requestTypeIdLength = model.RequestTypeId.ToString().Length;
                        string requestTypeSlug = "";

                        if (zeros.Length == requestTypeIdLength)
                            requestTypeSlug = model.RequestTypeId.ToString();
                        else
                            requestTypeSlug = zeros.Substring(0, (zeros.Length - requestTypeIdLength)) + model.RequestTypeId.ToString();

                        string slug = controlShortText + "_" + requestTypeSlug + "_" + concatDateTime;
                        field.Slug = slug;
                        field.Name = model.FieldName;
                        field.FieldType = (int)model.FieldType;
                        field.Description = model.FieldDescription;
                        field.DataType = model.FieldDataType;
                        field.DecimalDigit = model.FieldDecimalDigit;
                        field.CurrencySymbol = model.FieldCurrencySymbol;
                        field.MinLength = model.FieldMinLength;
                        field.MaxLength = model.FieldMaxLength;
                        field.MinNumber = model.FieldMinNumber;
                        field.MaxNumber = model.FieldMaxNumber;
                        field.LOVs = model.FieldLOVs;
                        _dbCntxt.GeneralFields.Add(field);
                        _dbCntxt.SaveChanges(); //wait until saving of general field is done
                        form.FieldId = field.Id;
                    }
                    else
                        form.FieldId = model.FieldId;

                    form.Published = model.Published;
                    form.IsRequired = model.IsRequired;
                    form.Sequence = model.Sequence;
                    form.Editable = model.Editable;
                    form.DefaultValue = model.DefaultValue;
                    form.DefaultValue2 = model.DefaultValue2;
                    _dbCntxt.RequestForms.Add(form);
                }
                else
                {
                    //Edit purposes
                    var requestFormInDb = await _dbCntxt.RequestForms
                                                    .Where
                                                    (
                                                        r => r.RequestTypeId == model.RequestTypeId &&
                                                        r.Version == model.Version &&
                                                        r.FieldId == model.OldFieldId
                                                    ).FirstOrDefaultAsync();

                    if (requestFormInDb != null)
                    {
                        //rearrange sequence
                        if (!(await this.CheckValidSequence(model.RequestTypeId, model.Version, model.Sequence)))
                            throw new CustomException("Sequence is out of range.", 400);

                        if (requestFormInDb.Sequence != model.Sequence)
                            await ReArrangeSequence(model.RequestTypeId, model.Version, model.OldFieldId, model.Sequence, requestFormInDb.Sequence);

                        //Edit but new column is added
                        requestFormInDb.RequestTypeId = model.RequestTypeId;
                        requestFormInDb.Version = model.Version;

                        if (model.FieldId == 0)
                        {
                            if (model.FieldLOVs.Length > 0)
                            {
                                //Parse LOV to be saved
                                var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(model.FieldLOVs);

                                foreach (var lov in lovs)
                                {
                                    lov.OldName = "";
                                    lov.OldValue = "";
                                }

                                model.FieldLOVs = JsonSerializer.Serialize(lovs);
                            }

                            string controlShortText = GeneralHelper.GetFieldTypeShorText(model.FieldType);
                            string concatDateTime = DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "") + DateTime.Now.ToString("hh:mm:ss:fff").Replace(":", "");
                            string zeros = "00000";
                            int requestTypeIdLength = model.RequestTypeId.ToString().Length;
                            string requestTypeSlug = "";

                            if (zeros.Length == requestTypeIdLength)
                                requestTypeSlug = model.RequestTypeId.ToString();
                            else
                                requestTypeSlug = zeros.Substring(0, (zeros.Length - requestTypeIdLength)) + model.RequestTypeId.ToString();

                            string slug = controlShortText + "_" + requestTypeSlug + "_" + concatDateTime;

                            GeneralField field = new GeneralField();
                            field.Slug = slug;
                            field.Name = model.FieldName;
                            field.FieldType = (int)model.FieldType;
                            field.Description = model.FieldDescription;
                            field.DataType = model.FieldDataType;
                            field.DecimalDigit = model.FieldDecimalDigit;
                            field.CurrencySymbol = model.FieldCurrencySymbol;
                            field.MinLength = model.FieldMinLength;
                            field.MaxLength = model.FieldMaxLength;
                            field.MinNumber = model.FieldMinNumber;
                            field.MaxNumber = model.FieldMaxNumber;
                            field.MaxLength = model.FieldMaxLength;
                            field.LOVs = model.FieldLOVs;
                            _dbCntxt.GeneralFields.Add(field);
                            _dbCntxt.SaveChanges(); //wait until saving of general field is done
                            requestFormInDb.FieldId = field.Id;
                        }
                        else
                        {
                            bool needToReconstructConditionText = false;

                            if (model.OldFieldId == model.FieldId)
                            {
                                //Edit and existing column
                                var generalField = await _dbCntxt.GeneralFields.Where(f => f.Id == model.FieldId).FirstOrDefaultAsync();

                                if (generalField != null)
                                {
                                    var sourceField = _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Field Name").FirstOrDefault();

                                    if (generalField.FieldType != (int)model.FieldType)
                                    {
                                        //if fieldy is changed
                                        //Change slug
                                        string controlShortText = GeneralHelper.GetFieldTypeShorText(model.FieldType);
                                        generalField.Slug = controlShortText + generalField.Slug.Substring(3, generalField.Slug.Length - 3);
                                    }

                                    if (this.IsInStepCondition(model.RequestTypeId, model.Version, model.OldFieldId, "", true))
                                    {
                                        if (generalField.Name.Trim().ToUpper() != model.FieldName.Trim().ToUpper())
                                            needToReconstructConditionText = true;
                                    }

                                    generalField.Name = model.FieldName;
                                    generalField.FieldType = (int)model.FieldType;
                                    generalField.Description = model.FieldDescription;
                                    generalField.DataType = model.FieldDataType;
                                    generalField.DecimalDigit = model.FieldDecimalDigit;
                                    generalField.CurrencySymbol = model.FieldCurrencySymbol;
                                    generalField.MinLength = model.FieldMinLength;
                                    generalField.MaxLength = model.FieldMaxLength;
                                    generalField.MinNumber = model.FieldMinNumber;
                                    generalField.MaxNumber = model.FieldMaxNumber;
                                    generalField.MaxLength = model.FieldMaxLength;

                                    if (model.FieldLOVs.Length > 0)
                                    {
                                        //Parse LOV to be saved
                                        var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(model.FieldLOVs);

                                        var lovsInDB = JsonSerializer.Deserialize<List<LOVViewModel>>(generalField.LOVs);

                                        foreach (var lovInDB in lovsInDB)
                                        {
                                            //Delete inactive condition once LOV is deleted in front end
                                            var checkLOV = lovs.FirstOrDefault(l => l.OldName != null && l.OldName.Trim().ToUpper() == lovInDB.Name.Trim().ToUpper());

                                            if (checkLOV == null)
                                            {
                                                //already deleted
                                                var isMappedInStepCondition = this.IsInStepCondition(model.RequestTypeId, model.Version, model.OldFieldId, lovInDB.Name, false);

                                                if (isMappedInStepCondition)
                                                {
                                                    var conditionsToBeDeleted = _dbCntxt.RequestStepConditionDetails
                                                                                        .Include(r => r.ConditionHeader)
                                                                                        .Where
                                                                                        (
                                                                                            r =>
                                                                                            r.ConditionHeader.RequestTypeId == model.RequestTypeId &&
                                                                                            r.ConditionHeader.Version == model.Version &&
                                                                                            r.Source == sourceField.Id &&
                                                                                            r.ReferenceDetail == model.OldFieldId &&
                                                                                            r.Value.Trim().ToUpper() == lovInDB.Name.Trim().ToUpper() &&
                                                                                            r.Published == false
                                                                                        ).ToList();

                                                    if (conditionsToBeDeleted.Count > 0)
                                                    {
                                                        var headerConditionIds = conditionsToBeDeleted.Select(c => c.RequestStepConditionId).Distinct().ToList();
                                                        var detailConditionIds = conditionsToBeDeleted.Select(c => c.Id).ToList();

                                                        //delete all inactive condition details
                                                        _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(c => detailConditionIds.Contains(c.Id)));
                                                        _dbCntxt.SaveChanges();

                                                        //Rearrange sequence and align rows with logical operator per header id
                                                        foreach (var headerId in headerConditionIds)
                                                        {
                                                            var conditionDetails = await _dbCntxt.RequestStepConditionDetails
                                                                                           .Where
                                                                                           (
                                                                                               r =>
                                                                                               r.RequestStepConditionId == headerId
                                                                                           )
                                                                                           .OrderBy(r => r.Sequence)
                                                                                           .ToListAsync();

                                                            if (conditionDetails.Count > 0)
                                                            {
                                                                int sequence = 1;
                                                                foreach (var detail in conditionDetails)
                                                                {
                                                                    detail.Sequence = sequence;

                                                                    if (sequence == conditionDetails.Count)
                                                                        //set to null if last row
                                                                        detail.LogicalOperator = null;

                                                                    _dbCntxt.Entry(detail).State = EntityState.Modified;
                                                                    await _dbCntxt.SaveChangesAsync();

                                                                    sequence += 1;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        foreach (var lov in lovs.Where(l => l.OldName != null))
                                        {
                                            if (lov.Name.Trim().ToUpper() != lov.OldName.Trim().ToUpper())
                                            {
                                                //Update conditions once LOV is mapped to process
                                                var isMappedInAssignees = this.IsInConditionalAssignee(model.RequestTypeId, model.Version, model.OldFieldId, lov.OldName);

                                                if (isMappedInAssignees)
                                                {
                                                    //Assignee Field 1
                                                    string strQuery = "";

                                                    strQuery = @"Update
	                                                                dbo.RequestStepAssignees
                                                                Set
	                                                                Value1 = '" + lov.Name.Replace("'", "''") + @"'
                                                                from 
	                                                                dbo.RequestStepAssignees
                                                                Where
	                                                                IsConditional = 1 And
	                                                                RequestTypeId = " + model.RequestTypeId + @" And
	                                                                Version = " + model.Version + @" And
	                                                                Isnull(Field1, 0) = " + model.OldFieldId + @" And
	                                                                Isnull(Upper(Rtrim(Ltrim(Value1))), '') = '" + lov.OldName.Trim().ToUpper().Replace("'", "''") + @"'";

                                                    _dbCntxt.Database.ExecuteSqlRaw(strQuery);
                                                    _dbCntxt.SaveChanges();

                                                    //Assignee Field 2
                                                    strQuery = @"Update
	                                                                dbo.RequestStepAssignees
                                                                Set
	                                                                Value2 = '" + lov.Name.Replace("'", "''") + @"'
                                                                from 
	                                                                dbo.RequestStepAssignees
                                                                Where
	                                                                IsConditional = 1 And
	                                                                RequestTypeId = " + model.RequestTypeId + @" And
	                                                                Version = " + model.Version + @" And
	                                                                Isnull(Field2, 0) = " + model.OldFieldId + @" And
	                                                                Isnull(Upper(Rtrim(Ltrim(Value2))), '') = '" + lov.OldName.Trim().ToUpper().Replace("'", "''") + @"'";

                                                    _dbCntxt.Database.ExecuteSqlRaw(strQuery);
                                                    _dbCntxt.SaveChanges();

                                                    //Assignee Field 3
                                                    strQuery = @"Update
	                                                                dbo.RequestStepAssignees
                                                                Set
	                                                                Value3 = '" + lov.Name.Replace("'", "''") + @"'
                                                                from 
	                                                                dbo.RequestStepAssignees
                                                                Where
	                                                                IsConditional = 1 And
	                                                                RequestTypeId = " + model.RequestTypeId + @" And
	                                                                Version = " + model.Version + @" And
	                                                                Isnull(Field3, 0) = " + model.OldFieldId + @" And
	                                                                Isnull(Upper(Rtrim(Ltrim(Value3))), '') = '" + lov.OldName.Trim().ToUpper().Replace("'", "''") + @"'";

                                                    _dbCntxt.Database.ExecuteSqlRaw(strQuery);
                                                    _dbCntxt.SaveChanges();

                                                }

                                                //Update conditions once LOV is mapped to process
                                                var isMappedInStepCondition = this.IsInStepCondition(model.RequestTypeId, model.Version, model.OldFieldId, lov.OldName);

                                                if (isMappedInStepCondition)
                                                {
                                                    needToReconstructConditionText = true;

                                                    var conditionsToBeUpdated = _dbCntxt.RequestStepConditionDetails
                                                                                        .Include(r => r.ConditionHeader)
                                                                                        .Where
                                                                                        (
                                                                                            r =>
                                                                                            r.ConditionHeader.RequestTypeId == model.RequestTypeId &&
                                                                                            r.ConditionHeader.Version == model.Version &&
                                                                                            r.Source == sourceField.Id &&
                                                                                            r.ReferenceDetail == model.OldFieldId &&
                                                                                            r.Value.Trim().ToUpper() == lov.OldName.Trim().ToUpper()
                                                                                        ).ToList();

                                                    if (conditionsToBeUpdated.Count > 0)
                                                    {
                                                        foreach (var toUpdate in conditionsToBeUpdated)
                                                        {
                                                            var conditionToUpdate = _dbCntxt.RequestStepConditionDetails
                                                                                            .FirstOrDefault(r => r.Id == toUpdate.Id);

                                                            if (conditionToUpdate != null)
                                                            {
                                                                conditionToUpdate.Value = lov.Name;
                                                                _dbCntxt.Entry(conditionToUpdate).State = EntityState.Modified;
                                                                _dbCntxt.SaveChanges();
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            lov.OldName = "";
                                            lov.OldValue = "";
                                        }

                                        model.FieldLOVs = JsonSerializer.Serialize(lovs);
                                    }

                                    generalField.LOVs = model.FieldLOVs;
                                    _dbCntxt.Entry(generalField).State = EntityState.Modified;
                                    _dbCntxt.SaveChanges(); //wait until saving of general field is done

                                    //Reconstruct Condition here
                                    if (needToReconstructConditionText)
                                    {
                                        //code here
                                        var affectedRecord = await _dbCntxt.RequestStepConditionDetails
                                                                     .Include(r => r.ConditionHeader)
                                                                    .Where
                                                                    (
                                                                        r =>
                                                                        r.ConditionHeader.RequestTypeId == model.RequestTypeId &&
                                                                        r.ConditionHeader.Version == model.Version &&
                                                                        r.Published == true &&
                                                                        r.Source == sourceField.Id &&
                                                                        r.ReferenceDetail == model.OldFieldId
                                                                    )
                                                                    .Select(r => r.RequestStepConditionId)
                                                                    .Distinct()
                                                                    .ToListAsync();

                                        if (affectedRecord.Count > 0)
                                        {
                                            foreach (int id in affectedRecord)
                                            {
                                                var affectedStepConditions = await _dbCntxt.RequestStepConditionDetails
                                                                             .Where(r => r.RequestStepConditionId == id && r.Published == true)
                                                                             .ToListAsync();

                                                if (affectedStepConditions.Count > 0)
                                                {
                                                    //Build condition header
                                                    var constructedConditions = await this.ConstructStepConditions(id, affectedStepConditions);

                                                    if (constructedConditions.Count > 0)
                                                    {
                                                        //Update condition header
                                                        var conditionHeader = await _dbCntxt.RequestStepConditions
                                                                                      .FirstOrDefaultAsync(r => r.Id == id);

                                                        if (conditionHeader != null)
                                                        {
                                                            conditionHeader.Condition = constructedConditions[0];
                                                            conditionHeader.ConditionInText = constructedConditions[1];
                                                            _dbCntxt.Entry(conditionHeader).State = EntityState.Modified;
                                                            await _dbCntxt.SaveChangesAsync();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            requestFormInDb.FieldId = model.FieldId;
                        }

                        requestFormInDb.Published = model.Published;
                        requestFormInDb.IsRequired = model.IsRequired;
                        requestFormInDb.Sequence = model.Sequence;
                        requestFormInDb.Editable = model.Editable;
                        requestFormInDb.DefaultValue = model.DefaultValue;
                        requestFormInDb.DefaultValue2 = model.DefaultValue2;
                        _dbCntxt.Entry(requestFormInDb).State = EntityState.Modified;
                    }
                }

                await _dbCntxt.SaveChangesAsync();

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestTypeInDb != null)
                {
                    requestTypeInDb.ModifiedByPK = cid;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);
                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details, customex.ListMessage);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task Delete(object obj, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var requestTypeParams = (RequestTypeDeletionParam)obj;
                var requestType = await this.FindAll().Where(r => r.Id == requestTypeParams.Id && r.Version == requestTypeParams.Version).FirstOrDefaultAsync();

                if (requestType != null)
                {
                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    /// check if request type is locked
                    var lockedRequestTypeInDb = await _dbCntxt.LockedRequestTypes
                                                           .Include(u => u.UserInfo)
                                                           .Include(up => up.UserInfo.AspNetUsersProfile)
                                                           .Where(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version).FirstOrDefaultAsync();

                    if (lockedRequestTypeInDb != null)
                    {
                        if (lockedRequestTypeInDb.User != cid)
                            throw new CustomException("This request type is currently edited by: " + lockedRequestTypeInDb.UserInfo.AspNetUsersProfile.FirstName + " " + lockedRequestTypeInDb.UserInfo.AspNetUsersProfile.LastName, 400);
                    }

                    var checkIfExistingRequestTypesGrouping = _dbCntxt.RequestTypesGroupingDetails.FirstOrDefaultAsync(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version);

                    if (checkIfExistingRequestTypesGrouping != null)
                        throw new CustomException("This request type is currently being maintained in request types grouping.", 400);

                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);
                    //Locked Request Type
                    _dbCntxt.LockedRequestTypes.RemoveRange(_dbCntxt.LockedRequestTypes.Where(x => x.RequestTypeId == requestTypeParams.Id && x.Version == requestTypeParams.Version));
                    await _dbCntxt.SaveChangesAsync();
                    //Request Step Details
                    //Delete parallel workflows

                    //Delete workflows
                    _dbCntxt.RequestTypeWorkFlows.RemoveRange(_dbCntxt.RequestTypeWorkFlows.Where(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version));
                    await _dbCntxt.SaveChangesAsync();
                    //Delete Assignees
                    var assigneesHeaders = await _dbCntxt.RequestStepAssignees.Where(a => a.RequestTypeId == requestTypeParams.Id && a.Version == requestTypeParams.Version).ToListAsync();

                    if (assigneesHeaders.Count > 0)
                    {
                        //remove details
                        foreach (var assigneesHeader in assigneesHeaders)
                        {
                            _dbCntxt.RequestStepAssigneeDetails.RemoveRange(_dbCntxt.RequestStepAssigneeDetails.Where(a => a.RequestStepAssigneeId == assigneesHeader.Id));
                            await _dbCntxt.SaveChangesAsync();
                        }

                        //remove header
                        _dbCntxt.RequestStepAssignees.RemoveRange(_dbCntxt.RequestStepAssignees.Where(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version));
                        await _dbCntxt.SaveChangesAsync();
                    }

                    //Delete Conditions
                    var conditionHeaders = await _dbCntxt.RequestStepConditions.Where(a => a.RequestTypeId == requestTypeParams.Id && a.Version == requestTypeParams.Version).ToListAsync();

                    if (conditionHeaders.Count > 0)
                    {
                        //remove details
                        foreach (var conditionHeader in conditionHeaders)
                        {
                            _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(a => a.RequestStepConditionId == conditionHeader.Id));
                            await _dbCntxt.SaveChangesAsync();
                        }
                        //remove header
                        _dbCntxt.RequestStepConditions.RemoveRange(_dbCntxt.RequestStepConditions.Where(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version));
                        await _dbCntxt.SaveChangesAsync();
                    }

                    //Delete steps
                    var steps = await _dbCntxt.RequestSteps.Where(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version).ToListAsync();

                    _dbCntxt.RequestSteps.RemoveRange(_dbCntxt.RequestSteps.Where(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version));
                    await _dbCntxt.SaveChangesAsync();

                    foreach (var step in steps)
                    {
                        //Delete in WorkflowSteps if the steps is not used in request steps 
                        var requestStep = await _dbCntxt.RequestSteps.FirstOrDefaultAsync(r => r.StepId == step.StepId);

                        if (requestStep == null)
                        {
                            _dbCntxt.WorkFlowSteps.RemoveRange(_dbCntxt.WorkFlowSteps.Where(r => r.Id == step.StepId));
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }

                    //Request Type Process Owners
                    _dbCntxt.RequestTypeProcessOwners.RemoveRange(_dbCntxt.RequestTypeProcessOwners.Where(x => x.RequestTypeId == requestTypeParams.Id && x.Version == requestTypeParams.Version));
                    await _dbCntxt.SaveChangesAsync();
                    //Request Type Forms
                    var fields = await _dbCntxt.RequestForms.Where(r => r.RequestTypeId == requestTypeParams.Id && r.Version == requestTypeParams.Version).ToListAsync();

                    _dbCntxt.RequestForms.RemoveRange(_dbCntxt.RequestForms.Where(x => x.RequestTypeId == requestTypeParams.Id && x.Version == requestTypeParams.Version));
                    await _dbCntxt.SaveChangesAsync();
                    //General fields based from deleted request forms
                    foreach (var field in fields)
                    {
                        var fieldRequestForm = await _dbCntxt.RequestForms.FirstOrDefaultAsync(r => r.FieldId == field.FieldId);

                        if (fieldRequestForm == null)
                        {
                            _dbCntxt.GeneralFields.RemoveRange(_dbCntxt.GeneralFields.Where(r => r.Id == field.FieldId));
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }

                    if (requestTypeParams.ReasonId > 0)
                    {
                        if (String.IsNullOrWhiteSpace(requestTypeParams.Remarks))
                            throw new CustomException("Remarks for deletion is required", 400);

                        //Update deletion reason
                        var requestTypeToDelete = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == requestTypeParams.Id && r.Version == requestTypeParams.Version);

                        if (requestTypeToDelete != null)
                        {
                            requestTypeToDelete.DeletionReasonId = requestTypeParams.ReasonId;
                            requestTypeToDelete.DeletionRemarks = requestTypeParams.Remarks;
                            //For future purposes incase deletion will be a soft delete only
                            //requestTypeToDelete.ModifiedByPK = cid;
                            //requestTypeToDelete.ModifiedDate = DateTime.Now;
                            _dbCntxt.Entry(requestTypeToDelete).State = EntityState.Modified;
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                    //Request Type
                    _dbCntxt.RequestTypes.RemoveRange(_dbCntxt.RequestTypes.Where(r => r.Id == requestTypeParams.Id && r.Version == requestTypeParams.Version));
                    await _dbCntxt.SaveChangesAsync();
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);
                }
                else
                    throw new CustomException("Request Type is not found.", 404);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteAll(RequestTypeViewModel model, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            throw new NotImplementedException();
        }
        public IQueryable<RequestTypeViewModel> Find(object obj)
        {
            try
            {
                IQueryable<RequestTypeViewModel> query = null;

                var condition = (SearchRequestType)obj;

                query = this.Get()
                            .Where
                            (
                                r =>
                                r.Id == condition.Id &&
                                r.Version == condition.Version
                            )
                            .Select
                            (
                                r => new RequestTypeViewModel
                                {
                                    Id = r.Id,
                                    RequestCategoryId = r.RequestCategoryId,
                                    RequestCategory = r.RequestCategory.Name,
                                    RequestCategoryPublished = r.RequestCategory.Published,
                                    Version = r.Version,
                                    Version2 = r.Version2,
                                    Name = r.Name,
                                    Description = r.Description,
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate,
                                    Status = (RequestTypeStatus)r.Status,
                                    StatusDescForSorting = (r.Status == 1 ? "Draft" : r.Status == 2 ? "Finalized" : r.Status == 3 ? "Active" : "Inactive"),
                                    RequestForms = _dbCntxt.RequestForms
                                                           .Where
                                                           (
                                                                x =>
                                                                x.RequestTypeId == r.Id &&
                                                                x.Version == r.Version
                                                           )
                                                           .Select
                                                           (
                                                                x => new RequestFormViewModel
                                                                {
                                                                    RequestTypeId = x.RequestTypeId,
                                                                    Version = x.Version,
                                                                    FieldId = x.FieldId,
                                                                    FieldSlug = x.GeneralField.Slug,
                                                                    FieldName = x.GeneralField.Name,
                                                                    FieldType = (FieldType)x.GeneralField.FieldType,
                                                                    FieldDescription = x.GeneralField.Description,
                                                                    FieldDataType = x.GeneralField.DataType,
                                                                    FieldDecimalDigit = x.GeneralField.DecimalDigit,
                                                                    FieldMinLength = x.GeneralField.MinLength,
                                                                    FieldMaxLength = x.GeneralField.MaxLength,
                                                                    FieldMinNumber = x.GeneralField.MinNumber,
                                                                    FieldMaxNumber = x.GeneralField.MaxNumber,
                                                                    FieldCurrencySymbol = x.GeneralField.CurrencySymbol,
                                                                    FieldLOVs = x.GeneralField.LOVs,
                                                                    Published = x.Published,
                                                                    IsRequired = x.IsRequired,
                                                                    Sequence = x.Sequence,
                                                                    Editable = x.Editable,
                                                                    DefaultValue = x.DefaultValue,
                                                                    DefaultValue2 = x.DefaultValue2
                                                                }
                                                           )
                                                           .ToList(),
                                    ProcessOwners = _dbCntxt.RequestTypeProcessOwners
                                                           .Where
                                                           (
                                                                x =>
                                                                x.RequestTypeId == r.Id &&
                                                                x.Version == r.Version
                                                           )
                                                           .Select
                                                           (
                                                                x => new RequestTypeProcessOwnersViewModel
                                                                {
                                                                    Id = x.Id,
                                                                    RequestTypeId = x.RequestTypeId,
                                                                    Version = x.Version,
                                                                    Owner = x.Owner,
                                                                    OwnerFirstName = x.User.AspNetUsersProfile.FirstName,
                                                                    OwnerLastName = x.User.AspNetUsersProfile.LastName,
                                                                    OwnerFullName = x.User.AspNetUsersProfile.LastName + ", " + x.User.AspNetUsersProfile.FirstName,
                                                                    OwnerFullName2 = x.User.AspNetUsersProfile.FirstName + " " + x.User.AspNetUsersProfile.LastName,
                                                                    OwnerPosition = x.User.AspNetUsersProfile.Position,
                                                                    OwnerDepartment = x.User.AspNetUsersProfile.Department,
                                                                    OwnerEmail = x.User.Email

                                                                }
                                                           ).ToList(),
                                    WithCreatedProcessOrStep = _dbCntxt.RequestSteps
                                                                       .Where
                                                                       (
                                                                            s => s.RequestTypeId == r.Id &&
                                                                            s.Version == r.Version
                                                                       ).ToList().Count > 0 ? true : false
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
        public IQueryable<RequestTypeViewModel> FindAll()
        {
            try
            {
                IQueryable<RequestTypeViewModel> query = null;
                query = this.Get()
                            .Select
                            (
                                r => new RequestTypeViewModel
                                {
                                    Id = r.Id,
                                    RequestCategoryId = r.RequestCategoryId,
                                    RequestCategory = r.RequestCategory.Name,
                                    RequestCategoryPublished = r.RequestCategory.Published,
                                    Version = r.Version,
                                    Version2 = r.Version2,
                                    Name = r.Name,
                                    Description = (r.Description == null ? "" : r.Description),
                                    CreatedByPK = r.CreatedByPK,
                                    CreatedDate = r.CreatedDate,
                                    ModifiedByPK = r.ModifiedByPK,
                                    ModifiedDate = r.ModifiedDate,
                                    Status = (RequestTypeStatus)r.Status,
                                    StatusDescForSorting = (r.Status == 1 ? "Draft" : r.Status == 2 ? "Finalized" : r.Status == 3 ? "Active" : "Inactive"),
                                    Locked = _dbCntxt.LockedRequestTypes
                                                .Include(u => u.UserInfo)
                                                .Include(up => up.UserInfo.AspNetUsersProfile)
                                                .Where
                                                (
                                                    x =>
                                                    x.RequestTypeId == r.Id &&
                                                    x.Version == r.Version
                                                )
                                                .Select
                                                (
                                                    r => new LockedRequestTypesViewModel()
                                                    {
                                                        User = r.User,
                                                        UserFirstName = r.UserInfo.AspNetUsersProfile.FirstName,
                                                        UserLastName = r.UserInfo.AspNetUsersProfile.LastName,
                                                        UserPosition = r.UserInfo.AspNetUsersProfile.Position,
                                                        UserDepartment = r.UserInfo.AspNetUsersProfile.Department,
                                                        UserEmail = r.UserInfo.Email,
                                                        DateLocked = r.DateLocked
                                                    }
                                                )
                                                .ToList(),
                                    WithCreatedProcessOrStep = _dbCntxt.RequestSteps
                                                                       .Where
                                                                       (
                                                                            s => s.RequestTypeId == r.Id &&
                                                                            s.Version == r.Version
                                                                       ).ToList().Count > 0 ? true : false
                                }
                            )
                            .AsQueryable();

                return query;
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<bool> IfExists(string toSearch, object entityPrimaryKey = null)
        {
            bool isExists = false;
            int condition = 0;

            try
            {
                if (entityPrimaryKey == null)
                {
                    var requestTypesInDb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch);

                    if (requestTypesInDb != null)
                        isExists = true;
                }
                else
                {
                    condition = Convert.ToInt32(entityPrimaryKey);
                    var requestTypesInDb = await this.FindAll().FirstOrDefaultAsync(r => r.Name == toSearch && r.Id != condition);

                    if (requestTypesInDb != null)
                        isExists = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return isExists;
        }
        public PagingResponse<RequestTypeViewModel> PagingFeature(IQueryable<RequestTypeViewModel> query, PagingRequest paging, object id = null)
        {
            try
            {
                if (paging.Draw > 1)
                    // sort order call method from base class
                    query = QueryHelper.SortOrder<RequestTypeViewModel>(query, paging);

                // paginate call method from base class
                return QueryHelper.Paginate<RequestTypeViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task Update(RequestTypeViewModel model, object id = null, bool saveIpPerSession = true, IDbContextTransaction transaction = null)
        {
            if (model.RequestCategoryId <= 0)
                throw new CustomException("Request Category is required.", 400);

            if (String.IsNullOrWhiteSpace(model.Name))
                throw new CustomException("Request Type is required.", 400);

            //Request Type Status
            if (!(GeneralHelper.IsValidRequestTypeStatus(Convert.ToInt32(model.Status))))
                throw new CustomException("Invalid Request Type Status.", 400);

            //  calls the Dispose method on the object in the correct way when it goes out of scope.
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                if (await this.IfExists(model.Name, model.Id))
                    throw new CustomException("Request Name provided for the category is already existing.", 400);

                var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.Id && r.Version == model.Version);

                if (requestType != null)
                {
                    if (model.ModifiedDate != requestType.ModifiedDate)
                        throw new CustomException("The information is no longer updated. Please close and reopen the form to get updated information.", 400);

                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    // get current session id
                    int spId = await _userIpAddressPerSession.GetSPID();
                    // get IP address
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //add useripperssion
                    var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                    await _userIpAddressPerSession.Save(userIp);

                    requestType.Name = model.Name;
                    requestType.RequestCategoryId = model.RequestCategoryId;
                    requestType.Description = model.Description;
                    requestType.Status = (int)model.Status;
                    requestType.Version = model.Version;
                    requestType.Version2 = model.Version2;
                    requestType.ModifiedByPK = cid;
                    requestType.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestType).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                    //remove useripperssion
                    await _userIpAddressPerSession.Remove(spId);

                    dbContextTransaction.Commit();
                }
                else
                    throw new CustomException("Request Type and version is not found", 404);
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private IQueryable<RequestType> Get()
        {
            try
            {
                IQueryable<RequestType> query = null;

                query = _dbCntxt.RequestTypes
                                .AsQueryable();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<RequestFormViewModel>> GetFields(int requestTypeId = 0, int version = 0, bool? isLov = null, bool? isActiveLovOnly = null)
        {
            try
            {
                List<RequestFormViewModel> fieldList = new List<RequestFormViewModel>();

                var generalFields = await this.GetGeneralFields();

                if (requestTypeId > 0 && version > 0)
                    generalFields = generalFields.Where(f => f.RequestTypeId == requestTypeId && f.Version == version);

                if (isLov != null)
                {
                    if (Convert.ToBoolean(isLov))
                    {
                        generalFields = generalFields.Where(f => f.FieldLOVs.Length > 0);

                        if (isActiveLovOnly != null)
                        {
                            foreach (var field in generalFields)
                            {
                                //Parse LOV to be saved
                                var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(field.FieldLOVs);

                                var lovsToReplaced = lovs.Where(v => v.IsActive == isActiveLovOnly);

                                field.FieldLOVs = JsonSerializer.Serialize(lovsToReplaced);
                            }
                        }
                    }
                    else
                        generalFields = generalFields.Where(f => f.FieldLOVs.Length <= 0);
                }
                else
                {
                    if (isActiveLovOnly != null)
                    {
                        foreach (var field in generalFields)
                        {
                            if (field.FieldLOVs.Length > 0)
                            {
                                //Parse LOV to be saved
                                var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(field.FieldLOVs);

                                var lovsToReplaced = lovs.Where(v => v.IsActive == isActiveLovOnly);

                                field.FieldLOVs = JsonSerializer.Serialize(lovsToReplaced);
                            }
                        }
                    }
                }

                fieldList = generalFields.ToList();

                return fieldList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task<IEnumerable<RequestFormViewModel>> GetGeneralFields()
        {
            try
            {
                List<RequestFormViewModel> fieldList = new List<RequestFormViewModel>();

                fieldList = await _dbCntxt.RequestForms
                                        .Include(f => f.GeneralField)
                                            .Select
                                            (
                                                f => new RequestFormViewModel()
                                                {
                                                    RequestTypeId = f.RequestTypeId,
                                                    Version = f.Version,
                                                    FieldId = f.FieldId,
                                                    OldFieldId = f.FieldId,
                                                    FieldSlug = f.GeneralField.Slug,
                                                    FieldName = f.GeneralField.Name,
                                                    FieldType = (FieldType)f.GeneralField.FieldType,
                                                    FieldDescription = f.GeneralField.Description,
                                                    FieldDataType = f.GeneralField.DataType,
                                                    FieldDecimalDigit = f.GeneralField.DecimalDigit,
                                                    FieldMinLength = f.GeneralField.MinLength,
                                                    FieldMaxLength = f.GeneralField.MaxLength,
                                                    FieldMinNumber = f.GeneralField.MinNumber,
                                                    FieldMaxNumber = f.GeneralField.MaxNumber,
                                                    FieldCurrencySymbol = f.GeneralField.CurrencySymbol,
                                                    FieldLOVs = f.GeneralField.LOVs,
                                                    Published = f.Published,
                                                    IsRequired = f.IsRequired,
                                                    Sequence = f.Sequence,
                                                    Editable = f.Editable,
                                                    DefaultValue = f.DefaultValue,
                                                    DefaultValue2 = f.DefaultValue2,
                                                    Id = f.FieldId,
                                                    Name = f.GeneralField.Name
                                                }
                                            )
                                            .ToListAsync();

                return fieldList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task SaveRequestTypeProcessOwners(RequestTypeProcessOwnersViewModel model)
        {
            if (model == null)
                throw new CustomException("Kindly provide atleast one process owner to proceed.", 400);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                if (model.UserList.Count == 0)
                    throw new CustomException("Kindly provide atleast one process owner to proceed.", 400);

                var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestType == null)
                    throw new CustomException("Request Type ID and Version is not found.", 404);

                foreach (var mn in model.UserList)
                {
                    //check if user is existing user in db
                    var user = await _dbCntxt.AspNetUsers.FirstOrDefaultAsync(u => u.Id == mn);

                    if (user == null)
                        throw new CustomException("User in the list is not found. ", 404);

                    var owner = await _dbCntxt.RequestTypeProcessOwners.FirstOrDefaultAsync(r => r.RequestTypeId == model.RequestTypeId && r.Version == model.Version && r.Owner == mn);

                    if (owner == null)
                    {
                        RequestTypeProcessOwner processOwner = new RequestTypeProcessOwner();
                        processOwner.RequestTypeId = model.RequestTypeId;
                        processOwner.Version = model.Version;
                        processOwner.Owner = mn;
                        _dbCntxt.RequestTypeProcessOwners.Add(processOwner);
                        await _dbCntxt.SaveChangesAsync();
                    }
                }

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestTypeInDb != null)
                {
                    // get current user id
                    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    requestTypeInDb.ModifiedByPK = userId;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteRequestForm(int requestTypeId, int version, int fieldId = 0)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                //check if request type has created process or step
                var stepProcesses = await this.GetRequestSteps(requestTypeId, version, false);
                bool withCreatedProcess = false;

                if (stepProcesses.ToList().Count > 0)
                    withCreatedProcess = true;

                if (fieldId > 0)
                {
                    var requestType = await _dbCntxt.RequestForms.FirstOrDefaultAsync(r => r.RequestTypeId == requestTypeId && r.Version == version && r.FieldId == fieldId);

                    if (requestType == null)
                        throw new CustomException("Request Type Field is not found.", 404);

                    //There should be atleast one request form to check if has created workflow
                    var requestFormsToBeCounted = await _dbCntxt.RequestForms.Where(r => r.RequestTypeId == requestTypeId && r.Version == version).ToListAsync();

                    if (requestFormsToBeCounted.Count > 0)
                    {
                        if (requestFormsToBeCounted.Count == 1)
                        {
                            //check if request type has created process or step
                            if (withCreatedProcess)
                                throw new CustomException("Unable to delete whole request form since step(s) or processes were already created.", 400);
                        }
                    }
                    else
                        throw new CustomException("No request form to delete", 404);

                    bool isActivelyMapped = await this.CheckFieldIsInvolvedInProcessAsync(requestTypeId, version, fieldId, true);

                    if (isActivelyMapped)
                        throw new CustomException("Must remove the mapped conditions on the Steps/Activities before you could delete this field", 400);

                    //auto delete record in step conditions when there's an inactive condition related to this field for deletion and reconstruct all steps conditions
                    bool isInActivelyMapped = await this.IsInStepConditionAsync(requestTypeId, version, fieldId, "", false);

                    if (isInActivelyMapped)
                    {
                        var sourceField = _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Field Name").FirstOrDefault();

                        var affectedRecords = await _dbCntxt.RequestStepConditionDetails
                                                            .Include(r => r.ConditionHeader)
                                                                    .Where
                                                                    (
                                                                        r =>
                                                                        r.ConditionHeader.RequestTypeId == requestTypeId &&
                                                                        r.ConditionHeader.Version == version &&
                                                                        r.Published == false &&
                                                                        r.Source == sourceField.Id &&
                                                                        r.ReferenceDetail == fieldId
                                                                    )
                                                                    .ToListAsync();

                        if (affectedRecords.Count > 0)
                        {
                            var headerConditionIds = affectedRecords.Select(c => c.RequestStepConditionId).Distinct().ToList();
                            var detailConditionIds = affectedRecords.Select(c => c.Id).ToList();

                            //delete all inactive condition details
                            _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(c => detailConditionIds.Contains(c.Id)));
                            await _dbCntxt.SaveChangesAsync();

                            //Rearrange sequence and align rows with logical operator per header id
                            foreach (var headerId in headerConditionIds)
                            {
                                var conditionDetails = await _dbCntxt.RequestStepConditionDetails
                                                               .Where
                                                               (
                                                                   r =>
                                                                   r.RequestStepConditionId == headerId
                                                               )
                                                               .OrderBy(r => r.Sequence)
                                                               .ToListAsync();

                                if (conditionDetails.Count > 0)
                                {
                                    int sequence = 1;
                                    foreach (var detail in conditionDetails)
                                    {
                                        detail.Sequence = sequence;

                                        if (sequence == conditionDetails.Count)
                                            //set to null if last row
                                            detail.LogicalOperator = null;

                                        _dbCntxt.Entry(detail).State = EntityState.Modified;
                                        await _dbCntxt.SaveChangesAsync();

                                        sequence += 1;
                                    }
                                }
                            }
                        }
                    }

                    //Delete request form field
                    _dbCntxt.RequestForms.RemoveRange(_dbCntxt.RequestForms.Where(r => r.RequestTypeId == requestTypeId && r.Version == version && r.FieldId == fieldId));
                    await _dbCntxt.SaveChangesAsync();
                    //Delete in general field if the field is not used in request form
                    var field = await _dbCntxt.RequestForms.FirstOrDefaultAsync(r => r.FieldId == fieldId);

                    if (field == null)
                    {
                        _dbCntxt.GeneralFields.RemoveRange(_dbCntxt.GeneralFields.Where(r => r.Id == fieldId));
                        await _dbCntxt.SaveChangesAsync();
                    }

                    //Rearrange sequence from ascending order
                    var requestForms = await _dbCntxt.RequestForms
                                                        .Where(r => r.RequestTypeId == requestTypeId && r.Version == version)
                                                        .OrderBy(r => r.Sequence)
                                                        .ToListAsync();

                    if (requestForms.Count > 0)
                    {
                        int ctr = 1;
                        foreach (var requestForm in requestForms)
                        {
                            var rf = await _dbCntxt.RequestForms
                                                    .FirstOrDefaultAsync
                                                    (
                                                        r =>
                                                        r.RequestTypeId == requestForm.RequestTypeId &&
                                                        r.Version == requestForm.Version &&
                                                        r.FieldId == requestForm.FieldId
                                                    );
                            if (rf != null)
                            {
                                rf.Sequence = ctr;
                                _dbCntxt.Entry(rf).State = EntityState.Modified;
                                await _dbCntxt.SaveChangesAsync();
                            }

                            ctr++;
                        }
                    }
                }
                else
                {
                    //delete the whole request form
                    var requestForm = await _dbCntxt.RequestForms.Where(r => r.RequestTypeId == requestTypeId && r.Version == version).ToListAsync();

                    if (requestForm.Count <= 0)
                        throw new CustomException("No request form to delete.", 404);

                    //check if request type has created process or step
                    if (withCreatedProcess)
                        throw new CustomException("Unable to delete whole request form since step(s) or processes were already created.", 400);

                    bool isActivelyMapped = false;

                    foreach (var field in requestForm)
                    {
                        if (await this.CheckFieldIsInvolvedInProcessAsync(field.RequestTypeId, field.Version, field.FieldId, true))
                            isActivelyMapped = true;

                        if (isActivelyMapped)
                            break;
                    }

                    if (isActivelyMapped)
                        throw new CustomException("Must remove the mapped conditions on the Steps/Activities before you could delete the whole request form", 400);

                    foreach (var field in requestForm)
                    {
                        //auto delete field in step conditions when there's an inactive condition related to this field for deletion and reconstruct all steps conditions
                        bool isInActivelyMapped = await this.IsInStepConditionAsync(field.RequestTypeId, field.Version, field.FieldId, "", false);

                        if (isInActivelyMapped)
                        {
                            var sourceField = _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Field Name").FirstOrDefault();

                            var affectedRecords = await _dbCntxt.RequestStepConditionDetails
                                                                        .Where
                                                                        (
                                                                            r =>
                                                                            r.Published == false &&
                                                                            r.Source == sourceField.Id &&
                                                                            r.ReferenceDetail == field.FieldId
                                                                        )
                                                                        .ToListAsync();

                            if (affectedRecords.Count > 0)
                            {
                                var headerConditionIds = affectedRecords.Select(c => c.RequestStepConditionId).Distinct().ToList();
                                var detailConditionIds = affectedRecords.Select(c => c.Id).ToList();

                                //delete all inactive condition details
                                _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(c => detailConditionIds.Contains(c.Id)));
                                await _dbCntxt.SaveChangesAsync();

                                //Rearrange sequence and align rows with logical operator per header id
                                foreach (var headerId in headerConditionIds)
                                {
                                    var conditionDetails = await _dbCntxt.RequestStepConditionDetails
                                                                   .Where
                                                                   (
                                                                       r =>
                                                                       r.RequestStepConditionId == headerId
                                                                   )
                                                                   .OrderBy(r => r.Sequence)
                                                                   .ToListAsync();

                                    if (conditionDetails.Count > 0)
                                    {
                                        int sequence = 1;
                                        foreach (var detail in conditionDetails)
                                        {
                                            detail.Sequence = sequence;

                                            if (sequence == conditionDetails.Count)
                                                //set to null if last row
                                                detail.LogicalOperator = null;

                                            _dbCntxt.Entry(detail).State = EntityState.Modified;
                                            await _dbCntxt.SaveChangesAsync();

                                            sequence += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Delete request form
                    _dbCntxt.RequestForms.RemoveRange(_dbCntxt.RequestForms.Where(r => r.RequestTypeId == requestTypeId && r.Version == version));
                    await _dbCntxt.SaveChangesAsync();

                    foreach (var requestFormField in requestForm)
                    {
                        //Delete in general field if the field is not used in request form
                        var field = await _dbCntxt.RequestForms.FirstOrDefaultAsync(r => r.FieldId == requestFormField.FieldId);

                        if (field == null)
                        {
                            _dbCntxt.GeneralFields.RemoveRange(_dbCntxt.GeneralFields.Where(r => r.Id == requestFormField.FieldId));
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == requestTypeId && r.Version == version);

                if (requestTypeInDb != null)
                {
                    // get current user id
                    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    requestTypeInDb.ModifiedByPK = userId;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteRequestTypeProcessOwner(int id)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var owner = await _dbCntxt.RequestTypeProcessOwners.FirstOrDefaultAsync(r => r.Id == id);

                if (owner == null)
                    throw new CustomException("Process owner not found.", 404);

                _dbCntxt.RequestTypeProcessOwners.RemoveRange(_dbCntxt.RequestTypeProcessOwners.Where(r => r.Id == id));
                await _dbCntxt.SaveChangesAsync();

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == owner.RequestTypeId && r.Version == owner.Version);

                if (requestTypeInDb != null)
                {
                    // get current user id
                    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    requestTypeInDb.ModifiedByPK = userId;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<RequestFormControlsViewModel>> GetRequestForms(int requestTypeId, int version, bool? published = null, bool? activeLOVs = null)
        {
            try
            {
                var requestForms = await _dbCntxt.RequestForms
                                                 .Where(r => r.RequestTypeId == requestTypeId && r.Version == version)
                                                 .Select
                                                 (
                                                      r => new RequestFormViewModel()
                                                      {
                                                          OldFieldId = r.FieldId,
                                                          FieldId = r.FieldId,
                                                          FieldSlug = r.GeneralField.Slug,
                                                          FieldName = r.GeneralField.Name,
                                                          FieldType = (FieldType)r.GeneralField.FieldType,
                                                          FieldDescription = r.GeneralField.Description,
                                                          FieldDataType = r.GeneralField.DataType,
                                                          FieldDecimalDigit = r.GeneralField.DecimalDigit,
                                                          FieldMinLength = r.GeneralField.MinLength,
                                                          FieldMaxLength = r.GeneralField.MaxLength,
                                                          FieldMinNumber = r.GeneralField.MinNumber,
                                                          FieldMaxNumber = r.GeneralField.MaxNumber,
                                                          FieldCurrencySymbol = r.GeneralField.CurrencySymbol,
                                                          FieldLOVs = r.GeneralField.LOVs,
                                                          Published = r.Published,
                                                          IsRequired = r.IsRequired,
                                                          Sequence = r.Sequence,
                                                          Editable = r.Editable,
                                                          DefaultValue = r.DefaultValue,
                                                          DefaultValue2 = r.DefaultValue2
                                                      }
                                                 )
                                                 .ToListAsync();

                List<RequestFormControlsViewModel> requestFormsControls = new List<RequestFormControlsViewModel>();

                requestFormsControls = requestForms
                                       .Select
                                       (
                                          r => new RequestFormControlsViewModel()
                                          {
                                              Id = r.FieldId,
                                              OldId = r.OldFieldId,
                                              Name = (r.FieldType == FieldType.DateRange ? r.FieldSlug + "_1" : r.FieldSlug),
                                              Name2 = (r.FieldType == FieldType.DateRange ? r.FieldSlug + "_2" : null),
                                              Label = r.FieldName,
                                              Value = r.DefaultValue,
                                              Value2 = r.DefaultValue2,
                                              Type = r.FieldTypeDescription,
                                              DataType = r.FieldDataType,
                                              Editable = (r.DefaultValue == null ? null : r.Editable),
                                              Enabled = r.Published,
                                              Tooltip = r.FieldDescription,
                                              Lovs = r.FieldLOVs.Length > 0 ? activeLOVs == null ? JsonSerializer.Serialize(this.ParseFieldLOV(requestTypeId, version, r.OldFieldId, r.FieldLOVs, true)) : JsonSerializer.Serialize(this.ParseFieldLOV(requestTypeId, version, r.OldFieldId, r.FieldLOVs, true).Where(l => l.IsActive == activeLOVs)) : r.FieldLOVs,
                                              //Lovs = r.FieldLOVs,
                                              CurrencySymbol = r.FieldCurrencySymbol,
                                              Sequence = r.Sequence,
                                              Validators = requestForms
                                                           .Where(v => v.FieldId == r.FieldId)
                                                           .Select
                                                           (
                                                              v => new RequestFormControlsValidators()
                                                              {
                                                                  Required = (v.IsRequired == 1 ? true : false),
                                                                  NullValidator = !v.Published,
                                                                  Alphanumeric = (v.FieldDataType == "Alphanumeric" ? true : false),
                                                                  Alpha = (v.FieldDataType == "Alphabet" ? true : false),
                                                                  Currency = (v.FieldDataType == "Currency" ? v.FieldDecimalDigit : null),
                                                                  Numeric = (v.FieldDataType == "Numeric" ? v.FieldDecimalDigit : null),
                                                                  Url = (v.FieldType == FieldType.Url ? true : false),
                                                                  Email = (v.FieldType == FieldType.Email ? true : false),
                                                                  MinLength = ((v.FieldDataType == "Alphanumeric" || v.FieldDataType == "Alphabet") ? v.FieldMinLength : null),
                                                                  MaxLength = ((v.FieldDataType == "Alphanumeric" || v.FieldDataType == "Alphabet") ? v.FieldMaxLength : null),
                                                                  MinNumber = ((v.FieldDataType == "Numeric" || v.FieldDataType == "Currency") ? v.FieldMinNumber : null),
                                                                  MaxNumber = ((v.FieldDataType == "Numeric" || v.FieldDataType == "Currency") ? v.FieldMaxNumber : null),
                                                                  Date = (v.FieldType == FieldType.Date ? true : false),
                                                                  DateRange = (v.FieldType == FieldType.DateRange ? true : false)
                                                              }
                                                           ).FirstOrDefault(),
                                              IsMappedToProcess = this.CheckFieldIsInvolvedInProcess(requestTypeId, version, r.OldFieldId, true)
                                          }
                                       )
                                       .OrderBy(r => r.Sequence)
                                       .ToList();

                if (published != null)
                    requestFormsControls = requestFormsControls.Where(r => r.Enabled == published).ToList();

                return requestFormsControls;
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public IQueryable<RequestTypeProcessOwnersViewModel> GetRequestProcessOwners(PagingRequest paging = null, int requestTypeId = 0, int version = 0)
        {
            try
            {
                int rqtype = 0;
                int vrs = 0;

                if (paging == null)
                {
                    if (requestTypeId == 0 || version == 0)
                        throw new CustomException("Request type or version is required", 400);

                    rqtype = requestTypeId;
                    vrs = version;
                }
                else
                {
                    rqtype = paging.SearchCriteria.Id1 ?? 0;
                    vrs = paging.SearchCriteria.Id2 ?? 0;
                }

                var processOwners = _dbCntxt.RequestTypeProcessOwners
                                                  .Where
                                                  (
                                                     p =>
                                                     p.RequestTypeId == rqtype &&
                                                     p.Version == vrs
                                                  ).
                                                  Select
                                                  (
                                                     p => new RequestTypeProcessOwnersViewModel()
                                                     {
                                                         Id = p.Id,
                                                         RequestTypeId = p.RequestTypeId,
                                                         Version = p.Version,
                                                         Owner = p.Owner,
                                                         OwnerFirstName = (p.User.AspNetUsersProfile.FirstName == null ? "" : p.User.AspNetUsersProfile.FirstName),
                                                         OwnerLastName = (p.User.AspNetUsersProfile.LastName == null ? "" : p.User.AspNetUsersProfile.LastName),
                                                         OwnerFullName = (p.User.AspNetUsersProfile.LastName == null ? "" : p.User.AspNetUsersProfile.LastName) + ", " + (p.User.AspNetUsersProfile.FirstName == null ? "" : p.User.AspNetUsersProfile.FirstName),
                                                         OwnerFullName2 = (p.User.AspNetUsersProfile.FirstName == null ? "" : p.User.AspNetUsersProfile.FirstName) + " " + (p.User.AspNetUsersProfile.LastName == null ? "" : p.User.AspNetUsersProfile.LastName),
                                                         OwnerPosition = (p.User.AspNetUsersProfile.Position == null ? "" : p.User.AspNetUsersProfile.Position),
                                                         OwnerDepartment = (p.User.AspNetUsersProfile.Department == null ? "" : p.User.AspNetUsersProfile.Department),
                                                         OwnerEmail = p.User.Email
                                                     }
                                                  )
                                                  .AsQueryable();

                if (paging != null)
                {
                    // default search 
                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        processOwners = processOwners.Where(p => p.OwnerFullName.Contains(search) || p.OwnerDepartment.Contains(search) ||
                                                            p.OwnerPosition.Contains(search));

                    }
                }

                return processOwners;
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task LockRequestType(LockedRequestTypesViewModel model)
        {
            if (model == null)
                throw new CustomException("Kindly provide atleast 1 record to proceed.", 400);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestType == null)
                    throw new CustomException("Request Type ID and Version is not found.", 404);

                var lockedRequestTypeInDb = await this.GetLockedRequestType(model.RequestTypeId, model.Version);

                if (lockedRequestTypeInDb != null)
                {
                    if (lockedRequestTypeInDb.User != cid)
                        throw new CustomException("This request type is currently edited by: " + lockedRequestTypeInDb.UserFullName, 400);
                }
                else
                {
                    LockedRequestType lockedRequestType = new LockedRequestType();
                    lockedRequestType.RequestTypeId = model.RequestTypeId;
                    lockedRequestType.Version = model.Version;
                    lockedRequestType.User = String.IsNullOrWhiteSpace(model.User) ? cid : model.User;
                    lockedRequestType.DateLocked = Convert.ToDateTime(model.DateLocked == null ? DateTime.Now : model.DateLocked);
                    _dbCntxt.LockedRequestTypes.Add(lockedRequestType);
                    await _dbCntxt.SaveChangesAsync();
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteLockedRequestType(int requestTypeId, int version)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var requestType = await this.GetLockedRequestType(requestTypeId, version);

                if (requestType == null)
                    throw new CustomException("Request type is not found in storage of locked request types.", 404);

                _dbCntxt.LockedRequestTypes.RemoveRange(_dbCntxt.LockedRequestTypes.Where(r => r.RequestTypeId == requestTypeId && r.Version == version));
                await _dbCntxt.SaveChangesAsync();
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task ReArrangeSequence(int requestTypeId, int version, int fieldId, int sequence, int currentSequence)
        {
            bool incrementHigher = false;
            int sequenceOfPreviousField = 0;
            bool isMatchSequence = false;

            try
            {
                var requestForm = await _dbCntxt.RequestForms.Where(f => f.RequestTypeId == requestTypeId && f.Version == version).ToListAsync();

                if (requestForm.Count > 0)
                {
                    if (currentSequence > sequence)
                        incrementHigher = true;

                    if (incrementHigher)
                    {
                        foreach (var field in requestForm.Where(f => f.Sequence >= sequence).OrderBy(s => s.Sequence))
                        {
                            if (field.FieldId == fieldId)
                                continue;

                            if (field.Sequence == sequence)
                            {
                                isMatchSequence = true;
                                field.Sequence += 1;
                                sequenceOfPreviousField = field.Sequence;
                                continue;
                            }

                            if (isMatchSequence)
                            {
                                sequenceOfPreviousField++;
                                field.Sequence = sequenceOfPreviousField;
                            }
                        }
                    }
                    else
                    {
                        foreach (var field in requestForm.Where(f => f.Sequence <= sequence).OrderByDescending(s => s.Sequence))
                        {
                            if (field.FieldId == fieldId)
                                continue;

                            if (field.Sequence == sequence)
                            {
                                isMatchSequence = true;
                                field.Sequence -= 1;
                                sequenceOfPreviousField = field.Sequence;
                                continue;
                            }

                            if (isMatchSequence)
                            {
                                sequenceOfPreviousField--;
                                field.Sequence = sequenceOfPreviousField;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task<bool> CheckValidSequence(int requestTypeId, int version, int sequence)
        {
            bool inRange = true;

            try
            {
                var field = await _dbCntxt.RequestForms.Where(r => r.RequestTypeId == requestTypeId && r.Version == version && r.Sequence == sequence).ToListAsync();

                if (field.Count <= 0)
                    inRange = false;

                return inRange;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public PagingResponse<RequestTypeProcessOwnersViewModel> ProcessOwnersPagingFeatureProcessOwners(IQueryable<RequestTypeProcessOwnersViewModel> query, PagingRequest paging)
        {
            try
            {
                // sort order call method from base class
                query = QueryHelper.SortOrder<RequestTypeProcessOwnersViewModel>(query, paging);
                // paginate call method from base class
                return QueryHelper.Paginate<RequestTypeProcessOwnersViewModel>(query, paging);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public IEnumerable<RequestTypeStates> GetRequestTypeStatuses()
        {
            List<RequestTypeStates> states = new List<RequestTypeStates>();

            RequestTypeStates draft = new RequestTypeStates()
            {
                Id = 1,
                Name = "Draft"
            };

            states.Add(draft);

            RequestTypeStates finalized = new RequestTypeStates()
            {
                Id = 2,
                Name = "Finalized"
            };

            states.Add(finalized);

            RequestTypeStates active = new RequestTypeStates()
            {
                Id = 3,
                Name = "Active"
            };

            states.Add(active);

            RequestTypeStates inactive = new RequestTypeStates()
            {
                Id = 4,
                Name = "Inactive"
            };

            states.Add(inactive);

            return states;
        }
        public async Task SwithFieldSequence(FieldSequenceSwitchViewModel model)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var requestTypeForms = await _dbCntxt.RequestForms.Where(r => r.RequestTypeId == model.RequestTypeId && r.Version == model.Version).FirstOrDefaultAsync();

                if (requestTypeForms == null)
                    throw new CustomException("Request type is not found in request forms.", 404);

                //Sequence From
                var fieldSequenceFrom = await _dbCntxt.RequestForms.Where(r => r.Sequence == model.SequenceFrom && r.RequestTypeId == model.RequestTypeId && r.Version == model.Version).FirstOrDefaultAsync();

                if (fieldSequenceFrom != null)
                {
                    fieldSequenceFrom.Sequence = model.SequenceTo;
                    _dbCntxt.Entry(fieldSequenceFrom).State = EntityState.Modified;
                }
                else
                    throw new CustomException("Field for sequence from is not found.", 404);

                //Sequence To
                var fieldSequenceTo = await _dbCntxt.RequestForms.Where(r => r.Sequence == model.SequenceTo && r.RequestTypeId == model.RequestTypeId && r.Version == model.Version).FirstOrDefaultAsync();

                if (fieldSequenceTo != null)
                {
                    fieldSequenceTo.Sequence = model.SequenceFrom;
                    _dbCntxt.Entry(fieldSequenceTo).State = EntityState.Modified;
                }
                else
                    throw new CustomException("Field for sequence to is not found.", 404);

                await _dbCntxt.SaveChangesAsync();

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestTypeInDb != null)
                {
                    // get current user id
                    var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    requestTypeInDb.ModifiedByPK = cid;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public IQueryable<RequestTypeViewModel> GetRequestTypes(PagingRequest paging = null, int status = 0, bool? requestCategoryPublished = null)
        {
            try
            {
                IQueryable<RequestTypeViewModel> query = null;
                query = this.FindAll()
                            .AsQueryable();

                if (status > 0)
                {
                    if (!GeneralHelper.IsValidRequestTypeStatus(status))
                        throw new CustomException("Invalid Request Type Status.", 400);

                    query = query.Where(r => r.Status == (RequestTypeStatus)status);
                }

                if (requestCategoryPublished != null)
                    query = query.Where(r => r.RequestCategoryPublished == requestCategoryPublished);


                if (paging != null)
                {
                    if (paging.SearchCriteria != null)
                    {
                        int requestCategoryId = Convert.ToInt32((paging.SearchCriteria.Id1 == null ? 0 : paging.SearchCriteria.Id1));
                        int requestTypeId = Convert.ToInt32((paging.SearchCriteria.Id2 == null ? 0 : paging.SearchCriteria.Id2));
                        int requestTypeStatus = Convert.ToInt32((paging.SearchCriteria.Id3 == null ? 0 : paging.SearchCriteria.Id3));
                        DateTime? dateFrom = paging.SearchCriteria.Date1;
                        DateTime? dateTo = paging.SearchCriteria.Date2;

                        if (requestCategoryId > 0)
                            query = query.Where(p => p.RequestCategoryId == requestCategoryId);

                        if (requestTypeId > 0)
                            query = query.Where(p => p.Id == requestTypeId);

                        if (requestTypeStatus > 0)
                        {
                            RequestTypeStatus state = (RequestTypeStatus)requestTypeStatus;
                            query = query.Where(p => p.Status == state);
                        }

                        DateTime dfrom = Convert.ToDateTime(dateFrom);
                        DateTime dto = Convert.ToDateTime(dateTo);
                        if (dateFrom != null && dateTo != null)
                        {
                            query = query.Where
                                            (
                                                p =>
                                                (
                                                    p.CreatedDate.Date >= dfrom.Date && p.CreatedDate.Date <= dto.Date
                                                ) ||
                                                (
                                                    (p.ModifiedDate.HasValue && p.ModifiedDate.Value.Date >= dfrom.Date) && (p.ModifiedDate.HasValue && p.ModifiedDate.Value.Date <= dto.Date)
                                                )
                                            );
                        }
                        else if (dateFrom != null)
                            query = query.Where
                                            (
                                                p => p.CreatedDate.Date == dfrom.Date || (p.ModifiedDate.HasValue && p.ModifiedDate.Value.Date == dfrom.Date)
                                            );
                        else if (dateTo != null)
                            query = query.Where
                                            (
                                                p => p.CreatedDate.Date == dto.Date || (p.ModifiedDate.HasValue && p.ModifiedDate.Value.Date == dto.Date)
                                            );
                    }

                    var search = paging.Search.Value.ToLower();

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                        query = query.Where(p => p.RequestCategory.Contains(search) || p.Name.Contains(search) ||
                                                p.StatusDescForSorting.Contains(search));
                }

                return query;
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<LockedRequestTypesViewModel>> GetLockedRequestTypes()
        {
            try
            {
                var locked = await _dbCntxt.LockedRequestTypes
                                            .Include(u => u.UserInfo)
                                            .Include(up => up.UserInfo.AspNetUsersProfile)
                                            .Select
                                            (
                                                r => new LockedRequestTypesViewModel()
                                                {
                                                    RequestTypeId = r.RequestTypeId,
                                                    Version = r.Version,
                                                    User = r.User,
                                                    UserFirstName = r.UserInfo.AspNetUsersProfile.FirstName,
                                                    UserLastName = r.UserInfo.AspNetUsersProfile.LastName,
                                                    UserFullName = r.UserInfo.AspNetUsersProfile.FirstName + " " + r.UserInfo.AspNetUsersProfile.LastName,
                                                    UserEmail = r.UserInfo.Email,
                                                    UserPosition = r.UserInfo.AspNetUsersProfile.Position,
                                                    UserDepartment = r.UserInfo.AspNetUsersProfile.Department,
                                                    DateLocked = r.DateLocked
                                                }
                                            ).ToListAsync();

                return locked;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<LockedRequestTypesViewModel> GetLockedRequestType(int requestTypeId, int version)
        {
            try
            {
                var lockedrequesttypes = await this.GetLockedRequestTypes();

                var locked = lockedrequesttypes
                              .Where
                                (
                                    r => r.RequestTypeId == requestTypeId &&
                                    r.Version == version
                                ).FirstOrDefault();

                return locked;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task SaveRequestStep(SaveRequestStepViewModel model)
        {
            if (model == null)
                throw new CustomException("Kindly provide atleast one step to proceed.", 404);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                var requestType = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestType == null)
                    throw new CustomException("Request Type ID and Version is not found.", 404);

                if (await _dbCntxt.RequestForms.FirstOrDefaultAsync(r => r.RequestTypeId == model.RequestTypeId && r.Version == model.Version) == null)
                    throw new CustomException("Request Form is required.", 400);

                if (model.OldStepId > 0)
                {
                    var stepsInDB = await this.GetRequestSteps(model.RequestTypeId, model.Version, false);

                    var stepInfo = stepsInDB.FirstOrDefault(s => s.StepId == model.OldStepId);

                    var workFlows = await _dbCntxt.RequestTypeWorkFlows
                                            .Where
                                            (
                                                w =>
                                                (
                                                    w.RequestTypeId == model.RequestTypeId &&
                                                    w.Version == model.Version &&
                                                    w.StepId == model.OldStepId &&
                                                    (w.NextStepId.HasValue && w.NextStepId > 0)
                                                ) ||
                                                (
                                                    (w.NextStepRequestTypeId == model.RequestTypeId && w.NextStepRequestTypeId.HasValue) &&
                                                    (w.NextStepVersion == model.Version && w.NextStepVersion.HasValue) &&
                                                    (w.NextStepId == model.OldStepId && w.NextStepId.HasValue)
                                                )
                                            ).ToListAsync();

                    if (workFlows.Count > 0)
                    {
                        if (model.IsConditional != stepInfo.IsConditional)
                            throw new CustomException("Field Condition must not be changed since this step have already successors/predecessors.", 404);
                    }
                }

                if (String.IsNullOrWhiteSpace(model.StepName))
                    throw new CustomException("Step Name is required", 400);

                if (model.StepTypeId <= 0)
                    throw new CustomException("Type of step is required", 400);

                if (model.TAT <= 0)
                    throw new CustomException("Turn Around Time is required", 400);

                if (model.IsConditional && model.Condition == null)
                    throw new CustomException("Step Condition is required", 400);
                //Commented on 10/17/2022
                //Purpose: Do not validate list of conditions if it's not conditional, this is only for unpublished request type, for the user to easily manipulate and test conditions
                //if (model.IsConditional == false && model.Condition != null)
                //    throw new CustomException("Step Condition is not required", 400);

                if (model.IsConditional)
                {
                    var publishedConditions = model.Condition.ConditionDetails.Where(c => c.Published == true).ToList();

                    if (publishedConditions.Count <= 0)
                        throw new CustomException("Atleast one active Step Condition is required. ", 400);
                }

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var requestStep = new RequestStep();

                //Check Validations for assignees and conditions
                if (model.Condition != null)
                {
                    model.Condition.RequestTypeId = model.RequestTypeId;

                    model.Condition.Version = model.Version;

                    var conditionId = model.Condition.Id;

                    //Remove GUI deleted condition details in DB
                    var requestConditionDetails = await _dbCntxt.RequestStepConditionDetails
                                                         .Include(r => r.ConditionHeader)
                                                         .Where
                                                         (
                                                            r =>
                                                            r.ConditionHeader.RequestTypeId == model.RequestTypeId &&
                                                            r.ConditionHeader.Version == model.Version &&
                                                            r.ConditionHeader.StepId == model.OldStepId &&
                                                            r.ConditionHeader.Id != conditionId
                                                         ).ToListAsync();

                    foreach (var details in requestConditionDetails)
                    {
                        _dbCntxt.RequestStepConditionDetails.Remove(details);
                        await _dbCntxt.SaveChangesAsync();
                    }

                    //Remove GUI deleted condition in DB
                    _dbCntxt.RequestStepConditions.RemoveRange(_dbCntxt.RequestStepConditions.Where(r => r.RequestTypeId == model.RequestTypeId && r.Version == model.Version && r.StepId == model.OldStepId && r.Id != conditionId));
                    await _dbCntxt.SaveChangesAsync();

                    //Remove GUI deleted condition details in DB
                    var conditionDetailsIds = model.Condition.ConditionDetails.Select(c => c.Id);

                    requestConditionDetails = await _dbCntxt.RequestStepConditionDetails
                                                        .Where
                                                        (
                                                        r =>
                                                        r.RequestStepConditionId == conditionId &&
                                                        !conditionDetailsIds.Contains(r.Id)
                                                        ).ToListAsync();

                    foreach (var details in requestConditionDetails)
                    {
                        _dbCntxt.RequestStepConditionDetails.Remove(details);
                        await _dbCntxt.SaveChangesAsync();
                    }

                    if (model.IsConditional)
                    {
                        var conditionValidationObject = await this.ValidateStepConditions(model.Condition);

                        if (conditionValidationObject.ReturnMessage != "")
                            throw new CustomException(conditionValidationObject.ReturnMessage, 400, null, conditionValidationObject.DetailedMessages);
                    }
                }

                if (model.Assignees != null)
                {
                    //Assignees
                    if (model.Assignees.Count > 0)
                    {
                        model.Assignees
                             .ForEach(c => c.RequestTypeId = model.RequestTypeId);

                        model.Assignees
                             .ForEach(c => c.Version = model.Version);

                        var assigneesIds = model.Assignees.Select(c => c.Id);

                        //Remove GUI deleted assignee details in DB
                        var requestAssigneeDetails = await _dbCntxt.RequestStepAssigneeDetails
                                                             .Include(r => r.RequestStepAssignee)
                                                             .Where
                                                             (
                                                                r =>
                                                                r.RequestStepAssignee.RequestTypeId == model.RequestTypeId &&
                                                                r.RequestStepAssignee.Version == model.Version &&
                                                                r.RequestStepAssignee.StepId == model.OldStepId &&
                                                                !assigneesIds.Contains(r.RequestStepAssignee.Id)
                                                             ).ToListAsync();

                        foreach (var details in requestAssigneeDetails)
                        {
                            _dbCntxt.RequestStepAssigneeDetails.Remove(details);
                            await _dbCntxt.SaveChangesAsync();
                        }

                        //Remove GUI deleted assignee in DB
                        _dbCntxt.RequestStepAssignees.RemoveRange(_dbCntxt.RequestStepAssignees.Where(r => r.RequestTypeId == model.RequestTypeId && r.Version == model.Version && r.StepId == model.OldStepId && !assigneesIds.Contains(r.Id)));
                        await _dbCntxt.SaveChangesAsync();

                        //Remove GUI deleted assignee details in DB
                        foreach (var assignee in model.Assignees)
                        {
                            var assigneesDetailsIds = assignee.RequestStepAssigneeDetails.Select(c => c.Id);

                            requestAssigneeDetails = await _dbCntxt.RequestStepAssigneeDetails
                                                             .Where
                                                             (
                                                                r =>
                                                                r.RequestStepAssigneeId == assignee.Id &&
                                                                !assigneesDetailsIds.Contains(r.Id)
                                                             ).ToListAsync();

                            foreach (var details in requestAssigneeDetails)
                            {
                                _dbCntxt.RequestStepAssigneeDetails.Remove(details);
                                await _dbCntxt.SaveChangesAsync();
                            }
                        }

                        //Assignees Validations
                        string assigneeValidationMessage = await this.ValidateStepAssignees(model.Assignees);

                        if (assigneeValidationMessage != "")
                            throw new CustomException(assigneeValidationMessage, 400);
                    }
                    else
                        throw new CustomException("Assignees are required.", 400);
                }
                else
                    throw new CustomException("Assignees are required.", 400);

                if (model.WorkFlows != null)
                {
                    if (model.WorkFlows.Count > 0)
                    {
                        model.WorkFlows
                            .ForEach(w => w.RequestTypeId = model.RequestTypeId);

                        model.WorkFlows
                             .ForEach(w => w.Version = model.Version);

                        var workflowsIds = model.WorkFlows.Select(c => c.Id);

                        //Remove GUI deleted parallels in DB
                        var requestWorkflowParallel = await _dbCntxt.RequestTypeWorkFlowParallels
                                                             .Include(r => r.WorkFlow)
                                                             .Where
                                                             (
                                                                r =>
                                                                r.WorkFlow.RequestTypeId == model.RequestTypeId &&
                                                                r.WorkFlow.Version == model.Version &&
                                                                r.WorkFlow.StepId == model.OldStepId &&
                                                                !workflowsIds.Contains(r.WorkFlow.Id)
                                                             ).ToListAsync();

                        foreach (var details in requestWorkflowParallel)
                        {
                            _dbCntxt.RequestTypeWorkFlowParallels.Remove(details);
                            await _dbCntxt.SaveChangesAsync();
                        }

                        //Remove GUI deleted Workflows in DB
                        _dbCntxt.RequestTypeWorkFlows.RemoveRange(_dbCntxt.RequestTypeWorkFlows.Where(r => r.RequestTypeId == model.RequestTypeId && r.Version == model.Version && r.StepId == model.OldStepId && !workflowsIds.Contains(r.Id)));
                        await _dbCntxt.SaveChangesAsync();

                        var workflowValidationObject = await this.ValidateWorkFlows(model.WorkFlows);

                        if (workflowValidationObject.ReturnMessage != "")
                            throw new CustomException(workflowValidationObject.ReturnMessage, 400, null, workflowValidationObject.DetailedMessages);
                    }
                }
                //End of Check Validations for assignees and conditions

                if (model.OldStepId <= 0)
                {
                    //get Max Sequence By Request Type and version
                    var stepSequence = await _dbCntxt.RequestSteps.Where(r => r.RequestTypeId == model.RequestTypeId && r.Version == model.Version).ToListAsync();

                    int sequence = 0;

                    if (stepSequence.Count > 0)
                        sequence = stepSequence.Max(r => r.Sequence) + 1;
                    else
                        sequence = 1;

                    requestStep.RequestTypeId = model.RequestTypeId;
                    requestStep.Version = model.Version;

                    if (model.StepId == 0)
                    {
                        var stepType = await this.GetStepTypeById(model.StepTypeId);
                        string stepTypeShortText = GeneralHelper.GetStepTypeShorText(stepType.Name);
                        string concatDateTime = DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "") + DateTime.Now.ToString("hh:mm:ss:fff").Replace(":", "");

                        string zeros = "00000";
                        int requestTypeIdLength = model.RequestTypeId.ToString().Length;
                        string requestTypeSlug = "";

                        if (zeros.Length == requestTypeIdLength)
                            requestTypeSlug = model.RequestTypeId.ToString();
                        else
                            requestTypeSlug = zeros.Substring(0, (zeros.Length - requestTypeIdLength)) + model.RequestTypeId.ToString();

                        string slug = stepTypeShortText + "_" + requestTypeSlug + "_" + concatDateTime;

                        var workFlowStep = new WorkFlowStep();
                        workFlowStep.Slug = slug;
                        workFlowStep.Name = model.StepName;
                        workFlowStep.Description = model.StepDescription;
                        workFlowStep.StepTypeId = model.StepTypeId;

                        _dbCntxt.WorkFlowSteps.Add(workFlowStep);
                        _dbCntxt.SaveChanges();

                        requestStep.StepId = workFlowStep.Id;
                    }
                    else
                        requestStep.StepId = model.StepId;

                    requestStep.IsConditional = model.IsConditional;
                    requestStep.IsScheduled = model.IsScheduled;
                    requestStep.TAT = model.TAT;
                    requestStep.EnableEmail = model.EnableEmail;
                    requestStep.EnableSMS = model.EnableSMS;
                    requestStep.Sequence = sequence;

                    _dbCntxt.RequestSteps.Add(requestStep);
                }
                else
                {
                    //updating of step
                    requestStep = _dbCntxt.RequestSteps
                                            .Where
                                            (
                                                r => r.RequestTypeId == model.RequestTypeId &&
                                                r.Version == model.Version &&
                                                r.StepId == model.OldStepId
                                            ).FirstOrDefault();

                    if (requestStep != null)
                    {
                        var workFlowStep = new WorkFlowStep();

                        if (model.StepId == 0)
                        {
                            var stepType = await this.GetStepTypeById(model.StepTypeId);
                            string stepTypeShortText = GeneralHelper.GetStepTypeShorText(stepType.Name);
                            string concatDateTime = DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "") + DateTime.Now.ToString("hh:mm:ss:fff").Replace(":", "");

                            string zeros = "00000";
                            int requestTypeIdLength = model.RequestTypeId.ToString().Length;
                            string requestTypeSlug = "";

                            if (zeros.Length == requestTypeIdLength)
                                requestTypeSlug = model.RequestTypeId.ToString();
                            else
                                requestTypeSlug = zeros.Substring(0, (zeros.Length - requestTypeIdLength)) + model.RequestTypeId.ToString();

                            string slug = stepTypeShortText + "_" + requestTypeSlug + "_" + concatDateTime;

                            workFlowStep.Slug = slug;
                            workFlowStep.Name = model.StepName;
                            workFlowStep.Description = model.StepDescription;
                            workFlowStep.StepTypeId = model.StepTypeId;

                            _dbCntxt.WorkFlowSteps.Add(workFlowStep);
                            _dbCntxt.SaveChanges();

                            requestStep.StepId = workFlowStep.Id;
                        }
                        else
                        {
                            if (model.OldStepId == model.StepId)
                            {
                                bool needToReconstructConditionText = false;

                                workFlowStep = _dbCntxt.WorkFlowSteps.FirstOrDefault(s => s.Id == model.StepId);

                                if (workFlowStep != null)
                                {
                                    var sourceStep = _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Step Description").FirstOrDefault();

                                    if (workFlowStep.StepTypeId != model.StepTypeId)
                                    {
                                        //if fieldy is changed
                                        //Change slug
                                        var stepType = await this.GetStepTypeById(model.StepTypeId);
                                        string stepTypeShortText = GeneralHelper.GetStepTypeShorText(stepType.Name);
                                        workFlowStep.Slug = stepTypeShortText + workFlowStep.Slug.Substring(3, workFlowStep.Slug.Length - 3);
                                    }

                                    if (this.IsStepInStepCondition(model.RequestTypeId, model.Version, model.OldStepId, true, true))
                                    {
                                        if (workFlowStep.Name.Trim().ToUpper() != model.StepName.Trim().ToUpper())
                                            needToReconstructConditionText = true;
                                    }

                                    workFlowStep.Name = model.StepName;
                                    workFlowStep.Description = model.StepDescription;
                                    workFlowStep.StepTypeId = model.StepTypeId;
                                    _dbCntxt.Entry(workFlowStep).State = EntityState.Modified;
                                    _dbCntxt.SaveChanges();

                                    if (needToReconstructConditionText)
                                    {
                                        //code here
                                        var affectedRecord = await _dbCntxt.RequestStepConditionDetails
                                                                     .Include(r => r.ConditionHeader)
                                                                    .Where
                                                                    (
                                                                        r =>
                                                                        r.ConditionHeader.RequestTypeId == model.RequestTypeId &&
                                                                        r.ConditionHeader.Version == model.Version &&
                                                                        r.Published == true &&
                                                                        r.Source == sourceStep.Id &&
                                                                        r.ReferenceDetail == model.OldStepId
                                                                    )
                                                                    .Select(r => r.RequestStepConditionId)
                                                                    .Distinct()
                                                                    .ToListAsync();

                                        if (affectedRecord.Count > 0)
                                        {
                                            foreach (int id in affectedRecord)
                                            {
                                                var affectedStepConditions = await _dbCntxt.RequestStepConditionDetails
                                                                             .Where(r => r.RequestStepConditionId == id && r.Published == true)
                                                                             .ToListAsync();

                                                if (affectedStepConditions.Count > 0)
                                                {
                                                    //Build condition header
                                                    var constructedConditions = await this.ConstructStepConditions(id, affectedStepConditions);

                                                    if (constructedConditions.Count > 0)
                                                    {
                                                        //Update condition header
                                                        var conditionHeader = await _dbCntxt.RequestStepConditions
                                                                                      .FirstOrDefaultAsync(r => r.Id == id);

                                                        if (conditionHeader != null)
                                                        {
                                                            conditionHeader.Condition = constructedConditions[0];
                                                            conditionHeader.ConditionInText = constructedConditions[1];
                                                            _dbCntxt.Entry(conditionHeader).State = EntityState.Modified;
                                                            await _dbCntxt.SaveChangesAsync();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            requestStep.StepId = model.StepId;
                        }

                        requestStep.IsConditional = model.IsConditional;
                        requestStep.IsScheduled = model.IsScheduled;
                        requestStep.TAT = model.TAT;
                        requestStep.EnableEmail = model.EnableEmail;
                        requestStep.EnableSMS = model.EnableSMS;
                    }
                }

                await _dbCntxt.SaveChangesAsync();
                //Conditional Steps
                bool withConditions = false;
                if (model.Condition != null)
                {
                    withConditions = true;

                    model.Condition.StepId = requestStep.StepId;
                    //.ForEach(c => c.StepId = requestStep.StepId);
                    await this.SaveStepConditions(model.Condition);
                }

                if (!withConditions)
                {
                    //delete all conditions from DB that once current model is empty
                    string sqlQuery = @"Declare @RequestTypeId Int, @Version Int, @StepId Int
                                        Set @RequestTypeId = " + model.RequestTypeId + @"
                                        Set @Version = " + model.Version + @"
                                        Set @StepId = " + requestStep.StepId + @"

                                        Select Id into #TempConditions from dbo.RequestStepConditions 
                                        Where 
	                                        RequestTypeId = @RequestTypeId And 
	                                        [Version] = @Version And 
	                                        StepId = @StepId

                                        Delete from dbo.RequestStepConditionDetails Where RequestStepConditionId in (Select Id from #TempConditions)
                                        Delete from dbo.RequestStepConditions Where Id in (Select Id from #TempConditions)

                                        Drop Table #TempConditions";

                    _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                    await _dbCntxt.SaveChangesAsync();
                }

                bool withAssignees = false;
                if (model.Assignees != null)
                {
                    //Assignees
                    if (model.Assignees.Count > 0)
                    {
                        withAssignees = true;

                        model.Assignees
                            .ForEach(c => c.StepId = requestStep.StepId);

                        await this.SaveStepAssignees(model.Assignees);
                    }
                }

                if (!withAssignees)
                {
                    //delete all assignee from DB that once current model is empty
                    string sqlQuery = @"Declare @RequestTypeId Int, @Version Int, @StepId Int
                                        Set @RequestTypeId = " + model.RequestTypeId + @"
                                        Set @Version = " + model.Version + @"
                                        Set @StepId = " + requestStep.StepId + @"

                                        Select Id into #TempAssignees from dbo.RequestStepAssignees 
                                        Where 
	                                        RequestTypeId = @RequestTypeId And 
	                                        [Version] = @Version And 
	                                        StepId = @StepId

                                        Delete from dbo.RequestStepAssigneeDetails Where RequestStepAssigneeId in (Select Id from #TempAssignees)
                                        Delete from dbo.RequestStepAssignees Where Id in (Select Id from #TempAssignees)

                                        Drop Table #TempAssignees";

                    _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                    await _dbCntxt.SaveChangesAsync();
                }
                //Insert WorkFlow if with data during saving of step
                bool withWorkFlows = false;
                if (model.WorkFlows != null)
                {
                    if (model.WorkFlows.Count > 0)
                    {
                        withWorkFlows = true;

                        model.WorkFlows
                             .ForEach(c => c.StepId = requestStep.StepId);

                        await this.SaveWorkFlows(model.WorkFlows);
                    }
                }

                if (!withWorkFlows)
                {
                    //delete all workflow from DB that once current model is empty
                    string sqlQuery = @"Declare @RequestTypeId Int, @Version Int, @StepId Int
                                        Set @RequestTypeId = " + model.RequestTypeId + @"
                                        Set @Version = " + model.Version + @"
                                        Set @StepId = " + requestStep.StepId + @"

                                        Select Id into #TempWorkFlows from dbo.RequestTypeWorkFlows 
                                        Where 
	                                        RequestTypeId = @RequestTypeId And 
	                                        [Version] = @Version And 
	                                        StepId = @StepId

                                        Delete from dbo.RequestTypeWorkFlowParallels Where RequestTypeWorkFlowId in (Select Id from #TempWorkFlows)
                                        Delete from dbo.RequestTypeWorkFlows Where Id in (Select Id from #TempWorkFlows)

                                        Drop Table #TempWorkFlows";

                    _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                    await _dbCntxt.SaveChangesAsync();
                }
                //Update modified date in RequestTypes table
                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == model.RequestTypeId && r.Version == model.Version);

                if (requestTypeInDb != null)
                {
                    requestTypeInDb.ModifiedByPK = cid;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);
                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details, customex.ListMessage);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task<WorkFlowStepTypeViewModel> GetStepTypeById(int stepId)
        {
            WorkFlowStepTypeViewModel stepType = new WorkFlowStepTypeViewModel();

            try
            {
                var stepTypes = await this.GetWorkFlowStepTypes();

                stepType = stepTypes.FirstOrDefault(s => s.Id == stepId);

                if (stepType == null)
                    throw new CustomException("Invalid value for step type id", 404);
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return stepType;
        }
        private bool CheckIfDuplicateConditionInModel(RequestStepConditionViewModel model, List<RequestStepConditionViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where(c => c.Condition == model.Condition).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateConditionWithinStepInDatabase(RequestStepConditionDetailViewModel model)
        {
            bool returnValue = false;

            List<RequestStepConditionDetail> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = _dbCntxt.RequestStepConditionDetails
                                         .Where
                                         (
                                             c => c.RequestStepConditionId == model.RequestStepConditionId &&
                                             c.Source == model.Source &&
                                             c.ReferenceDetail == model.ReferenceDetail &&
                                             c.ComparisonOperatorId == model.ComparisonOperatorId &&
                                             c.Value == model.Value &&
                                             c.Value2 == model.Value2 &&
                                             c.Id != model.Id
                                         ).ToList();
            }
            else
            {
                checkDuplicate = _dbCntxt.RequestStepConditionDetails
                                         .Where
                                         (
                                             c => c.RequestStepConditionId == model.RequestStepConditionId &&
                                             c.Source == model.Source &&
                                             c.ReferenceDetail == model.ReferenceDetail &&
                                             c.ComparisonOperatorId == model.ComparisonOperatorId &&
                                             c.Value == model.Value &&
                                             c.Value2 == model.Value2
                                         ).ToList();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateConditionWithinStepInModel(RequestStepConditionDetailViewModel model, List<RequestStepConditionDetailViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel.Where
                                            (
                                                c => c.RequestStepConditionId == model.RequestStepConditionId &&
                                                c.Source == model.Source &&
                                                c.ReferenceDetail == model.ReferenceDetail &&
                                                c.ComparisonOperatorId == model.ComparisonOperatorId &&
                                                c.Value == model.Value &&
                                                c.Value2 == model.Value2
                                            ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private List<RequestStepCondition> CheckIfDuplicateConditionInOtherStepInDatabase(RequestStepConditionViewModel model)
        {
            List<RequestStepCondition> returnValue = new List<RequestStepCondition>();

            var conditionalSteps = _dbCntxt.RequestSteps
                                              .Where
                                              (
                                                  s =>
                                                  s.RequestTypeId == model.RequestTypeId &&
                                                  s.Version == model.Version &&
                                                  s.IsConditional == true
                                              )
                                              .Select
                                              (
                                                  s => s.StepId
                                              ).ToList();

            List<RequestStepCondition> checkDuplicate = new();

            checkDuplicate = this.GetStepConditions(model.RequestTypeId, model.Version)
                                 .Where
                                (
                                    c =>
                                    c.Condition == model.Condition &&
                                    c.StepId != model.StepId &&
                                    conditionalSteps.Contains(c.StepId)
                                ).ToList();

            returnValue = checkDuplicate;

            return returnValue;
        }
        private List<RequestStepCondition> GetStepConditions(int requestTypeId, int version)
        {
            List<RequestStepCondition> checkDuplicate = new();

            checkDuplicate = _dbCntxt.RequestStepConditions
                                    .Where
                                    (
                                        c => c.RequestTypeId == requestTypeId &&
                                        c.Version == version
                                    ).ToList();

            return checkDuplicate;
        }
        private async Task<List<RequestStepConditionViewModel>> GetStepConditionsAsync(int requestTypeId, int version)
        {
            List<RequestStepConditionViewModel> stepConditions = new();

            stepConditions = await _dbCntxt.RequestStepConditions
                                    .Where
                                    (
                                        c => c.RequestTypeId == requestTypeId &&
                                        c.Version == version
                                    )
                                    .Select
                                    (
                                        s => new RequestStepConditionViewModel
                                        {
                                            Id = s.Id,
                                            RequestTypeId = s.RequestTypeId,
                                            Version = s.Version,
                                            StepId = s.StepId,
                                            Condition = s.Condition,
                                            ConditionInText = s.ConditionInText
                                        }
                                    )
                                    .ToListAsync();

            return stepConditions;
        }
        private bool CheckIfDuplicateAssigneeConditionInModel(RequestStepAssigneeViewModel model, List<RequestStepAssigneeViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel
                                 .Where
                                 (
                                    c =>
                                    c.Field1 == model.Field1 &&
                                    c.Value1 == model.Value1 &&
                                    c.Field2 == model.Field2 &&
                                    c.Value2 == model.Value2 &&
                                    c.Field3 == model.Field3 &&
                                    c.Value3 == model.Value3 &&
                                    c.IsConditional == true
                                 ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateAssigneeConditionInDatabase(RequestStepAssigneeViewModel model)
        {
            bool returnValue = false;

            List<RequestStepAssignee> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = _dbCntxt.RequestStepAssignees
                                         .Where
                                         (
                                            c =>
                                            c.Field1 == model.Field1 &&
                                            c.Value1 == model.Value1 &&
                                            c.Field2 == model.Field2 &&
                                            c.Value2 == model.Value2 &&
                                            c.Field3 == model.Field3 &&
                                            c.Value3 == model.Value3 &&
                                            c.IsConditional == true &&
                                            c.RequestTypeId == model.RequestTypeId &&
                                            c.Version == model.Version &&
                                            c.StepId == model.StepId &&
                                            c.Id != model.Id
                                         ).ToList();
            }
            else
            {
                checkDuplicate = _dbCntxt.RequestStepAssignees
                                         .Where
                                         (
                                            c =>
                                            c.Field1 == model.Field1 &&
                                            c.Value1 == model.Value1 &&
                                            c.Field2 == model.Field2 &&
                                            c.Value2 == model.Value2 &&
                                            c.Field3 == model.Field3 &&
                                            c.Value3 == model.Value3 &&
                                            c.IsConditional == true &&
                                            c.RequestTypeId == model.RequestTypeId &&
                                            c.Version == model.Version &&
                                            c.StepId == model.StepId
                                         ).ToList();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateAssigneesInModel(RequestStepAssigneeDetailViewModel model, List<RequestStepAssigneeDetailViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel
                                 .Where
                                 (
                                    c =>
                                    c.Assignee == model.Assignee
                                 ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateAssigneesInDatabase(RequestStepAssigneeDetailViewModel model)
        {
            bool returnValue = false;

            List<RequestStepAssigneeDetail> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = _dbCntxt.RequestStepAssigneeDetails
                                         .Where
                                         (
                                            c =>
                                            c.Assignee == model.Assignee &&
                                            c.RequestStepAssigneeId == model.RequestStepAssigneeId &&
                                            c.Id != model.Id
                                         ).ToList();
            }
            else
            {
                checkDuplicate = _dbCntxt.RequestStepAssigneeDetails
                                         .Where
                                         (
                                            c =>
                                            c.Assignee == model.Assignee &&
                                            c.RequestStepAssigneeId == model.RequestStepAssigneeId
                                         ).ToList();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateWorkFlowInModel(RequestTypeWorkFlowViewModel model, List<RequestTypeWorkFlowViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel
                                 .Where
                                 (
                                    c =>
                                    c.RequestTypeId == model.RequestTypeId &&
                                    c.Version == model.Version &&
                                    c.StepId == model.StepId &&
                                    c.NextStepId == model.NextStepId &&
                                    c.NextStepRequestTypeId == model.NextStepRequestTypeId &&
                                    c.NextStepVersion == model.NextStepVersion &&
                                    c.Action == model.Action &&
                                    c.TypeWhenRejected == model.TypeWhenRejected
                                 ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateWorkFlowInDatabase(RequestTypeWorkFlowViewModel model)
        {
            bool returnValue = false;

            List<RequestTypeWorkFlow> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = _dbCntxt.RequestTypeWorkFlows
                                 .Where
                                 (
                                    c =>
                                    c.RequestTypeId == model.RequestTypeId &&
                                    c.Version == model.Version &&
                                    c.StepId == model.StepId &&
                                    c.NextStepId == model.NextStepId &&
                                    c.NextStepRequestTypeId == model.NextStepRequestTypeId &&
                                    c.NextStepVersion == model.NextStepVersion &&
                                    c.Action == model.Action &&
                                    c.TypeWhenRejected == (model.TypeWhenRejected != null ? Convert.ToInt32(model.TypeWhenRejected) : null) &&
                                    c.Id != model.Id
                                 ).ToList();
            }
            else
            {
                checkDuplicate = _dbCntxt.RequestTypeWorkFlows
                                 .Where
                                 (
                                    c =>
                                    c.RequestTypeId == model.RequestTypeId &&
                                    c.Version == model.Version &&
                                    c.StepId == model.StepId &&
                                    c.NextStepId == model.NextStepId &&
                                    c.NextStepRequestTypeId == model.NextStepRequestTypeId &&
                                    c.NextStepVersion == model.NextStepVersion &&
                                    c.Action == model.Action &&
                                    c.TypeWhenRejected == (model.TypeWhenRejected != null ? Convert.ToInt32(model.TypeWhenRejected) : null)
                                 ).ToList();
            }

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        public async Task<IEnumerable<RequestStepViewModel>> GetRequestSteps(int requestTypeId, int version, bool withDetails = true)
        {
            List<RequestStepViewModel> requestSteps = new List<RequestStepViewModel>();
            try
            {
                if (withDetails == true)
                {
                    requestSteps = await _dbCntxt.RequestSteps
                                             .Where(s => s.RequestTypeId == requestTypeId && s.Version == version)
                                             .Select
                                             (
                                                s => new RequestStepViewModel()
                                                {
                                                    RequestTypeId = s.RequestTypeId,
                                                    Version = s.Version,
                                                    StepId = s.StepId,
                                                    OldStepId = s.StepId,
                                                    StepSlug = s.WorkFlowStep.Slug,
                                                    StepName = s.WorkFlowStep.Name,
                                                    StepDescription = s.WorkFlowStep.Description,
                                                    StepTypeId = s.WorkFlowStep.StepTypeId,
                                                    StepTypeName = s.WorkFlowStep.StepType.Name,
                                                    StepTypeColor = s.WorkFlowStep.StepType.Color,
                                                    StepApplyApprovalPolicyForAssignees = s.WorkFlowStep.StepType.ApplyApprovalPolicyForAssignees,
                                                    StepsTypeButtons = _dbCntxt.WorkFlowStepTypeButtons
                                                                        .Where
                                                                        (
                                                                            b =>
                                                                            b.StepTypeId == s.WorkFlowStep.StepTypeId &&
                                                                            b.ActionButton.OptionGroup == "WorkFlow Step Actions"
                                                                        )
                                                                        .Select
                                                                        (
                                                                            b => new WorkFlowStepTypeButtonViewModel()
                                                                            {
                                                                                Id = b.Id,
                                                                                StepTypeId = b.StepTypeId,
                                                                                ActionButtonId = b.ActionButtonId,
                                                                                IsActionToNextStep = b.IsActionToNextStep,
                                                                                ActionButton = b.ActionButton.Name,
                                                                                ActionButtonDescription = b.ActionButton.Description,
                                                                                WithActionInWorkflow = (b.ActionButton.Name.ToUpper() == "APPROVE" || b.ActionButton.Name.ToUpper() == "DONE" || b.ActionButton.Name.ToUpper() == "ENDORSE" || b.ActionButton.Name.ToUpper() == "REJECT") ? true : false
                                                                            }
                                                                        )
                                                                        .ToList(),
                                                    IsConditional = s.IsConditional,
                                                    IsScheduled = s.IsScheduled,
                                                    TAT = s.TAT,
                                                    EnableEmail = s.EnableEmail,
                                                    EnableSMS = s.EnableSMS,
                                                    Sequence = s.Sequence,
                                                    Id = s.StepId,
                                                    Name = s.WorkFlowStep.Name,
                                                    Assignees = _dbCntxt.RequestStepAssignees
                                                                .Where
                                                                (
                                                                    a => a.RequestTypeId == s.RequestTypeId &&
                                                                    a.Version == s.Version &&
                                                                    a.StepId == s.StepId
                                                                )
                                                                .Select
                                                                (
                                                                    a => new RequestStepAssigneeViewModel()
                                                                    {
                                                                        Id = a.Id,
                                                                        RequestTypeId = a.RequestTypeId,
                                                                        Version = a.Version,
                                                                        StepId = a.StepId,
                                                                        IsConditional = a.IsConditional,
                                                                        AssigneeType = (AssigneeType)a.AssigneeType,
                                                                        RequiredAssigneeToExecute = a.RequiredAssigneeToExecute,
                                                                        Field1 = a.Field1,
                                                                        FieldName1 = (a.Field1 == null) ? null : _dbCntxt.GeneralFields.Where(f => f.Id == a.Field1).FirstOrDefault().Name,
                                                                        Value1 = a.Value1,
                                                                        Field2 = a.Field2,
                                                                        FieldName2 = (a.Field2 == null) ? null : _dbCntxt.GeneralFields.Where(f => f.Id == a.Field2).FirstOrDefault().Name,
                                                                        Value2 = a.Value2,
                                                                        Field3 = a.Field3,
                                                                        FieldName3 = (a.Field3 == null) ? null : _dbCntxt.GeneralFields.Where(f => f.Id == a.Field3).FirstOrDefault().Name,
                                                                        Value3 = a.Value3,
                                                                        RequestStepAssigneeDetails = _dbCntxt.RequestStepAssigneeDetails
                                                                                                     .Where
                                                                                                     (
                                                                                                        d =>
                                                                                                        d.RequestStepAssigneeId == a.Id
                                                                                                     )
                                                                                                     .Select
                                                                                                     (
                                                                                                        d => new RequestStepAssigneeDetailViewModel()
                                                                                                        {
                                                                                                            RequestStepAssigneeDetailId = d.Id,
                                                                                                            Id = d.Id,
                                                                                                            RequestStepAssigneeId = d.RequestStepAssigneeId,
                                                                                                            Assignee = d.Assignee,
                                                                                                            AssigneeFirstName = (a.AssigneeType == 1 ? (d.Assignee == "requestor" ? null : _dbCntxt.AspNetUsers
                                                 .FirstOrDefault(u => u.Id == d.Assignee)
                                                 .AspNetUsersProfile.FirstName) : null),
                                                                                                            AssigneeLastName = (a.AssigneeType == 1 ? (d.Assignee == "requestor" ? "Requestor" : _dbCntxt.AspNetUsers
                                                 .FirstOrDefault(u => u.Id == d.Assignee)
                                                 .AspNetUsersProfile.LastName) : _dbCntxt.ApprovalLevels.FirstOrDefault(a => a.Id == Convert.ToInt32(d.Assignee)).Name),
                                                                                                        }

                                                                                                     )
                                                                                                     .ToList()
                                                                    }
                                                                ).ToList(),
                                                    Condition = _dbCntxt.RequestStepConditions
                                                                 .Where
                                                                 (
                                                                    c => c.RequestTypeId == s.RequestTypeId &&
                                                                    c.Version == s.Version &&
                                                                    c.StepId == s.StepId
                                                                 )
                                                                 .Select
                                                                 (
                                                                    c => new RequestStepConditionViewModel()
                                                                    {
                                                                        Id = c.Id,
                                                                        RequestTypeId = c.RequestTypeId,
                                                                        Version = c.Version,
                                                                        StepId = c.StepId,
                                                                        Condition = c.Condition,
                                                                        ConditionInText = c.ConditionInText,
                                                                        ConditionDetails = _dbCntxt.RequestStepConditionDetails
                                                                                           .Where
                                                                                           (
                                                                                              d =>
                                                                                              d.RequestStepConditionId == c.Id
                                                                                           )
                                                                                           .Select
                                                                                           (
                                                                                              d => new RequestStepConditionDetailViewModel()
                                                                                              {
                                                                                                  Id = d.Id,
                                                                                                  RequestStepConditionId = d.RequestStepConditionId,
                                                                                                  Source = d.Source,
                                                                                                  SourceName = d.SourceInfo.Name,
                                                                                                  ReferenceDetail = d.ReferenceDetail,
                                                                                                  ReferenceDetailDescription = (d.Source == 1 ?
                                                                                                                                 _dbCntxt.GeneralFields.FirstOrDefault(f => f.Id == d.ReferenceDetail).Name : _dbCntxt.WorkFlowSteps.FirstOrDefault(s => s.Id == d.ReferenceDetail).Name),
                                                                                                  ComparisonOperatorId = d.ComparisonOperatorId,
                                                                                                  ComparisonOperatorName = d.ComparisonOperator.Name,
                                                                                                  LogicalOperator = d.LogicalOperator,
                                                                                                  Value = d.Value,
                                                                                                  Value2 = d.Value2,
                                                                                                  Value3 = String.IsNullOrWhiteSpace(d.Value2) ? null : d.Value + " - " + d.Value2,
                                                                                                  Sequence = d.Sequence,
                                                                                                  Published = d.Published
                                                                                              }
                                                                                           ).ToList()
                                                                    }
                                                                 ).FirstOrDefault(),
                                                    WorkFlows = _dbCntxt.RequestTypeWorkFlows
                                                                        .Where
                                                                        (
                                                                            w => w.RequestTypeId == s.RequestTypeId &&
                                                                            w.Version == s.Version &&
                                                                            w.StepId == s.StepId
                                                                        )
                                                                        .Select
                                                                        (
                                                                            w => new RequestTypeWorkFlowViewModel()
                                                                            {
                                                                                Id = w.Id,
                                                                                RequestTypeId = w.RequestTypeId,
                                                                                Version = w.Version,
                                                                                StepId = w.StepId,
                                                                                StepName = w.RequestStep.WorkFlowStep.Name,
                                                                                NextStepRequestTypeId = w.NextStepRequestTypeId,
                                                                                NextStepVersion = w.NextStepVersion,
                                                                                NextStepId = w.NextStepId,
                                                                                NextStepName = w.NextRequestStep.WorkFlowStep.Name,
                                                                                Name = (w.NextStepId == null) ? null : w.NextRequestStep.Sequence.ToString() + " - " + w.NextRequestStep.WorkFlowStep.Name,
                                                                                Action = w.Action,
                                                                                ActionName = _dbCntxt.WorkFlowStepTypeButtons
                                                                                                     .Include(b => b.ActionButton)
                                                                                                     .Where(b => b.Id == w.Action && b.ActionButton.OptionGroup == "WorkFlow Step Actions")
                                                                                                    .FirstOrDefault().ActionButton.Name,
                                                                                TypeWhenRejected = w.TypeWhenRejected == null ? null : (TypeWhenRejected)w.TypeWhenRejected
                                                                            }
                                                                        ).ToList(),
                                                    InvolvedInWorkFlows = _dbCntxt.RequestTypeWorkFlows
                                                                                  .Where
                                                                                  (
                                                                                        w =>
                                                                                        (
                                                                                            w.RequestTypeId == s.RequestTypeId &&
                                                                                            w.Version == s.Version &&
                                                                                            w.StepId == s.StepId &&
                                                                                            (w.NextStepId.HasValue && w.NextStepId > 0)
                                                                                        ) ||
                                                                                        (
                                                                                            (w.NextStepRequestTypeId == s.RequestTypeId && w.NextStepRequestTypeId.HasValue) &&
                                                                                            (w.NextStepVersion == s.Version && w.NextStepVersion.HasValue) &&
                                                                                            (w.NextStepId == s.StepId && w.NextStepId.HasValue)
                                                                                        )
                                                                                  ).ToList().Count > 0 ? true : false
                                                }
                                             )
                                             .ToListAsync();

                    requestSteps = requestSteps
                                   .Select
                                   (
                                        s => new RequestStepViewModel()
                                        {
                                            RequestTypeId = s.RequestTypeId,
                                            Version = s.Version,
                                            StepId = s.StepId,
                                            OldStepId = s.StepId,
                                            StepSlug = s.StepSlug,
                                            StepName = s.StepName,
                                            StepDescription = s.StepDescription,
                                            StepTypeId = s.StepTypeId,
                                            StepTypeName = s.StepTypeName,
                                            StepTypeColor = s.StepTypeColor,
                                            StepApplyApprovalPolicyForAssignees = s.StepApplyApprovalPolicyForAssignees,
                                            StepsTypeButtons = s.StepsTypeButtons,
                                            IsConditional = s.IsConditional,
                                            IsScheduled = s.IsScheduled,
                                            TAT = s.TAT,
                                            EnableEmail = s.EnableEmail,
                                            EnableSMS = s.EnableSMS,
                                            Sequence = s.Sequence,
                                            Id = s.Id,
                                            Name = s.Name,
                                            Assignees = s.Assignees,
                                            Condition = s.Condition,
                                            WorkFlows = s.WorkFlows,
                                            InvolvedInWorkFlows = s.InvolvedInWorkFlows,
                                            IsMapToConditions = this.IsStepInStepCondition(s.RequestTypeId, s.Version, s.StepId, false, true)
                                        }
                                   ).ToList();

                }
                else
                {
                    requestSteps = await _dbCntxt.RequestSteps
                                            .Where(s => s.RequestTypeId == requestTypeId && s.Version == version)
                                            .Select
                                            (
                                               s => new RequestStepViewModel()
                                               {
                                                   OldStepId = s.StepId,
                                                   StepSlug = s.WorkFlowStep.Slug,
                                                   StepId = s.StepId,
                                                   StepName = s.WorkFlowStep.Name,
                                                   StepDescription = s.WorkFlowStep.Description,
                                                   StepTypeId = s.WorkFlowStep.StepTypeId,
                                                   StepTypeName = s.WorkFlowStep.StepType.Name,
                                                   StepTypeColor = s.WorkFlowStep.StepType.Color,
                                                   StepApplyApprovalPolicyForAssignees = s.WorkFlowStep.StepType.ApplyApprovalPolicyForAssignees,
                                                   Sequence = s.Sequence,
                                                   IsConditional = s.IsConditional,
                                                   Id = s.StepId,
                                                   Name = s.WorkFlowStep.Name
                                               }
                                            ).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return requestSteps;
        }
        public async Task<string> ValidateFieldForCondition(int fieldId, string value, string value2)
        {
            string returnMessage = "";
            try
            {
                var fields = await this.GetFields();

                var field = fields.FirstOrDefault(f => f.FieldId == fieldId);

                if (field != null)
                {
                    bool isLov = false;

                    if (field.FieldLOVs.Length > 0)
                        isLov = true;

                    if (!isLov)
                    {
                        //need to validate na value, length, data type and field type
                        bool hasDataType = field.FieldDataType.Length > 0;
                        int decimalDigit = Convert.ToInt32(field.FieldDecimalDigit == null ? 0 : field.FieldDecimalDigit);

                        bool isNumeric = false;
                        bool isCurrency = false;
                        bool isAlphabet = false;
                        bool isAlphaNumeric = false;

                        if (hasDataType)
                        {
                            if (field.FieldDataType == "Numeric")
                                isNumeric = true;
                            else if (field.FieldDataType == "Currency")
                                isCurrency = true;
                            else if (field.FieldDataType == "Alphabet")
                                isAlphabet = true;
                            else
                                isAlphaNumeric = true;
                        }
                        //If not date range
                        if (field.FieldType != FieldType.DateRange)
                        {
                            //Value2 is not required
                            if (!(String.IsNullOrWhiteSpace(value2)))
                                return returnMessage = "Value2 is not required";
                        }

                        if (field.FieldType == FieldType.DateRange)
                        {
                            //If field type is date range, if has specified value then require the two values
                            if ((!String.IsNullOrWhiteSpace(value) && String.IsNullOrWhiteSpace(value2)) || (String.IsNullOrWhiteSpace(value) && !String.IsNullOrWhiteSpace(value2)))
                                return returnMessage = "Value and Value2 are required";
                        }

                        if (field.FieldType == FieldType.Date || field.FieldType == FieldType.DateRange)
                        {
                            if (!(GeneralHelper.IsValidDate(value)))
                                return returnMessage = "Value is not a valid date.";
                        }
                        else
                        {
                            if (field.FieldType == FieldType.Email)
                            {
                                //Check if email is valid
                                if (!(GeneralHelper.IsValidEmail(value)))
                                    return returnMessage = "Value is not a valid email format.";
                            }

                            if (hasDataType)
                            {
                                //If value is a valid numeric values
                                if (isNumeric)
                                {
                                    if (!(GeneralHelper.IsNumeric(value, decimalDigit, false)))
                                        return returnMessage = "Value is not a numeric format.";

                                    if (decimalDigit > 0)
                                    {
                                        bool withPercentage = false;
                                        if (value.Contains("%"))
                                        {
                                            withPercentage = true;
                                            //remove percentage first (if existing) before rounding off
                                            value = value.Replace("%", "");
                                        }

                                        value = GeneralHelper.ReconstructValue(value, decimalDigit);

                                        int decimalPointIndex = value.IndexOf(".");
                                        decimalPointIndex++;
                                        string decimalPlaces = value.Substring(decimalPointIndex, (value.Length - decimalPointIndex));
                                        int countDecimalPlacesInDefaultValue = decimalPlaces.Length;

                                        if (withPercentage)
                                        {
                                            if (countDecimalPlacesInDefaultValue > decimalDigit)
                                                //round to the nearest decimal digit
                                                value = Math.Round(Convert.ToDouble(value), decimalDigit).ToString();
                                            //append percentage
                                            value += "%";
                                        }
                                        else
                                        {
                                            if (countDecimalPlacesInDefaultValue > decimalDigit)
                                                value = Math.Round(Convert.ToDouble(value), decimalDigit).ToString();
                                        }
                                    }
                                }
                                //If value is a valid currency values
                                if (isCurrency)
                                {
                                    if (!(GeneralHelper.IsNumeric(value, decimalDigit, true)))
                                        return returnMessage = "Value is not a currency format.";

                                    if (decimalDigit > 0)
                                    {
                                        value = GeneralHelper.ReconstructValue(value, decimalDigit);

                                        int decimalPointIndex = value.IndexOf(".");
                                        decimalPointIndex++;
                                        string decimalPlaces = value.Substring(decimalPointIndex, (value.Length - decimalPointIndex));

                                        int countDecimalPlacesInDefaultValue = decimalPlaces.Length;

                                        if (countDecimalPlacesInDefaultValue > decimalDigit)
                                            value = Math.Round(Convert.ToDouble(value), decimalDigit).ToString();
                                    }
                                }
                                //If value is alphabet 
                                if (isAlphabet)
                                {
                                    if (!(GeneralHelper.IsAlpha(value)))
                                        return returnMessage = "Value is not an alphabet.";
                                }
                                //If value is alphanumeric
                                if (isAlphaNumeric)
                                {

                                }
                            }
                        }

                        if (!(field.FieldType == FieldType.Email || field.FieldType == FieldType.Url || field.FieldType == FieldType.Date || field.FieldType == FieldType.DateRange || field.FieldType == FieldType.Dropdown || field.FieldType == FieldType.CheckboxList || field.FieldType == FieldType.RadioButton))
                        {
                            //Valid Lengths entries 
                            if (isNumeric || isCurrency)
                            {
                                if (decimalDigit > 0)
                                {
                                    if (field.FieldMinNumber > Convert.ToDouble(value))
                                        return returnMessage = "" + field.FieldMinNumber + " is minimum";

                                    if (field.FieldMaxNumber < Convert.ToDouble(value))
                                        return returnMessage = "" + field.FieldMaxNumber + " is maximum";
                                }
                                else
                                {
                                    if (field.FieldMinNumber > Convert.ToInt32(value))
                                        return returnMessage = "" + field.FieldMinNumber + " is minimum";

                                    if (field.FieldMaxNumber < Convert.ToInt32(value))
                                        return returnMessage = "" + field.FieldMaxNumber + " is maximum";
                                }
                            }
                            else
                            {
                                if (field.FieldMinLength > value.Length)
                                    return returnMessage = "Alteast " + field.FieldMinLength + "character is required";

                                if (field.FieldMaxLength < value.Length)
                                    return returnMessage = "Specified value exceeds the maximum.";
                            }
                        }
                        //Default Value 2 only for date range
                        if (!(String.IsNullOrWhiteSpace(value2)))
                        {
                            //assuming the defaul value has value
                            //check if valid date time
                            if (!(GeneralHelper.IsValidDate(value2)))
                                return returnMessage = "Value2 is not a valid date.";

                            var date1 = Convert.ToDateTime(value);
                            var date2 = Convert.ToDateTime(value2);

                            if (date2.Date < date1.Date)
                                return returnMessage = "Invalid date range.";
                        }
                    }
                    //No need to validate entries in LOV since this was already validated during saving of request form and no longer changeable when used in the transaction as well
                }

                return returnMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<IEnumerable<WorkFlowStepTypeViewModel>> GetStepTypes()
        {
            List<WorkFlowStepTypeViewModel> stepTypes = new List<WorkFlowStepTypeViewModel>();

            try
            {
                var workflowsteptypes = await this.GetWorkFlowStepTypes();

                stepTypes = workflowsteptypes.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return stepTypes;
        }
        private async Task<IEnumerable<WorkFlowStepTypeViewModel>> GetWorkFlowStepTypes()
        {
            List<WorkFlowStepTypeViewModel> stepTypes = new List<WorkFlowStepTypeViewModel>();

            try
            {
                stepTypes = await _dbCntxt.WorkFlowStepTypes
                                        .Select
                                        (
                                            s => new WorkFlowStepTypeViewModel()
                                            {
                                                Id = s.Id,
                                                Name = s.Name,
                                                Color = s.Color,
                                                Published = s.Published,
                                                ApplyApprovalPolicyForAssignees = s.ApplyApprovalPolicyForAssignees,
                                                StepsButtons = _dbCntxt.WorkFlowStepTypeButtons
                                                                    .Where
                                                                    (
                                                                        b =>
                                                                        b.StepTypeId == s.Id &&
                                                                        b.ActionButton.OptionGroup == "WorkFlow Step Actions"
                                                                    )
                                                                    .Select
                                                                    (
                                                                        b => new WorkFlowStepTypeButtonViewModel()
                                                                        {
                                                                            Id = b.Id,
                                                                            StepTypeId = b.StepTypeId,
                                                                            ActionButtonId = b.ActionButtonId,
                                                                            IsActionToNextStep = b.IsActionToNextStep,
                                                                            ActionButton = b.ActionButton.Name,
                                                                            ActionButtonDescription = b.ActionButton.Description,
                                                                            WithActionInWorkflow = (b.ActionButton.Name.ToUpper() == "APPROVE" || b.ActionButton.Name.ToUpper() == "DONE" || b.ActionButton.Name.ToUpper() == "ENDORSE" || b.ActionButton.Name.ToUpper() == "REJECT") ? true : false
                                                                        }
                                                                    )
                                                                    .ToList()
                                            }
                                        )
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return stepTypes;
        }
        public async Task<IEnumerable<ComparisonOperatorViewModel>> GetApplicableComparisonOperators(int fieldId)
        {
            List<ComparisonOperatorViewModel> operators = new List<ComparisonOperatorViewModel>();

            try
            {
                var fields = await this.GetFields();
                var comparisonOperatorsInDb = await this.GetComparisonOperators();
                var comparisonOperators = comparisonOperatorsInDb.ToList();

                if (fieldId <= 0)
                {
                    List<string> operatorNames = new List<string>();
                    operatorNames.Add("After");
                    operatorNames.Add("Before");
                    operatorNames.Add("Equal to");
                    operatorNames.Add("Not Equal to");
                    operatorNames.Add("Greater than");
                    operatorNames.Add("Greater than and equal to");
                    operatorNames.Add("Less than");
                    operatorNames.Add("Less than and equal to");

                    operators = _dbCntxt.ComparisonOperators
                                        .Where
                                        (
                                            o => operatorNames.Contains(o.Name)
                                        )
                                        .Select
                                        (
                                            o => new ComparisonOperatorViewModel()
                                            {
                                                Id = o.Id,
                                                Name = o.Name,
                                                TechnicalEquivalent = o.TechnicalEquivalent2
                                            }
                                        )
                                        .OrderBy(o => o.Name)
                                        .ToList();
                }
                else
                {
                    var field = fields.FirstOrDefault(f => f.FieldId == fieldId);

                    if (field != null)
                    {
                        int fieldType = Convert.ToInt32(field.FieldType);
                        string dataType = field.FieldDataType;
                        bool isnumericOrCurrency = false;

                        if (dataType == "Numeric" || dataType == "Currency")
                            isnumericOrCurrency = true;

                        bool useInMultipleTypes = false;

                        foreach (var optr in comparisonOperators)
                        {
                            useInMultipleTypes = false;

                            if (optr.UseInMultipleTypes)
                                useInMultipleTypes = true;

                            if (!String.IsNullOrWhiteSpace(optr.ApplicableDataTypes) && !String.IsNullOrWhiteSpace(optr.ApplicableFieldTypes))
                            {
                                if (optr.ApplicableDataTypes.Contains(dataType) && optr.ApplicableFieldTypes.Contains(fieldType.ToString()))
                                {
                                    if (isnumericOrCurrency && useInMultipleTypes)
                                        optr.TechnicalEquivalent = optr.TechnicalEquivalent2;

                                    operators.Add(optr);
                                }
                            }
                            else if (!String.IsNullOrWhiteSpace(optr.ApplicableDataTypes))
                            {
                                if (optr.ApplicableDataTypes.Contains(dataType))
                                {
                                    if (isnumericOrCurrency && useInMultipleTypes)
                                        optr.TechnicalEquivalent = optr.TechnicalEquivalent2;

                                    operators.Add(optr);
                                }
                            }
                            else
                            {
                                if (optr.ApplicableFieldTypes.Contains(fieldType.ToString()))
                                {
                                    if (isnumericOrCurrency && useInMultipleTypes)
                                        optr.TechnicalEquivalent = optr.TechnicalEquivalent2;

                                    operators.Add(optr);
                                }
                            }
                        }

                        operators = operators
                                .Select
                                (
                                    o => new ComparisonOperatorViewModel()
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        TechnicalEquivalent = o.TechnicalEquivalent
                                    }
                                )
                                .OrderBy(o => o.Name)
                                .ToList();
                    }
                }
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return operators;
        }
        private async Task<IEnumerable<ComparisonOperatorViewModel>> GetComparisonOperators()
        {
            List<ComparisonOperatorViewModel> operators = new List<ComparisonOperatorViewModel>();

            try
            {
                operators = await _dbCntxt.ComparisonOperators
                                          .Select
                                          (
                                             o => new ComparisonOperatorViewModel()
                                             {
                                                 Id = o.Id,
                                                 Name = o.Name,
                                                 ApplicableDataTypes = o.ApplicableDataTypes == null ? "" : o.ApplicableDataTypes,
                                                 ApplicableFieldTypes = o.ApplicableFieldTypes == null ? "" : o.ApplicableFieldTypes,
                                                 TechnicalEquivalent = o.TechnicalEquivalent,
                                                 TechnicalEquivalent2 = o.TechnicalEquivalent2,
                                                 UseInMultipleTypes = o.UseInMultipleTypes

                                             }
                                          )
                                          .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return operators;
        }
        #region For reference in the future
        //public async Task<IEnumerable<RequestStepViewModel>> GetApplicableWorkFlowNextStep1(int requestTypeId, int version, int stepId)
        //{
        //    List<RequestStepViewModel> requeststeps = new List<RequestStepViewModel>();

        //    try
        //    {
        //        var allsteps = await this.GetRequestSteps(requestTypeId, version, true);

        //        var requeststepsindb = allsteps
        //                               .Where
        //                                (
        //                                    s =>
        //                                    s.RequestTypeId == requestTypeId &&
        //                                    s.Version == version &&
        //                                    s.StepId != stepId
        //                                )
        //                                .OrderBy(s => s.Sequence)
        //                                .ToList();


        //        if (requeststepsindb.Count > 0)
        //        {
        //            foreach (var requeststep in requeststepsindb)
        //            {
        //                bool isnotqualified = false;
        //                bool requeststepisconditional = requeststep.IsConditional;

        //                var workflowsuccessors = await this.GetStepSuccessors(stepId, requestTypeId, version);

        //                var successors = workflowsuccessors.ToList();

        //                bool desiredprecedessorhassuccessors = false;

        //                //Check if desired successor already added as successor on the desired predecessor
        //                if (successors.Count > 0)
        //                {
        //                    desiredprecedessorhassuccessors = true;
        //                    foreach (var successor in successors)
        //                    {
        //                        if (successor.NextStepId == requeststep.StepId)
        //                            isnotqualified = true;

        //                        if (isnotqualified)
        //                            break;
        //                    }

        //                    if (isnotqualified)
        //                        continue;
        //                }

        //                if (requeststepisconditional)
        //                {
        //                    if (desiredprecedessorhassuccessors)
        //                    {
        //                        foreach (var successor in successors)
        //                        {
        //                            //get step info
        //                            var stepindb = allsteps.FirstOrDefault(s => s.StepId == successor.NextStepId);

        //                            //Check if has same condition with successors
        //                            if (stepindb != null)
        //                            {
        //                                if (stepindb.IsConditional)
        //                                {
        //                                    foreach (var condition in requeststep.Conditions)
        //                                    {
        //                                        var samecondition = stepindb.Conditions.Where(c => c.Condition == condition.Condition).FirstOrDefault();

        //                                        if (samecondition != null)
        //                                            isnotqualified = true;

        //                                        if (isnotqualified)
        //                                            break;
        //                                    }
        //                                }
        //                            }

        //                            if (isnotqualified)
        //                                break;
        //                        }
        //                    }
        //                }

        //                if (isnotqualified)
        //                    continue;

        //                //************** Check Parallel *********************//
        //                isnotqualified = false;

        //                //Check if step to be added has successors
        //                var requeststepsuccesors = await this.GetStepSuccessors(requeststep.StepId, requestTypeId, version);

        //                if (desiredprecedessorhassuccessors)
        //                {
        //                    //Check if desired predecessor is parallel
        //                    var desiredpredecessorpredsors = await this.GetStepPredecessors(stepId, requestTypeId, version);

        //                    if (desiredpredecessorpredsors.Count() > 0)
        //                    {
        //                        //loop thru all predecessors
        //                        foreach (var desiredpredecessorpredsor in desiredpredecessorpredsors)
        //                        {
        //                            var predecessorparallel = _dbCntxt.RequestTypeWorkFlowParallels
        //                                          .Include(c => c.WorkFlow)
        //                                           .FirstOrDefault
        //                                           (
        //                                               p => p.RequestTypeWorkFlowId == desiredpredecessorpredsor.Id
        //                                           );

        //                            if (predecessorparallel != null)
        //                                isnotqualified = true;

        //                            if (isnotqualified)
        //                                break;
        //                        }

        //                        if (isnotqualified)
        //                            continue;
        //                    }

        //                    //get atleast 1 successor to check if parallel
        //                    var onesuccessor = successors[0];

        //                    int onesuccessornextstep = Convert.ToInt32(onesuccessor.NextStepId);

        //                    var workflowsuccessorofonesuccesors = await this.GetStepSuccessors(onesuccessornextstep, requestTypeId, version);

        //                    var successorparallel = _dbCntxt.RequestTypeWorkFlowParallels
        //                                           .Include(c => c.WorkFlow)
        //                                            .FirstOrDefault
        //                                            (
        //                                                p => p.RequestTypeWorkFlowId == onesuccessor.Id
        //                                            );

        //                    bool successorisparallel = false;

        //                    if (successorparallel != null)
        //                        successorisparallel = true;

        //                    if (successorisparallel)
        //                    {
        //                        //Check if step to be added has same successor(s) with successors of desired predecessor
        //                        if (requeststepsuccesors.Count() > 0 && workflowsuccessorofonesuccesors.Count() > 0)
        //                        {
        //                            var requeststepsuccesorsstepid = requeststepsuccesors.Select(s => s.NextStepId).ToList();

        //                            var diffsuccessors = workflowsuccessorofonesuccesors.Where(s => !requeststepsuccesorsstepid.Contains(s.NextStepId)).ToList();

        //                            //Step to be added is not qualified if has different successors with successors of desired predecessor
        //                            if (diffsuccessors.Count > 0)
        //                                continue;
        //                        }
        //                        else if (!(requeststepsuccesors.Count() == 0 && workflowsuccessorofonesuccesors.Count() == 0))
        //                            continue;
        //                    }
        //                    else
        //                    {
        //                        if (successors.Count == 1)
        //                        {
        //                            var onesuccessorstepinfo = allsteps.FirstOrDefault(s => s.StepId == onesuccessor.NextStepId);

        //                            if (!onesuccessorstepinfo.IsConditional || !requeststepisconditional)
        //                            {
        //                                if (workflowsuccessorofonesuccesors.Count() > 0 && requeststepsuccesors.Count() > 0)
        //                                {
        //                                    if (!(workflowsuccessorofonesuccesors.Count() == 1 && requeststepsuccesors.Count() == 1))
        //                                        continue;

        //                                    //Step to be added is not qualified if has different successors with successors of desired predecessor
        //                                    if (requeststepsuccesors.FirstOrDefault().NextStepId != workflowsuccessorofonesuccesors.FirstOrDefault().NextStepId)
        //                                        continue;

        //                                }
        //                                else if (!(requeststepsuccesors.Count() == 0 && workflowsuccessorofonesuccesors.Count() == 0))
        //                                    continue;
        //                            }
        //                        }
        //                    }
        //                }

        //                //******* End of Checking Parallel *******************//

        //                //Check if desired predecessor existing within the path of desired successor
        //                foreach (var requeststepsuccesor in requeststepsuccesors)
        //                {
        //                    if (requeststepsuccesor.NextStepId == stepId)
        //                        isnotqualified = true;

        //                    if (isnotqualified)
        //                        break;

        //                    int nextstepid = Convert.ToInt32(requeststepsuccesor.NextStepId);

        //                    if (await this.CheckExistingStepInPath(true, nextstepid, requestTypeId, version, stepId))
        //                        isnotqualified = true;

        //                    if (isnotqualified)
        //                        break;
        //                }

        //                //Check if desired successor existing within the path of desired predecessor
        //                var diseredredecessorpredecessors = await this.GetStepPredecessors(stepId, requestTypeId, version);

        //                if (diseredredecessorpredecessors.Count() > 0)
        //                {
        //                    foreach (var diseredredecessorpredecessor in diseredredecessorpredecessors)
        //                    {
        //                        if (diseredredecessorpredecessor.StepId == requeststep.StepId)
        //                            isnotqualified = true;

        //                        if (isnotqualified)
        //                            break;

        //                        int prevstepid = Convert.ToInt32(diseredredecessorpredecessor.StepId);

        //                        if (await this.CheckExistingStepInPath(false, prevstepid, requestTypeId, version, requeststep.StepId))
        //                            isnotqualified = true;

        //                        if (isnotqualified)
        //                            break;
        //                    }
        //                }

        //                if (isnotqualified)
        //                    continue;

        //                if (!isnotqualified)
        //                {
        //                    RequestStepViewModel stepViewModel = new RequestStepViewModel();

        //                    stepViewModel.RequestTypeId = requeststep.RequestTypeId;
        //                    stepViewModel.Version = requeststep.Version;
        //                    stepViewModel.StepId = requeststep.StepId;
        //                    stepViewModel.StepName = requeststep.StepName;
        //                    stepViewModel.Sequence = requeststep.Sequence;
        //                    stepViewModel.Id = requeststep.StepId;
        //                    stepViewModel.Name = requeststep.StepName;
        //                    requeststeps.Add(stepViewModel);
        //                }
        //            }
        //        }

        //        requeststeps = requeststeps.OrderBy(s => s.Sequence).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex.InnerException);
        //    }

        //    return requeststeps;
        //}
        #endregion
        public async Task<IEnumerable<RequestStepViewModel>> GetApplicableWorkFlowNextStep(int requestTypeId, int version, int stepId = 0)
        {
            List<RequestStepViewModel> requestSteps = new List<RequestStepViewModel>();

            try
            {
                var allSteps = await this.GetRequestSteps(requestTypeId, version, true);
                var workFlowSuccessors = await this.GetStepSuccessors(stepId, requestTypeId, version);

                var requestStepsindb = allSteps
                                       .Where
                                        (
                                            s =>
                                            s.RequestTypeId == requestTypeId &&
                                            s.Version == version &&
                                            s.StepId != stepId
                                        )
                                        .OrderBy(s => s.Sequence)
                                        .ToList();

                if (requestStepsindb.Count > 0)
                {
                    foreach (var requestStep in requestStepsindb)
                    {
                        bool isNotQualified = false;
                        var successors = workFlowSuccessors.ToList();

                        //Check if desired successor already added as successor on the desired predecessor
                        if (successors.Count > 0)
                        {
                            foreach (var successor in successors)
                            {
                                if (successor.NextStepId == requestStep.StepId)
                                    isNotQualified = true;

                                if (isNotQualified)
                                    break;
                            }

                            if (isNotQualified)
                                continue;
                        }

                        if (!isNotQualified)
                        {
                            RequestStepViewModel stepViewModel = new RequestStepViewModel();

                            stepViewModel.RequestTypeId = requestStep.RequestTypeId;
                            stepViewModel.Version = requestStep.Version;
                            stepViewModel.StepId = requestStep.StepId;
                            stepViewModel.StepName = requestStep.StepName;
                            stepViewModel.Sequence = requestStep.Sequence;
                            stepViewModel.Id = requestStep.StepId;
                            stepViewModel.Name = requestStep.Sequence + " - " + requestStep.StepName;
                            requestSteps.Add(stepViewModel);
                        }
                    }
                }

                requestSteps = requestSteps.OrderBy(s => s.Sequence).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return requestSteps;
        }
        private async Task<IEnumerable<RequestTypeWorkFlowViewModel>> GetStepSuccessors(int stepId, int requestTypeId, int version)
        {
            List<RequestTypeWorkFlowViewModel> successors = new List<RequestTypeWorkFlowViewModel>();

            try
            {
                var workFlows = await this.GetRequestTypeWorkFlows(requestTypeId, version);

                successors = workFlows
                            .Where
                            (
                                w =>
                                w.RequestTypeId == requestTypeId &&
                                w.Version == version &&
                                w.StepId == stepId &&
                                (w.NextStepId.HasValue && w.NextStepId > 0)
                            )
                            .Select
                            (
                                w => new RequestTypeWorkFlowViewModel()
                                {
                                    Id = w.Id,
                                    RequestTypeId = w.RequestTypeId,
                                    Version = w.Version,
                                    StepId = w.StepId,
                                    NextStepId = w.NextStepId,
                                    NextStepRequestTypeId = w.NextStepRequestTypeId,
                                    NextStepVersion = w.NextStepVersion,
                                    Action = w.Action,
                                    TypeWhenRejected = w.TypeWhenRejected
                                }
                            )
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return successors;
        }
        private async Task<IEnumerable<RequestTypeWorkFlowViewModel>> GetStepPredecessors(int stepId, int requestTypeId, int version)
        {
            List<RequestTypeWorkFlowViewModel> predecessors = new List<RequestTypeWorkFlowViewModel>();

            try
            {
                var workFlows = await this.GetRequestTypeWorkFlows(requestTypeId, version);

                predecessors = workFlows
                                .Where
                                (
                                    w =>
                                    w.RequestTypeId == requestTypeId &&
                                    w.Version == version &&
                                    (w.NextStepId.HasValue && w.NextStepId == stepId)
                                )
                                .Select
                                (
                                    w => new RequestTypeWorkFlowViewModel()
                                    {
                                        Id = w.Id,
                                        RequestTypeId = w.RequestTypeId,
                                        Version = w.Version,
                                        StepId = w.StepId,
                                        NextStepId = w.NextStepId,
                                        NextStepRequestTypeId = w.NextStepRequestTypeId,
                                        NextStepVersion = w.NextStepVersion,
                                        Action = w.Action,
                                        TypeWhenRejected = w.TypeWhenRejected
                                    }
                                )
                                .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return predecessors;
        }
        private async Task<bool> CheckExistingStepInPath(bool succedingSteps, int stepId, int requestTypeId, int version, int stepIdToCheck)
        {
            bool returnValue = false;

            if (succedingSteps)
            {
                var successors = await this.GetStepSuccessors(stepId, requestTypeId, version);

                if (successors.Count() > 0)
                {
                    foreach (var succesor in successors)
                    {
                        if (succesor.NextStepId == stepIdToCheck)
                            returnValue = true;

                        if (returnValue)
                            break;

                        int nextStepId = Convert.ToInt32(succesor.NextStepId);

                        returnValue = await this.CheckExistingStepInPath(true, nextStepId, requestTypeId, version, stepIdToCheck);

                        if (returnValue)
                            break;
                    }
                }
            }
            else
            {
                var predeccessors = await this.GetStepPredecessors(stepId, requestTypeId, version);

                if (predeccessors.Count() > 0)
                {
                    foreach (var predeccessor in predeccessors)
                    {
                        if (predeccessor.StepId == stepIdToCheck)
                            returnValue = true;

                        if (returnValue)
                            break;

                        int prevStepId = Convert.ToInt32(predeccessor.StepId);

                        returnValue = await this.CheckExistingStepInPath(false, prevStepId, requestTypeId, version, stepIdToCheck);

                        if (returnValue)
                            break;
                    }
                }
            }

            return returnValue;
        }
        private async Task<List<string>> ValidateSelectedNextSteps(List<RequestTypeWorkFlowViewModel> workFlows)
        {
            List<string> consoErrorMessage = new List<string>();
            bool withError = false;
            bool readOtherConditions = true;

            int requestTypeId = workFlows.Select(w => w.RequestTypeId).Distinct().FirstOrDefault();
            int version = workFlows.Select(w => w.Version).Distinct().FirstOrDefault();
            int stepId = workFlows.Select(w => w.StepId).Distinct().FirstOrDefault();

            var allSteps = await this.GetRequestSteps(requestTypeId, version, false);

            foreach (var workFlow in workFlows)
            {
                withError = false;

                if (workFlow.NextStepId.HasValue && workFlow.NextStepId > 0)
                {
                    //Duplicate Successors
                    var workFlowNextStep = allSteps.FirstOrDefault(s => s.StepId == workFlow.NextStepId);
                    var workFlowStep = allSteps.FirstOrDefault(s => s.StepId == stepId);

                    if (this.CheckIfDuplicateSuccessorInModel(workFlow, workFlows))
                    {
                        consoErrorMessage.Add("Next Step: '" + workFlowNextStep.Sequence.ToString() + " - " + workFlowNextStep.StepName + "' is duplicate entry for a successor.");
                        withError = true;
                    }

                    if (withError)
                        break;
                    //Duplicate Successors
                    if (this.CheckIfDuplicateSuccessorInDatabase(workFlow))
                    {
                        consoErrorMessage.Add("Next Step: '" + workFlowNextStep.StepName + "' is already maintained as successor.");
                        withError = true;
                    }

                    if (withError)
                        break;

                    if (readOtherConditions)
                    {
                        //Conditional in Model
                        if (workFlowNextStep.IsConditional)
                        {
                            //Duplicate conditions
                            if (await this.CheckIfDuplicateNextWorkFlowConditionsInModel(workFlow, workFlows))
                            {
                                consoErrorMessage.Add("Next Step: '" + workFlowNextStep.Sequence.ToString() + " - " + workFlowNextStep.StepName + "' has duplicate condition with the other desired successors. ");
                                withError = true;
                            }

                            if (withError)
                                break;
                        }

                        if (withError)
                            break;

                        //Parallel

                        //Within the path
                        //Check if desired predecessor existing within the path of desired successor
                        var nextStepSuccessors = await this.GetStepSuccessors(Convert.ToInt32(workFlow.NextStepId), requestTypeId, version);
                        foreach (var nextStepSuccessor in nextStepSuccessors)
                        {
                            if (nextStepSuccessor.NextStepId == stepId)
                            {
                                consoErrorMessage.Add("Step: '" + workFlowStep.Sequence.ToString() + " - " + workFlowStep.StepName + "' is already maintained within the path of Next Step: '" + workFlowNextStep.Sequence.ToString() + " - " + workFlowNextStep.StepName + "'");
                                withError = true;
                            }

                            if (withError)
                                break;

                            int nextStepId = Convert.ToInt32(nextStepSuccessor.NextStepId);

                            if (await this.CheckExistingStepInPath(true, nextStepId, requestTypeId, version, stepId))
                            {
                                consoErrorMessage.Add("Step: '" + workFlowStep.Sequence.ToString() + " - " + workFlowStep.StepName + "' is already maintained within the path of Next Step: '" + workFlowNextStep.Sequence.ToString() + " - " + workFlowNextStep.StepName + "'");
                                withError = true;
                            }

                            if (withError)
                                break;
                        }

                        if (withError)
                            break;

                        //Check if desired successor existing within the path of desired predecessor
                        var desiredPredecessorPredecessors = await this.GetStepPredecessors(stepId, requestTypeId, version);

                        if (desiredPredecessorPredecessors.Count() > 0)
                        {
                            foreach (var desiredPredecessorPredecessor in desiredPredecessorPredecessors)
                            {
                                if (desiredPredecessorPredecessor.StepId == workFlow.NextStepId)
                                {
                                    consoErrorMessage.Add("Next Step: " + workFlowNextStep.Sequence.ToString() + " - " + workFlowNextStep.StepName + " is already maintained within the path of before Work Flow Step: '" + workFlowStep.Sequence.ToString() + " - " + workFlowStep.StepName + "'");
                                    withError = true;
                                }

                                if (withError)
                                    break;

                                int prevStepId = Convert.ToInt32(desiredPredecessorPredecessor.StepId);

                                if (await this.CheckExistingStepInPath(false, prevStepId, requestTypeId, version, Convert.ToInt32(workFlow.NextStepId)))
                                {
                                    consoErrorMessage.Add("Next Step: " + workFlowNextStep.Sequence.ToString() + " - " + workFlowNextStep.StepName + " is already maintained within the path of before Work Flow Step: '" + workFlowStep.Sequence.ToString() + " - " + workFlowStep.StepName + "'");
                                    withError = true;
                                }

                                if (withError)
                                    break;
                            }

                            if (withError)
                                break;
                        }
                    }
                }

                if (withError)
                    break;
            }

            return consoErrorMessage;
        }
        private async Task<bool> CheckIfDuplicateNextWorkFlowConditionsInModel(RequestTypeWorkFlowViewModel model, List<RequestTypeWorkFlowViewModel> listOfModel)
        {
            bool returnValue = false;
            int nextStepId = Convert.ToInt32(model.NextStepId);
            int nextRequestTypeId = Convert.ToInt32(model.NextStepRequestTypeId);
            int nextVersion = Convert.ToInt32(model.NextStepVersion);

            var allSteps = await this.GetRequestSteps(nextRequestTypeId, nextVersion, false);
            bool isConditional = allSteps.FirstOrDefault(s => s.StepId == nextStepId).IsConditional;

            if (isConditional)
            {
                //get condition of parameter step
                var stepCondition = this.GetStepConditions(nextRequestTypeId, nextVersion)
                                        .Where
                                        (
                                           c => c.StepId == nextStepId
                                        ).FirstOrDefault();

                //Get list of workflows not equal to workflow parameters this is to remove itself from the checking procedure
                var filterListOfModel = listOfModel
                                 .Where
                                 (
                                    c =>
                                    c.RequestTypeId != model.RequestTypeId &&
                                    c.Version != model.Version &&
                                    c.StepId != model.StepId &&
                                    c.NextStepId != nextStepId &&
                                    c.NextStepRequestTypeId != nextRequestTypeId &&
                                    c.NextStepVersion != nextVersion &&
                                    c.Action != model.Action &&
                                    c.TypeWhenRejected != model.TypeWhenRejected
                                 ).ToList();

                if (filterListOfModel.Count > 0)
                {
                    foreach (var m in filterListOfModel)
                    {
                        int mStepId = Convert.ToInt32(m.NextStepId);
                        int mRequestTypeId = Convert.ToInt32(m.NextStepRequestTypeId);
                        int mVersion = Convert.ToInt32(m.NextStepVersion);
                        bool mIsConditional = allSteps.FirstOrDefault(s => s.StepId == mStepId).IsConditional;

                        if (mIsConditional)
                        {
                            var checkCondition = this.GetStepConditions(mRequestTypeId, mVersion)
                                                    .Where
                                                    (
                                                        c =>
                                                        c.StepId == mStepId &&
                                                        c.Condition == stepCondition.Condition
                                                    ).FirstOrDefault();

                            if (checkCondition != null)
                                returnValue = true;
                        }

                        if (returnValue)
                            break;
                    }
                }
            }

            return returnValue;
        }
        public async Task<IEnumerable<ConditionSourceViewModel>> GetStepsConditionSources()
        {
            List<ConditionSourceViewModel> conditionSources = new List<ConditionSourceViewModel>();

            try
            {
                conditionSources = await _dbCntxt.Options
                                          .Where(o => o.OptionGroup == "Condition Sources")
                                          .Select
                                          (
                                             o => new ConditionSourceViewModel()
                                             {
                                                 Id = o.Id,
                                                 Name = o.Name,
                                                 Published = o.Published
                                             }
                                          )
                                          .OrderBy(o => o.Id)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return conditionSources;
        }
        private bool CheckIfDuplicateSuccessorInModel(RequestTypeWorkFlowViewModel model, List<RequestTypeWorkFlowViewModel> listOfModel)
        {
            bool returnValue = false;

            var checkDuplicate = listOfModel
                                 .Where
                                 (
                                    c =>
                                    (c.NextStepId == model.NextStepId && c.NextStepId.HasValue)
                                 ).ToList();

            if (checkDuplicate.Count > 1)
                returnValue = true;

            return returnValue;
        }
        private bool CheckIfDuplicateSuccessorInDatabase(RequestTypeWorkFlowViewModel model)
        {
            bool returnValue = false;

            List<RequestTypeWorkFlow> checkDuplicate = new();

            if (model.Id > 0)
            {
                checkDuplicate = _dbCntxt.RequestTypeWorkFlows
                                         .Where
                                         (
                                             c => c.RequestTypeId == model.RequestTypeId &&
                                             c.Version == model.Version &&
                                             c.StepId == model.StepId &&
                                             (c.NextStepId == model.NextStepId && c.NextStepId.HasValue) &&
                                             (c.NextStepVersion == model.NextStepVersion && c.NextStepVersion.HasValue) &&
                                             (c.NextStepRequestTypeId == model.NextStepRequestTypeId && c.NextStepRequestTypeId.HasValue) &&
                                             c.Id != model.Id
                                         ).ToList();
            }
            else
            {
                checkDuplicate = _dbCntxt.RequestTypeWorkFlows
                                         .Where
                                         (
                                             c => c.RequestTypeId == model.RequestTypeId &&
                                             c.Version == model.Version &&
                                             c.StepId == model.StepId &&
                                             (c.NextStepId == model.NextStepId && c.NextStepId.HasValue) &&
                                             (c.NextStepVersion == model.NextStepVersion && c.NextStepVersion.HasValue) &&
                                             (c.NextStepRequestTypeId == model.NextStepRequestTypeId && c.NextStepRequestTypeId.HasValue)
                                         ).ToList();
            }

            if (checkDuplicate.Count > 0)
                returnValue = true;

            return returnValue;
        }
        private async Task<bool> CheckIfNextStepWithinOtherNextStepPathInModel(RequestTypeWorkFlowViewModel model, List<RequestTypeWorkFlowViewModel> listOfModel)
        {
            bool returnValue = false;
            int nStepid = Convert.ToInt32(model.NextStepId);
            int nRequestTypeId = Convert.ToInt32(model.NextStepRequestTypeId);
            int nVersion = Convert.ToInt32(model.NextStepVersion);
            //Get list of workflows not equal to workflow parameters this is to remove itself from the checking procedure
            var filterListOfModel = listOfModel
                             .Where
                             (
                                c =>
                                c.RequestTypeId != model.RequestTypeId &&
                                c.Version != model.Version &&
                                c.StepId != model.StepId &&
                                c.NextStepId != nStepid &&
                                c.NextStepRequestTypeId != nRequestTypeId &&
                                c.NextStepVersion != nVersion &&
                                c.Action != model.Action &&
                                c.TypeWhenRejected != model.TypeWhenRejected
                             ).ToList();

            if (filterListOfModel.Count > 0)
            {
                foreach (var m in filterListOfModel)
                {
                    //Check successor
                    var nextStepSuccessors = await this.GetStepSuccessors(Convert.ToInt32(m.NextStepId), nRequestTypeId, nVersion);
                    foreach (var nextStepSuccessor in nextStepSuccessors)
                    {
                        if (nextStepSuccessor.NextStepId == nStepid)
                            returnValue = true;

                        if (returnValue)
                            break;

                        int nextStepId = Convert.ToInt32(nextStepSuccessor.NextStepId);

                        if (await this.CheckExistingStepInPath(true, nextStepId, nRequestTypeId, nVersion, nStepid))
                            returnValue = true;

                        if (returnValue)
                            break;
                    }

                    if (returnValue)
                        break;
                    //Check predecessor
                    var desiredPredecessorPredecessors = await this.GetStepPredecessors(Convert.ToInt32(m.NextStepId), nRequestTypeId, nVersion);

                    if (desiredPredecessorPredecessors.Count() > 0)
                    {
                        foreach (var desiredPredecessorPredecessor in desiredPredecessorPredecessors)
                        {
                            if (desiredPredecessorPredecessor.StepId == nStepid)
                                returnValue = true;

                            if (returnValue)
                                break;

                            int prevStepId = Convert.ToInt32(desiredPredecessorPredecessor.StepId);

                            if (await this.CheckExistingStepInPath(false, prevStepId, nRequestTypeId, nVersion, nStepid))
                                returnValue = true;

                            if (returnValue)
                                break;
                        }
                    }

                    if (returnValue)
                        break;
                }
            }

            return returnValue;
        }
        private async Task<bool> CheckIfNextStepWithinOtherNextStepPathInDatabase(RequestTypeWorkFlowViewModel model)
        {
            bool returnValue = false;
            int nStepid = Convert.ToInt32(model.NextStepId);
            int nRequestTypeId = Convert.ToInt32(model.NextStepRequestTypeId);
            int nVersion = Convert.ToInt32(model.NextStepVersion);
            //get successors of desired 
            var predecessorSuccessors = await this.GetStepSuccessors(model.StepId, model.RequestTypeId, model.Version);

            foreach (var successors in predecessorSuccessors)
            {
                if (successors.NextStepId != nStepid)
                {
                    //Check successor
                    var nextStepSuccessors = await this.GetStepSuccessors(Convert.ToInt32(successors.NextStepId), nRequestTypeId, nVersion);
                    foreach (var nextStepSuccessor in nextStepSuccessors)
                    {
                        if (nextStepSuccessor.NextStepId == nStepid)
                            returnValue = true;

                        if (returnValue)
                            break;

                        int nextStepId = Convert.ToInt32(nextStepSuccessor.NextStepId);

                        if (await this.CheckExistingStepInPath(true, nextStepId, nRequestTypeId, nVersion, nStepid))
                            returnValue = true;

                        if (returnValue)
                            break;
                    }

                    if (returnValue)
                        break;

                    //Check predecessor
                    var desiredPredecessorPredecessors = await this.GetStepPredecessors(Convert.ToInt32(successors.NextStepId), nRequestTypeId, nVersion);

                    if (desiredPredecessorPredecessors.Count() > 0)
                    {
                        foreach (var desiredPredecessorPredecessor in desiredPredecessorPredecessors)
                        {
                            if (desiredPredecessorPredecessor.StepId == nStepid)
                                returnValue = true;

                            if (returnValue)
                                break;

                            int prevStepId = Convert.ToInt32(desiredPredecessorPredecessor.StepId);

                            if (await this.CheckExistingStepInPath(false, prevStepId, nRequestTypeId, nVersion, nStepid))
                                returnValue = true;

                            if (returnValue)
                                break;
                        }

                        if (returnValue)
                            break;
                    }

                    if (returnValue)
                        break;
                }

                if (returnValue)
                    break;
            }

            return returnValue;
        }
        public async Task<IEnumerable<RequestTypeWorkFlowViewModel>> GetRequestTypeWorkFlows(int requestTypeId, int version)
        {
            List<RequestTypeWorkFlowViewModel> workFlows = new List<RequestTypeWorkFlowViewModel>();

            try
            {
                workFlows = await _dbCntxt.RequestTypeWorkFlows
                                          .Where(w => w.RequestTypeId == requestTypeId && w.Version == version)
                                            .Select
                                            (
                                                w => new RequestTypeWorkFlowViewModel()
                                                {
                                                    Id = w.Id,
                                                    RequestTypeId = w.RequestTypeId,
                                                    Version = w.Version,
                                                    StepId = w.StepId,
                                                    NextStepRequestTypeId = w.NextStepRequestTypeId,
                                                    NextStepVersion = w.NextStepVersion,
                                                    NextStepId = w.NextStepId,
                                                    Action = w.Action,
                                                    TypeWhenRejected = (TypeWhenRejected)w.TypeWhenRejected
                                                }
                                            )
                                            .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return workFlows;
        }
        public async Task<StepFlowViewModel> GetRequestTypeProcessWorkFlows(int requestTypeId, int version)
        {
            StepFlowViewModel stepWorkFlows = new StepFlowViewModel();
            stepWorkFlows.StepNodes = new List<StepViewModel>();
            stepWorkFlows.LinkNodes = new List<LinkStepViewModel>();
            stepWorkFlows.OtherLinkNodes = new List<OtherLinkStepViewModel>();
            List<LinkStepViewModel> listOfNodes = new List<LinkStepViewModel>();
            List<OtherLinkStepViewModel> OtherListOfNodes = new List<OtherLinkStepViewModel>();

            try
            {
                var workFlows = (await this.GetRequestTypeWorkFlows(requestTypeId, version)).ToList();

                var steps = await this.GetRequestSteps(requestTypeId, version, false);

                var workFlowNextStepIds = workFlows.Select(w => w.NextStepId).Distinct();

                var workFlowStepIds = workFlows.Select(w => w.StepId).Distinct();

                stepWorkFlows.StepNodes = steps
                                          .Select
                                          (
                                             s => new StepViewModel()
                                             {
                                                 Id = s.StepId.ToString(),
                                                 Label = s.Sequence.ToString() + " - " + s.StepName,
                                                 Color = s.StepTypeColor
                                             }
                                          ).ToList();

                bool needToAddSubmissionNode = false;

                if (stepWorkFlows.StepNodes.Count > 0)
                    needToAddSubmissionNode = true;

                if (needToAddSubmissionNode)
                {
                    //Dummy nodes for entry point
                    stepWorkFlows.StepNodes.Add(new StepViewModel()
                    {
                        Id = "s1",
                        Label = "Request Submission",
                        Color = "#D5D6EA"
                    });
                }

                var workFlowEntryPoint = workFlows
                                        .Where
                                        (
                                            w =>
                                            !workFlowNextStepIds.Contains(w.StepId) &&
                                            (w.NextStepId.HasValue && w.NextStepId > 0)
                                        )
                                        .OrderBy(w => w.Id)
                                        .ToList();

                var linkStepViewModel = new LinkStepViewModel();

                //Will insert entry point for request submission
                var distinctEntryPoint = workFlowEntryPoint.Select(c => c.StepId).Distinct();

                int counter = 1;
                foreach (var entrypoint in distinctEntryPoint)
                {
                    //add submission to requestor
                    linkStepViewModel = new LinkStepViewModel();
                    linkStepViewModel.Id = "ws" + counter;
                    linkStepViewModel.Source = "s1";
                    linkStepViewModel.Target = entrypoint.ToString();
                    linkStepViewModel.Label = "";
                    listOfNodes.Add(linkStepViewModel);

                    counter += 1;
                }

                foreach (var entryPoint in workFlowEntryPoint)
                {
                    linkStepViewModel = new LinkStepViewModel();
                    linkStepViewModel.Id = "w" + entryPoint.Id.ToString();
                    linkStepViewModel.Source = entryPoint.StepId.ToString();
                    linkStepViewModel.Target = entryPoint.NextStepId.ToString();
                    linkStepViewModel.Label = "";
                    listOfNodes.Add(linkStepViewModel);

                    int nextStepId = Convert.ToInt32(entryPoint.NextStepId);

                    listOfNodes.AddRange(await this.GetRequestTypeWorkFlowSuccessor(nextStepId, entryPoint.RequestTypeId, entryPoint.Version, workFlows));
                }

                stepWorkFlows.LinkNodes = listOfNodes
                                          .GroupBy
                                          (
                                             c => new
                                             {
                                                 c.Id,
                                                 c.Source,
                                                 c.Target,
                                                 c.Label
                                             }
                                          )
                                          .Select
                                          (
                                             c => c.First()
                                          ).ToList();

                var workFlowEntryPointFromLast = workFlows
                                                .Where
                                                (
                                                    w =>
                                                    !workFlowStepIds.Contains(Convert.ToInt32(w.NextStepId)) &&
                                                    (w.NextStepId.HasValue && w.NextStepId > 0)
                                                )
                                                .ToList();

                var otherLinkStepViewModel = new OtherLinkStepViewModel();
                foreach (var entryPointFromLast in workFlowEntryPointFromLast)
                {
                    var stepName = steps.FirstOrDefault(s => s.StepId == entryPointFromLast.NextStepId).StepName;

                    otherLinkStepViewModel = new OtherLinkStepViewModel();
                    otherLinkStepViewModel.Id = entryPointFromLast.NextStepId.ToString();
                    otherLinkStepViewModel.Name = stepName;
                    otherLinkStepViewModel.BackgroundColor = "#00FFFF";
                    otherLinkStepViewModel.UpperManagerId = entryPointFromLast.StepId.ToString();

                    OtherListOfNodes.Add(otherLinkStepViewModel);

                    int stepId = Convert.ToInt32(entryPointFromLast.StepId);

                    OtherListOfNodes.AddRange(await this.GetRequestTypeWorkFlowPredecessor(stepId, entryPointFromLast.RequestTypeId, entryPointFromLast.Version, workFlows));

                }

                var ids = workFlowEntryPoint.Select(w => w.StepId).Distinct();

                //starting point
                foreach (var startingPoint in ids)
                {
                    var stepName = steps.FirstOrDefault(s => s.StepId == startingPoint).Name;

                    otherLinkStepViewModel = new OtherLinkStepViewModel();
                    otherLinkStepViewModel.Id = startingPoint.ToString();
                    otherLinkStepViewModel.Name = stepName;
                    otherLinkStepViewModel.BackgroundColor = "#DC143C";
                    otherLinkStepViewModel.UpperManagerId = "0";
                    OtherListOfNodes.Add(otherLinkStepViewModel);
                }

                stepWorkFlows.OtherLinkNodes = OtherListOfNodes
                                                 .GroupBy
                                                 (
                                                    c => new
                                                    {
                                                        c.Id,
                                                        c.Name,
                                                        c.BackgroundColor,
                                                        c.UpperManagerId
                                                    }
                                                 )
                                                 .Select
                                                 (
                                                    c => c.First()
                                                 ).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return stepWorkFlows;
        }
        public async Task<IEnumerable<LinkStepViewModel>> GetRequestTypeWorkFlowSuccessor(int stepId, int requestTypeId, int version, List<RequestTypeWorkFlowViewModel> workFlowsData)
        {
            List<LinkStepViewModel> workFlows = new List<LinkStepViewModel>();

            var successors = workFlowsData
                             .Where
                            (
                                w =>
                                w.RequestTypeId == requestTypeId &&
                                w.Version == version &&
                                w.StepId == stepId &&
                                (w.NextStepId.HasValue && w.NextStepId > 0)
                            ).ToList();

            if (successors.Count() > 0)
            {
                var linkStepViewModel = new LinkStepViewModel();

                foreach (var succesor in successors)
                {
                    linkStepViewModel = new LinkStepViewModel();
                    linkStepViewModel.Id = "w" + succesor.Id.ToString();
                    linkStepViewModel.Source = succesor.StepId.ToString();
                    linkStepViewModel.Target = succesor.NextStepId.ToString();
                    linkStepViewModel.Label = "";
                    workFlows.Add(linkStepViewModel);

                    int nextStepId = Convert.ToInt32(succesor.NextStepId);

                    workFlows.AddRange(await this.GetRequestTypeWorkFlowSuccessor(nextStepId, succesor.RequestTypeId, succesor.Version, workFlowsData));
                }
            }

            return workFlows;
        }
        public async Task<IEnumerable<OtherLinkStepViewModel>> GetRequestTypeWorkFlowPredecessor(int stepId, int requestTypeId, int version, List<RequestTypeWorkFlowViewModel> workFlowsData)
        {
            List<OtherLinkStepViewModel> workFlows = new List<OtherLinkStepViewModel>();

            var predecessors = workFlowsData
                             .Where
                             (
                                w =>
                                w.RequestTypeId == requestTypeId &&
                                w.Version == version &&
                                (w.NextStepId.HasValue && w.NextStepId == stepId)
                             ).ToList();

            if (predecessors.Count() > 0)
            {
                var otherLinkStepViewModel = new OtherLinkStepViewModel();

                foreach (var predecessor in predecessors)
                {
                    var stepName = _dbCntxt.WorkFlowSteps.FirstOrDefault(s => s.Id == predecessor.NextStepId).Name;

                    otherLinkStepViewModel = new OtherLinkStepViewModel();
                    otherLinkStepViewModel.Id = predecessor.NextStepId.ToString();
                    otherLinkStepViewModel.Name = stepName;
                    otherLinkStepViewModel.BackgroundColor = "#00FFFF";
                    otherLinkStepViewModel.UpperManagerId = predecessor.StepId.ToString();
                    workFlows.Add(otherLinkStepViewModel);

                    int preStepId = Convert.ToInt32(predecessor.StepId);

                    workFlows.AddRange(await this.GetRequestTypeWorkFlowPredecessor(preStepId, predecessor.RequestTypeId, predecessor.Version, workFlowsData));
                }
            }

            return workFlows;
        }
        public async Task SaveRequestTypeWorkflow(SaveRequestTypeWorkFlowViewModel model)
        {
            if (model.Workflows == null)
                throw new CustomException("Kindly provide atleast one workflow to proceed.", 404);

            if (model.Workflows.Count <= 0)
                throw new CustomException("Kindly provide atleast one workflow to proceed.", 404);

            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {

                var singleWorkFlow = model.Workflows[0];

                var workflowsIds = model.Workflows.Select(c => c.Id);

                //Remove GUI deleted parallels in DB
                var requestWorkflowParallel = await _dbCntxt.RequestTypeWorkFlowParallels
                                                     .Include(r => r.WorkFlow)
                                                     .Where
                                                     (
                                                        r =>
                                                        r.WorkFlow.RequestTypeId == singleWorkFlow.RequestTypeId &&
                                                        r.WorkFlow.Version == singleWorkFlow.Version &&
                                                        r.WorkFlow.StepId == singleWorkFlow.StepId &&
                                                        !workflowsIds.Contains(r.RequestTypeWorkFlowId)
                                                     ).ToListAsync();

                foreach (var details in requestWorkflowParallel)
                {
                    _dbCntxt.RequestTypeWorkFlowParallels.Remove(details);
                    await _dbCntxt.SaveChangesAsync();
                }

                //Remove GUI deleted Workflows in DB
                _dbCntxt.RequestTypeWorkFlows.RemoveRange(_dbCntxt.RequestTypeWorkFlows.Where(r => r.RequestTypeId == singleWorkFlow.RequestTypeId && r.Version == singleWorkFlow.Version && r.StepId == singleWorkFlow.StepId && !workflowsIds.Contains(r.Id)));
                await _dbCntxt.SaveChangesAsync();

                var workflowValidations = await this.ValidateWorkFlows(model.Workflows, model.IsForNextStep);

                if (workflowValidations.ReturnMessage != "")
                    throw new CustomException(workflowValidations.ReturnMessage, 400, null, workflowValidations.DetailedMessages);

                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                await this.SaveWorkFlows(model.Workflows, model.IsForNextStep);

                //Update modified date in RequestTypes table
                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == singleWorkFlow.RequestTypeId && r.Version == singleWorkFlow.Version);

                if (requestTypeInDb != null)
                {
                    requestTypeInDb.ModifiedByPK = cid;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode, customex.Details, customex.ListMessage);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteRequestTypeWorkFlow(int id = 0, int requestTypeId = 0, int version = 0)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                int varRequestTypeId = 0;
                int varVersion = 0;

                if (id > 0)
                {
                    var workFlow = await _dbCntxt.RequestTypeWorkFlows.FindAsync(id);

                    if (workFlow == null)
                        throw new CustomException("Workflow is not found.", 404);

                    await this.DeleteWorkFlow(id);

                    varRequestTypeId = workFlow.RequestTypeId;
                    varVersion = workFlow.Version;
                }
                else
                {
                    if (requestTypeId <= 0 || version <= 0)
                        throw new CustomException("Both Request type id and version is required.", 400);

                    varRequestTypeId = requestTypeId;
                    varVersion = version;

                    var workFlow = await _dbCntxt.RequestTypeWorkFlows.Where(w => w.RequestTypeId == varRequestTypeId && w.Version == version).FirstOrDefaultAsync();

                    if (workFlow == null)
                        throw new CustomException("Request type id or version is not found.", 404);

                    await this.DeleteWorkFlow(id, varRequestTypeId, varVersion);
                }

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == varRequestTypeId && r.Version == varVersion);

                if (requestTypeInDb != null)
                {
                    requestTypeInDb.ModifiedByPK = cid;
                    requestTypeInDb.ModifiedDate = DateTime.Now;

                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteRequestStepAssignees(int id, int detailId = 0)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var assigneeHeader = await _dbCntxt.RequestStepAssignees.FindAsync(id);

                if (assigneeHeader == null)
                    throw new CustomException("Assignee Header is not found.", 404);

                if (detailId > 0)
                {
                    var assigneeDetail = await _dbCntxt.RequestStepAssigneeDetails.FindAsync(detailId);

                    if (assigneeDetail == null)
                        throw new CustomException("Assignee Detail is not found.", 404);

                    await this.DeleteAssigneesDetail(detailId);
                }
                else
                {
                    await this.DeleteAssignees(id);
                }

                int requestTypeId = assigneeHeader.RequestTypeId;
                int version = assigneeHeader.Version;

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == requestTypeId && r.Version == version);

                if (requestTypeInDb != null)
                {
                    requestTypeInDb.ModifiedByPK = cid;
                    requestTypeInDb.ModifiedDate = DateTime.Now;

                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteRequestStepConditions(int id)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                var condition = await _dbCntxt.RequestStepConditionDetails.FindAsync(id);

                if (condition == null)
                    throw new CustomException("Condition is not found.", 404);

                var conditionHeader = await _dbCntxt.RequestStepConditions.FindAsync(condition.RequestStepConditionId);

                await this.DeleteConditions(condition.RequestStepConditionId, id);

                int requestTypeId = 0;
                int version = 0;

                if (conditionHeader != null)
                {
                    requestTypeId = conditionHeader.RequestTypeId;
                    version = conditionHeader.Version;
                }

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == requestTypeId && r.Version == version);

                if (requestTypeInDb != null)
                {
                    requestTypeInDb.ModifiedByPK = cid;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }

                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task DeleteWorkFlow(int id = 0, int requestTypeId = 0, int version = 0)
        {
            try
            {
                if (id > 0)
                {
                    _dbCntxt.RequestTypeWorkFlows.RemoveRange(_dbCntxt.RequestTypeWorkFlows.Where(r => r.Id == id));
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                {
                    _dbCntxt.RequestTypeWorkFlows.RemoveRange(_dbCntxt.RequestTypeWorkFlows.Where(r => r.RequestTypeId == requestTypeId && r.Version == version));
                    await _dbCntxt.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task DeleteAssignees(int id)
        {
            try
            {
                //remove details and header
                _dbCntxt.RequestStepAssigneeDetails.RemoveRange(_dbCntxt.RequestStepAssigneeDetails.Where(r => r.RequestStepAssigneeId == id));
                await _dbCntxt.SaveChangesAsync();

                _dbCntxt.RequestStepAssignees.RemoveRange(_dbCntxt.RequestStepAssignees.Where(r => r.Id == id));
                await _dbCntxt.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task DeleteAssigneesDetail(int id)
        {
            try
            {
                _dbCntxt.RequestStepAssigneeDetails.RemoveRange(_dbCntxt.RequestStepAssigneeDetails.Where(r => r.Id == id));
                await _dbCntxt.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task DeleteConditions(int id, int detailId)
        {
            try
            {
                //remove details
                _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(r => r.Id == detailId));
                await _dbCntxt.SaveChangesAsync();

                var countRemainingIds = await _dbCntxt.RequestStepConditionDetails.Where(c => c.RequestStepConditionId == id).ToListAsync();

                if (countRemainingIds.Count <= 0)
                {
                    //delete header when all details are removed
                    _dbCntxt.RequestStepConditions.RemoveRange(_dbCntxt.RequestStepConditions.Where(r => r.Id == id));
                    await _dbCntxt.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task DeleteRequestStep(int requestTypeId, int version, int stepId = 0)
        {
            using var dbContextTransaction = _dbCntxt.Database.BeginTransaction();
            try
            {
                // get current user id
                var cid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get current session id
                int spId = await _userIpAddressPerSession.GetSPID();
                // get IP address
                string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                //add useripperssion
                var userIp = _userIpAddressPerSession.AddData(spId, cid, ip);
                await _userIpAddressPerSession.Save(userIp);

                if (stepId > 0)
                {
                    var requestType = await _dbCntxt.RequestSteps.FirstOrDefaultAsync(r => r.RequestTypeId == requestTypeId && r.Version == version && r.StepId == stepId);

                    if (requestType == null)
                        throw new CustomException("Request Step is not found.", 404);
                    //Check if with created workflows
                    var workFlows = await this.GetRequestTypeWorkFlows(requestTypeId, version);

                    var stepWorkFlows = workFlows.Where(s => (s.StepId == stepId && (s.NextStepId.HasValue && s.NextStepId > 0)) || (s.NextStepId.HasValue && s.NextStepId == stepId)).ToList();

                    if (stepWorkFlows.Count > 0)
                        throw new CustomException("Cannot delete specific step due to connected predecessor(s)/successor(s).", 404);

                    bool isActivelyMapped = await this.IsStepInStepConditionAsync(requestTypeId, version, stepId, false, true);

                    if (isActivelyMapped)
                        throw new CustomException("Must remove the mapped conditions on the Steps/Activities before you could delete this step", 400);

                    //auto delete record in step conditions when there's an inactive condition related to this step for deletion and reconstruct all steps conditions
                    bool isInActivelyMapped = await this.IsStepInStepConditionAsync(requestTypeId, version, stepId, true, false);

                    if (isInActivelyMapped)
                    {
                        var sourceStep = _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Step Description").FirstOrDefault();

                        var affectedRecords = await _dbCntxt.RequestStepConditionDetails
                                                            .Include(r => r.ConditionHeader)
                                                                    .Where
                                                                    (
                                                                        r =>
                                                                        r.ConditionHeader.RequestTypeId == requestTypeId &&
                                                                        r.ConditionHeader.Version == version &&
                                                                        r.Published == false &&
                                                                        r.Source == sourceStep.Id &&
                                                                        r.ReferenceDetail == stepId
                                                                    )
                                                                    .ToListAsync();

                        if (affectedRecords.Count > 0)
                        {
                            var headerConditionIds = affectedRecords.Select(c => c.RequestStepConditionId).Distinct().ToList();
                            var detailConditionIds = affectedRecords.Select(c => c.Id).ToList();

                            //delete all inactive condition details
                            _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(c => detailConditionIds.Contains(c.Id)));
                            await _dbCntxt.SaveChangesAsync();

                            //Rearrange sequence and align rows with logical operator per header id
                            foreach (var headerId in headerConditionIds)
                            {
                                var conditionDetails = await _dbCntxt.RequestStepConditionDetails
                                                               .Where
                                                               (
                                                                   r =>
                                                                   r.RequestStepConditionId == headerId
                                                               )
                                                               .OrderBy(r => r.Sequence)
                                                               .ToListAsync();

                                if (conditionDetails.Count > 0)
                                {
                                    int sequence = 1;
                                    foreach (var detail in conditionDetails)
                                    {
                                        detail.Sequence = sequence;

                                        if (sequence == conditionDetails.Count)
                                            //set to null if last row
                                            detail.LogicalOperator = null;

                                        _dbCntxt.Entry(detail).State = EntityState.Modified;
                                        await _dbCntxt.SaveChangesAsync();

                                        sequence += 1;
                                    }
                                }
                            }
                        }
                    }

                    //Delete Assignees
                    var assigneesHeaders = await _dbCntxt.RequestStepAssignees.Where(a => a.RequestTypeId == requestTypeId && a.Version == version && a.StepId == stepId).ToListAsync();

                    if (assigneesHeaders.Count > 0)
                    {
                        //remove details
                        foreach (var assigneesHeader in assigneesHeaders)
                        {
                            _dbCntxt.RequestStepAssigneeDetails.RemoveRange(_dbCntxt.RequestStepAssigneeDetails.Where(a => a.RequestStepAssigneeId == assigneesHeader.Id));
                            await _dbCntxt.SaveChangesAsync();
                        }
                        //remove header
                        _dbCntxt.RequestStepAssignees.RemoveRange(_dbCntxt.RequestStepAssignees.Where(r => r.RequestTypeId == requestTypeId && r.Version == version && r.StepId == stepId));
                        await _dbCntxt.SaveChangesAsync();
                    }
                    //Delete Conditions
                    var conditionHeaders = await _dbCntxt.RequestStepConditions.Where(a => a.RequestTypeId == requestTypeId && a.Version == version && a.StepId == stepId).ToListAsync();

                    if (conditionHeaders.Count > 0)
                    {
                        //remove details
                        foreach (var conditionHeader in conditionHeaders)
                        {
                            _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(a => a.RequestStepConditionId == conditionHeader.Id));
                            await _dbCntxt.SaveChangesAsync();
                        }
                        //remove header
                        _dbCntxt.RequestStepConditions.RemoveRange(_dbCntxt.RequestStepConditions.Where(r => r.RequestTypeId == requestTypeId && r.Version == version && r.StepId == stepId));
                        await _dbCntxt.SaveChangesAsync();
                    }
                    //Delete steps
                    _dbCntxt.RequestSteps.RemoveRange(_dbCntxt.RequestSteps.Where(r => r.RequestTypeId == requestTypeId && r.Version == version && r.StepId == stepId));
                    await _dbCntxt.SaveChangesAsync();
                    //Delete in WorkflowSteps if the field is not used in request steps 
                    var requestStep = await _dbCntxt.RequestSteps.FirstOrDefaultAsync(r => r.StepId == stepId);

                    if (requestStep == null)
                    {
                        _dbCntxt.WorkFlowSteps.RemoveRange(_dbCntxt.WorkFlowSteps.Where(r => r.Id == stepId));
                        await _dbCntxt.SaveChangesAsync();
                    }
                    //Rearrange sequence from ascending order
                    var steps = await _dbCntxt.RequestSteps
                                            .Where(r => r.RequestTypeId == requestTypeId && r.Version == version)
                                            .OrderBy(r => r.Sequence)
                                            .ToListAsync();

                    if (steps.Count > 0)
                    {
                        int ctr = 1;
                        foreach (var step in steps)
                        {
                            var st = await _dbCntxt.RequestSteps
                                                    .FirstOrDefaultAsync
                                                    (
                                                        r =>
                                                        r.RequestTypeId == step.RequestTypeId &&
                                                        r.Version == step.Version &&
                                                        r.StepId == step.StepId
                                                    );
                            if (st != null)
                            {
                                st.Sequence = ctr;
                                _dbCntxt.Entry(st).State = EntityState.Modified;
                                await _dbCntxt.SaveChangesAsync();
                            }

                            ctr++;
                        }
                    }
                }
                else
                {
                    //Delete all steps
                    //delete the whole request steps
                    var requestSteps = await _dbCntxt.RequestSteps.Where(r => r.RequestTypeId == requestTypeId && r.Version == version).ToListAsync();

                    if (requestSteps.Count <= 0)
                        throw new CustomException("No request step/process to delete.", 404);

                    //Delete parallel workflows

                    //Delete workflows
                    _dbCntxt.RequestTypeWorkFlows.RemoveRange(_dbCntxt.RequestTypeWorkFlows.Where(r => r.RequestTypeId == requestTypeId && r.Version == version));
                    await _dbCntxt.SaveChangesAsync();
                    //Delete Assignees
                    var assigneesHeaders = await _dbCntxt.RequestStepAssignees.Where(a => a.RequestTypeId == requestTypeId && a.Version == version).ToListAsync();

                    if (assigneesHeaders.Count > 0)
                    {
                        //remove details
                        foreach (var assigneesHeader in assigneesHeaders)
                        {
                            _dbCntxt.RequestStepAssigneeDetails.RemoveRange(_dbCntxt.RequestStepAssigneeDetails.Where(a => a.RequestStepAssigneeId == assigneesHeader.Id));
                            await _dbCntxt.SaveChangesAsync();
                        }

                        //remove header
                        _dbCntxt.RequestStepAssignees.RemoveRange(_dbCntxt.RequestStepAssignees.Where(r => r.RequestTypeId == requestTypeId && r.Version == version));
                        await _dbCntxt.SaveChangesAsync();
                    }

                    //Delete Conditions
                    var conditionHeaders = await _dbCntxt.RequestStepConditions.Where(a => a.RequestTypeId == requestTypeId && a.Version == version).ToListAsync();

                    if (conditionHeaders.Count > 0)
                    {
                        //remove details
                        foreach (var conditionHeader in conditionHeaders)
                        {
                            _dbCntxt.RequestStepConditionDetails.RemoveRange(_dbCntxt.RequestStepConditionDetails.Where(a => a.RequestStepConditionId == conditionHeader.Id));
                            await _dbCntxt.SaveChangesAsync();
                        }
                        //remove header
                        _dbCntxt.RequestStepConditions.RemoveRange(_dbCntxt.RequestStepConditions.Where(r => r.RequestTypeId == requestTypeId && r.Version == version));
                        await _dbCntxt.SaveChangesAsync();
                    }

                    //Delete steps
                    var steps = await _dbCntxt.RequestSteps.Where(r => r.RequestTypeId == requestTypeId && r.Version == version).ToListAsync();

                    _dbCntxt.RequestSteps.RemoveRange(_dbCntxt.RequestSteps.Where(r => r.RequestTypeId == requestTypeId && r.Version == version));
                    await _dbCntxt.SaveChangesAsync();

                    foreach (var step in steps)
                    {
                        //Delete in WorkflowSteps if the steps is not used in request steps 
                        var requestStep = await _dbCntxt.RequestSteps.FirstOrDefaultAsync(r => r.StepId == step.StepId);

                        if (requestStep == null)
                        {
                            _dbCntxt.WorkFlowSteps.RemoveRange(_dbCntxt.WorkFlowSteps.Where(r => r.Id == step.StepId));
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }

                var requestTypeInDb = await _dbCntxt.RequestTypes.FirstOrDefaultAsync(r => r.Id == requestTypeId && r.Version == version);

                if (requestTypeInDb != null)
                {
                    // get current user id
                    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    requestTypeInDb.ModifiedByPK = userId;
                    requestTypeInDb.ModifiedDate = DateTime.Now;
                    _dbCntxt.Entry(requestTypeInDb).State = EntityState.Modified;
                    await _dbCntxt.SaveChangesAsync();
                }
                //remove useripperssion
                await _userIpAddressPerSession.Remove(spId);

                dbContextTransaction.Commit();
            }
            catch (CustomException customex)
            {
                dbContextTransaction.Rollback();
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task<ConditionValidationViewModel> ValidateStepConditions(RequestStepConditionViewModel model)
        {
            ConditionValidationViewModel returnOject = new ConditionValidationViewModel();
            try
            {

                returnOject.ReturnMessage = "";
                returnOject.DetailedMessages = null;

                if (String.IsNullOrWhiteSpace(model.Condition))
                {
                    returnOject.ReturnMessage = "Condition is required";
                    returnOject.DetailedMessages = null;
                    return returnOject;
                }

                if (String.IsNullOrWhiteSpace(model.ConditionInText))
                {
                    returnOject.ReturnMessage = "Condition in text is required";
                    returnOject.DetailedMessages = null;
                    return returnOject;
                }

                if (model.ConditionDetails == null)
                {
                    returnOject.ReturnMessage = "Condition is required";
                    returnOject.DetailedMessages = null;
                    return returnOject;
                }

                if (model.ConditionDetails.Count <= 0)
                {
                    returnOject.ReturnMessage = "Condition is required";
                    returnOject.DetailedMessages = null;
                    return returnOject;
                }
                //Check if has duplicate condition to model and database
                var duplicateConditions = this.CheckIfDuplicateConditionInOtherStepInDatabase(model).ToList();

                if (duplicateConditions.Count > 0)
                {
                    List<string> detailedMessages = new List<string>();
                    foreach (var condition in duplicateConditions)
                    {
                        var steps = await this.GetRequestSteps(condition.RequestTypeId, condition.Version, false);

                        var stepInfo = steps.FirstOrDefault(s => s.StepId == condition.StepId);

                        if (stepInfo != null)
                            detailedMessages.Add("Step: " + stepInfo.Sequence.ToString() + " - " + stepInfo.StepName);
                    }

                    returnOject.ReturnMessage = "There are duplicated conditions with another the step(s).";
                    returnOject.DetailedMessages = detailedMessages;
                    return returnOject;
                }

                var conditionSources = await _dbCntxt.Options.Where(s => s.OptionGroup == "Condition Sources").ToListAsync();

                foreach (var conditionDtl in model.ConditionDetails)
                {
                    if (conditionDtl.Source <= 0)
                    {
                        returnOject.ReturnMessage = "Condition Source is required";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    var checkSource = conditionSources.FirstOrDefault(s => s.Id == conditionDtl.Source);

                    if (checkSource == null)
                    {
                        returnOject.ReturnMessage = "Invalid entry for Condition Source";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    bool isField = false;
                    string fieldLov = "";

                    if (checkSource.Name.ToUpper() == "FIELD NAME")
                        isField = true;

                    if (conditionDtl.ReferenceDetail <= 0)
                    {
                        returnOject.ReturnMessage = "Condition Reference is required";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    if (isField)
                    {
                        var fieldInfo = await this.GetGeneralFields();

                        var field = fieldInfo.FirstOrDefault(f => f.FieldId == conditionDtl.ReferenceDetail);

                        if (field == null)
                        {
                            returnOject.ReturnMessage = "Invalid entry for Reference Detail";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                        else
                            fieldLov = field.FieldLOVs;
                    }
                    else
                    {
                        var stepInfo = await this.GetRequestSteps(model.RequestTypeId, model.Version, false);

                        var step = stepInfo.FirstOrDefault(f => f.StepId == conditionDtl.ReferenceDetail);

                        if (step == null)
                        {
                            returnOject.ReturnMessage = "Invalid entry for Reference Detail";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }

                    if (conditionDtl.ComparisonOperatorId <= 0)
                    {
                        returnOject.ReturnMessage = "Condition Comparison Operator is required";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    var operatorInfo = await this.GetComparisonOperators();

                    var optr = operatorInfo.FirstOrDefault(o => o.Id == conditionDtl.ComparisonOperatorId);

                    if (optr == null)
                    {
                        returnOject.ReturnMessage = "Invalid entry for Comparison Operator";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    if (String.IsNullOrWhiteSpace(conditionDtl.Value))
                    {
                        returnOject.ReturnMessage = "Condition Value is required";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    if (conditionDtl.Sequence <= 0)
                    {
                        returnOject.ReturnMessage = "Condition Sequence is required";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    bool conditionSequenceExists = false;
                    if (conditionDtl.Id <= 0)
                    {
                        var seq = await _dbCntxt.RequestStepConditionDetails
                                          .FirstOrDefaultAsync
                                          (
                                             s => s.RequestStepConditionId == conditionDtl.RequestStepConditionId &&
                                             s.Sequence == conditionDtl.Sequence
                                          );
                        if (seq != null)
                            conditionSequenceExists = true;
                    }
                    else
                    {
                        var seq = await _dbCntxt.RequestStepConditionDetails
                                          .FirstOrDefaultAsync
                                          (
                                             s => s.RequestStepConditionId == conditionDtl.RequestStepConditionId &&
                                             s.Sequence == conditionDtl.Sequence &&
                                             s.Id != conditionDtl.Id
                                          );
                        if (seq != null)
                            conditionSequenceExists = true;
                    }

                    if (conditionSequenceExists)
                    {
                        returnOject.ReturnMessage = "Condition Sequence already exists";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    if (isField)
                    {
                        //Check if field LOV value is still active
                        if (fieldLov.Length > 0)
                        {
                            var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(fieldLov);

                            var isActive = lovs.Where(v => v.Name == conditionDtl.Value && v.IsActive == true).FirstOrDefault() != null ? true : false;

                            if (!isActive)
                            {
                                returnOject.ReturnMessage = "Lov value: '" + conditionDtl.Value + "' used in condition is no longer activated.";
                                returnOject.DetailedMessages = null;
                                return returnOject;
                            }
                        }

                        string validationMessage = await this.ValidateFieldForCondition(conditionDtl.ReferenceDetail, conditionDtl.Value, conditionDtl.Value2);

                        if (validationMessage != "")
                        {
                            returnOject.ReturnMessage = validationMessage;
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }

                    if (this.CheckIfDuplicateConditionWithinStepInModel(conditionDtl, model.ConditionDetails))
                    {
                        returnOject.ReturnMessage = "There are duplicated conditions within the step.";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    if (this.CheckIfDuplicateConditionWithinStepInDatabase(conditionDtl))
                    {
                        returnOject.ReturnMessage = "There are duplicated conditions within the step.";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }
                }
                //Validate Logical Operator
                int lastIndex = model.ConditionDetails.Count - 1;

                for (int ctr = model.ConditionDetails.Count - 1; ctr >= 0; ctr--)
                {
                    if (ctr == lastIndex)
                    {
                        var condtn = model.ConditionDetails[ctr];

                        if (!String.IsNullOrWhiteSpace(condtn.LogicalOperator))
                        {
                            returnOject.ReturnMessage = "Condition Logical Operator is not required in the last condition";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }
                    else
                    {
                        var condtn = model.ConditionDetails[ctr];

                        if (String.IsNullOrWhiteSpace(condtn.LogicalOperator))
                        {
                            returnOject.ReturnMessage = "Condition Logical Operator is required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnOject;
        }
        private async Task<string> ValidateStepAssignees(List<RequestStepAssigneeViewModel> model)
        {
            string returnMessage = "";

            try
            {
                //Check if with multiple stepid
                int stepIdFlowDistinctCount = model
                                               .ToList()
                                               .Select(c => c.StepId)
                                               .Distinct()
                                               .Count();

                if (stepIdFlowDistinctCount > 1)
                {
                    returnMessage = "Only one unique step is allowed.";
                    return returnMessage;
                }

                var nonConditionalAssignees = model.FirstOrDefault(a => a.IsConditional == false);

                if (nonConditionalAssignees != null && model.Count > 1)
                {
                    returnMessage = "Atleast one non-conditional assignees is allowed.";
                    return returnMessage;
                }

                //Assignees Validations
                foreach (var assignee in model)
                {
                    if (assignee.IsConditional)
                    {
                        if (assignee.Field1 == null)
                        {
                            returnMessage = "Field1 is required";
                            return returnMessage;
                        }

                        if (String.IsNullOrWhiteSpace(assignee.Value1))
                        {
                            returnMessage = "Field1 value is required";
                            return returnMessage;
                        }

                        var fieldInfo = await this.GetGeneralFields();

                        if (assignee.Field1 != null)
                        {
                            var field = fieldInfo.FirstOrDefault(f => f.FieldId == Convert.ToInt32(assignee.Field1));

                            if (field == null)
                            {
                                returnMessage = "Invalid entry for Field1";
                                return returnMessage;
                            }
                        }

                        if (assignee.Field2 != null)
                        {
                            var field = fieldInfo.FirstOrDefault(f => f.FieldId == Convert.ToInt32(assignee.Field2));

                            if (field == null)
                            {
                                returnMessage = "Invalid entry for Field2";
                                return returnMessage;
                            }

                            if (String.IsNullOrWhiteSpace(assignee.Value2))
                            {
                                returnMessage = "Field2 value is required";
                                return returnMessage;
                            }
                        }

                        if (assignee.Field3 != null)
                        {
                            var field = fieldInfo.FirstOrDefault(f => f.FieldId == Convert.ToInt32(assignee.Field3));

                            if (field == null)
                            {
                                returnMessage = "Invalid entry for Field3";
                                return returnMessage;
                            }

                            if (String.IsNullOrWhiteSpace(assignee.Value3))
                            {
                                returnMessage = "Field3 value is required";
                                return returnMessage;
                            }
                        }

                        //validate same condition
                        //check within the model and within the database
                        if (CheckIfDuplicateAssigneeConditionInModel(assignee, model))
                        {
                            returnMessage = "There are duplicated field value conditions for the conditional assignees.";
                            return returnMessage;
                        }

                        if (CheckIfDuplicateAssigneeConditionInDatabase(assignee))
                        {
                            returnMessage = "There are duplicated field value conditions for the conditional assignees.";
                            return returnMessage;
                        }
                    }
                    else
                    {
                        //Fields and values are not required
                        if (assignee.Field1 != null || assignee.Field2 != null || assignee.Field3 != null)
                        {
                            returnMessage = "Fields for condition is/are not required.";
                            return returnMessage;
                        }

                        if (!String.IsNullOrWhiteSpace(assignee.Value1) || !String.IsNullOrWhiteSpace(assignee.Value2) || !String.IsNullOrWhiteSpace(assignee.Value3))
                        {
                            returnMessage = "Field Values for condition is/are not required.";
                            return returnMessage;
                        }
                    }

                    if (!(GeneralHelper.IsValidAssigneeType(Convert.ToInt32(assignee.AssigneeType))))
                    {
                        returnMessage = "Type of assignee is required";
                        return returnMessage;
                    }

                    if (assignee.RequestStepAssigneeDetails == null)
                    {
                        returnMessage = "Please provide assignee(s) on all LOVs for the field.";
                        return returnMessage;
                    }

                    if (assignee.RequestStepAssigneeDetails.Count <= 0)
                    {
                        returnMessage = "Please provide assignee(s) on all LOVs for the field.";
                        return returnMessage;
                    }

                    if (assignee.AssigneeType == AssigneeType.Position)
                    {
                        if (assignee.RequestStepAssigneeDetails.Count > 1)
                        {
                            returnMessage = "Only one assignee details is requred for assignee type Approval Level.";
                            return returnMessage;
                        }
                    }

                    foreach (var assigneeDtl in assignee.RequestStepAssigneeDetails)
                    {
                        if (String.IsNullOrWhiteSpace(assigneeDtl.Assignee))
                        {
                            returnMessage = "Alteast one assignee is required";
                            return returnMessage;
                        }

                        if (assignee.AssigneeType == AssigneeType.Name)
                        {
                            if (assigneeDtl.Assignee.ToUpper() != "REQUESTOR")
                            {
                                var userAssignee = await _dbCntxt.AspNetUsers
                                                            .FirstOrDefaultAsync(u => u.Id == assigneeDtl.Assignee);

                                if (userAssignee == null)
                                {
                                    returnMessage = "Assignee as user is not found";
                                    return returnMessage;
                                }
                            }
                        }
                        else
                        {

                            if (!(GeneralHelper.IsNumeric(assigneeDtl.Assignee)))
                            {
                                returnMessage = "Please specify a numeric entry for approval level as assignee";
                                return returnMessage;
                            }

                            var approvalLevelAssignee = await _dbCntxt.ApprovalLevels
                                                                      .FirstOrDefaultAsync(a => a.Id == Convert.ToInt32(assigneeDtl.Assignee));

                            if (approvalLevelAssignee == null)
                            {
                                returnMessage = "Assignee as approval level is not found";
                                return returnMessage;
                            }
                        }

                        //check within the model and within the database
                        if (CheckIfDuplicateAssigneesInModel(assigneeDtl, assignee.RequestStepAssigneeDetails))
                        {
                            returnMessage = "Assignee(s) has duplicate entry";
                            return returnMessage;
                        }

                        if (CheckIfDuplicateAssigneesInDatabase(assigneeDtl))
                        {
                            returnMessage = "Assignee(s) has duplicate entry";
                            return returnMessage;
                        }
                    }

                    if (assignee.AssigneeType == AssigneeType.Name)
                    {
                        if (assignee.RequestStepAssigneeDetails.Count < assignee.RequiredAssigneeToExecute)
                        {
                            returnMessage = "Specified assignee(s) must be more than the required assignee(s) to process.";
                            return returnMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnMessage;
        }
        private async Task<WorkFlowValidationViewModel> ValidateWorkFlows(List<RequestTypeWorkFlowViewModel> model, bool? isForNextStep = null)
        {
            WorkFlowValidationViewModel returnOject = new WorkFlowValidationViewModel();

            try
            {
                returnOject.ReturnMessage = "";
                returnOject.DetailedMessages = null;

                //Check if with multiple stepid
                int stepIdFlowDistinctCount = model
                                           .ToList()
                                           .Select(c => c.StepId)
                                           .Distinct()
                                           .Count();

                if (stepIdFlowDistinctCount > 1)
                {
                    returnOject.ReturnMessage = "Only one unique step is allowed.";
                    returnOject.DetailedMessages = null;
                    return returnOject;
                }

                if (isForNextStep != null)
                {
                    if (!Convert.ToBoolean(isForNextStep))
                    {
                        if (model.Count > 1)
                        {
                            returnOject.ReturnMessage = "Only one entry is allowed.";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }
                }

                //Check if with multiple rejected workflow
                int rejectedWorkFlowCount = model
                                            .Where(w => w.NextStepId == null)
                                            .ToList()
                                            .Count();

                if (rejectedWorkFlowCount > 1)
                {
                    returnOject.ReturnMessage = "Only one rejected workflow is allowed.";
                    returnOject.DetailedMessages = null;
                    return returnOject;
                }

                foreach (var workFlow in model)
                {
                    if (workFlow.Action <= 0)
                    {
                        returnOject.ReturnMessage = "Action button is required";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }
                    //get actions
                    var actions = _dbCntxt.WorkFlowStepTypeButtons
                                        .Include(b => b.ActionButton)
                                        .Where(b => b.Id == workFlow.Action && b.ActionButton.OptionGroup == "WorkFlow Step Actions")
                                        .FirstOrDefault();

                    if (actions == null)
                    {
                        returnOject.ReturnMessage = "Invalid action specified";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }

                    if (!actions.IsActionToNextStep)
                    {
                        if (workFlow.NextStepId != null)
                        {
                            returnOject.ReturnMessage = "Next Step is not required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }

                        if (workFlow.NextStepRequestTypeId != null)
                        {
                            returnOject.ReturnMessage = "Next Step Request Type Id is not required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }

                        if (workFlow.NextStepVersion != null)
                        {
                            returnOject.ReturnMessage = "Next Step Version is not required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }

                    if (actions.IsActionToNextStep)
                    {
                        if (workFlow.NextStepId == null)
                        {
                            returnOject.ReturnMessage = "Next Step is required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }

                        if (workFlow.NextStepRequestTypeId == null)
                        {
                            returnOject.ReturnMessage = "Next Step Request Type Id is required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }

                        if (workFlow.NextStepVersion == null)
                        {
                            returnOject.ReturnMessage = "Next Step Version is required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }

                    if (workFlow.NextStepId == null && actions.ActionButton.Name == "Reject")
                    {
                        if (workFlow.TypeWhenRejected == null)
                        {
                            returnOject.ReturnMessage = "Type When Rejected is required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                        //check if action is rejected
                        if (!(GeneralHelper.IsValidTypeWhenRejected(Convert.ToInt32(workFlow.TypeWhenRejected))))
                        {
                            returnOject.ReturnMessage = "Invalid value for Type of When rejected";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }
                    else
                    {
                        if (workFlow.TypeWhenRejected != null)
                        {
                            returnOject.ReturnMessage = "Type When Rejected is not required";
                            returnOject.DetailedMessages = null;
                            return returnOject;
                        }
                    }

                    //Duplicate workflows in model
                    if (this.CheckIfDuplicateWorkFlowInModel(workFlow, model))
                    {
                        returnOject.ReturnMessage = "Workflow step: " + workFlow.StepId + " and Next Step: " + workFlow.NextStepId + " has duplicate entry.";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }
                    //Duplicate workflows in database
                    if (this.CheckIfDuplicateWorkFlowInDatabase(workFlow))
                    {
                        returnOject.ReturnMessage = "Workflow step: " + workFlow.StepId + " and Next Step: " + workFlow.NextStepId + " has duplicate entry.";
                        returnOject.DetailedMessages = null;
                        return returnOject;
                    }
                }

                var nextStepValidationMessage = await this.ValidateSelectedNextSteps(model);

                if (nextStepValidationMessage.Count > 0)
                {
                    returnOject.ReturnMessage = "Selected next step is/are not allowed";
                    returnOject.DetailedMessages = nextStepValidationMessage;
                    return returnOject;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnOject;
        }
        private async Task SaveStepConditions(RequestStepConditionViewModel model)
        {
            try
            {
                var conditionId = model.Id;
                //delete all condition from DB that are not included in current model
                string sqlQuery = @"Declare @RequestTypeId Int, @Version Int, @StepId Int
                                    Set @RequestTypeId = " + model.RequestTypeId + @"
                                    Set @Version = " + model.Version + @"
                                    Set @StepId = " + model.StepId + @"

                                    Select Id into #TempConditions from dbo.RequestStepConditions 
                                    Where 
	                                    RequestTypeId = @RequestTypeId And 
	                                    [Version] = @Version And 
	                                    StepId = @StepId And
	                                    Id Not in (" + conditionId + @")

                                    Delete from dbo.RequestStepConditionDetails Where RequestStepConditionId in (Select Id from #TempConditions)
                                    Delete from dbo.RequestStepConditions Where Id in (Select Id from #TempConditions)

                                    Drop Table #TempConditions";

                _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                await _dbCntxt.SaveChangesAsync();

                //new condition
                var requestStepCondition = new RequestStepCondition();

                if (model.Id <= 0)
                {
                    requestStepCondition.RequestTypeId = model.RequestTypeId;
                    requestStepCondition.Version = model.Version;
                    requestStepCondition.StepId = model.StepId;
                    requestStepCondition.Condition = model.Condition;
                    requestStepCondition.ConditionInText = model.ConditionInText;
                    _dbCntxt.RequestStepConditions.Add(requestStepCondition);
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                {
                    //update condition
                    requestStepCondition = _dbCntxt.RequestStepConditions.FirstOrDefault(r => r.Id == model.Id);

                    if (requestStepCondition != null)
                    {
                        requestStepCondition.Condition = model.Condition;
                        requestStepCondition.ConditionInText = model.ConditionInText;
                        _dbCntxt.Entry(requestStepCondition).State = EntityState.Modified;
                        await _dbCntxt.SaveChangesAsync();
                    }
                }

                model.ConditionDetails
                            .ForEach(r => r.RequestStepConditionId = requestStepCondition.Id);

                var conditionDetailsIds = model.ConditionDetails.Select(c => c.Id);
                var concatConditionDetailsIds = string.Join(",", conditionDetailsIds.Select(x => x.ToString()).ToArray());

                //delete all detailed assignee from DB that are not included in current model
                sqlQuery = @"Declare @RequestStepConditionId Int
                        Set @RequestStepConditionId = " + requestStepCondition.Id + @"

                        Delete from dbo.RequestStepConditionDetails 
                        Where RequestStepConditionId =  @RequestStepConditionId And Id not in (" + concatConditionDetailsIds + @")";

                _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                await _dbCntxt.SaveChangesAsync();

                foreach (var conditionDtl in model.ConditionDetails)
                {
                    var requestStepconditiondetail = new RequestStepConditionDetail();
                    if (conditionDtl.Id <= 0)
                    {
                        requestStepconditiondetail.RequestStepConditionId = conditionDtl.RequestStepConditionId;
                        requestStepconditiondetail.Source = Convert.ToInt32(conditionDtl.Source);
                        requestStepconditiondetail.ReferenceDetail = conditionDtl.ReferenceDetail;
                        requestStepconditiondetail.ComparisonOperatorId = conditionDtl.ComparisonOperatorId;
                        requestStepconditiondetail.LogicalOperator = conditionDtl.LogicalOperator == null ? null : conditionDtl.LogicalOperator.ToUpper();
                        requestStepconditiondetail.Value = conditionDtl.Value;
                        requestStepconditiondetail.Value2 = conditionDtl.Value2;
                        requestStepconditiondetail.Sequence = conditionDtl.Sequence;
                        requestStepconditiondetail.Published = conditionDtl.Published;
                        _dbCntxt.RequestStepConditionDetails.Add(requestStepconditiondetail);
                        await _dbCntxt.SaveChangesAsync();
                    }
                    else
                    {
                        requestStepconditiondetail = _dbCntxt.RequestStepConditionDetails.FirstOrDefault(r => r.Id == conditionDtl.Id);

                        if (requestStepconditiondetail != null)
                        {
                            requestStepconditiondetail.Source = Convert.ToInt32(conditionDtl.Source);
                            requestStepconditiondetail.ReferenceDetail = conditionDtl.ReferenceDetail;
                            requestStepconditiondetail.ComparisonOperatorId = conditionDtl.ComparisonOperatorId;
                            requestStepconditiondetail.LogicalOperator = conditionDtl.LogicalOperator == null ? null : conditionDtl.LogicalOperator.ToUpper();
                            requestStepconditiondetail.Value = conditionDtl.Value;
                            requestStepconditiondetail.Value2 = conditionDtl.Value2;
                            requestStepconditiondetail.Sequence = conditionDtl.Sequence;
                            requestStepconditiondetail.Published = conditionDtl.Published;
                            _dbCntxt.Entry(requestStepconditiondetail).State = EntityState.Modified;
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task SaveStepAssignees(List<RequestStepAssigneeViewModel> model)
        {
            try
            {
                var singleRow = model.FirstOrDefault();

                int requestTypeId = singleRow.RequestTypeId;
                int version = singleRow.Version;
                int stepId = singleRow.StepId;

                var assigneesIds = model.Select(c => c.Id);
                var concatAssigneesIds = string.Join(",", assigneesIds.Select(x => x.ToString()).ToArray());
                //delete all assignee from DB that are not included in current model
                string sqlQuery = @"Declare @RequestTypeId Int, @Version Int, @StepId Int
                                        Set @RequestTypeId = " + requestTypeId + @"
                                        Set @Version = " + version + @"
                                        Set @StepId = " + stepId + @"

                                        Select Id into #TempAssignees from dbo.RequestStepAssignees 
                                        Where 
	                                        RequestTypeId = @RequestTypeId And 
	                                        [Version] = @Version And 
	                                        StepId = @StepId And
	                                        Id Not in (" + concatAssigneesIds + @")

                                        Delete from dbo.RequestStepAssigneeDetails Where RequestStepAssigneeId in (Select Id from #TempAssignees)
                                        Delete from dbo.RequestStepAssignees Where Id in (Select Id from #TempAssignees)

                                        Drop Table #TempAssignees";

                _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                await _dbCntxt.SaveChangesAsync();
                //save assignees
                foreach (var assignee in model)
                {
                    //new assignee
                    var requestStepAssignee = new RequestStepAssignee();

                    if (assignee.Id <= 0)
                    {
                        requestStepAssignee.RequestTypeId = assignee.RequestTypeId;
                        requestStepAssignee.Version = assignee.Version;
                        requestStepAssignee.StepId = assignee.StepId;
                        requestStepAssignee.IsConditional = assignee.IsConditional;
                        requestStepAssignee.AssigneeType = Convert.ToInt32(assignee.AssigneeType);
                        requestStepAssignee.RequiredAssigneeToExecute = assignee.RequiredAssigneeToExecute;
                        requestStepAssignee.Field1 = assignee.Field1;
                        requestStepAssignee.Value1 = assignee.Value1;
                        requestStepAssignee.Field2 = assignee.Field2;
                        requestStepAssignee.Value2 = assignee.Value2;
                        requestStepAssignee.Field3 = assignee.Field3;
                        requestStepAssignee.Value3 = assignee.Value3;

                        _dbCntxt.RequestStepAssignees.Add(requestStepAssignee);
                        await _dbCntxt.SaveChangesAsync();
                    }
                    else
                    {
                        //update assignee
                        requestStepAssignee = _dbCntxt.RequestStepAssignees.FirstOrDefault(r => r.Id == assignee.Id);

                        if (requestStepAssignee != null)
                        {
                            requestStepAssignee.IsConditional = assignee.IsConditional;
                            requestStepAssignee.AssigneeType = Convert.ToInt32(assignee.AssigneeType);
                            requestStepAssignee.RequiredAssigneeToExecute = assignee.RequiredAssigneeToExecute;
                            requestStepAssignee.Field1 = assignee.Field1;
                            requestStepAssignee.Value1 = assignee.Value1;
                            requestStepAssignee.Field2 = assignee.Field2;
                            requestStepAssignee.Value2 = assignee.Value2;
                            requestStepAssignee.Field3 = assignee.Field3;
                            requestStepAssignee.Value3 = assignee.Value3;
                            _dbCntxt.Entry(requestStepAssignee).State = EntityState.Modified;
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }

                    assignee.RequestStepAssigneeDetails
                            .ForEach(r => r.RequestStepAssigneeId = requestStepAssignee.Id);

                    var assigneeDetailsIds = assignee.RequestStepAssigneeDetails.Select(c => c.Id);
                    var concatAssigneeDetailsIds = string.Join(",", assigneeDetailsIds.Select(x => x.ToString()).ToArray());

                    //delete all detailed assignee from DB that are not included in current model
                    sqlQuery = @"Declare @RequestStepAssigneeId Int
                                Set @RequestStepAssigneeId = " + requestStepAssignee.Id + @"

                                Delete from dbo.RequestStepAssigneeDetails 
                                Where RequestStepAssigneeId =  @RequestStepAssigneeId And Id not in (" + concatAssigneeDetailsIds + @")";

                    _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                    await _dbCntxt.SaveChangesAsync();

                    foreach (var assigneeDtl in assignee.RequestStepAssigneeDetails)
                    {
                        var requestStepAssigneeDetail = new RequestStepAssigneeDetail();
                        if (assigneeDtl.Id <= 0)
                        {
                            requestStepAssigneeDetail.RequestStepAssigneeId = assigneeDtl.RequestStepAssigneeId;
                            requestStepAssigneeDetail.Assignee = assigneeDtl.Assignee;
                            _dbCntxt.RequestStepAssigneeDetails.Add(requestStepAssigneeDetail);
                            await _dbCntxt.SaveChangesAsync();
                        }
                        else
                        {
                            requestStepAssigneeDetail = _dbCntxt.RequestStepAssigneeDetails.FirstOrDefault(r => r.Id == assigneeDtl.Id);

                            if (requestStepAssigneeDetail != null)
                            {
                                requestStepAssigneeDetail.Assignee = assigneeDtl.Assignee;
                                _dbCntxt.Entry(requestStepAssigneeDetail).State = EntityState.Modified;
                                await _dbCntxt.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private async Task SaveWorkFlows(List<RequestTypeWorkFlowViewModel> model, bool? isForNextStep = null)
        {
            try
            {
                var singleRow = model.FirstOrDefault();
                int requestTypeId = singleRow.RequestTypeId;
                int version = singleRow.Version;
                int stepId = singleRow.StepId;

                var workFlowIds = model.Select(c => c.Id);
                var concatWorkFlowIds = string.Join(",", workFlowIds.Select(x => x.ToString()).ToArray());

                string sqlQuery = "";

                if (isForNextStep == null || (isForNextStep != null && Convert.ToBoolean(isForNextStep)))
                {
                    //delete all workflows from DB that are not included in current model
                    sqlQuery = @"Declare @RequestTypeId Int, @Version Int, @StepId Int
                                Set @RequestTypeId = " + requestTypeId + @"
                                Set @Version = " + version + @"
                                Set @StepId = " + stepId + @"

                                Select Id into #TempWorkFlows from dbo.RequestTypeWorkFlows 
                                Where 
	                                RequestTypeId = @RequestTypeId And 
	                                [Version] = @Version And 
	                                StepId = @StepId And
	                                Id Not in (" + concatWorkFlowIds + @")

                                Delete from dbo.RequestTypeWorkFlowParallels Where RequestTypeWorkFlowId in (Select Id from #TempWorkFlows)

                                /**Check if remaining parallel is 1 then must be deleted 
                                Select
	                                P.Id
                                Into
	                                #RemainingParallels
                                from
	                                dbo.RequestTypeWorkFlows AS R
	                                inner join  dbo.RequestTypeWorkFlowParallels AS P ON R.Id = P.RequestTypeWorkFlowId
                                Where
	                                R.StepId = @StepId

                                if (Select Count(Id) from #RemainingParallels) = 1 Begin
	                                Delete from dbo.RequestTypeWorkFlowParallels
	                                Where Id in (Select Id from #RemainingParallels)
                                End

                                Drop Table #RemainingParallels **/

                                Delete from dbo.RequestTypeWorkFlows Where Id in (Select Id from #TempWorkFlows)

                                Drop Table #TempWorkFlows";

                    _dbCntxt.Database.ExecuteSqlRaw(sqlQuery);
                    await _dbCntxt.SaveChangesAsync();
                }
                else
                {
                    //for rejected workflow
                    //delete all rejected workflows from DB that are not included in current model
                    sqlQuery = @"Declare @RequestTypeId Int, @Version Int, @StepId Int
                        Set @RequestTypeId = " + requestTypeId + @"
                        Set @Version = " + version + @"
                        Set @StepId = " + stepId + @"

                        Select Id into #TempWorkFlows from dbo.RequestTypeWorkFlows 
                        Where 
	                        RequestTypeId = @RequestTypeId And 
	                        [Version] = @Version And 
	                        StepId = @StepId And
                            Isnull(NextStepId, 0) = 0 And
	                        Id Not in (" + concatWorkFlowIds + @")

                        Delete from dbo.RequestTypeWorkFlows Where Id in (Select Id from #TempWorkFlows)

                        Drop Table #TempWorkFlows";
                }

                foreach (var workFlow in model)
                {
                    RequestTypeWorkFlow requestWorkFlow = new RequestTypeWorkFlow();

                    if (workFlow.Id <= 0)
                    {
                        requestWorkFlow.RequestTypeId = workFlow.RequestTypeId;
                        requestWorkFlow.Version = workFlow.Version;
                        requestWorkFlow.StepId = workFlow.StepId;
                        requestWorkFlow.NextStepRequestTypeId = workFlow.NextStepRequestTypeId;
                        requestWorkFlow.NextStepVersion = workFlow.NextStepVersion;
                        requestWorkFlow.NextStepId = workFlow.NextStepId;
                        requestWorkFlow.Action = workFlow.Action;
                        requestWorkFlow.TypeWhenRejected = workFlow.TypeWhenRejected == null ? null : Convert.ToInt32(workFlow.TypeWhenRejected);

                        _dbCntxt.RequestTypeWorkFlows.Add(requestWorkFlow);
                        await _dbCntxt.SaveChangesAsync();
                    }
                    else
                    {
                        requestWorkFlow = _dbCntxt.RequestTypeWorkFlows.FirstOrDefault(r => r.Id == workFlow.Id);

                        if (requestWorkFlow != null)
                        {
                            requestWorkFlow.NextStepRequestTypeId = workFlow.NextStepRequestTypeId;
                            requestWorkFlow.NextStepVersion = workFlow.NextStepVersion;
                            requestWorkFlow.NextStepId = workFlow.NextStepId;
                            requestWorkFlow.Action = workFlow.Action;
                            requestWorkFlow.TypeWhenRejected = workFlow.TypeWhenRejected == null ? null : Convert.ToInt32(workFlow.TypeWhenRejected);

                            _dbCntxt.Entry(requestWorkFlow).State = EntityState.Modified;
                            await _dbCntxt.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private bool IsInConditionalAssignee(int requestTypeId, int version, int fieldId, string value = "")
        {
            bool returnValue = false;
            List<RequestStepAssignee> stepAssignees = new List<RequestStepAssignee>();

            try
            {
                //Check in Assignees
                if (value != "")
                {
                    stepAssignees = _dbCntxt.RequestStepAssignees
                                            .Where
                                            (
                                                r =>
                                                r.RequestTypeId == requestTypeId &&
                                                r.Version == version &&
                                                r.IsConditional == true &&
                                                (
                                                    (
                                                        r.Field1.HasValue && r.Field1 == fieldId &&
                                                        r.Value1.Trim().ToUpper() == value.Trim().ToUpper()
                                                    ) ||
                                                    (
                                                        r.Field2.HasValue && r.Field2 == fieldId &&
                                                        r.Value2.Trim().ToUpper() == value.Trim().ToUpper()
                                                    ) ||
                                                    (
                                                        r.Field3.HasValue && r.Field3 == fieldId &&
                                                        r.Value3.Trim().ToUpper() == value.Trim().ToUpper()
                                                    )
                                                )
                                            )
                                            .ToList();
                }
                else
                {
                    stepAssignees = _dbCntxt.RequestStepAssignees
                                            .Where
                                            (
                                                r =>
                                                r.RequestTypeId == requestTypeId &&
                                                r.Version == version &&
                                                r.IsConditional == true &&
                                                (
                                                    r.Field1.HasValue && r.Field1 == fieldId ||
                                                    r.Field2.HasValue && r.Field2 == fieldId ||
                                                    r.Field3.HasValue && r.Field3 == fieldId
                                                )
                                            )
                                            .ToList();
                }

                if (stepAssignees.Count > 0)
                    returnValue = true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        private async Task<bool> IsInConditionalAssigneeAsync(int requestTypeId, int version, int fieldId, string value = "")
        {
            bool returnValue = false;
            List<RequestStepAssignee> stepAssignees = new List<RequestStepAssignee>();

            try
            {
                //Check in Assignees
                if (value != "")
                {
                    stepAssignees = await _dbCntxt.RequestStepAssignees
                                            .Where
                                            (
                                                r =>
                                                r.RequestTypeId == requestTypeId &&
                                                r.Version == version &&
                                                r.IsConditional == true &&
                                                (
                                                    (
                                                        r.Field1.HasValue && r.Field1 == fieldId &&
                                                        r.Value1.Trim().ToUpper() == value.Trim().ToUpper()
                                                    ) ||
                                                    (
                                                        r.Field2.HasValue && r.Field2 == fieldId &&
                                                        r.Value2.Trim().ToUpper() == value.Trim().ToUpper()
                                                    ) ||
                                                    (
                                                        r.Field3.HasValue && r.Field3 == fieldId &&
                                                        r.Value3.Trim().ToUpper() == value.Trim().ToUpper()
                                                    )
                                                )
                                            )
                                            .ToListAsync();
                }
                else
                {
                    stepAssignees = await _dbCntxt.RequestStepAssignees
                                            .Where
                                            (
                                                r =>
                                                r.RequestTypeId == requestTypeId &&
                                                r.Version == version &&
                                                r.IsConditional == true &&
                                                (
                                                    r.Field1.HasValue && r.Field1 == fieldId ||
                                                    r.Field2.HasValue && r.Field2 == fieldId ||
                                                    r.Field3.HasValue && r.Field3 == fieldId
                                                )
                                            )
                                            .ToListAsync();
                }

                if (stepAssignees.Count > 0)
                    returnValue = true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        private bool IsInStepCondition(int requestTypeId, int version, int fieldId, string value = "", bool? published = null)
        {
            bool returnValue = false;
            List<RequestStepConditionDetail> steConditions = new List<RequestStepConditionDetail>();

            try
            {
                //Check in Conditions
                var sourceField = _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Field Name").FirstOrDefault();

                if (value != "")
                {
                    steConditions = _dbCntxt.RequestStepConditionDetails
                                             .Include(r => r.ConditionHeader)
                                            .Where
                                            (
                                                r =>
                                                r.ConditionHeader.RequestTypeId == requestTypeId &&
                                                r.ConditionHeader.Version == version &&
                                                r.Source == sourceField.Id &&
                                                (
                                                    r.ReferenceDetail == fieldId &&
                                                    r.Value.Trim().ToUpper() == value.Trim().ToUpper()
                                                )
                                            )
                                            .ToList();
                }
                else
                {
                    steConditions = _dbCntxt.RequestStepConditionDetails
                                             .Include(r => r.ConditionHeader)
                                            .Where
                                            (
                                                r =>
                                                r.ConditionHeader.RequestTypeId == requestTypeId &&
                                                r.ConditionHeader.Version == version &&
                                                r.Source == sourceField.Id &&
                                                (
                                                    r.ReferenceDetail == fieldId
                                                )
                                            )
                                            .ToList();
                }

                if (published != null)
                    steConditions = steConditions.Where(r => r.Published == published).ToList();

                if (steConditions.Count > 0)
                    returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        private async Task<bool> IsInStepConditionAsync(int requestTypeId, int version, int fieldId, string value = "", bool? published = null)
        {
            bool returnValue = false;
            List<RequestStepConditionDetail> steConditions = new List<RequestStepConditionDetail>();

            try
            {
                //Check in Conditions
                var sourceField = await _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Field Name").FirstOrDefaultAsync();

                if (value != "")
                {
                    steConditions = await _dbCntxt.RequestStepConditionDetails
                                                  .Include(r => r.ConditionHeader)
                                                .Where
                                                (
                                                    r =>
                                                    r.ConditionHeader.RequestTypeId == requestTypeId &&
                                                    r.ConditionHeader.Version == version &&
                                                    r.Source == sourceField.Id &&
                                                    (
                                                        r.ReferenceDetail == fieldId &&
                                                        r.Value.Trim().ToUpper() == value.Trim().ToUpper()
                                                    )
                                                )
                                                .ToListAsync();
                }
                else
                {
                    steConditions = await _dbCntxt.RequestStepConditionDetails
                                                  .Include(r => r.ConditionHeader)
                                                .Where
                                                (
                                                    r =>
                                                    r.ConditionHeader.RequestTypeId == requestTypeId &&
                                                    r.ConditionHeader.Version == version &&
                                                    r.Source == sourceField.Id &&
                                                    (
                                                        r.ReferenceDetail == fieldId
                                                    )
                                                )
                                                .ToListAsync();
                }

                if (published != null)
                    steConditions = steConditions.Where(r => r.Published == published).ToList();

                if (steConditions.Count > 0)
                    returnValue = true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        public bool CheckFieldIsInvolvedInProcess(int requestTypeId, int version, int fieldId, bool? published = null)
        {
            bool returnValue = false;

            try
            {
                //Check in Assignees
                if (this.IsInConditionalAssignee(requestTypeId, version, fieldId))
                    returnValue = true;

                //Check in Step Condition
                if (this.IsInStepCondition(requestTypeId, version, fieldId, "", published))
                    returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        public async Task<bool> CheckFieldIsInvolvedInProcessAsync(int requestTypeId, int version, int fieldId, bool? published = null)
        {
            bool returnValue = false;

            try
            {
                //Check in Assignees
                if (await this.IsInConditionalAssigneeAsync(requestTypeId, version, fieldId))
                    returnValue = true;

                //Check in Step Condition
                if (await this.IsInStepConditionAsync(requestTypeId, version, fieldId, "", published))
                    returnValue = true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        public async Task<RequestFormViewModel> GetField(int requestTypeId, int version, int fieldId)
        {
            RequestFormViewModel field = new RequestFormViewModel();
            try
            {
                var generalField = await this.GetGeneralFields();

                field = generalField.FirstOrDefault(f => f.RequestTypeId == requestTypeId && f.Version == version && f.FieldId == fieldId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return field;
        }
        public bool CheckLOVIsInvolvedInProcess(int requestTypeId, int version, int fieldId, string lovValue, bool? published = null)
        {
            bool returnValue = false;

            try
            {
                //Check in Assignees
                if (this.IsInConditionalAssignee(requestTypeId, version, fieldId, lovValue))
                    returnValue = true;

                //Check in Step Condition
                if (this.IsInStepCondition(requestTypeId, version, fieldId, lovValue, published))
                    returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        public async Task<bool> CheckLOVIsInvolvedInProcessAsync(int requestTypeId, int version, int fieldId, string lovValue, bool? published = null)
        {
            bool returnValue = false;

            try
            {
                //Check in Assignees
                if (await this.IsInConditionalAssigneeAsync(requestTypeId, version, fieldId, lovValue))
                    returnValue = true;

                //Check in Step Condition
                if (await this.IsInStepConditionAsync(requestTypeId, version, fieldId, lovValue, published))
                    returnValue = true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        private List<LOVViewModel> ParseFieldLOV(int requestTypeId, int version, int fieldId, string lovString, bool? published = null)
        {
            List<LOVViewModel> returnLOV = new List<LOVViewModel>();

            try
            {
                var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(lovString);

                foreach (var lov in lovs)
                {
                    lov.OldName = lov.Name;
                    lov.OldValue = lov.Value;
                    lov.IsMappedToProcess = this.CheckLOVIsInvolvedInProcess(requestTypeId, version, fieldId, lov.Name, published);
                }

                returnLOV = lovs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnLOV;
        }
        private async Task<List<LOVViewModel>> ParseFieldLOVAsync(int requestTypeId, int version, int fieldId, string lovString, bool? published = null)
        {
            List<LOVViewModel> returnLOV = new List<LOVViewModel>();

            try
            {
                var lovs = JsonSerializer.Deserialize<List<LOVViewModel>>(lovString);

                foreach (var lov in lovs)
                {
                    lov.OldName = lov.Name;
                    lov.OldValue = lov.Value;
                    lov.IsMappedToProcess = await this.CheckLOVIsInvolvedInProcessAsync(requestTypeId, version, fieldId, lov.Name, published);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnLOV;
        }
        private bool IsStepInStepCondition(int requestTypeId, int version, int stepId, bool checkWithinStep, bool? published = null)
        {
            bool returnValue = false;
            List<RequestStepConditionDetail> steConditions = new List<RequestStepConditionDetail>();

            try
            {
                //Check in Conditions
                var sourceStep = _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Step Description").FirstOrDefault();

                if (checkWithinStep)
                {
                    steConditions = _dbCntxt.RequestStepConditionDetails
                                         .Include(r => r.ConditionHeader)
                                        .Where
                                        (
                                            r =>
                                            r.ConditionHeader.RequestTypeId == requestTypeId &&
                                            r.ConditionHeader.Version == version &&
                                            r.Source == sourceStep.Id &&
                                            (
                                                r.ReferenceDetail == stepId
                                            )
                                        )
                                        .ToList();
                }
                else
                {
                    steConditions = _dbCntxt.RequestStepConditionDetails
                                         .Include(r => r.ConditionHeader)
                                        .Where
                                        (
                                            r =>
                                            r.ConditionHeader.RequestTypeId == requestTypeId &&
                                            r.ConditionHeader.Version == version &&
                                            r.ConditionHeader.StepId != stepId &&
                                            r.Source == sourceStep.Id &&
                                            (
                                                r.ReferenceDetail == stepId
                                            )
                                        )
                                        .ToList();
                }

                if (published != null)
                    steConditions = steConditions.Where(r => r.Published == published).ToList();

                if (steConditions.Count > 0)
                    returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        private async Task<bool> IsStepInStepConditionAsync(int requestTypeId, int version, int stepId, bool checkWithinStep, bool? published = null)
        {
            bool returnValue = false;
            List<RequestStepConditionDetail> steConditions = new List<RequestStepConditionDetail>();

            try
            {
                //Check in Conditions
                var sourceStep = await _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources" && o.Name == "Step Description").FirstOrDefaultAsync();

                if (checkWithinStep)
                {
                    steConditions = await _dbCntxt.RequestStepConditionDetails
                                         .Include(r => r.ConditionHeader)
                                        .Where
                                        (
                                            r =>
                                            r.ConditionHeader.RequestTypeId == requestTypeId &&
                                            r.ConditionHeader.Version == version &&
                                            r.Source == sourceStep.Id &&
                                            (
                                                r.ReferenceDetail == stepId
                                            )
                                        )
                                        .ToListAsync();
                }
                else
                {
                    steConditions = await _dbCntxt.RequestStepConditionDetails
                                         .Include(r => r.ConditionHeader)
                                        .Where
                                        (
                                            r =>
                                            r.ConditionHeader.RequestTypeId == requestTypeId &&
                                            r.ConditionHeader.Version == version &&
                                            r.ConditionHeader.StepId != stepId &&
                                            r.Source == sourceStep.Id &&
                                            (
                                                r.ReferenceDetail == stepId
                                            )
                                        )
                                        .ToListAsync();
                }

                if (published != null)
                    steConditions = steConditions.Where(r => r.Published == published).ToList();

                if (steConditions.Count > 0)
                    returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return returnValue;
        }
        private async Task<List<string>> ConstructStepConditions(int headerId, List<RequestStepConditionDetail> conditionDetails)
        {
            List<string> conditions = new List<string>();

            if (conditionDetails.Count > 0)
            {
                var requestStepHeader = await _dbCntxt.RequestStepConditions.FirstOrDefaultAsync(r => r.Id == headerId);
                var comparisonOperators = await this.GetComparisonOperators();
                var fields = await this.GetFields();
                var steps = await this.GetRequestSteps(requestStepHeader.RequestTypeId, requestStepHeader.Version);
                var conditionSources = await _dbCntxt.Options.Where(o => o.OptionGroup == "Condition Sources").ToListAsync();

                string condition = "";
                string conditionInText = "";

                var countPublishedRecord = conditionDetails.Where(c => c.Published == true).ToList().Count();

                string[] logicalOperator = new string[countPublishedRecord];
                string[] expression = new string[countPublishedRecord];
                string[] expressionInText = new string[countPublishedRecord];
                int ctr = 0;

                foreach (var conditionDetail in conditionDetails)
                {
                    if (conditionDetail.Published)
                    {
                        var conditionSource = conditionSources.FirstOrDefault(c => c.Id == conditionDetail.Source);
                        bool isField = false;

                        if (conditionSource != null)
                        {
                            if (conditionSource.Name == "Field Name")
                                isField = true;
                        }

                        var comparisonOperator = comparisonOperators.FirstOrDefault(o => o.Id == conditionDetail.ComparisonOperatorId);

                        string technicalEquivalent = "";

                        var step = new RequestStepViewModel();
                        var field = new RequestFormViewModel();

                        string criteriaName = "";
                        string criteriaNameInText = "";

                        if (!isField)
                        {
                            step = steps.FirstOrDefault(s => s.StepId == conditionDetail.ReferenceDetail);
                            technicalEquivalent = comparisonOperator.TechnicalEquivalent2;
                            criteriaName = "S-" + step.StepId.ToString();
                            criteriaNameInText = step.StepName + " (TAT)";
                        }
                        else
                        {
                            field = fields.FirstOrDefault(f => f.FieldId == conditionDetail.ReferenceDetail);

                            bool isnumericOrCurrency = false;

                            if (field.FieldDataType == "Numeric" || field.FieldDataType == "Currency")
                                isnumericOrCurrency = true;

                            bool useInMultipleTypes = comparisonOperator.UseInMultipleTypes;

                            if (isnumericOrCurrency && useInMultipleTypes)
                                technicalEquivalent = comparisonOperator.TechnicalEquivalent2;
                            else
                                technicalEquivalent = comparisonOperator.TechnicalEquivalent;

                            criteriaName = "F-" + field.FieldId.ToString();
                            criteriaNameInText = field.FieldName;
                        }

                        expression[ctr] = technicalEquivalent;
                        expressionInText[ctr] = "";

                        //Normal expression
                        expression[ctr] = expression[ctr].Replace("[criteria]", "[%%" + criteriaName + "%%]").Replace("[value]", conditionDetail.Value);
                        expressionInText[ctr] = criteriaNameInText + ", " + comparisonOperator.Name + ", " + conditionDetail.Value;

                        if (field.FieldType == FieldType.DateRange && comparisonOperator.Name == "Between")
                        {
                            expression[ctr] = expression[ctr].Replace("[criteria_to]", "[%%" + criteriaName + "-to%%]").Replace("[value2]", conditionDetail.Value2);
                            expressionInText[ctr] = criteriaNameInText + ", " + comparisonOperator.Name + ", " + conditionDetail.Value + " AND " + conditionDetail.Value2;
                        }

                        if (field.FieldType == FieldType.Date && comparisonOperator.Name == "Between")
                        {
                            expression[ctr] = expression[ctr].Replace("[criteria_to]", "[%%" + criteriaName + "%%]").Replace("[value2]", conditionDetail.Value2);
                            expressionInText[ctr] = criteriaNameInText + ", " + comparisonOperator.Name + ", " + conditionDetail.Value + " AND " + conditionDetail.Value2;
                        }

                        logicalOperator[ctr] = string.IsNullOrWhiteSpace(conditionDetail.LogicalOperator) ? "" : conditionDetail.LogicalOperator;

                        ctr++;
                    }
                }

                if (expression.Length == 3)
                {
                    if (logicalOperator[0] == "OR" && logicalOperator[1] == "AND")
                    {
                        condition = "(" + expression[0] + " " + logicalOperator[0] + " " + expression[1] + ") " + logicalOperator[1] + " " + expression[2];
                        conditionInText = "(" + expressionInText[0] + ", " + logicalOperator[0] + ", " + expressionInText[1] + ",) " + logicalOperator[1] + ", " + expressionInText[2];
                    }
                    else if (logicalOperator[0] == "AND" && logicalOperator[1] == "OR")
                    {
                        condition = expression[0] + " " + logicalOperator[0] + " (" + expression[1] + " " + logicalOperator[1] + " " + expression[2] + ")";
                        conditionInText = expressionInText[0] + ", " + logicalOperator[0] + ", (" + expressionInText[1] + ", " + logicalOperator[1] + ", " + expressionInText[2] + ")";
                    }
                    else
                    {
                        condition = expression[0] + " " + logicalOperator[0] + " " + expression[1] + " " + logicalOperator[1] + " " + expression[2];
                        conditionInText = expressionInText[0] + ", " + logicalOperator[0] + ", " + expressionInText[1] + ", " + logicalOperator[1] + ", " + expressionInText[2];
                    }
                }
                else
                {
                    string lastLogicalOperator = "";
                    for (ctr = 0; ctr < expression.Length; ctr++)
                    {
                        condition += (lastLogicalOperator != "" ? " " : "") + lastLogicalOperator + (lastLogicalOperator != "" ? " " : "") + expression[ctr];
                        conditionInText += (lastLogicalOperator != "" ? ", " : "") + lastLogicalOperator + (lastLogicalOperator != "" ? ", " : "") + expressionInText[ctr];

                        lastLogicalOperator = logicalOperator[ctr];
                    }
                }

                conditions.Add(condition);
                conditions.Add(conditionInText);
            }

            return conditions;
        }

        public async Task<IEnumerable<RequestTypeDisplayViewModel>> GetRequestTypesForGrouping(int status = 0, bool? requestCategoryPublished = null)
        {
            List<RequestTypeDisplayViewModel> requestTypes = new List<RequestTypeDisplayViewModel>();
            try
            {
                if (status > 0)
                {
                    if (!GeneralHelper.IsValidRequestTypeStatus(status))
                        throw new CustomException("Invalid Request Type Status.", 400);
                }

                var categories = await _dbCntxt.RequestCategories.ToListAsync();

                if (requestCategoryPublished != null)
                    categories = categories.Where(c => c.Published == requestCategoryPublished).ToList();

                if (categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        var requestType = new RequestTypeDisplayViewModel();

                        requestType.Id = category.Id;
                        requestType.Type = "Category";
                        requestType.Label = category.Name;
                        requestType.Value = category.Id.ToString();
                        requestType.Version = 0;
                        var requestTypesPerCategory = await this.FindAll()
                                                                .Where(r => r.RequestCategoryId == category.Id)
                                                                .OrderBy(r => r.Name)
                                                                .ToListAsync();

                        if (status > 0)
                            requestTypesPerCategory = requestTypesPerCategory
                                                      .Where(r => r.Status == (RequestTypeStatus)status).ToList();

                        requestType.Items = requestTypesPerCategory
                                            .Select
                                            (
                                                r => new RequestTypeDisplayViewModel
                                                {
                                                    Id = r.Id,
                                                    Type = "RequestType",
                                                    Label = r.Name,
                                                    Value = r.Id.ToString(),
                                                    Version = r.Version,
                                                    Items = null
                                                }
                                            ).ToList();

                        if (requestType.Items.Count > 0)
                            requestTypes.Add(requestType);
                    }
                }

                return requestTypes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<RequestTypeViewModel>> GetRequestTypesForTicketCreation(string userId, int requestCategoryId = 0)
        {
            List<RequestTypeViewModel> requestTypes = new List<RequestTypeViewModel>();
            try
            {
                var userRequestTypesGroupings = await _dbCntxt.UserRequestTypesGroupings
                                                              .Include(u => u.RequestTypesGrouping)
                                                              .Include(u => u.RequestTypesGrouping.RequestTypesGroupingDetails)
                                                              .ThenInclude(r => r.RequestTypes)
                                                              .ThenInclude(r => r.RequestCategory)
                                                            .Where
                                                            (
                                                                u => u.UserId == userId &&
                                                                u.Published == true
                                                            )
                                                            .ToListAsync();

                foreach (var userRequestTypesGrouping in userRequestTypesGroupings)
                {
                    var requestTypesGrouping = userRequestTypesGrouping.RequestTypesGrouping;

                    if (requestCategoryId > 0)
                        requestTypes.AddRange(requestTypesGrouping.RequestTypesGroupingDetails
                                                           .Where
                                                           (
                                                                r =>
                                                                r.RequestTypes.RequestCategoryId == requestCategoryId && r.RequestTypes.RequestCategory.Published == true
                                                            )
                                                           .Select
                                                           (
                                                               r => new RequestTypeViewModel
                                                               {
                                                                   Id = r.RequestTypeId,
                                                                   Name = r.RequestTypes.Name,
                                                                   Version = r.Version
                                                               }
                                                           )
                                                           .ToList());
                    else
                        requestTypes.AddRange(requestTypesGrouping.RequestTypesGroupingDetails
                                                           .Where
                                                            (
                                                                r =>
                                                                r.RequestTypes.RequestCategory.Published == true
                                                            )
                                                           .Select
                                                           (
                                                               r => new RequestTypeViewModel
                                                               {
                                                                   Id = r.RequestTypeId,
                                                                   Name = r.RequestTypes.Name,
                                                                   Version = r.Version
                                                               }
                                                           )
                                                           .ToList());

                }

                if (requestTypes.Count > 0)
                {
                    //order by name
                    requestTypes = requestTypes
                                   .OrderBy(r => r.Name).ToList();

                    //group by tp unique
                    requestTypes = requestTypes
                                   .GroupBy
                                   (
                                        r => new
                                        {
                                            r.Id,
                                            r.Name,
                                            r.Version
                                        }
                                   )
                                   .Select(r => r.First())
                                   .ToList();
                }

                return requestTypes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<RequestTypeBaseWorkFlowViewModel>> GetRequestTypeBaseWorkFlows(int requestTypeId, int version, List<TicketRequestDetailViewModel> requestForms, int userOrganizationalStructureId)
        {
            List<RequestTypeBaseWorkFlowViewModel> baseWorkFlows = new List<RequestTypeBaseWorkFlowViewModel>();
            try
            {
                var requestSteps = (await this.GetRequestSteps(requestTypeId, version)).ToList();
                var stepConditions = await this.GetStepConditionsAsync(requestTypeId, version);
                var fields = (await this.GetFields(requestTypeId, version)).ToList();

                foreach (var requestForm in requestForms)
                {
                    requestForm.GeneralField = fields
                                               .Select
                                               (
                                                  f => new GeneralFieldViewModel()
                                                  {
                                                      Id = f.FieldId,
                                                      Slug = f.FieldSlug,
                                                      Name = f.Name,
                                                      FieldType = f.FieldType,
                                                      Description = f.FieldDescription,
                                                      DataType = f.FieldDataType,
                                                      DecimalDigit = f.FieldDecimalDigit
                                                  }
                                               )
                                               .FirstOrDefault(f => f.Id == requestForm.FieldId);

                }

                //Convert ticket request details to data table
                DataTable dtRequestForms = this.ConvertTicketRequestDetailsToDataTable(requestForms);

                var requestWorkFlows = await this.GetRequestTypeProcessWorkFlows(requestTypeId, version);

                var steps = requestWorkFlows.StepNodes.Where(s => s.Id != "s1").ToList();
                var workFlows = requestWorkFlows.LinkNodes.ToList();

                //get all nodes after request submission
                var requestSubmissionNodes = workFlows.Where(w => w.Id.Contains("ws")).ToList();

                //get specific target node
                var requestSubmissionTargetNodes = requestSubmissionNodes.Select(e => e.Target).ToList();

                //remove request submission nodes
                workFlows = workFlows.Where(r => !r.Id.Contains("ws")).ToList();

                //remove letter w in ids
                workFlows.ForEach(w => w.Id = w.Id.Substring(1, w.Id.Length - 1));


                var conditionalSteps = requestSteps.Where(s => s.IsConditional == true)
                                                   .Select(s => s.StepId)
                                                   .ToList();

                var nonConditionalSteps = requestSteps.Where(s => s.IsConditional == false)
                                                      .Select(s => s.StepId)
                                                      .ToList();

                var conditionalPathSteps = requestSubmissionNodes
                                             .Where(w => conditionalSteps.Contains(Convert.ToInt32(w.Target)))
                                             .OrderBy(w => Convert.ToInt32(w.Id.Replace("ws", "")));

                var nonConditionalPathSteps = requestSubmissionNodes
                                             .Where(w => nonConditionalSteps.Contains(Convert.ToInt32(w.Target)))
                                             .OrderBy(w => Convert.ToInt32(w.Id.Replace("ws", "")));

                //conditional steps must be the initial steps before non-conditional
                requestSubmissionNodes = new List<LinkStepViewModel>();

                requestSubmissionNodes.AddRange(conditionalPathSteps);
                requestSubmissionNodes.AddRange(nonConditionalPathSteps);

                bool suitedPath = false;

                int pathNodesCount = requestSubmissionNodes.Count;
                int unMatchedConditionCount = 0;

                foreach (var requestSubmissionNode in requestSubmissionNodes)
                {
                    if (pathNodesCount == unMatchedConditionCount)
                        throw new CustomException("After request submission the system could not identify successors due to conditional issue.", 400);

                    suitedPath = false;

                    int submissionTargetStepNode = Convert.ToInt32(requestSubmissionNode.Target);

                    var entryPointNodeStepInfo = requestSteps.FirstOrDefault(s => s.StepId == Convert.ToInt32(submissionTargetStepNode));
                    var baseWorkFlow = new RequestTypeBaseWorkFlowViewModel();

                    if (entryPointNodeStepInfo.IsConditional)
                    {
                        var condition = stepConditions.FirstOrDefault(s => s.StepId == entryPointNodeStepInfo.StepId);

                        string buildCondition = this.BuildCondition(condition.Condition, fields, requestSteps);

                        if (dtRequestForms.Select(buildCondition).Length <= 0)
                        {
                            unMatchedConditionCount++;
                            continue;
                        }
                    }

                    baseWorkFlow.StepId = entryPointNodeStepInfo.StepId;
                    baseWorkFlow.StepTypeId = entryPointNodeStepInfo.StepTypeId;
                    baseWorkFlow.TAT = entryPointNodeStepInfo.TAT;

                    var assigneeWithThreeConditions = entryPointNodeStepInfo.Assignees.Where(a => a.Field3.HasValue && a.Field3 > 0 && a.IsConditional == true).OrderBy(a => a.Id).ToList();
                    var assigneeWithTwoConditions = entryPointNodeStepInfo.Assignees.Where(a => a.Field2.HasValue && a.Field2 > 0 && !a.Field3.HasValue && a.IsConditional == true).OrderBy(a => a.Id).ToList();
                    var assigneeWithOneCondition = entryPointNodeStepInfo.Assignees.Where(a => a.Field1.HasValue && a.Field1 > 0 && !a.Field2.HasValue && a.IsConditional == true).OrderBy(a => a.Id).ToList();
                    var assigeeNonConditional = entryPointNodeStepInfo.Assignees.Where(a => a.IsConditional == false).ToList();

                    entryPointNodeStepInfo.Assignees = new List<RequestStepAssigneeViewModel>();

                    entryPointNodeStepInfo.Assignees.AddRange(assigneeWithThreeConditions);
                    entryPointNodeStepInfo.Assignees.AddRange(assigneeWithTwoConditions);
                    entryPointNodeStepInfo.Assignees.AddRange(assigneeWithOneCondition);
                    entryPointNodeStepInfo.Assignees.AddRange(assigeeNonConditional);

                    bool assigneeIdFound = false;

                    foreach (var entryPointNodeStepInfoAssignee in entryPointNodeStepInfo.Assignees)
                    {
                        if (entryPointNodeStepInfoAssignee.IsConditional)
                        {
                            int field1 = entryPointNodeStepInfoAssignee.Field1 != null ? Convert.ToInt32(entryPointNodeStepInfoAssignee.Field1) : 0;
                            string value1 = entryPointNodeStepInfoAssignee.Value1;

                            int field2 = entryPointNodeStepInfoAssignee.Field2 != null ? Convert.ToInt32(entryPointNodeStepInfoAssignee.Field2) : 0;
                            string value2 = entryPointNodeStepInfoAssignee.Value2;

                            int field3 = entryPointNodeStepInfoAssignee.Field3 != null ? Convert.ToInt32(entryPointNodeStepInfoAssignee.Field3) : 0;
                            string value3 = entryPointNodeStepInfoAssignee.Value3;

                            string requestFormValue1 = "";
                            string requestFormValue2 = "";
                            string requestFormValue3 = "";

                            FieldType requestFormValue1FieldType = FieldType.TextBox;
                            FieldType requestFormValue2FieldType = FieldType.TextBox;
                            FieldType requestFormValue3FieldType = FieldType.TextBox;

                            if (field1 > 0)
                            {
                                var requestForm1 = requestForms.FirstOrDefault(r => r.FieldId == field1);
                                requestFormValue1 = requestForm1.Value;
                                requestFormValue1FieldType = requestForm1.GeneralField.FieldType;
                            }

                            if (field2 > 0)
                            {
                                var requestForm2 = requestForms.FirstOrDefault(r => r.FieldId == field2);
                                requestFormValue2 = requestForm2.Value;
                                requestFormValue2FieldType = requestForm2.GeneralField.FieldType;
                            }

                            if (field3 > 0)
                            {
                                var requestForm3 = requestForms.FirstOrDefault(r => r.FieldId == field3);
                                requestFormValue3 = requestForm3.Value;
                                requestFormValue3FieldType = requestForm3.GeneralField.FieldType;
                            }

                            if (field1 > 0 && field2 > 0 && field3 > 0)
                            {
                                if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue2FieldType == FieldType.CheckboxList && requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue2 in splittedValuesForm2)
                                        {
                                            foreach (var splittedValue3 in splittedValuesForm3)
                                            {
                                                if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue2.ToUpper() == value2.ToUpper() && splittedValue3.ToUpper() == value3.ToUpper())
                                                    assigneeIdFound = true;

                                                if (assigneeIdFound)
                                                    break;
                                            }

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue3 in splittedValuesForm3)
                                        {
                                            if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue3.ToUpper() == value3.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue2 in splittedValuesForm2)
                                        {
                                            if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue2.ToUpper() == value2.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue2FieldType == FieldType.CheckboxList && requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue2 in splittedValuesForm2)
                                    {
                                        foreach (var splittedValue3 in splittedValuesForm3)
                                        {
                                            if (splittedValue2.ToUpper() == value2.ToUpper() && splittedValue3.ToUpper() == value3.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm1)
                                    {
                                        if (splittedValue.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm2)
                                    {
                                        if (splittedValue.ToUpper() == value2.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm3)
                                    {
                                        if (splittedValue.ToUpper() == value3.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else
                                {
                                    if (requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                        assigneeIdFound = true;
                                }

                            }
                            else if (field1 > 0 && field2 > 0)
                            {
                                if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue2 in splittedValuesForm2)
                                        {
                                            if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue2.ToUpper() == value2.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm1)
                                    {
                                        if (splittedValue.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm2)
                                    {
                                        if (splittedValue.ToUpper() == value2.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else
                                {
                                    if (requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                        assigneeIdFound = true;
                                }
                            }
                            else
                            {
                                if (requestFormValue1FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValues = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValues)
                                    {
                                        if (splittedValue.ToUpper() == value1.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else
                                {
                                    if (requestFormValue1.ToUpper() == value1.ToUpper())
                                        assigneeIdFound = true;
                                }
                            }
                        }
                        else
                            assigneeIdFound = true;

                        if (assigneeIdFound)
                        {
                            baseWorkFlow.AssigneeId = entryPointNodeStepInfoAssignee.Id;
                            baseWorkFlow.AssigneeType = Convert.ToInt32(entryPointNodeStepInfoAssignee.AssigneeType);
                            baseWorkFlow.RequiredToExecute = entryPointNodeStepInfoAssignee.RequiredAssigneeToExecute;

                            if (entryPointNodeStepInfoAssignee.AssigneeType == AssigneeType.Name)
                            {
                                baseWorkFlow.BaseWorkAssignees = entryPointNodeStepInfoAssignee
                                                                .RequestStepAssigneeDetails
                                                                .Select
                                                                (
                                                                    a => new RequestTypeBaseWorkAssigneeFlowViewModel
                                                                    {
                                                                        ApprovalLevelDetailId = null,
                                                                        ApprovalLevelDetail = "",
                                                                        ApprovalLevelId = null,
                                                                        ApprovalLevel = "",
                                                                        UserId = a.Assignee,
                                                                        SortOrder = null
                                                                    }
                                                                ).ToList();
                            }
                            else
                            {
                                var assignees = (await _user.GetUserOrganizationalStructurePath(userOrganizationalStructureId, true, true, true, true, true)).ToList();

                                //This code will be transferred during execution of workflows
                                //int assigneeDetail = Convert.ToInt32(entryPointNodeStepInfoAssignee
                                //                         .RequestStepAssigneeDetails
                                //                         .FirstOrDefault().Assignee);

                                //var assigneeDetailOrganizationalEntityHierarchy = (await _dbCntxt.ApprovalLevels
                                //                                                                .Include(o => o.OrganizationEntity)
                                //                                                                .FirstOrDefaultAsync())
                                //                                                                .OrganizationEntity.Hierarchy;

                                //var approvalLevelInAssignees = assignees.Where(a => a.ApprovalLevelId == assigneeDetail)
                                //                                        .ToList();

                                ////if approval level in request step setup found in the list of possible approvers
                                //if (approvalLevelInAssignees.Count > 0)
                                //{
                                //    var hierarchy = approvalLevelInAssignees.FirstOrDefault().OrganizationalEntityHierarchy;

                                //    if (entryPointNodeStepInfoAssignee.RequiredAssigneeToExecute == 0)
                                //    {
                                //        //upto
                                //        assignees = assignees
                                //                .Where
                                //                (
                                //                    a =>
                                //                    a.OrganizationalEntityHierarchy >= hierarchy
                                //                )
                                //                .OrderBy(a => a.SortOrder)
                                //                .ToList();
                                //    }
                                //    else
                                //    {
                                //        //only one
                                //        assignees = assignees
                                //                .Where
                                //                (
                                //                    a =>
                                //                    a.OrganizationalEntityHierarchy == hierarchy
                                //                )
                                //                .OrderBy(a => a.SortOrder)
                                //                .ToList();
                                //    }
                                //}
                                //else
                                //{
                                //    var lastAssignee = assignees
                                //                        .OrderBy(a => a.SortOrder)
                                //                        .FirstOrDefault();

                                //    if (lastAssignee.OrganizationalEntityHierarchy > assigneeDetailOrganizationalEntityHierarchy)
                                //    {
                                //        if (entryPointNodeStepInfoAssignee.RequiredAssigneeToExecute == 0)
                                //        {
                                //            //upto
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy >= lastAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();
                                //        }
                                //        else
                                //        {
                                //            //only one
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy == lastAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();
                                //        }
                                //    }
                                //    else
                                //    {
                                //        var nextAssignee = assignees
                                //                            .Where(a => a.OrganizationalEntityHierarchy <=           assigneeDetailOrganizationalEntityHierarchy)
                                //                            .OrderBy(a => a.SortOrder)
                                //                            .FirstOrDefault();

                                //        if (entryPointNodeStepInfoAssignee.RequiredAssigneeToExecute == 0)
                                //        {
                                //            //upto
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy >= nextAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();
                                //        }
                                //        else
                                //        {
                                //            //only
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy == nextAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();

                                //        }      
                                //    }
                                //}

                                baseWorkFlow.BaseWorkAssignees = assignees;
                            }

                            break;
                        }

                    }

                    if (!assigneeIdFound)
                    {
                        baseWorkFlow.AssigneeId = 0;
                        baseWorkFlow.AssigneeType = 0;
                        baseWorkFlow.RequiredToExecute = 0;
                        baseWorkFlow.BaseWorkAssignees = new List<RequestTypeBaseWorkAssigneeFlowViewModel>();

                        throw new CustomException("No particular assignee for step: " + entryPointNodeStepInfo.Sequence.ToString() + " - " + entryPointNodeStepInfo.Name, 400);
                    }

                    //public string ApplicableButtons { get; set; }

                    baseWorkFlows.Add(baseWorkFlow);

                    suitedPath = true;

                    baseWorkFlows.AddRange(await GetRequestTypeBaseWorkFlowsPath(submissionTargetStepNode, workFlows, dtRequestForms, fields, requestSteps, requestForms, stepConditions, userOrganizationalStructureId));

                    if (suitedPath)
                        break;
                }

                return baseWorkFlows;
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task<IEnumerable<RequestTypeBaseWorkFlowViewModel>> GetRequestTypeBaseWorkFlowsPath(int stepId, List<LinkStepViewModel> workFlows, DataTable dtRequestForms, List<RequestFormViewModel> fields, List<RequestStepViewModel> requestSteps, List<TicketRequestDetailViewModel> requestForms, List<RequestStepConditionViewModel> stepConditions, int userOrganizationalStructureId)
        {
            List<RequestTypeBaseWorkFlowViewModel> baseWorkFlows = new List<RequestTypeBaseWorkFlowViewModel>();
            try
            {
                var mainStep = requestSteps.FirstOrDefault(s => s.StepId == stepId);

                var pathNodes = workFlows.Where(w => w.Source == stepId.ToString()).ToList();

                var conditionalSteps = requestSteps.Where(s => s.IsConditional == true)
                                                   .Select(s => s.StepId)
                                                   .ToList();

                var nonConditionalSteps = requestSteps.Where(s => s.IsConditional == false)
                                                      .Select(s => s.StepId)
                                                      .ToList();

                var conditionalPathSteps = pathNodes
                                             .Where(w => conditionalSteps.Contains(Convert.ToInt32(w.Target)))
                                             .OrderBy(w => Convert.ToInt32(w.Id.Replace("w", "")));

                var nonConditionalPathSteps = pathNodes
                                             .Where(w => nonConditionalSteps.Contains(Convert.ToInt32(w.Target)))
                                             .OrderBy(w => Convert.ToInt32(w.Id.Replace("w", "")));

                //conditional steps must be the initial steps before non-conditional
                pathNodes = new List<LinkStepViewModel>();

                pathNodes.AddRange(conditionalPathSteps);
                pathNodes.AddRange(nonConditionalPathSteps);

                var baseWorkFlow = new RequestTypeBaseWorkFlowViewModel();

                bool suitedPath = false;

                int pathNodesCount = pathNodes.Count;
                int unMatchedConditionCount = 0; 

                foreach (var pathNode in pathNodes)
                {
                    if (pathNodesCount == unMatchedConditionCount)
                        throw new CustomException("Step: " + mainStep.Sequence + " - " + mainStep.Name + "could not identify successors due to conditional issue.", 400);

                    suitedPath = false;

                    int pathStepNode = Convert.ToInt32(pathNode.Target);

                    var pathNodeStepInfo = requestSteps.FirstOrDefault(s => s.StepId == pathStepNode);
                    baseWorkFlow = new RequestTypeBaseWorkFlowViewModel();

                    if (pathNodeStepInfo.IsConditional)
                    {
                        var condition = stepConditions.FirstOrDefault(s => s.StepId == pathNodeStepInfo.StepId);

                        string buildCondition = this.BuildCondition(condition.Condition, fields, requestSteps);

                        if (dtRequestForms.Select(buildCondition).Length <= 0)
                        {
                            unMatchedConditionCount++;
                            continue;
                        }
                    }

                    baseWorkFlow.StepId = pathNodeStepInfo.StepId;
                    baseWorkFlow.StepTypeId = pathNodeStepInfo.StepTypeId;
                    baseWorkFlow.TAT = pathNodeStepInfo.TAT;

                    var targetAssigneeWithThreeConditions = pathNodeStepInfo.Assignees.Where(a => a.Field3.HasValue && a.Field3 > 0 && a.IsConditional == true).OrderBy(a => a.Id).ToList();
                    var targetAssigneeWithTwoConditions = pathNodeStepInfo.Assignees.Where(a => a.Field2.HasValue && a.Field2 > 0 && !a.Field3.HasValue && a.IsConditional == true).OrderBy(a => a.Id).ToList();
                    var targetAssigneeWithOneCondition = pathNodeStepInfo.Assignees.Where(a => a.Field1.HasValue && a.Field1 > 0 && !a.Field2.HasValue && a.IsConditional == true).OrderBy(a => a.Id).ToList();
                    var targetAssigneeNonConditional = pathNodeStepInfo.Assignees.Where(a => a.IsConditional == false).ToList();

                    pathNodeStepInfo.Assignees = new List<RequestStepAssigneeViewModel>();

                    pathNodeStepInfo.Assignees.AddRange(targetAssigneeWithThreeConditions);
                    pathNodeStepInfo.Assignees.AddRange(targetAssigneeWithTwoConditions);
                    pathNodeStepInfo.Assignees.AddRange(targetAssigneeWithOneCondition);
                    pathNodeStepInfo.Assignees.AddRange(targetAssigneeNonConditional);

                    bool assigneeIdFound = false;
                    foreach (var pathNodeStepInfoAssignee in pathNodeStepInfo.Assignees)
                    {
                        if (pathNodeStepInfoAssignee.IsConditional)
                        {
                            int field1 = pathNodeStepInfoAssignee.Field1 != null ? Convert.ToInt32(pathNodeStepInfoAssignee.Field1) : 0;
                            string value1 = pathNodeStepInfoAssignee.Value1;

                            int field2 = pathNodeStepInfoAssignee.Field2 != null ? Convert.ToInt32(pathNodeStepInfoAssignee.Field2) : 0;
                            string value2 = pathNodeStepInfoAssignee.Value2;

                            int field3 = pathNodeStepInfoAssignee.Field3 != null ? Convert.ToInt32(pathNodeStepInfoAssignee.Field3) : 0;
                            string value3 = pathNodeStepInfoAssignee.Value3;

                            string requestFormValue1 = "";
                            string requestFormValue2 = "";
                            string requestFormValue3 = "";

                            FieldType requestFormValue1FieldType = FieldType.TextBox;
                            FieldType requestFormValue2FieldType = FieldType.TextBox;
                            FieldType requestFormValue3FieldType = FieldType.TextBox;

                            if (field1 > 0)
                            {
                                var requestForm1 = requestForms.FirstOrDefault(r => r.FieldId == field1);
                                requestFormValue1 = requestForm1.Value;
                                requestFormValue1FieldType = requestForm1.GeneralField.FieldType;
                            }

                            if (field2 > 0)
                            {
                                var requestForm2 = requestForms.FirstOrDefault(r => r.FieldId == field2);
                                requestFormValue2 = requestForm2.Value;
                                requestFormValue2FieldType = requestForm2.GeneralField.FieldType;
                            }

                            if (field3 > 0)
                            {
                                var requestForm3 = requestForms.FirstOrDefault(r => r.FieldId == field3);
                                requestFormValue3 = requestForm3.Value;
                                requestFormValue3FieldType = requestForm3.GeneralField.FieldType;
                            }

                            if (field1 > 0 && field2 > 0 && field3 > 0)
                            {
                                if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue2FieldType == FieldType.CheckboxList && requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue2 in splittedValuesForm2)
                                        {
                                            foreach (var splittedValue3 in splittedValuesForm3)
                                            {
                                                if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue2.ToUpper() == value2.ToUpper() && splittedValue3.ToUpper() == value3.ToUpper())
                                                    assigneeIdFound = true;

                                                if (assigneeIdFound)
                                                    break;
                                            }

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue3 in splittedValuesForm3)
                                        {
                                            if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue3.ToUpper() == value3.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue2 in splittedValuesForm2)
                                        {
                                            if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue2.ToUpper() == value2.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue2FieldType == FieldType.CheckboxList && requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue2 in splittedValuesForm2)
                                    {
                                        foreach (var splittedValue3 in splittedValuesForm3)
                                        {
                                            if (splittedValue2.ToUpper() == value2.ToUpper() && splittedValue3.ToUpper() == value3.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm1)
                                    {
                                        if (splittedValue.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm2)
                                    {
                                        if (splittedValue.ToUpper() == value2.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue3FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm3 = requestFormValue3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm3)
                                    {
                                        if (splittedValue.ToUpper() == value3.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else
                                {
                                    if (requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper() && requestFormValue3.ToUpper() == value3.ToUpper())
                                        assigneeIdFound = true;
                                }

                            }
                            else if (field1 > 0 && field2 > 0)
                            {
                                if (requestFormValue1FieldType == FieldType.CheckboxList && requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue1 in splittedValuesForm1)
                                    {
                                        foreach (var splittedValue2 in splittedValuesForm2)
                                        {
                                            if (splittedValue1.ToUpper() == value1.ToUpper() && splittedValue2.ToUpper() == value2.ToUpper())
                                                assigneeIdFound = true;

                                            if (assigneeIdFound)
                                                break;
                                        }

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue1FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm1 = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm1)
                                    {
                                        if (splittedValue.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else if (requestFormValue2FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValuesForm2 = requestFormValue2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValuesForm2)
                                    {
                                        if (splittedValue.ToUpper() == value2.ToUpper() && requestFormValue1.ToUpper() == value1.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else
                                {
                                    if (requestFormValue1.ToUpper() == value1.ToUpper() && requestFormValue2.ToUpper() == value2.ToUpper())
                                        assigneeIdFound = true;
                                }
                            }
                            else
                            {
                                if (requestFormValue1FieldType == FieldType.CheckboxList)
                                {
                                    var splittedValues = requestFormValue1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var splittedValue in splittedValues)
                                    {
                                        if (splittedValue.ToUpper() == value1.ToUpper())
                                            assigneeIdFound = true;

                                        if (assigneeIdFound)
                                            break;
                                    }
                                }
                                else
                                {
                                    if (requestFormValue1.ToUpper() == value1.ToUpper())
                                        assigneeIdFound = true;
                                }
                            }
                        }
                        else
                            assigneeIdFound = true;

                        if (assigneeIdFound)
                        {
                            baseWorkFlow.AssigneeId = pathNodeStepInfoAssignee.Id;
                            baseWorkFlow.AssigneeType = Convert.ToInt32(pathNodeStepInfoAssignee.AssigneeType);
                            baseWorkFlow.RequiredToExecute = pathNodeStepInfoAssignee.RequiredAssigneeToExecute;

                            if (pathNodeStepInfoAssignee.AssigneeType == AssigneeType.Name)
                            {
                                baseWorkFlow.BaseWorkAssignees = pathNodeStepInfoAssignee
                                                                .RequestStepAssigneeDetails
                                                                .Select
                                                                (
                                                                    a => new RequestTypeBaseWorkAssigneeFlowViewModel
                                                                    {
                                                                        ApprovalLevelDetailId = null,
                                                                        ApprovalLevelDetail = "",
                                                                        ApprovalLevelId = null,
                                                                        ApprovalLevel = "",
                                                                        UserId = a.Assignee,
                                                                        SortOrder = null
                                                                    }
                                                                ).ToList();
                            }
                            else
                            {
                                var assignees = (await _user.GetUserOrganizationalStructurePath(userOrganizationalStructureId, true, true, true, true, true)).ToList();

                                //int assigneeDetail = Convert.ToInt32(pathNodeStepInfoAssignee
                                //                         .RequestStepAssigneeDetails
                                //                         .FirstOrDefault().Assignee);

                                //var assigneeDetailOrganizationalEntityHierarchy = (await _dbCntxt.ApprovalLevels
                                //                                                                .Include(o => o.OrganizationEntity)
                                //                                                                .FirstOrDefaultAsync())
                                //                                                                .OrganizationEntity.Hierarchy;

                                //var approvalLevelInAssignees = assignees.Where(a => a.ApprovalLevelId == assigneeDetail)
                                //                                        .ToList();

                                //if (approvalLevelInAssignees.Count > 0)
                                //{
                                //    var hierarchy = approvalLevelInAssignees.FirstOrDefault().OrganizationalEntityHierarchy;

                                //    if (pathNodeStepInfoAssignee.RequiredAssigneeToExecute == 0)
                                //    {
                                //        //upto
                                //        assignees = assignees
                                //                .Where
                                //                (
                                //                    a =>
                                //                    a.OrganizationalEntityHierarchy >= hierarchy
                                //                )
                                //                .OrderBy(a => a.SortOrder)
                                //                .ToList();
                                //    }
                                //    else
                                //    {
                                //        //only one
                                //        assignees = assignees
                                //                .Where
                                //                (
                                //                    a =>
                                //                    a.OrganizationalEntityHierarchy == hierarchy
                                //                )
                                //                .OrderBy(a => a.SortOrder)
                                //                .ToList();
                                //    }
                                //}
                                //else
                                //{
                                //    var lastAssignee = assignees
                                //                        .OrderBy(a => a.SortOrder)
                                //                        .FirstOrDefault();

                                //    if (lastAssignee.OrganizationalEntityHierarchy > assigneeDetailOrganizationalEntityHierarchy)
                                //    {
                                //        if (pathNodeStepInfoAssignee.RequiredAssigneeToExecute == 0)
                                //        {
                                //            //upto
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy >= lastAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();
                                //        }
                                //        else
                                //        {
                                //            //only one
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy == lastAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();
                                //        }
                                //    }
                                //    else
                                //    {
                                //        var nextAssignee = assignees
                                //                            .Where(a => a.OrganizationalEntityHierarchy <= assigneeDetailOrganizationalEntityHierarchy)
                                //                            .OrderBy(a => a.SortOrder)
                                //                            .FirstOrDefault();

                                //        if (pathNodeStepInfoAssignee.RequiredAssigneeToExecute == 0)
                                //        {
                                //            //upto
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy >= nextAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();
                                //        }
                                //        else
                                //        {
                                //            //only
                                //            assignees = assignees
                                //                    .Where
                                //                    (
                                //                        a =>
                                //                        a.OrganizationalEntityHierarchy == nextAssignee.OrganizationalEntityHierarchy
                                //                    )
                                //                    .OrderBy(a => a.SortOrder)
                                //                    .ToList();

                                //        }
                                //    }
                                //}

                                baseWorkFlow.BaseWorkAssignees = assignees;
                            }

                            break;
                        }
                    }

                    if (!assigneeIdFound)
                    {
                        baseWorkFlow.AssigneeId = 0;
                        baseWorkFlow.AssigneeType = 0;
                        baseWorkFlow.RequiredToExecute = 0;
                        baseWorkFlow.BaseWorkAssignees = new List<RequestTypeBaseWorkAssigneeFlowViewModel>();

                        throw new CustomException("No particular assignee for step: " + pathNodeStepInfo.Sequence.ToString() + " - " + pathNodeStepInfo.Name, 400);
                    }

                    //public string ApplicableButtons { get; set; }

                    baseWorkFlows.Add(baseWorkFlow);

                    suitedPath = true;

                    baseWorkFlows.AddRange(await GetRequestTypeBaseWorkFlowsPath(pathStepNode, workFlows, dtRequestForms, fields, requestSteps, requestForms, stepConditions, userOrganizationalStructureId));

                    if (suitedPath)
                        break;
                }

                return baseWorkFlows;
            }
            catch (CustomException customex)
            {
                throw new CustomException(customex.Message, customex.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private DataTable ConvertTicketRequestDetailsToDataTable (List<TicketRequestDetailViewModel> requestForms)
        {
            DataSet dsCheckBoxes = new DataSet();
            DataTable dtConvertedDetails = new DataTable();
            DataTable dtConvertedNoCheckbox = new DataTable();

            if (requestForms.Count > 0)
            {
                var requestFormNoCheckBox = requestForms.Where(r => r.GeneralField.FieldType != FieldType.CheckboxList).ToList();

                var requestFormCheckBoxes = requestForms.Where(r => r.GeneralField.FieldType == FieldType.CheckboxList).ToList();

                if (requestFormNoCheckBox.Count > 0)
                {
                    //Build the columns
                    foreach (var requestForm in requestFormNoCheckBox)
                    {
                        if (requestForm.GeneralField.DataType == "Numeric" || requestForm.GeneralField.DataType == "Currency")
                        {
                            if (requestForm.GeneralField.DataType == "Currency")
                                dtConvertedNoCheckbox.Columns.Add(requestForm.GeneralField.Slug, typeof(double));
                            else
                            {
                                int decimalDigit = requestForm.GeneralField.DecimalDigit == null ? 0 : Convert.ToInt32(requestForm.GeneralField.DecimalDigit);

                                if (decimalDigit > 0)
                                    //double data type
                                    dtConvertedNoCheckbox.Columns.Add(requestForm.GeneralField.Slug, typeof(double));
                                else
                                    //integer data type
                                    dtConvertedNoCheckbox.Columns.Add(requestForm.GeneralField.Slug, typeof(int));
                            }

                        }
                        else
                            dtConvertedNoCheckbox.Columns.Add(requestForm.GeneralField.Slug, typeof(string));

                        if (requestForm.GeneralField.FieldType == FieldType.DateRange)
                            dtConvertedNoCheckbox.Columns.Add(requestForm.GeneralField.Slug + "_To");

                        dtConvertedNoCheckbox.AcceptChanges();
                    }

                    //Insert rows
                    DataRow drConverted;
                    drConverted = dtConvertedNoCheckbox.NewRow();

                    int rowCount = 0;
                    foreach (var requestForm in requestFormNoCheckBox)
                    {
                        if (requestForm.GeneralField.DataType == "Numeric" || requestForm.GeneralField.DataType == "Currency")
                        {
                            if (requestForm.GeneralField.DataType == "Currency")
                                drConverted[rowCount] = Convert.ToDouble(requestForm.Value);
                            else
                            {
                                int decimalDigit = requestForm.GeneralField.DecimalDigit == null ? 0 : Convert.ToInt32(requestForm.GeneralField.DecimalDigit);

                                if (decimalDigit > 0)
                                    //double data type
                                    drConverted[rowCount] = Convert.ToDouble(requestForm.Value);
                                else
                                    //integer data type
                                    drConverted[rowCount] = Convert.ToInt32(requestForm.Value);
                            }
                        }
                        else
                            drConverted[rowCount] = requestForm.Value;

                        if (requestForm.GeneralField.FieldType == FieldType.DateRange)
                        {
                            rowCount++;
                            drConverted[rowCount] = requestForm.Value2;
                        }

                        rowCount++;
                    }

                    dtConvertedNoCheckbox.Rows.Add(drConverted);
                }

                if (requestFormCheckBoxes.Count > 0)
                {
                    foreach(var requestForm in requestFormCheckBoxes)
                    {
                        DataTable dtCheckBox = new DataTable();

                        if (requestForm.GeneralField.DataType == "Numeric" || requestForm.GeneralField.DataType == "Currency")
                        {
                            if (requestForm.GeneralField.DataType == "Currency")
                                dtCheckBox.Columns.Add(requestForm.GeneralField.Slug, typeof(double));
                            else
                            {
                                int decimalDigit = requestForm.GeneralField.DecimalDigit == null ? 0 : Convert.ToInt32(requestForm.GeneralField.DecimalDigit);

                                if (decimalDigit > 0)
                                    //double data type
                                    dtCheckBox.Columns.Add(requestForm.GeneralField.Slug, typeof(double));
                                else
                                    //integer data type
                                    dtCheckBox.Columns.Add(requestForm.GeneralField.Slug, typeof(int));
                            }

                        }
                        else
                            dtCheckBox.Columns.Add(requestForm.GeneralField.Slug, typeof(string));

                        if (requestForm.GeneralField.FieldType == FieldType.DateRange)
                            dtCheckBox.Columns.Add(requestForm.GeneralField.Slug + "_To");

                        dtCheckBox.AcceptChanges();

                        var parsedValues = requestForm.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach(var parseValue in parsedValues)
                        {
                            DataRow dr;

                            dr = dtCheckBox.NewRow();
                            dr[0] = parseValue;

                            if (requestForm.GeneralField.DataType == "Numeric" || requestForm.GeneralField.DataType == "Currency")
                            {
                                if (requestForm.GeneralField.DataType == "Currency")
                                    dr[0] = Convert.ToDouble(parseValue);
                                else
                                {
                                    int decimalDigit = requestForm.GeneralField.DecimalDigit == null ? 0 : Convert.ToInt32(requestForm.GeneralField.DecimalDigit);

                                    if (decimalDigit > 0)
                                        //double data type
                                        dr[0] = Convert.ToDouble(parseValue);
                                    else
                                        //integer data type
                                        dr[0] = Convert.ToInt32(parseValue);
                                }
                            }
                            else
                                dr[0] = parseValue;

                            dtCheckBox.Rows.Add(dr);
                        }

                        dtCheckBox.AcceptChanges();

                        dsCheckBoxes.Tables.Add(dtCheckBox);

                        dsCheckBoxes.AcceptChanges();
                    }
                }

                if (dtConvertedNoCheckbox.Rows.Count > 0)
                    dtConvertedDetails = dtConvertedNoCheckbox;

                if (dsCheckBoxes.Tables.Count > 0)
                {   
                    for(int counter = 0; counter < dsCheckBoxes.Tables.Count; counter++)
                    {
                        if (dtConvertedNoCheckbox.Rows.Count > 0)
                        {
                            DataTable dt = new DataTable();

                            dt = dtConvertedDetails.Copy();

                            var results = (from dt1 in dt.AsEnumerable()
                                            from dt2 in dsCheckBoxes.Tables[counter].AsEnumerable()
                                            select new { dt1, dt2 }).ToList();

                            dtConvertedDetails.Rows.Clear();

                            Type columnType = dsCheckBoxes.Tables[counter].Columns[0].DataType;

                            dtConvertedDetails.Columns.Add(dsCheckBoxes.Tables[counter].Columns[0].ColumnName, columnType);

                            foreach (var result in results)
                            {
                                int count = result.dt1.ItemArray.Length;

                                DataRow dr;
                                dr = dtConvertedDetails.NewRow();
                                int ctr = 0;
                                for (int cnt = 0; cnt < count; cnt++)
                                {
                                    dr[cnt] = result.dt1.ItemArray[cnt];

                                    ctr = cnt;
                                }

                                dr[ctr + 1] = result.dt2.ItemArray[0];

                                dtConvertedDetails.Rows.Add(dr);
                            }

                            dtConvertedDetails.AcceptChanges();
                        }
                        else
                        {
                            if (counter == 0)
                                dtConvertedDetails = dsCheckBoxes.Tables[counter];
                            else
                            {
                                DataTable dt = new DataTable();

                                dt = dtConvertedDetails.Copy();

                                var results = (from dt1 in dt.AsEnumerable()
                                               from dt2 in dsCheckBoxes.Tables[counter].AsEnumerable()
                                               select new { dt1, dt2 }).ToList();

                                dtConvertedDetails.Rows.Clear();

                                Type columnType = dsCheckBoxes.Tables[counter].Columns[0].DataType;

                                dtConvertedDetails.Columns.Add(dsCheckBoxes.Tables[counter].Columns[0].ColumnName, columnType);

                                foreach (var result in results)
                                {
                                    int count = result.dt1.ItemArray.Length;

                                    DataRow dr;
                                    dr = dtConvertedDetails.NewRow();
                                    int ctr = 0;
                                    for (int cnt = 0; cnt < count; cnt++)
                                    {
                                        dr[cnt] = result.dt1.ItemArray[cnt];

                                        ctr = cnt;
                                    }

                                    dr[ctr + 1] = result.dt2.ItemArray[0];

                                    dtConvertedDetails.Rows.Add(dr);
                                }

                                dtConvertedDetails.AcceptChanges();

                            }
                        }
                    }
                }
            }

            return dtConvertedDetails;
        }

        private string BuildCondition(string condition, List<RequestFormViewModel> fields, List<RequestStepViewModel> steps)
        {
            string finalcondition = "";

            string mystringtobeparsed = condition;

            while (mystringtobeparsed.Contains("["))
            {
                int firstindex = mystringtobeparsed.IndexOf("[");
                int secondindex = mystringtobeparsed.IndexOf("]");

                var myvar = mystringtobeparsed.Substring(firstindex, ((secondindex - firstindex) + 1));

                mystringtobeparsed = mystringtobeparsed.Replace(myvar, "");

                string replacedvar = myvar.Replace("[%%", "").Replace("%%]", "");

                string slug = "";

                bool isField = false;
                if (replacedvar.Contains("F-"))
                    isField = true;

                replacedvar = replacedvar.Replace("F-", "").Replace("S-", "");

                if (isField)
                {
                    if (replacedvar.Contains("-to"))
                    {
                        replacedvar = replacedvar.Replace("-to", "");

                        slug = fields.FirstOrDefault(c => c.Id == Convert.ToInt32(replacedvar)).FieldSlug;
                        slug += "_To";
                    }
                    else
                    {
                        slug = fields.FirstOrDefault(c => c.Id == Convert.ToInt32(replacedvar)).FieldSlug;
                    }
                }
                else
                    slug = steps.FirstOrDefault(c => c.Id == Convert.ToInt32(replacedvar)).StepSlug;
                
                condition = condition.Replace(myvar, slug);
            }

            finalcondition = condition;

            return finalcondition;
        }
    }
}
