using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFlowerShopAdminApplication.Models
{
    public class Flower
    {
        public Guid Id { get; set; }
        public string FlowerName { get; set; }

        public string FlowerImage { get; set; }

        public string FlowerDescription { get; set; }

        public double FlowerPrice { get; set; }
    }
}
