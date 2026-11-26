using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.UpdatePaymentStatus
{
    public interface IUpdatePaymentStatusRepository
    {
        Task<Payment?> GetPaymentByOrderIdAsync(
            long orderId,
            CancellationToken cancellationToken = default);

        Task<bool> SaveChangesAsync(
     CancellationToken cancellationToken = default);
    }
}
