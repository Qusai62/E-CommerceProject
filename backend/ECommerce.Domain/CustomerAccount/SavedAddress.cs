using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.CustomerAccount
{
    public class SavedAddress
    {
        public long SavedAddressId { get; set; }

        public long CustomerId { get; set; }

        public string RecipientName { get; set; } = null!;

        public string ContactNumber { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Neighborhood { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string BuildingInfo { get; set; } = null!;

        public string? AdditionalAddressInfo { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Customer Customer { get; set; } = null!;
    }
}
