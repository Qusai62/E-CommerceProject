using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.SelectSavedAddress
{
    public sealed record CheckoutAddressSnapshot(
     long SavedAddressId,
     string RecipientName,
     string ContactNumber,
     string City,
     string Neighborhood,
     string Street,
     string BuildingInfo,
     string? AdditionalAddressInfo,
     bool IsDefault
 );
}
