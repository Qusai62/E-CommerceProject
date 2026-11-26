using ECommerce.Application.StoreOrders.UpdatePaymentStatus;
using ECommerce.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.StoreOrders
{
    public sealed class UpdatePaymentStatusRepository
     : IUpdatePaymentStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public UpdatePaymentStatusRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .SingleOrDefaultAsync(
                    p => p.OrderId == orderId,
                    cancellationToken);
        }

        public async Task<bool> SaveChangesAsync(
    CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
    }
}
