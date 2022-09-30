using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFlowerShopAdminApplication.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public EFlowerShopApplicationUser User { get; set; }

        public IEnumerable<FlowerInOrder> FlowerInOrders { get; set; }
    }
}
