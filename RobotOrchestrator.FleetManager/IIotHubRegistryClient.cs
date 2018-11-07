// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace RobotOrchestrator.FleetManager
{
    public interface IIotHubRegistryClient
    {
        Task<string> AddDeviceAsync(string deviceId);

        Task DeleteDeviceAsync(string deviceId);
    }
}
