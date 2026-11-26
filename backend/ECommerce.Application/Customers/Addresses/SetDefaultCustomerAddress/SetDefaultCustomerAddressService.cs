using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.Addresses.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.SetDefaultCustomerAddress
{
    public class SetDefaultCustomerAddressService
    {
        private readonly ICustomerAddressRepository _repository;
        private readonly TimeProvider _timeProvider;

        public SetDefaultCustomerAddressService(
            ICustomerAddressRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<SetDefaultCustomerAddressResponse>> ExecuteAsync(
            Guid applicationUserId,
            long addressId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the address ID
            if (addressId <= 0)
            {
                return Result<SetDefaultCustomerAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Address.InvalidId",
                        "A valid address ID is required.")
                    });
            }

            // Step 2: Resolve the authenticated customer
            var customerId =
                await _repository.GetCustomerIdByUserIdAsync(
                    applicationUserId,
                    cancellationToken);

            if (customerId is null)
            {
                return Result<SetDefaultCustomerAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Customer.NotFound",
                        "Customer account was not found.")
                    });
            }

            // Step 3: Get the current UTC timestamp
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            // Step 4: Perform the atomic default switch
            var success = await _repository.SetDefaultAsync(
                customerId.Value,
                addressId,
                now,
                cancellationToken);

            // Step 5: Handle address not found or not owned
            if (!success)
            {
                return Result<SetDefaultCustomerAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Address.NotFound",
                        "Address was not found.")
                    });
            }

            // Step 6: Build response
            var response = new SetDefaultCustomerAddressResponse(
                addressId,
                true);

            return Result<SetDefaultCustomerAddressResponse>.Success(
                response);
        }
    }
}
