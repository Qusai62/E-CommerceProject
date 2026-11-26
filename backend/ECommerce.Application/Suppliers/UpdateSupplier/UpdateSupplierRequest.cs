using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.UpdateSupplier
{
    public sealed record UpdateSupplierRequest(
    string Name,
    string? PhoneNumber,
    string? Email
);
}
