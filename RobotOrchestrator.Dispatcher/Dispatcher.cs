using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RobotOrchestrator.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private readonly IIotHubServiceClient iotHubServiceClient;
        private readonly ILogger logger;
        private static string TOPIC_NAME = "jobs";
        private static string PACKAGE_NAME = "orchestrator_msgs";

        public Dispatcher(IIotHubServiceClient iotHubServiceClient, ILogger<Dispatcher> logger)
        {
            this.iotHubServiceClient = iotHubServiceClient;
            this.logger = logger;
        }

        public async Task SendJobAsync(Job job)
        {
            var rosMessage = new RosMessage<Job>()
            {
                Topic = TOPIC_NAME,
                MessageType = $"{PACKAGE_NAME}.msg:{RosMessageType.Job.ToString()}",
                Payload = job
            };

            var rosMessageJson = JsonConvert.SerializeObject(rosMessage);

            await iotHubServiceClient.SendMessageFromCloudToDeviceAsync(job.RobotId, rosMessageJson);
        }
    }
}
