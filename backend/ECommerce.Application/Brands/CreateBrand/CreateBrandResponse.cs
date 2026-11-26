using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.CreateBrand
{
    public sealed record CreateBrandResponse(
    long BrandId,
    string Name,
    string Slug,
    string? Description,
    string? LogoStorageKey,
    bool IsActive,
    DateTime CreatedAtUtc
);
}
