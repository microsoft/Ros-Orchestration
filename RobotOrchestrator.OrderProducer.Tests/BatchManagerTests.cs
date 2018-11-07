// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading;
using Moq;
using RobotOrchestrator.OrderProducer;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer.Tests
{
    public class BatchManagerTests
    {
        [Fact]
        public void StartBatchJob_WithNoExistingJob_StartsBatchJob()
        {
            BatchJobOptions options = new BatchJobOptions()
            {
                MaxItems = 100,
                BatchSize = 2,
                DelayInSecs = 1
            };

            BatchManager manager = CreateBatchManager();
            OrderHandler handler = CreateOrderHandler();

            Assert.False(manager.HasActiveBatchJob());

            manager.StartBatchJob(handler.HandleBatch, options);
            Assert.True(manager.HasActiveBatchJob());
        }

        [Fact]
        public void StopBatchJob_WithExistingJob_StopsBatchJob()
        {
            BatchJobOptions options = new BatchJobOptions()
            {
                MaxItems = 100,
                BatchSize = 2,
                DelayInSecs = 1
            };

            BatchManager manager = CreateBatchManager();
            OrderHandler handler = CreateOrderHandler();

            manager.StartBatchJob(handler.HandleBatch, options);
            Assert.True(manager.HasActiveBatchJob());

            manager.StopBatchJob();
            Assert.False(manager.HasActiveBatchJob());
        }

        [Fact]
        public void StartBatchJob_WithExistingJob_ThrowsInvalidOperation()
        {
            BatchJobOptions options = new BatchJobOptions()
            {
                MaxItems = 100,
                BatchSize = 2,
                DelayInSecs = 1
            };

            BatchManager manager = CreateBatchManager();
            OrderHandler handler = CreateOrderHandler();

            manager.StartBatchJob(handler.HandleBatch, options);
            Assert.True(manager.HasActiveBatchJob());

            Assert.Throws<InvalidOperationException>(() => 
                manager.StartBatchJob(handler.HandleBatch, options));
        }

        [Fact]
        public void StopBatchJob_WithNoExistingJob_ThrowsInvalidOperation()
        {
            BatchJobOptions options = new BatchJobOptions()
            {
                MaxItems = 100,
                BatchSize = 2,
                DelayInSecs = 1
            };

            BatchManager manager = CreateBatchManager();
            OrderHandler handler = CreateOrderHandler();

            Assert.False(manager.HasActiveBatchJob());
            Assert.Throws<InvalidOperationException>(() => manager.StopBatchJob());
        }

        [Theory]
        [InlineData(0,0,-1)]
        [InlineData(-2,-1,-1)]
        public void StartBatchJob_WithInvalidOptions_ThrowsArgumentException(
                int maxItems, int batchSize, int delayInSecs)
        {
            BatchJobOptions options = new BatchJobOptions()
            {
                MaxItems = maxItems,
                BatchSize = batchSize,
                DelayInSecs = delayInSecs
            };

            BatchManager manager = CreateBatchManager();
            OrderHandler handler = CreateOrderHandler();

            Assert.Throws<ArgumentException>(() => manager.StartBatchJob(handler.HandleBatch, options));
        }

        [Fact]
        public void StartBatchJob_WithNullOptions_ThrowsArgumentNullException()
        {
            BatchManager manager = CreateBatchManager();
            OrderHandler handler = CreateOrderHandler();

            Assert.Throws<ArgumentNullException>(() => manager.StartBatchJob(handler.HandleBatch, null));
        }

        private BatchManager CreateBatchManager()
        {
            var logger = Mock.Of<ILogger<BatchManager>>();
            BatchManager manager = new BatchManager(logger);

            return manager;
        }

        private OrderHandler CreateOrderHandler() 
        {
            var factory = CreateOrderFactory();
            var orderManagerClient = Mock.Of<IOrderManagerClient>();
            var orderHandlerLogger = Mock.Of<ILogger<OrderHandler>>();

            return new OrderHandler(orderManagerClient, factory, orderHandlerLogger);
        }

        private OrderFactory CreateOrderFactory() 
        {
            var factoryLogger = Mock.Of<ILogger<OrderFactory>>();
            var factory = new OrderFactory(factoryLogger);

            return factory;
        }
    }
}
