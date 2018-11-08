// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotOrchestrator.OrderManager
{
    public interface IOrderManager
    {
        Task<IEnumerable<Order>> AcceptOrdersAsync(IEnumerable<Order> orders);

        Task<Order> AcceptOrderAsync(Order order, string robotId = null);

        Task<IEnumerable<Order>> GetOrdersAsync(OrderStatus? status, int? numOrders);

        Task<Order> GetOrderAsync(Guid id);
    }
}
