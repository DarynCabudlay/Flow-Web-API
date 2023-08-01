using System.Threading.Tasks;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface ILog 
    {
        Task<ObjectReturnModel> GetLogDashboard(int offset);
        Task LogPageVisit(string path);
        Task<ObjectReturnModel> GetPageVisitHistoryData();
        Task<ObjectReturnModel> GetLoginHistoryData();
    }
}
