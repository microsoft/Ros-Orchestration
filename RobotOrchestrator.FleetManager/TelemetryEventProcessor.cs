// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RobotOrchestrator.FleetManager
{
    public class TelemetryEventProcessor : IEventProcessor
    {
        private readonly IFleetManager fleetManager;
        private readonly ILogger logger;

        public TelemetryEventProcessor(IFleetManager fleetManager, ILogger<TelemetryEventProcessor> logger)
        {
            this.fleetManager = fleetManager;
            this.logger = logger;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            logger.LogDebug($"TelemetryEventProcessor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            logger.LogDebug($"TelemetryEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            logger.LogError($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var telemetryMessages = new List<RobotTelemetry>();

            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                logger.LogDebug($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");

                var telemetry = ConvertToRobotTelemetry(data);

                if (telemetry != null)
                {
                    telemetryMessages.Add(telemetry);
                }
            }

            await fleetManager.InsertTelemetriesAndUpdateRobotsAsync(telemetryMessages);

            await context.CheckpointAsync();
        }

        private RobotTelemetry ConvertToRobotTelemetry(string rawData)
        {
            try
            {
                var rosMessage = JsonConvert.DeserializeObject<RosMessage<RobotTelemetry>>(rawData);
                var telemetry = rosMessage.Payload;

                return telemetry;
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to deserialize telemetry message.", ex);
            }

            return null;
        }
    }
}