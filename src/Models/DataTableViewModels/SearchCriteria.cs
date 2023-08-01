using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace workflow.Models.DataTableViewModels
{
    public class SearchCriteria
    {
        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; set; }

        [JsonProperty(PropertyName = "isPageLoad")]
        public bool IsPageLoad { get; set; }
        public int? Id1 { get; set; }
        public int? Id2 { get; set; }
        public int? Id3 { get; set; }
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }
    }
}
