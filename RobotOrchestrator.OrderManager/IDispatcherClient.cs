using System.Threading.Tasks;

namespace RobotOrchestrator.OrderManager
{
    public interface IDispatcherClient
    {
        Task<bool> SendJobAsync(Job job);
    }
}
