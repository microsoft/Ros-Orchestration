using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IOrderManagerClient orderManagerClient;
        private readonly IOrderFactory factory;

        public OrdersController(IOrderManagerClient orderManagerClient, IOrderFactory factory, ILogger<OrdersController> logger)
        {
            this.orderManagerClient = orderManagerClient;
            this.factory = factory;
            this.logger = logger;
        }
        
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] string message)
        {
            var order = factory.CreateOrder(message);

            var result = await orderManagerClient.SendOrderAsync(order);
            var okResult = new OkObjectResult(result);

            return okResult;
        }
    }
}
