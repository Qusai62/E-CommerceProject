using ECommerce.Application.Suppliers.CreateSupplier;
using ECommerce.Domain.Supplier_Invoice___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Suppliers
{
    public sealed class CreateSupplierRepository
     : ICreateSupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public CreateSupplierRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            Supplier supplier,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(supplier);

            await _context.Suppliers.AddAsync(
                supplier,
                cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}
