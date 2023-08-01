using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public enum FieldType
    {
        TextBox = 1,
        Dropdown = 2,
        Date = 3,
        CheckboxList = 4,
        RadioButton = 5,
        Email = 6,
        TextArea = 7,
        Url = 8,
        DateRange = 9
    }

    public class GeneralFieldViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
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
        public string Description { get; set; }
        public string DataType { get; set; }
        public int? DecimalDigit { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? MinNumber { get; set; }
        public int? MaxNumber { get; set; }
        public string CurrencySymbol  { get; set; }
        public string LOVs { get; set; }
        public virtual ICollection<RequestForm> RequestForms { get; set; }
    }
}
