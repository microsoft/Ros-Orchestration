using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RobotOrchestrator.OrderProducer
{
    public class OrderFactory : IOrderFactory
    {
        public double MinX { get; set; } = 20;
        public double MaxX { get; set; } = 50;

        public double MinY { get; set; } = 20;
        public double MaxY { get; set; } = 50;

        private ILogger logger;

        private Random random;

        public OrderFactory(ILogger<OrderFactory> logger)
        {
            this.logger = logger;
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public Order CreateOrder(string message = null)
        {
            var order = new Order()
            {
                StartPosition = GenerateRandomPosition(),
                EndPosition = GenerateRandomPosition(),
                Message = message
            };

            logger.LogDebug($"Created Order{JsonConvert.SerializeObject(order)}");

            return order;
        }

        public IEnumerable<Order> CreateOrders(int numOfOrders, string message = null)
        {
            if (numOfOrders <= 0)
            {
                throw new ArgumentOutOfRangeException("numOfOrders");
            }

            var orders = Enumerable.Range(0, numOfOrders).Select(i => CreateOrder($"Order{i}"));

            return orders;
        }

        private Position GenerateRandomPosition() {

            var x = random.NextDouble() * (MaxX - MinX) + MinX;
            var y = random.NextDouble() * (MaxY - MinY) + MinY;

            var position = new Position(x, y);

            return position;
        }
    }
}
