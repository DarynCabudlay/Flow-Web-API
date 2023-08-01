using System.Collections.Generic;
using System.Threading.Tasks;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface ISetting 
    {
        List<SettingViewModel> GetSettingData();
        Task Update(SettingViewModel model);
    }
}
