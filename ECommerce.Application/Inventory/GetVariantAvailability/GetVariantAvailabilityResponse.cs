using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.GetVariantAvailability
{
    public sealed record GetVariantAvailabilityResponse(
    long ProductVariantId,
    int AvailableQuantity,
    bool IsInStock
);
}
