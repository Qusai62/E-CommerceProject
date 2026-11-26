using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.UpdateCustomerAddress
{
    public sealed record UpdateCustomerAddressResponse(
    long SavedAddressId,
    string RecipientName,
    string ContactNumber,
    string City,
    string Neighborhood,
    string Street,
    string BuildingInfo,
    string? AdditionalAddressInfo,
    bool IsDefault,
    DateTime UpdatedAtUtc
);
}
