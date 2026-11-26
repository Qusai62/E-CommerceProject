using ECommerce.Application.Checkout.SelectPaymentMethod;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Orders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.PlaceOrder
{
    public sealed class PlaceOrderService
    {
        private readonly IPlaceOrderRepository _placeOrderRepository;
        private readonly IPaymentMethodValidator _paymentMethodValidator;
        private readonly ILogger<PlaceOrderService> _logger;
        public PlaceOrderService(
            IPlaceOrderRepository placeOrderRepository,
            IPaymentMethodValidator paymentMethodValidator,
             ILogger<PlaceOrderService> logger)
        {
            _placeOrderRepository = placeOrderRepository;
            _paymentMethodValidator = paymentMethodValidator;
            _logger = logger;
        }

        public async Task<Result<PlaceOrderResponse>> ExecuteAsync(
            Guid applicationUserId,
            PlaceOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate authenticated customer identity.
            if (applicationUserId == Guid.Empty)
            {
                return Result<PlaceOrderResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Order.InvalidCustomer",
                        "A valid customer identity is required.")
                    });
            }

            // Step 2: Validate checkout submission identifier.
            if (command.CheckoutSubmissionId == Guid.Empty)
            {
                return Result<PlaceOrderResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Order.InvalidSubmissionId",
                        "A valid checkout submission ID is required.")
                    });
            }

            // Step 3: Validate selected delivery address ID.
            if (command.SavedAddressId <= 0)
            {
                return Result<PlaceOrderResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Order.InvalidAddress",
                        "A valid delivery address must be selected.")
                    });
            }

            // Step 4: Validate selected payment method.
            if (!_paymentMethodValidator.IsSupported(
                command.PaymentMethod))
            {
                return Result<PlaceOrderResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Order.UnsupportedPaymentMethod",
                        "The selected payment method is not supported.")
                    });
            }



            // Step 5: Execute transactional order placement.

            var placedOrder =
    await _placeOrderRepository.PlaceAtomicallyAsync(
        applicationUserId,
        command,
        cancellationToken);

           
                _logger.LogInformation(
                    "Order {OrderId} placed successfully",
                    placedOrder.Value.OrderId);
            

            return placedOrder;


        }
    }
}
