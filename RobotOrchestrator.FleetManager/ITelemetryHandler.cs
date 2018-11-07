// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotOrchestrator.FleetManager
{
    public interface ITelemetryHandler
    {
        Task InsertTelemetryAsync(IEnumerable<RobotTelemetry> robotTelemetry);

        Task<IEnumerable<RobotTelemetry>> GetLatestTelemetriesAsync(string robotId, int? n);
    }
}
