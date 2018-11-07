// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RobotOrchestrator.FleetManager.Tests
{
    public class FleetManagerTests
    {
        private Mock<IIotHubRegistryClient> mockIotClient;
        private Mock<ICosmosDbClient<Robot>> mockCosmosClientRobot;
        private Mock<ICosmosDbClient<RobotTelemetry>> mockCosmosClientTelemetry;
        private ITelemetryHandler telemetryHandler;
        private Mock<ILogger<FleetManager>> mockLogger;

        public FleetManagerTests()
        {
            mockIotClient = new Mock<IIotHubRegistryClient>();
            mockCosmosClientRobot = new Mock<ICosmosDbClient<Robot>>();
            mockCosmosClientTelemetry = new Mock<ICosmosDbClient<RobotTelemetry>>();
            telemetryHandler = new TelemetryHandler(mockCosmosClientTelemetry.Object, Mock.Of<ILogger<TelemetryHandler>>());
            mockLogger = new Mock<ILogger<FleetManager>>();
        }

        /// <summary>
        /// Verifies that if an exception is thrown while updating robots,
        /// we will attempt to update all robots and not bail out
        /// </summary>
        /// <param name="numItems"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(6)]
        public async Task TestInsertTelemetryAndUpdateRobotsAsync_UpdateRobot_ExpectedExecutions(int numItems)
        {
            mockCosmosClientRobot.Setup(x =>
            x.UpdateItemAsync(It.IsAny<string>(), It.IsAny<Robot>(), It.IsAny<PartitionKey>())).Throws(new Exception());

            FleetManager fleetManager = new FleetManager(mockIotClient.Object, mockCosmosClientRobot.Object, telemetryHandler, mockLogger.Object);

            IEnumerable<RobotTelemetry> testTelemetry = GetTestTelemetry(numItems);

            // handles exception
            await fleetManager.InsertTelemetriesAndUpdateRobotsAsync(testTelemetry);
            mockCosmosClientRobot.Verify(x => x.UpdateItemAsync(It.IsAny<string>(), It.IsAny<Robot>(), It.IsAny<PartitionKey>()), Times.Exactly(numItems));
        }

        /// <summary>
        /// Verifies that if an exception is thrown while upserting telemetry,
        /// we will attempt to upsert all telemetry and not bail out
        /// </summary>
        /// <param name="numItems"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(10)]
        public async Task TestInsertTelemetryAndUpdateRobotsAsync_UpsertTelemetry_ExpectedExecutions(int numItems)
        {
            mockCosmosClientTelemetry.Setup(x =>
                x.UpsertItemAsync(It.IsAny<RobotTelemetry>(), It.IsAny<PartitionKey>())).Throws(new Exception("invalid"));

            FleetManager fleetManager = new FleetManager(mockIotClient.Object, mockCosmosClientRobot.Object, telemetryHandler, mockLogger.Object);

            IEnumerable<RobotTelemetry> testTelemetry = GetTestTelemetry(numItems);

            // should not throw exception
            await fleetManager.InsertTelemetriesAndUpdateRobotsAsync(testTelemetry);
            mockCosmosClientTelemetry.Verify(x => x.UpsertItemAsync(It.IsAny<RobotTelemetry>(), It.IsAny<PartitionKey>()), Times.Exactly(numItems));
        }

        private IEnumerable<RobotTelemetry> GetTestTelemetry(int numItems)
        {
            var testTelemetry = new List<RobotTelemetry>();

            for (int i = 0; i < numItems; i ++)
            {
                var telemetry = new RobotTelemetry()
                {
                    RobotId = $"robot{numItems}",
                    Status = RobotStatus.Busy,
                    OrderId = $"Order{numItems}",
                    CreatedDateTime = DateTime.UtcNow,
                };

                testTelemetry.Add(telemetry);
            }

            return testTelemetry;
        }
    }
}
