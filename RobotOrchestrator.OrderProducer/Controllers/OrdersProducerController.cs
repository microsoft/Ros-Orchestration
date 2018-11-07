// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrdersProducerController : ControllerBase
    {
        private readonly IOrderHandler orderHandler;

        private readonly IBatchManager batchManager;

        private readonly ILogger logger;

        public OrdersProducerController(
            IOrderHandler orderHandler,
            IBatchManager batchManager,
            ILogger<OrdersController> logger)
        {
            this.orderHandler = orderHandler;
            this.batchManager = batchManager;
            this.logger = logger;
        }

        [HttpPost("start")]
        public IActionResult PostStartProducer(BatchJobOptions options)
        {
            IActionResult result;

            try
            {
                batchManager.StartBatchJob(orderHandler.HandleBatch, options);
            }
            catch (Exception ex)
            {
                result = new BadRequestObjectResult(ex.Message);
                return result;
            }

            result = new OkResult();
            return result;
        }

        [HttpPost("stop")]
        public IActionResult PostStopProducer()
        {
            IActionResult result;

            try
            {
                batchManager.StopBatchJob();
            }
            catch (Exception ex)
            {
                result = new BadRequestObjectResult(ex.Message);
                return result;
            }

            result = new OkResult();
            return result;
        }

        [HttpGet("status")]
        public IActionResult GetProducerStatus()
        {
            var streamStatus = batchManager.HasActiveBatchJob();

            var result = new OkObjectResult(new { IsRunning = streamStatus } );
            return result;

        }
    }
}
