namespace RobotOrchestrator
{
    public interface IEventProcessorHostConfig
    {
        string ConsumerGroupName { get; set; }

        string EventHubConnectionString { get; set; }

        string EventHubPath { get; set; }

        string HostName { get; }

        string LeaseContainerName { get; set; }

        string StorageConnectionString { get; set; }
    }
}
