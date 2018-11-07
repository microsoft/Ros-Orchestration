// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace RobotOrchestrator.OrderManager.Tests
{
    public class OrderManagerTests
    {
        [Fact]
        public async Task AcceptOrderAsync_WithSpecificRobot_AssignsOrderToRobot()
        {
            var fleetManagerClient = Mock.Of<IFleetManagerClient>();
            var dispatcherClient = Mock.Of<IDispatcherClient>();
            var dbClient = Mock.Of<ICosmosDbClient<Order>>();
            var orderManager = new OrderManager(fleetManagerClient, dispatcherClient, dbClient);
            
            Mock.Get(fleetManagerClient).Setup(f => f.GetRobot(It.IsAny<string>())).ReturnsAsync((string id) => generateAvailableRobot(id));
            Mock.Get(dispatcherClient).Setup(d => d.SendJobAsync(It.IsAny<Job>())).ReturnsAsync(true).Verifiable();

            var robotId = "test";
            var order = new Order();

            var processedOrder = await orderManager.AcceptOrderAsync(order, robotId);

            Assert.Equal(OrderStatus.InProgress, processedOrder.Status);

            var job = processedOrder.Jobs[0];
            Assert.Equal(job.RobotId, robotId);

            Mock.Get(dispatcherClient).Verify((d) => d.SendJobAsync(job), Times.Once());
        }

        [Fact]
        public async Task AcceptOrderAsync_WithoutSpecificRobot_AssignsOrderToAnyAvailableRobot()
        {
            var fleetManagerClient = Mock.Of<IFleetManagerClient>();
            var dispatcherClient = Mock.Of<IDispatcherClient>();
            var dbClient = Mock.Of<ICosmosDbClient<Order>>();
            var orderManager = new OrderManager(fleetManagerClient, dispatcherClient, dbClient);

            var robots = generateAvailableRobots(1);

            Mock.Get(fleetManagerClient).Setup(f => f.GetAvailableRobotsAsync()).ReturnsAsync(robots);
            Mock.Get(dispatcherClient).Setup(d => d.SendJobAsync(It.IsAny<Job>())).ReturnsAsync(true).Verifiable();

            var order = new Order();

            var processedOrder = await orderManager.AcceptOrderAsync(order);

            Assert.Equal(OrderStatus.InProgress, processedOrder.Status);

            var job = processedOrder.Jobs[0];
            Mock.Get(dispatcherClient).Verify((d) => d.SendJobAsync(job), Times.Once());
        }

        [Theory]
        [InlineData(1,1)]
        [InlineData(2,1)]
        [InlineData(1,2)]
        [InlineData(1,0)]
        [InlineData(0,1)]
        [InlineData(0,0)]
        public async Task AcceptOrdersAsync_WithOrdersAndAvailableRobots_AssignsCorrectNumberOfOrders(int numOfOrders, int numOfAvailableRobots)
        {
            var fleetManagerClient = Mock.Of<IFleetManagerClient>();
            var dispatcherClient = Mock.Of<IDispatcherClient>();
            var dbClient = Mock.Of<ICosmosDbClient<Order>>();
            var orderManager = new OrderManager(fleetManagerClient, dispatcherClient, dbClient);

            var orders = generateOrders(numOfOrders);
            var robots = generateAvailableRobots(numOfAvailableRobots);
            
            Mock.Get(fleetManagerClient).Setup(f => f.GetAvailableRobotsAsync()).ReturnsAsync(robots);
            Mock.Get(dispatcherClient).Setup(d => d.SendJobAsync(It.IsAny<Job>())).ReturnsAsync(true);

            var expectedAssignments = numOfOrders;
            var expectedSuccessfulAssignments = Math.Min(numOfOrders, numOfAvailableRobots);

            var processedOrders = await orderManager.AcceptOrdersAsync(orders);
            var successfulOrders = processedOrders.Where(o => o.Status == OrderStatus.InProgress).Count();

            Assert.Equal(expectedAssignments, processedOrders.Count());
            Assert.Equal(expectedSuccessfulAssignments, successfulOrders);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public async Task AcceptOrdersAsync_WithFailedSend_ReturnsOrdersWithFailedOrderStatus(int numOfOrders)
        {
            var fleetManagerClient = Mock.Of<IFleetManagerClient>();
            var dispatcherClient = Mock.Of<IDispatcherClient>();
            var dbClient = Mock.Of<ICosmosDbClient<Order>>();

            var orderManager = new OrderManager(fleetManagerClient, dispatcherClient, dbClient);

            var orders = generateOrders(numOfOrders);
            var robots = generateAvailableRobots(numOfOrders);

            Mock.Get(fleetManagerClient).Setup(f => f.GetAvailableRobotsAsync()).ReturnsAsync(robots);
            Mock.Get(dispatcherClient).Setup((d) => d.SendJobAsync(It.IsAny<Job>())).ReturnsAsync(false);

            var processedOrders = await orderManager.AcceptOrdersAsync(orders);
            var failedOrders = processedOrders.Where(o => o.Status == OrderStatus.Failed).Count();

            Assert.Equal(numOfOrders, failedOrders);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public async Task AcceptOrdersAsync_WithSuccessfulSend_ReturnsOrdersWithInProgressOrderStatus(int numOfOrders)
        {
            var fleetManagerClient = Mock.Of<IFleetManagerClient>();
            var dispatcherClient = Mock.Of<IDispatcherClient>();
            var dbClient = Mock.Of<ICosmosDbClient<Order>>();

            var orderManager = new OrderManager(fleetManagerClient, dispatcherClient, dbClient);

            var orders = generateOrders(numOfOrders);
            var robots = generateAvailableRobots(numOfOrders);

            Mock.Get(fleetManagerClient).Setup(f => f.GetAvailableRobotsAsync()).ReturnsAsync(robots);
            Mock.Get(dispatcherClient).Setup((d) => d.SendJobAsync(It.IsAny<Job>())).ReturnsAsync(true);

            var processedOrders = await orderManager.AcceptOrdersAsync(orders);
            var successfulOrders = processedOrders.Where(o => o.Status == OrderStatus.InProgress).Count();

            Assert.Equal(numOfOrders, successfulOrders);
        }

        private IEnumerable<Order> generateOrders(int numOfOrders)
        {
            var orders = new List<Order>();
            for (int i = 0; i < numOfOrders; i++)
            {
                orders.Add(new Order());
            }

            return orders;
        }

        private Robot generateAvailableRobot(string robotId)
        {
            var robot = new Robot()
            {
                DeviceId = robotId,
                Telemetry = new RobotTelemetry()
                {
                    Status = RobotStatus.Idle
                }
            };

            return robot;
        }

        private IEnumerable<Robot> generateAvailableRobots(int numOfRobots)
        {
            var robots = new List<Robot>();
            for (int i = 0; i < numOfRobots; i++)
            {
                robots.Add(new Robot()
                {
                    DeviceId = "robot" + i,
                    Telemetry = new RobotTelemetry()
                    {
                        Status = RobotStatus.Idle
                    }
                });
            }

            return robots;
        }

        private IEnumerable<OrderAssignment> generateOrderAssignments(int numOfAssignments)
        {
            var orderAssignments = new List<OrderAssignment>();
            for (int i = 0; i < numOfAssignments; i++)
            {
                orderAssignments.Add(new OrderAssignment(new Order(), new Robot()));
            }

            return orderAssignments;
        }
    }
}
