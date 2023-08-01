using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class ControlViewModel
    {
        //public int wView { get; set; }
        public int wAdd { get; set; }
        public int wEdit { get; set; }
        public int wDelete { get; set; }
        public int wExtract { get; set; }
        public int wPrint { get; set; }
        public int wDownload { get; set; }
        public int wUpload { get; set; }
        public int wSync { get; set; }
        public int wFetch { get; set; }
    }
}
