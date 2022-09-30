using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFlowerShopAdminApplication.Models
{
    public class EFlowerShopApplicationUser
    {
        public string Email { get; set; }

        public string NormalizedUserName { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}
