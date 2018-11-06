using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RobotOrchestrator.OrderProducer
{
    public class OrderManagerClient : IOrderManagerClient
    {
        private static readonly HttpClient httpClient;

        private readonly string orderManagerUri;

        static OrderManagerClient()
        {
            httpClient = new HttpClient();
        }

        public OrderManagerClient(string orderManagerUri)
        {
            if (string.IsNullOrWhiteSpace(orderManagerUri))
            {
                throw new ArgumentException("Value of orderManagerUri must be a valid Uri", "orderManagerUri");
            }
            this.orderManagerUri = orderManagerUri + "/batch";
        }

        public async Task<string> SendOrderAsync(Order order)
        {
            var orders = new List<Order> { order };
            var result = await SendOrdersAsync(orders);
            return result;
        }

        public async Task<string> SendOrdersAsync(List<Order> orders)
        {
            var ordersAsJson = JsonConvert.SerializeObject(orders);
            var content = new StringContent(ordersAsJson, Encoding.UTF8, "application/json");
            var httpResponseMessage = await httpClient.PostAsync(orderManagerUri, content);
            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            return response;
        }
    }
}
