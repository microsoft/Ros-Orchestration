// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.EventHubs.Processor;

namespace RobotOrchestrator
{
    public class IotHubEventProcessorFactory : IEventProcessorFactory
    {
        private readonly IEventProcessor eventProcessor;

        public IotHubEventProcessorFactory(IEventProcessor eventProcessor)
        {
            this.eventProcessor = eventProcessor;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return eventProcessor;
        }
    }
}
