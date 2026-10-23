using ECommerce.Domain.Cart;
using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.CustomerAccount
{
    public class Customer
    {
        public long CustomerId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public ICollection<SavedAddress> SavedAddresses { get; set; } = new List<SavedAddress>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ShoppingCart ShoppingCart { get; set; } = null!;
       
    }
}
