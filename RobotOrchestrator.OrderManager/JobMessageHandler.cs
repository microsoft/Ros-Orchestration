// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;

namespace RobotOrchestrator.OrderManager
{
    public class JobMessageHandler : IJobMessageHandler
    {
        private readonly ICosmosDbClient<Order> ordersDbClient;

        private readonly ILogger logger;

        public JobMessageHandler(ICosmosDbClient<Order> ordersDbClient, ILogger<JobMessageHandler> logger)
        {
            this.ordersDbClient = ordersDbClient;
            this.logger = logger;
        }

        public async Task UpdateOrdersAsync(IEnumerable<Job> jobs)
        {
            // TO-DO: parallelize and make more performant
            foreach (var job in jobs)
            {
                await UpdateOrderAsync(job);
            }
        }

        public async Task UpdateOrderAsync(Job job)
        {
            try
            {
                await GetAndUpdateOrderWithJobAsync(job);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        private async Task GetAndUpdateOrderWithJobAsync(Job job)
        {
            var orderId = job.OrderId.ToString();
            var partitionKey = new PartitionKey(orderId);
            var order = await ordersDbClient.GetItemAsync(orderId, partitionKey);

            // TO-DO: Store jobs in data structure that is easier for access and update
            var index = order.Jobs.FindIndex(j => j.Id == job.Id);

            if (index != -1) {
                order.Jobs[index] = job;

                order = UpdateOrderStatus(order, job);

                await ordersDbClient.UpdateItemAsync(orderId, order, partitionKey);
            } else {
                logger.LogError($"Job Id not found in Order: { orderId }.");
            }
        }

        private Order UpdateOrderStatus(Order order, Job job)
        {
            switch (job.Status)
            {
                case JobStatus.Queued:
                    order.Status = OrderStatus.InProgress;
                    break;
                case JobStatus.InProgress:
                    order.Status = OrderStatus.InProgress;
                    break;
                case JobStatus.Complete:
                    order.Status = OrderStatus.Complete;
                    break;
                case JobStatus.Failed:
                    order.Status = OrderStatus.Failed;
                    break;
                default:
                    throw new NotSupportedException($"Job Status {job.Status} is not supported in OrderStatus conversion.");
            }

            return order;
        }
    }
}
