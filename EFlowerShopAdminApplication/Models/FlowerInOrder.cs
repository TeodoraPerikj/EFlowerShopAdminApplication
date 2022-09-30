using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFlowerShopAdminApplication.Models
{
    public class FlowerInOrder
    {
        public Guid FlowerId { get; set; }
        public Flower Flower { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
    }
}
