// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotOrchestrator.FleetManager
{
    public class RobotConfig
    {
        public string DeviceId { get; set; }

        public string IoTHubConnectionString { get; set; }
    }
}
