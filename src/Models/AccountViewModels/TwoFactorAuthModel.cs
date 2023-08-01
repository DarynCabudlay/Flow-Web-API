using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.AccountViewModels
{
    public class TwoFactorAuthModel
    {
        [Required]
        public string TFACode { get; set; }
        public string Id { get; set; }
    }
}
