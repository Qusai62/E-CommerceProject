using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.CreateSupplier
{
    public sealed record CreateSupplierRequest(
    string Name,
    string? PhoneNumber,
    string? Email
);
}
