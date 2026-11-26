using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.Addresses.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.GetCustomerAddresses
{
    public class GetCustomerAddressesService
    {
        private readonly ICustomerAddressRepository _repository;

        public GetCustomerAddressesService(
            ICustomerAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IReadOnlyList<CustomerAddressResponse>>>
            ExecuteAsync(
                Guid applicationUserId,
                CancellationToken cancellationToken = default)
        {
            // Step 1: Resolve the authenticated customer
            var customerId =
                await _repository.GetCustomerIdByUserIdAsync(
                    applicationUserId,
                    cancellationToken);

            // Step 2: Handle customer not found
            if (customerId is null)
            {
                return Result<IReadOnlyList<CustomerAddressResponse>>
                    .Failure(new[]
                    {
                    new Error(
                        "Customer.NotFound",
                        "Customer account was not found.")
                    });
            }

            // Step 3: Retrieve the customer's saved addresses
            var addresses =
                await _repository.GetByCustomerIdAsync(
                    customerId.Value,
                    cancellationToken);

            // Step 4: Return the result
            return Result<IReadOnlyList<CustomerAddressResponse>>
                .Success(addresses);
        }
    }
}
