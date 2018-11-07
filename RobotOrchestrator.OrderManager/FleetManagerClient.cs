// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace RobotOrchestrator.OrderManager
{
    public class FleetManagerClient : IFleetManagerClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private readonly string fleetManagerUrl;

        private readonly ILogger logger;

        public FleetManagerClient(IOptions<FleetManagerClientOptions> options, ILogger<FleetManagerClient> logger)
        {
            var fleetManagerUrl = options?.Value.FleetManagerUrl;

            if (string.IsNullOrWhiteSpace(fleetManagerUrl))
            {
                throw new ArgumentNullException("FleetManagerUrl is required.");
            }

            this.fleetManagerUrl = fleetManagerUrl;
            this.logger = logger;
        }

        public async Task<Robot> GetRobot(string id)
        {
            var uri = $"{fleetManagerUrl}/{id}";
            var robot = await GetHttpResponse<Robot>(uri);

            logger.LogDebug("Got robot from fleet manager.");

            return robot;
        }

        public async Task<IEnumerable<Robot>> GetAvailableRobotsAsync()
        {
            logger.LogDebug("Getting available robots from fleet manager.");

            // Only find idle robots
            var robotIdleStatus = RobotStatus.Idle.ToString();
            var uri = $"{fleetManagerUrl}?status={robotIdleStatus}";
            var robots = await GetHttpResponse<IEnumerable<Robot>>(uri);

            logger.LogDebug("Got available robots from fleet manager.");

            return robots;
        }

        private async Task<T> GetHttpResponse<T>(string uri)
        {
            var httpResponseMessage = await httpClient.GetAsync(uri);
            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            var deserializedResponse = JsonConvert.DeserializeObject<T>(response);

            return deserializedResponse;
        }
    }
}

