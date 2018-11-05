using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace RobotOrchestrator.OrderManager
{
    public class OrderManager : IOrderManager
    {
        private readonly IFleetManagerClient fleetManagerClient;

        private readonly IDispatcherClient dispatcherClient;

        private readonly ICosmosDbClient<Order> cosmosdbClient;

        public OrderManager(IFleetManagerClient fleetManagerClient, 
            IDispatcherClient dispatcherClient, ICosmosDbClient<Order> cosmosdbClient)
        {
            this.fleetManagerClient = fleetManagerClient;
            this.dispatcherClient = dispatcherClient;
            this.cosmosdbClient = cosmosdbClient;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(OrderStatus? status)
        {
            IEnumerable<Order> orders;

            if (status == null)
            {
                orders = await cosmosdbClient.GetItemsAsync();
            }
            else
            {
                orders = await cosmosdbClient.GetItemsAsync(r => r.Status == status);
            }

            return orders;
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            var order = await cosmosdbClient.GetItemAsync(id.ToString(), new PartitionKey(id.ToString()));

            return order;
        }

        public async Task<Order> AcceptOrderAsync(Order order, string robotId = null)
        {
            await StoreOrderReceivedAsync(order);

            if (robotId != null)
            {
                await AssignOrderToRobotAsync(order, robotId);
            }
            else
            {
                await AssignOrdersToAvailableRobotsAsync(new List<Order>() { order });
            }

            return order;
        }

        public async Task<IEnumerable<Order>> AcceptOrdersAsync(IEnumerable<Order> orders)
        {
            await Task.WhenAll(orders.Select((order) => StoreOrderReceivedAsync(order)));

            await AssignOrdersToAvailableRobotsAsync(orders);

            return orders;
        }

        private async Task<Order> AssignOrderToRobotAsync(Order order, string robotId)
        {
            var robot = await fleetManagerClient.GetRobot(robotId);

            robot = robot?.Telemetry.Status == RobotStatus.Idle ? robot : null;

            var assignment = new OrderAssignment(order, robot);
            await HandleAssignmentOutcomeAsync(assignment);

            return order;
        }

        private async Task<IEnumerable<Order>> AssignOrdersToAvailableRobotsAsync(IEnumerable<Order> orders)
        {
            var availableRobots = await fleetManagerClient.GetAvailableRobotsAsync();

            var assignmentOutcome = AssignOrdersToRobots(orders, availableRobots);

            foreach (var assignment in assignmentOutcome)
            {
                await HandleAssignmentOutcomeAsync(assignment);
            }

            return orders;
        }

        private async Task<Order> HandleAssignmentOutcomeAsync(OrderAssignment assignment)
        {
            var order = assignment.Order;

            if (assignment.IsAssigned)
            {
                await DispatchOrderAssignmentAsync(assignment);
                await StoreOrderAsync(order);
            }
            else
            {
                await StoreOrderFailedAsync(assignment.Order);
            }

            return order;
        }

        private async Task<Order> StoreOrderAsync(Order order)
        {
            order = await cosmosdbClient.UpsertItemAsync(order, new PartitionKey(order.Id.ToString()));
            return order;
        }

        private async Task<Order> StoreOrderReceivedAsync(Order order)
        {
            order.Status = OrderStatus.Received;
            order = await StoreOrderAsync(order);
            return order;
        }

        private async Task<Order> StoreOrderFailedAsync(Order order)
        {
            order.Status = OrderStatus.Failed;
            await StoreOrderAsync(order);
            return order;
        }

        /// <summary>
        /// Simple assignment that assigns each order to first available robot.
        /// Returns dictionary of orders with a robot if assigned, else robot is null.
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="availableRobots"></param>
        /// <returns></returns>
        private IEnumerable<OrderAssignment> AssignOrdersToRobots(IEnumerable<Order> orders, IEnumerable<Robot> availableRobots)
        {
            var assignments = new List<OrderAssignment>();

            var robotIterator = availableRobots.GetEnumerator();
            
            foreach (var order in orders)
            {
                robotIterator.MoveNext();
                var robot = robotIterator.Current;

                if (robot != null)
                {
                    assignments.Add(new OrderAssignment(order, robot));
                }
                else
                {
                    assignments.Add(new OrderAssignment(order, null));
                }
            }

            return assignments;
        }

        private async Task<Order> DispatchOrderAssignmentAsync(OrderAssignment assignment)
        {
            var order = assignment.Order;
            var robot = assignment.Robot;

            var job = new Job()
            {
                RobotId = robot.DeviceId,
                OrderId = order.Id,
                StartPosition = order.StartPosition,
                EndPosition = order.EndPosition
            };

            var success = await dispatcherClient.SendJobAsync(job);

            if (!success)
            {
                order.Status = OrderStatus.Failed;
                job.Status = JobStatus.Failed;
            }
            else
            {
                order.Status = OrderStatus.InProgress;
                job.Status = JobStatus.Queued;
            }

            order.Jobs.Add(job);
            return order;
        }
    }
}