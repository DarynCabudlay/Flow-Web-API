using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class IdList
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    public class IdListStr
    {
        public string id { get; set; }
        public string description { get; set; }
    }

    public class IdLabelList
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class ListData
    {
        public int Id { get; set; }
        public bool isChecked { get; set; }
    }
}
