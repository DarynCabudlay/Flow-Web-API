using Newtonsoft.Json;
using System.Collections.Generic;

namespace workflow.Models.DataTableViewModels
{
    public class Order
    {
        [JsonProperty(PropertyName = "column")]
        public int Column  { get; set; }

        [JsonProperty(PropertyName = "dir")]
        public string Dir { get; set; }
    }
}
