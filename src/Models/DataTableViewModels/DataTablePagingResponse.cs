using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace workflow.Models.DataTableViewModels
{
    public class DataTablePagingResponse
    {
        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonProperty(PropertyName = "data")]
        public DataTable Data  { get; set; }

        [JsonProperty(PropertyName = "columnHeaders")]
        public List<string> ColumnHeaders { get; set; }
    }
}
