using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Options;

namespace RobotOrchestrator.Dispatcher
{
    public class IotHubServiceClient : IIotHubServiceClient
    {
        private readonly ServiceClient iotHubServiceClient;

        public IotHubServiceClient(IOptions<IotHubServiceClientOptions> options)
        {
            var iotHubConnectionString = options?.Value.IotHubServiceConnectionString;

            iotHubServiceClient = ServiceClient.CreateFromConnectionString(iotHubConnectionString);
        }

        public async Task SendMessageFromCloudToDeviceAsync(string deviceId, string message)
        {
            var commandMessage = new Message(Encoding.UTF8.GetBytes(message));

            await iotHubServiceClient.SendAsync(deviceId, commandMessage);
        }
    }
}
