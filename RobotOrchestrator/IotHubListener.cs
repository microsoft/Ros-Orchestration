// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace RobotOrchestrator
{
    public class IotHubListener : IHostedService
    {
        private readonly IEventProcessorFactory iotHubEventProcessorFactory;
        private readonly IEventProcessorHostConfig eventProcessorHostConfig;
        private readonly EventProcessorOptions eventProcessorOptions;

        private EventProcessorHost eventProcessorHost;

        public IotHubListener(
            IEventProcessorFactory iotHubEventProcessorFactory, 
            IEventProcessorHostConfig eventProcessorHostConfig,
            IOptions<EventProcessorOptions> eventProcessorOptions)
        {
            this.iotHubEventProcessorFactory = iotHubEventProcessorFactory;
            this.eventProcessorHostConfig = eventProcessorHostConfig;
            this.eventProcessorOptions = eventProcessorOptions?.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            eventProcessorHost = new EventProcessorHost(eventProcessorHostConfig.HostName,
                                                         eventProcessorHostConfig.EventHubPath,
                                                         eventProcessorHostConfig.ConsumerGroupName,
                                                         eventProcessorHostConfig.EventHubConnectionString,
                                                         eventProcessorHostConfig.StorageConnectionString,
                                                         eventProcessorHostConfig.LeaseContainerName);

            if (eventProcessorOptions == null)
            {
                await eventProcessorHost.RegisterEventProcessorFactoryAsync(iotHubEventProcessorFactory);
            }
            else
            {
                await eventProcessorHost.RegisterEventProcessorFactoryAsync(iotHubEventProcessorFactory, eventProcessorOptions);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
