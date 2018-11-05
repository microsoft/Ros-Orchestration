using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotOrchestrator.OrderManager
{
    public class OrderAssignment
    {
        public OrderAssignment(Order order, Robot robot)
        {
            Robot = robot;
            Order = order;
        }

        public Order Order { get; set; }

        public Robot Robot { get; set; }

        public bool IsAssigned => Robot != null;
    }
}
