using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RobotOrchestrator.OrderProducer
{
    public interface IOrderManagerClient
    {
        Task<string> SendOrderAsync(Order order);

        Task<string> SendOrdersAsync(List<Order> orders);
    }
}
