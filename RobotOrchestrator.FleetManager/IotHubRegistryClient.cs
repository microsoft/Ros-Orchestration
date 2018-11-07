// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RobotOrchestrator.FleetManager
{
    public class IotHubRegistryClient : IIotHubRegistryClient
    {
        private readonly RegistryManager registryManager;
        private readonly string hostName;
        private readonly ILogger<FleetManager> loggerFleetManager;

        public IotHubRegistryClient(IOptions<IotHubRegistryClientOptions> options, ILogger<FleetManager> loggerFleetManager)
            : this (options?.Value.IotHubRegistryConnectionString, loggerFleetManager)
        {
        }

        public IotHubRegistryClient(string connectionString, ILogger<FleetManager> loggerFleetManager)
        {
            this.loggerFleetManager = loggerFleetManager;
            hostName = GetHostName(connectionString);
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public static string GetHostName(string connectionString)
        {
            int hostNameLength = connectionString.IndexOf(";");
            string hostName = connectionString.Substring(0, hostNameLength);
            return hostName;
        }

        public async Task<string> AddDeviceAsync(string deviceId)
        {
            Device device;

            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                // If the device already exists, simply retrieve its info
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            string primaryKey = device.Authentication.SymmetricKey.PrimaryKey;

            string connectionString = GetDeviceConnectionString(deviceId, primaryKey);

            return connectionString;
        }

        public async Task DeleteDeviceAsync(string deviceId)
        {
            await registryManager.RemoveDeviceAsync(deviceId);
        }

        public string GetDeviceConnectionString(string deviceId, string key)
        {
            string connectionString = String.Format("{0};DeviceId={1};SharedAccessKey={2}", hostName, deviceId, key);
            return connectionString;
        }
    }
}
