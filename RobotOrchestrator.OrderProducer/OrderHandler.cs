// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer
{

    public class OrderHandler : IOrderHandler
    {
        private readonly IOrderManagerClient orderManagerClient;
        private readonly IOrderFactory factory;
        private readonly ILogger logger;

        public OrderHandler(IOrderManagerClient orderManagerClient, IOrderFactory factory, ILogger<OrderHandler> logger)
        {
            this.orderManagerClient = orderManagerClient;
            this.factory = factory;
            this.logger = logger;
        }

        public void HandleBatch(int batchSize)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var order = factory.CreateOrder($"Order{i}");

                try
                {
                    orderManagerClient.SendOrderAsync(order);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }

            logger.LogDebug($"OrderHandler called with batch size = {batchSize}");            
        }
    }
}