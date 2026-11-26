using ECommerce.Domain.Supplier_Invoice___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.CreateSupplier
{
    public interface ICreateSupplierRepository
    {
        Task AddAsync(
            Supplier supplier,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
