using System;

namespace RobotOrchestrator
{
    public class EventProcessorHostConfig : IEventProcessorHostConfig
    {
        public string HostName { get; } = Guid.NewGuid().ToString();

        public string EventHubConnectionString { get; set; }

        public string EventHubPath { get; set; }

        public string ConsumerGroupName { get; set; }

        public string StorageConnectionString { get; set; } 

        public string LeaseContainerName { get; set; } 
    }
}
