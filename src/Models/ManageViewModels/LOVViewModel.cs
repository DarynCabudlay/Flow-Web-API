using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class LOVViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string OldName { get; set; }
        public string OldValue { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelected { get; set; }
        public bool IsMappedToProcess { get; set; }
        public bool IsActive { get; set; }

    }
}
