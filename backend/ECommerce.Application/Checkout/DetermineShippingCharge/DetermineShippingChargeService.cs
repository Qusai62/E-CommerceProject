using ECommerce.Application.Common.Results;
using ECommerce.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.DetermineShippingCharge
{
    public sealed class DetermineShippingChargeService
    {
        private readonly IShippingConfigurationReadRepository _repository;

        public DetermineShippingChargeService(
            IShippingConfigurationReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<DetermineShippingChargeResponse>> ExecuteAsync(
            CancellationToken cancellationToken = default)
        {
            // 1. Read the Store Owner's shipping configuration.
            var configuration = await _repository.GetAsync(
                cancellationToken);

            // 2. Ensure shipping configuration exists.
            if (configuration is null)
            {
                return Result<DetermineShippingChargeResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.ShippingConfigurationMissing",
                        "Shipping configuration is unavailable.")
                    });
            }

            // 3. Validate the configured shipping mode.
            if (configuration.ShippingMode is not
                (ShippingMode.FixedRate or ShippingMode.FreeShipping))
            {
                return Result<DetermineShippingChargeResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.InvalidShippingMode",
                        "The shipping configuration has an invalid mode.")
                    });
            }

            // 4. Determine the effective shipping charge.
            decimal shippingCharge;

            if (configuration.ShippingMode == ShippingMode.FixedRate)
            {
                shippingCharge = 0m;
            }
            else
            {
                if (configuration.ShippingCharge < 0)
                {
                    return Result<DetermineShippingChargeResponse>.Failure(
                        new[]
                        {
                        new Error(
                            "Checkout.InvalidShippingCharge",
                            "The configured shipping charge is invalid.")
                        });
                }

                shippingCharge = configuration.ShippingCharge;
            }

            // 5. Return the effective shipping charge.
            return Result<DetermineShippingChargeResponse>.Success(
                new DetermineShippingChargeResponse(
                    configuration.ShippingMode,
                    shippingCharge));
        }
    }
}
