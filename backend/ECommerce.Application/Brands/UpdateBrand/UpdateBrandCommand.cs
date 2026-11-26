using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.UpdateBrand
{
    public sealed record UpdateBrandCommand(
     long BrandId,
     string Name,
     string? Description,
     string? LogoStorageKey
 );
}
