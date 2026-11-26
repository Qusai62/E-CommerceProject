using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.DeactivateBrand
{
    public sealed record DeactivateBrandResponse(
    long BrandId,
    string Name,
    bool IsActive,
    DateTime UpdatedAtUtc
);
}
