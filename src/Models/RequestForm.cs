using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestForm
    {
        public int RequestTypeId { get; set; }
        public int FieldId { get; set; }
        public int Version { get; set; }
        public bool Published { get; set; }
        public int IsRequired { get; set; }
        public int Sequence { get; set; }
        public string Editable { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultValue2 { get; set; }
        public virtual GeneralField GeneralField { get; set; }
        public virtual RequestType RequestType { get; set; }
    }
}