using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.GetSupplierById
{
    public interface IGetSupplierByIdRepository
    {
        Task<SupplierResponse?> GetByIdAsync(
            long supplierId,
            CancellationToken cancellationToken = default);
    }
}
