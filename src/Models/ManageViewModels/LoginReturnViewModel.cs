using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class LoginReturnViewModel
    {
        public int Code { get; set; }
        public int? LoginAttempt { get; set; }
        public int? LoginAttemptInSetup { get; set; }
    }
}
