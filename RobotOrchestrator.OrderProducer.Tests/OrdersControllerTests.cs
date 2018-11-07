// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Moq;
using RobotOrchestrator.OrderProducer.Controllers;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer.Tests
{
    public class OrdersControllerTests
    {
        private Mock<IOrderManagerClient> mockOrderManagerClient;
        private Mock<ILogger<OrdersController>> mockLogger;
        private Mock<IOrderFactory> mockFactory;
        private OrdersController ordersController;

        public OrdersControllerTests()
        {
            mockOrderManagerClient = new Mock<IOrderManagerClient>();
            mockLogger = new Mock<ILogger<OrdersController>>();
            mockFactory = new Mock<IOrderFactory>();
            ordersController = new OrdersController(mockOrderManagerClient.Object, mockFactory.Object, mockLogger.Object);
        }

        [Fact]
        public void Post_SingleOrder_ReturnsSuccess()
        {
            var actionResult = ordersController.PostOrder("test order");
            var createdResult = new OkObjectResult(actionResult);

            Assert.NotNull(createdResult);
            Assert.NotNull(createdResult.Value);

            Assert.Equal((int?)HttpStatusCode.OK, createdResult.StatusCode);
        }
    }
}
