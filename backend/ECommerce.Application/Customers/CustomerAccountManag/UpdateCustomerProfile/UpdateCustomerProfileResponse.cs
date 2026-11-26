using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.UpdateCustomerProfile
{
    public sealed class UpdateCustomerProfileResponse
    {
        public long CustomerId { get; init; }

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string PhoneNumber { get; init; } = string.Empty;

        public bool IsEmailVerified { get; init; }

        public DateTime UpdatedAtUtc { get; init; }
    }

}
