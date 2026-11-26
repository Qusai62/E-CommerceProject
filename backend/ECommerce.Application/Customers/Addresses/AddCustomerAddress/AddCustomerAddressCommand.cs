using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.AddCustomerAddress
{
    public sealed record AddCustomerAddressCommand(
     string RecipientName,
     string ContactNumber,
     string City,
     string Neighborhood,
     string Street,
     string BuildingInfo,
     string? AdditionalAddressInfo
 );
}
