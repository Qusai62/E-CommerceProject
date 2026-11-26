using ECommerce.Application.Inventory.CreateStockReservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.Abstractions
{
    public interface IStockReservationWriteRepository
    {
        Task<CreateStockReservationOutcome> CreateAsync(
        long paymentAttemptId,
        DateTime utcNow,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default);
    }
}
