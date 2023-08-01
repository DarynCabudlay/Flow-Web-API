using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using workflow.Models.ManageViewModels;

namespace workflow.Helpers
{
    public class GeneralHelper
    {
        public static Guid GenerateGuid()
        {
            Guid newguid = new Guid();
            return newguid;
        }

        //For image extension to be saved in DB
        public static string GenerateGuidForImage(string prefix = null)
        {
            Guid newguid = GenerateGuid();

            string GuidString = Convert.ToBase64String(newguid.ToByteArray());
            GuidString = GuidString.Replace("=", "")
                                   .Replace("+", "");

            return (prefix == null ? "?r=" : prefix) + GuidString;
        }

        //For GUID primary key in DB
        public static string GenerateGuidAsUniqueKey()
        {
            return Guid.NewGuid().ToString();
        }

        public static LoginReturnViewModel SetLoginReturn (int code, int LoginAttempt = 0, int LoginAttemptInSetup = 0)
        {
            LoginReturnViewModel details = new LoginReturnViewModel()
            {
                Code = code,
                LoginAttempt = LoginAttempt,
                LoginAttemptInSetup = LoginAttemptInSetup
            };

            return details;
        }

        public static APIReturnObject SetReturnDetails(int code, string message, object details = null, List<string> listmessage = null)
        {
            APIReturnObject obj = new APIReturnObject()
            {
                Code = code,
                Message = message,
                Details = details,
                ListMessage = listmessage
            };

            return obj;
        }

        public static bool IsValidUserAccountState(int state)
        {
            bool bolreturn = true;

            if (!Enum.IsDefined(typeof(UserAccountState), state))
                bolreturn = false;

            return bolreturn;
        }

        public static bool IsValidFieldType(int value)
        {
            bool bolreturn = true;

            if (!Enum.IsDefined(typeof(FieldType), value))
                bolreturn = false;

            return bolreturn;
        }

        public static bool IsValidRequestTypeStatus(int value)
        {
            bool bolreturn = true;

            if (!Enum.IsDefined(typeof(RequestTypeStatus), value))
                bolreturn = false;

            return bolreturn;
        }

        public static bool IsValidAssigneeType(int value)
        {
            bool bolreturn = true;

            if (!Enum.IsDefined(typeof(AssigneeType), value))
                bolreturn = false;

            return bolreturn;
        }
        //public static bool IsValidConditionSource(int value)
        //{
        //    bool bolreturn = true;

        //    if (!Enum.IsDefined(typeof(ConditionSource), value))
        //        bolreturn = false;

        //    return bolreturn;
        //}

        public static bool IsValidTypeWhenRejected(int value)
        {
            bool bolreturn = true;

            if (!Enum.IsDefined(typeof(TypeWhenRejected), value))
                bolreturn = false;

            return bolreturn;
        }

        public static string CreateRandomPassword(int length = 10)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            //string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public static bool IsValidEmail(string email)
        {
            bool bolreturn = true;
            try
            {
                Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                RegexOptions.CultureInvariant | RegexOptions.Singleline);

                bool isValidEmail = regex.IsMatch(email);
                if (!isValidEmail)
                {
                    bolreturn = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return bolreturn;

        }

        public static bool IsNumeric(string value, int decimaldigit, bool iscurrency)
        {
            if (decimaldigit > 0)
            {
                if (iscurrency)
                    return Regex.IsMatch(value, @"^((\-?)|(\+?))([\d]+)?(\.([\d]+)?)?$");
                //return Regex.IsMatch(value, @"^\-?[\d]+(?:\.[\d]{1," + decimaldigit + @"})?$");
                else
                    return Regex.IsMatch(value, @"^((\-?)|(\+?))([\d]+)?(\.([\d]+)?)?%?$");
                //return Regex.IsMatch(value, @"^\-?[\d]+(?:\.[\d]{1," + decimaldigit + @"})?%?$");
            }
            else
            {
                if (iscurrency)
                    return Regex.IsMatch(value, @"^((\-?)|(\+?))[\d]+$");
                else
                    return Regex.IsMatch(value, @"^((\-?)|(\+?))[\d]+%?$");
            }
        }

        public static bool IsAlpha(string value)
        {
            return Regex.IsMatch(value, @"\A[a-zA-Z\s]+\Z");
        }

        public static string ReconstructValue(string value, int decimaldigit)
        {
            string returnvalue = "";

            if (value.StartsWith("."))
                //start with . then put 0 in the beginning
                value = "0" + value;
            else
            {
                if (value.StartsWith("+") || (value.StartsWith("-")))
                {
                    //start with +/-
                    if (value.Length > 1)
                    {
                        //Default value should be greater than 1 since -/+ as single value is invalid
                        if (value.Substring(1, 1) == ".")
                        {
                            if (value.StartsWith("+"))
                                value = value.Replace("+.", "+0.");
                            else
                                value = value.Replace("-.", "-0.");
                        }
                    }
                }
            }

            if (value.EndsWith("."))
            {
                //ends with . then put 0 after
                string concatzeros = "";

                if (decimaldigit == 5)
                    concatzeros = "00000";
                else if (decimaldigit == 4)
                    concatzeros = "0000";
                else if (decimaldigit == 3)
                    concatzeros = "000";
                else if (decimaldigit == 2)
                    concatzeros = "00";
                else
                    concatzeros = "0";

                value = value + concatzeros;
            }

            returnvalue = value;

            return returnvalue;
        }

        public static string GetFieldTypeShorText(FieldType fieldType)
        {
            if (fieldType == FieldType.TextBox)
                return "txt";
            else if (fieldType == FieldType.Dropdown)
                return "drp";
            else if (fieldType == FieldType.Date)
                return "dte";
            else if (fieldType == FieldType.CheckboxList)
                return "chk";
            else if (fieldType == FieldType.RadioButton)
                return "rdb";
            else if (fieldType == FieldType.Email)
                return "eml";
            else if (fieldType == FieldType.TextArea)
                return "txa";
            else if (fieldType == FieldType.Url)
                return "url";
            else
                return "dtr";
        }

        public static string GetStepTypeShorText(string steptype)
        {
            if (steptype == "Approval")
                return "app";
            else if (steptype == "Activity")
                return "act";
            else if (steptype == "Review")
                return "rev";
            else
                return "not";
        }

        public static bool IsValidDate(string datestring)
        {
            DateTime temp;
            if (DateTime.TryParse(datestring, out temp))
            {
                return true;
            }
            return false;
        }

        public static string GetDefaultValue(string values)
        {
            string returnvalue = "";

            List<LOVViewModel> LOVs = JsonSerializer.Deserialize<List<LOVViewModel>>(values);

            foreach(var lov in LOVs)
            {
                if (lov.IsDefault)
                {
                    returnvalue = lov.Value;
                    break;
                }
            }

            return returnvalue;
        }

        public static bool IsNumeric(string text)
        {
            double test;
            return double.TryParse(text, out test);
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static bool IsValidTicketAttachmentCategory(int value)
        {
            bool bolreturn = true;

            if (!Enum.IsDefined(typeof(TicketAttachmentCategory), value))
                bolreturn = false;

            return bolreturn;
        }
        public static bool IsValidTicketStatus(int value)
        {
            bool bolreturn = true;

            if (!Enum.IsDefined(typeof(TicketStatus), value))
                bolreturn = false;

            return bolreturn;
        }

    }
}
