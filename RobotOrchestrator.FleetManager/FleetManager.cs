// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.FleetManager
{
    public class FleetManager : IFleetManager
    {
        private readonly ICosmosDbClient<Robot> cosmosDbClientRobot;
        private readonly IIotHubRegistryClient iotHubRegistryClient;
        private readonly ITelemetryHandler telemetryHandler;
        private readonly ILogger<FleetManager> logger;

        public FleetManager(
            IIotHubRegistryClient iotHubRegistryClient,
            ICosmosDbClient<Robot> cosmosDbClientRobot,
            ITelemetryHandler telemetryHandler,
            ILogger<FleetManager> logger)
        {
            this.iotHubRegistryClient = iotHubRegistryClient;
            this.cosmosDbClientRobot = cosmosDbClientRobot;
            this.telemetryHandler = telemetryHandler;
            this.logger = logger;
        }

        public async Task<Robot> UpsertRobotAsync(string deviceId)
        {
            Robot robot = new Robot()
            {
                DeviceId = deviceId,
                Telemetry = new RobotTelemetry()
            };

            var robotResult = await cosmosDbClientRobot.UpsertItemAsync(robot, new PartitionKey(robot.DeviceId));
            return robotResult;
        }

        private async Task<string> CreateIotHubConnectionAsync(string deviceId)
        {
            var iotResult = await iotHubRegistryClient.AddDeviceAsync(deviceId);
            return iotResult;
        }

        public async Task<string> CreateIfNotExistsRobotConnectionAsync(string deviceId)
        {
            await UpsertRobotAsync(deviceId);
            var connectionResult = await CreateIotHubConnectionAsync(deviceId);

            return connectionResult;
        }

        public async Task DeleteRobotAsync(string deviceId)
        {
            await cosmosDbClientRobot.DeleteItemAsync(deviceId, new PartitionKey(deviceId));
            await iotHubRegistryClient.DeleteDeviceAsync(deviceId );
        }

        public async Task<Robot> UpdateRobotAsync(Robot robot)
        {
            await cosmosDbClientRobot.UpdateItemAsync(robot.DeviceId, robot, new PartitionKey(robot.DeviceId));
            return robot;
        }

        public async Task<IEnumerable<Robot>> GetRobotsAsync(RobotStatus? status)
        {
            IEnumerable<Robot> robots;

            if (status == null)
            {
                robots = await cosmosDbClientRobot.GetItemsAsync();
            }
            else
            {
                robots = await cosmosDbClientRobot.GetItemsAsync(r => r.Telemetry.Status == status);
            }

            return robots;
        }

        public async Task<Robot> GetRobotAsync(string deviceId)
        {
            Robot robot = await cosmosDbClientRobot.GetItemAsync(deviceId, new PartitionKey(deviceId));

            return robot;
        }

        public async Task InsertTelemetriesAndUpdateRobotsAsync(IEnumerable<RobotTelemetry> robotTelemetry)
        {
            // telemetry handler inserts telemetry into cosmos
            await telemetryHandler.InsertTelemetryAsync(robotTelemetry);

            // update robot statuses in robot db
            foreach (RobotTelemetry telemetry in robotTelemetry)
            {
                var robot = new Robot()
                {
                    DeviceId = telemetry.RobotId,
                    Telemetry = telemetry
                };
                try
                {
                    await UpdateRobotAsync(robot);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<RobotTelemetry>> GetLatestTelemetriesAsync(string robotId, int? n)
        {
            var telemetry = await telemetryHandler.GetLatestTelemetriesAsync(robotId, n);
            return telemetry;
        }
    }
}
