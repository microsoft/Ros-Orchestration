// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace RobotOrchestrator.OrderProducer
{
    public interface IOrderFactory
    {
        Order CreateOrder(string message = null);

        IEnumerable<Order> CreateOrders(int numOfOrders, string message = null);
    }
}
