using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RequestFormViewModel
    {
        public int RequestTypeId { get; set; }
        public int OldFieldId { get; set; }
        public int FieldId { get; set; }
        public string FieldSlug { get; set; }
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; }
        public string FieldTypeDescription
        {
            get
            {
                if (FieldType == FieldType.TextBox)
                    return "textbox";
                else if (FieldType == FieldType.Dropdown)
                    return "dropdown";
                else if (FieldType == FieldType.Date)
                    return "date";
                else if (FieldType == FieldType.CheckboxList)
                    return "checkboxlist";
                else if (FieldType == FieldType.RadioButton)
                    return "radiobutton";
                else if (FieldType == FieldType.Email)
                    return "email";
                else if (FieldType == FieldType.TextArea)
                    return "textarea";
                else if (FieldType == FieldType.Url)
                    return "url";
                else
                    return "daterange";

            }
        }
        public string FieldDescription { get; set; }
        public string FieldDataType { get; set; }
        public int? FieldDecimalDigit { get; set; }
        public int? FieldMinLength { get; set; }
        public int? FieldMaxLength { get; set; }
        public int? FieldMinNumber { get; set; }
        public int? FieldMaxNumber { get; set; }
        public string FieldCurrencySymbol { get; set; }
        public string FieldLOVs { get; set; }
        public int Version { get; set; }
        public bool Published { get; set; }
        public int IsRequired { get; set; }
        public int Sequence { get; set; }
        public string Editable { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultValue2 { get; set; }

        //Temporary properties
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class RequestFormControlsViewModel
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "oldid")]
        public int OldId { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string Type { get; set; }
        public string Lovs { get; set; }
        [JsonProperty(PropertyName = "currencysymbol")]
        public string CurrencySymbol { get; set; }
        public int Sequence { get; set; }
        public string Editable { get; set; }
        public bool Enabled { get; set; }
        public string Tooltip { get; set; }
        public string Slug { get; set; }
        [JsonProperty(PropertyName = "datatype")]
        public string DataType { get; set; }
        public RequestFormControlsValidators Validators { get; set; }
        public bool IsMappedToProcess { get; set; }
    }

    public class RequestFormControlsValidators
    {
        public bool Required { get; set; }
        [JsonProperty(PropertyName = "nullvalidator")]
        public bool NullValidator { get; set; }
        public bool Alphanumeric { get; set; }
        public bool Alpha { get; set; }
        public int? Currency { get; set; }
        public int? Numeric { get; set; }
        public bool Url { get; set; }
        public bool Email { get; set; }
        [JsonProperty(PropertyName = "minlength")]
        public int? MinLength { get; set; }
        [JsonProperty(PropertyName = "maxlength")]
        public int? MaxLength { get; set; }
        [JsonProperty(PropertyName = "minnumber")]
        public int? MinNumber { get; set; }
        [JsonProperty(PropertyName = "maxnumber")]
        public int? MaxNumber { get; set; }
        public bool Date { get; set; }
        [JsonProperty(PropertyName = "daterange")]
        public bool DateRange { get; set; }

    }
}
