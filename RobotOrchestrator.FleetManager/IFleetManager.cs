// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotOrchestrator.FleetManager
{
    public interface IFleetManager
    {
        Task<string> CreateIfNotExistsRobotConnectionAsync(string deviceId);

        Task<Robot> UpsertRobotAsync(string deviceId);

        Task DeleteRobotAsync(string deviceId);

        Task<Robot> UpdateRobotAsync(Robot robot);

        Task<IEnumerable<Robot>> GetRobotsAsync(RobotStatus? status);

        Task<Robot> GetRobotAsync(string deviceId);

        Task InsertTelemetriesAndUpdateRobotsAsync(IEnumerable<RobotTelemetry> robotTelemetry);

        Task<IEnumerable<RobotTelemetry>> GetLatestTelemetriesAsync(string robotId, int? n);

    }
}
