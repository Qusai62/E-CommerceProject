using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.UpdatePaymentStatus
{
    public sealed class UpdatePaymentStatusService
    {
        private readonly IUpdatePaymentStatusRepository _repository;
        private readonly TimeProvider _timeProvider;

        public UpdatePaymentStatusService(
            IUpdatePaymentStatusRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<UpdatePaymentStatusResult> ExecuteAsync(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Get the payment associated with the order.
            var payment = await _repository.GetPaymentByOrderIdAsync(
                orderId,
                cancellationToken);

            // Step 2: Validate that payment exists.
            if (payment is null)
            {
                return UpdatePaymentStatusResult.PaymentNotFound;
            }

            // Step 3: Validate payment method.
            if (payment.Method != PaymentMethod.CashOnDelivery)
            {
                return UpdatePaymentStatusResult.InvalidPaymentMethod;
            }

            // Step 4: Validate current payment status.
            if (payment.Status != PaymentStatus.Unpaid)
            {
                return UpdatePaymentStatusResult.AlreadyPaid;
            }

            // Step 5: Update payment using Domain behavior.
            payment.MarkAsPaid(
                _timeProvider.GetUtcNow().UtcDateTime);

            // Step 6: Persist the changes.
            var saved = await _repository.SaveChangesAsync(
    cancellationToken);

            if (!saved)
            {
                return UpdatePaymentStatusResult.ConcurrencyConflict;
            }

            return UpdatePaymentStatusResult.Success;

            
        }
    }
}
