// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotOrchestrator.OrderManager
{
    public interface IFleetManagerClient
    {
        Task<Robot> GetRobot(string id);

        Task<IEnumerable<Robot>> GetAvailableRobotsAsync();
    }
}
