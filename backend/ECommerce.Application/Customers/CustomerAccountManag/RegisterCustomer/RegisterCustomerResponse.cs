using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.RegisterCustomer
{
    public sealed class RegisterCustomerResponse
    {
        public long CustomerId { get; init; }

        public string Email { get; init; } = string.Empty;

        public bool IsEmailVerified { get; init; }

        public DateTime CreatedAtUtc { get; init; }
    }
}
