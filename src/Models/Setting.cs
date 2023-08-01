using System;
using System.Collections.Generic;

namespace workflow.Models
{
    public partial class Setting
    {
        public int Id { get; set; }
        public string VSettingId { get; set; }
        public string VSettingName { get; set; }
        public string VSettingOption { get; set; }
        public string VSettingGroup { get; set; }
        public string VSettingLabel { get; set; }
    }
}
