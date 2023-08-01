using Newtonsoft.Json;
using System.Collections.Generic;

namespace workflow.Models.DataTableViewModels
{
    public class PagingResponse<T>
    {
        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T[] Data  { get; set; }

        [JsonProperty(PropertyName = "columnHeaders")]
        public List<string> ColumnHeaders { get; set; }
    }
}
