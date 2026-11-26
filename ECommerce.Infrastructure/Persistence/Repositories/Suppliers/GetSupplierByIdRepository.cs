using ECommerce.Application.Suppliers.GetSupplierById;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Suppliers
{
    public sealed class GetSupplierByIdRepository
     : IGetSupplierByIdRepository
    {
        private readonly ApplicationDbContext _context;

        public GetSupplierByIdRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupplierResponse?> GetByIdAsync(
            long supplierId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.SupplierId == supplierId)
                .Select(s => new SupplierResponse(
                    s.SupplierId,
                    s.Name,
                    s.PhoneNumber,
                    s.Email,
                    s.CreatedAtUtc,
                    s.UpdatedAtUtc
                ))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
