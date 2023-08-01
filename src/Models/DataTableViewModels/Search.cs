using Newtonsoft.Json;
using System.Collections.Generic;

namespace workflow.Models.DataTableViewModels
{
    public class Search
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "regex")]
        public bool Regex { get; set; }
    }
}
