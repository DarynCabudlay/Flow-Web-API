using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RoleWithMenuPermission
    {
        public AspNetRoles Role { get; set;}
        public List<AspNetUsersMenuPermission> MenuPermission { get; set;}
    }
}