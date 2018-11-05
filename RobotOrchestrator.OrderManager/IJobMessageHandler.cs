using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotOrchestrator.OrderManager
{
    public interface IJobMessageHandler
    {
        Task UpdateOrdersAsync(IEnumerable<Job> jobs);

        Task UpdateOrderAsync(Job job);
    }
}
