// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Linq;
using RobotOrchestrator.OrderProducer;
using Moq;

namespace RobotOrchestrator.OrderProducer.Tests
{
    public class OrderFactoryTests
    {
        [Fact]
        public void CreateOrder_SingleOrder_ReturnsSingleOrder()
        {
            var logger = Mock.Of<ILogger<OrderFactory>>();
            var factory = new OrderFactory(logger);

            var message = "custom message";
            var order = factory.CreateOrder(message);

            Assert.NotNull(order);
            Assert.Equal(message, order.Message);

            var noMessageOrder = factory.CreateOrder();

            Assert.NotNull(noMessageOrder);
            Assert.Null(noMessageOrder.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void CreateOrders_MultipleOrders_ReturnsMultipleOrders(int numOfOrders)
        {
            var logger = Mock.Of<ILogger<OrderFactory>>();
            var factory = new OrderFactory(logger);

            var orders = factory.CreateOrders(numOfOrders);

            Assert.NotNull(orders);
            Assert.Equal(numOfOrders, orders.Count());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void CreateOrders_InvalidNumOrders_ThrowsArgumentException(int numOfOrders)
        {
            var logger = Mock.Of<ILogger<OrderFactory>>();
            var factory = new OrderFactory(logger);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => factory.CreateOrders(numOfOrders));
        }
    }
}
