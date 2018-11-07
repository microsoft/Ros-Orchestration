// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotOrchestrator.FleetManager
{
    public class TelemetryHandler : ITelemetryHandler
    {
        private readonly ICosmosDbClient<RobotTelemetry> cosmosDbClient;
        private readonly ILogger logger;

        public TelemetryHandler(ICosmosDbClient<RobotTelemetry> cosmosDbClient, ILogger<TelemetryHandler> logger)
        {
            this.cosmosDbClient = cosmosDbClient;
            this.logger = logger;
        }

        public async Task InsertTelemetryAsync(IEnumerable<RobotTelemetry> robotTelemetry)
        {
            foreach(var telemetry in robotTelemetry)
            {
                try
                {
                    await cosmosDbClient.UpsertItemAsync(telemetry, new PartitionKey(telemetry.RobotId));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<RobotTelemetry>> GetLatestTelemetriesAsync(string robotId, int? n = null)
        {
            if (n < 0)
            {
                throw (new ArgumentOutOfRangeException("Number of items to retrieve cannot be negative"));
            }

            var result = await cosmosDbClient.GetItemsAsync(t => t.RobotId == robotId, t => t.CreatedDateTime, n);

            return result;
        }
    }
}
