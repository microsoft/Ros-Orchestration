using System.Threading.Tasks;

namespace RobotOrchestrator.Dispatcher
{
    public interface IIotHubServiceClient
    {
        Task SendMessageFromCloudToDeviceAsync(string deviceId, string message);
    }
}
