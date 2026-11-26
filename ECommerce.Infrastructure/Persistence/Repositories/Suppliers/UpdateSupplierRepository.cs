using ECommerce.Application.Suppliers.UpdateSupplier;
using ECommerce.Domain.Supplier_Invoice___Payment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Suppliers
{
    public sealed class UpdateSupplierRepository
     : IUpdateSupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public UpdateSupplierRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Supplier?> GetByIdAsync(
            long supplierId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(
                    s => s.SupplierId == supplierId,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
