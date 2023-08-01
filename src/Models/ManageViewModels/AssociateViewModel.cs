﻿namespace workflow.Models.ManageViewModels
{
    public class AssociateViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AssociateEmail { get; set; }
        public bool AssociateExistingAccount { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderKey { get; set; }
    }
}
