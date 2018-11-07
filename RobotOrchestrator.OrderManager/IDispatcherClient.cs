// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace RobotOrchestrator.OrderManager
{
    public interface IDispatcherClient
    {
        Task<bool> SendJobAsync(Job job);
    }
}
