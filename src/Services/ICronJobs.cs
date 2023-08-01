using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Services
{
    public interface ICronJobs
    {
        Task RemoveLoginAttemptsAndTaggedAsDormant();
        Task TagAsPasswordExpired();
    }
}
