using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IUserIPAddressPerSession
    {
        Task Save(UserIPAddressPerSessionsViewModel data);
        Task Remove(int spid);
        UserIPAddressPerSessionsViewModel AddData(int spid, string userid, string ipaddress);
        Task<int> GetSPID();
    }
}
