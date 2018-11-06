using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RobotOrchestrator.OrderManager
{
    public class JobEventProcessor : IEventProcessor
    {
        private readonly IJobMessageHandler jobMessageHandler;

        private readonly ILogger logger;

        public JobEventProcessor(IJobMessageHandler jobMessageHandler, ILogger<JobEventProcessor> logger)
        {
            this.jobMessageHandler = jobMessageHandler;
            this.logger = logger;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            logger.LogDebug($"JobEventProcessor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            logger.LogDebug($"JobEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            logger.LogError($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var jobMessages = new List<Job>();

            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                logger.LogDebug($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");

                var job = ConvertToJob(data);

                if (job != null)
                {
                    jobMessages.Add(job);
                }
            }

            await jobMessageHandler.UpdateOrdersAsync(jobMessages);

            await context.CheckpointAsync();
        }

        private Job ConvertToJob(string rawData)
        {
            try
            {
                var rosMessage = JsonConvert.DeserializeObject<RosMessage<Job>>(rawData);
                var job = rosMessage.Payload;

                return job;
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to deserialize telemetry message.", ex);
            }

            return null;
        }
    }
}
