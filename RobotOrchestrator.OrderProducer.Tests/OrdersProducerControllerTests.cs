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
    public class OrdersProducerControllerTests
    {
        [Fact]
        public void PostStartProducer_Start_ReturnsSuccess()
        {
            var controller = CreateController();

            var actionResult = controller.PostStartProducer(new BatchJobOptions());
            var okResult = actionResult as OkResult;

            Assert.NotNull(okResult);
            Assert.Equal((int?)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public void PostStopProducer_Stop_ReturnsSuccess()
        {
            var controller = CreateController();

            var actionResult = controller.PostStartProducer(new BatchJobOptions());
            var okResult = actionResult as OkResult;

            Assert.NotNull(okResult);
            Assert.Equal((int?)HttpStatusCode.OK, okResult.StatusCode);
        }

        private OrdersProducerController CreateController()
        {
            var orderHandler = Mock.Of<IOrderHandler>();
            var batchManager = Mock.Of<IBatchManager>();
            var logger = Mock.Of<ILogger<OrdersController>>();
            var controller = new OrdersProducerController(orderHandler, batchManager, logger);

            return controller;
        }
    }
}
