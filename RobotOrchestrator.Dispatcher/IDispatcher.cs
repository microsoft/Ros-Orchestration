using System.Threading.Tasks;

namespace RobotOrchestrator.Dispatcher
{
    public interface IDispatcher
    {
        Task SendJobAsync(Job job);
    }
}
