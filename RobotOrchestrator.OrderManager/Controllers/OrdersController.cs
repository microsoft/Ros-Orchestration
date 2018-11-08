// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace RobotOrchestrator.OrderManager.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManager orderManager;

        public OrdersController(IOrderManager orderManager)
        {
            this.orderManager = orderManager;
        }

        [HttpGet]
        [EnableCors("AllowAllOrigin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersAsync(OrderStatus? status, int? numOrders)
        {
            IEnumerable<Order> orders;

            try
            {
                orders = await orderManager.GetOrdersAsync(status, numOrders);
            }
            catch (Exception ex)
            {
                var badObjectResult = new BadRequestObjectResult(ex.Message);
                return badObjectResult;
            }

            var result = new OkObjectResult(orders);
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderAsync(Guid id)
        {
            Order order;

            try
            {
                order = await orderManager.GetOrderAsync(id);
            }
            catch (RecordNotFoundException)
            {
                var notFoundResult = new NotFoundResult();
                return notFoundResult;
            }
            catch (Exception ex)
            {
                var badObjectResult = new BadRequestObjectResult(ex.Message);
                return badObjectResult;
            }

            var result = new OkObjectResult(order);
            return result;
        }

        [HttpPost()]
        public async Task<ActionResult> PostOrderAsync([FromBody] Order order, string robotId = null)
        {
            order = await orderManager.AcceptOrderAsync(order, robotId);

            var result = new OkObjectResult(order);

            return result;
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Guid>>> PostOrdersAsync([FromBody] IEnumerable<Order> orders)
        {
            orders = await orderManager.AcceptOrdersAsync(orders);

            var orderIds = orders.Select(order => order.Id);

            var result = Ok(orderIds);
            return result;
        }
    }
}
