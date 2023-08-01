using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IPermission
    {
        Task<Dictionary<string, ControlViewModel>> GetPermissionControl(string vMenuId);
    }
}
