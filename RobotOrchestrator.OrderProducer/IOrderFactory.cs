using System.Collections.Generic;

namespace RobotOrchestrator.OrderProducer
{
    public interface IOrderFactory
    {
        Order CreateOrder(string message = null);

        IEnumerable<Order> CreateOrders(int numOfOrders, string message = null);
    }
}
