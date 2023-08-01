using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class UserPreferencesViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DefaultHomePage { get; set; }
        public string Theme { get; set; }
    }
}
