using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class PageVisitedViewModel
    {
        public string Name { get; set; }
        public string VPageVisitedId { get; set; }
        public string UserId { get; set; }
        public DateTime VisitedDate { get; set; }
        public string PageName { get; set; }
        public string IP { get; set; }
        public string Email { get; set; }
    }
}
