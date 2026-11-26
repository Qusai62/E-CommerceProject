using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.GetSupplierById
{
    public sealed record SupplierResponse(
      long SupplierId,
      string Name,
      string? PhoneNumber,
      string? Email,
      DateTime CreatedAtUtc,
      DateTime UpdatedAtUtc
  );
}
