using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.Addresses.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.DeleteCustomerAddress
{
    public class DeleteCustomerAddressService
    {
        private readonly ICustomerAddressRepository _repository;

        public DeleteCustomerAddressService(
            ICustomerAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<long>> ExecuteAsync(
            Guid applicationUserId,
            long addressId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the address ID
            if (addressId <= 0)
            {
                return Result<long>.Failure(
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
                return Result<long>.Failure(
                    new[]
                    {
                    new Error(
                        "Customer.NotFound",
                        "Customer account was not found.")
                    });
            }

            // Step 3: Retrieve the address and verify ownership
            var address = await _repository.GetTrackedByIdAsync(
                addressId,
                customerId.Value,
                cancellationToken);

            if (address is null)
            {
                return Result<long>.Failure(
                    new[]
                    {
                    new Error(
                        "Address.NotFound",
                        "Address was not found.")
                    });
            }

            // Step 4: Protect the default address
            if (address.IsDefault)
            {
                var hasOtherAddresses =
                    await _repository.HasOtherAddressesAsync(
                        customerId.Value,
                        addressId,
                        cancellationToken);

                if (hasOtherAddresses)
                {
                    return Result<long>.Failure(
                        new[]
                        {
                        new Error(
                            "Address.DefaultDeletionNotAllowed",
                            "Set another address as default before deleting this address.")
                        });
                }
            }

            // Step 5: Mark the address for deletion
            _repository.Remove(address);

            // Step 6: Commit changes to SQL Server
            await _repository.SaveChangesAsync(
                cancellationToken);

            // Step 7: Return success
            return Result<long>.Success(addressId);
        }
    }
}
