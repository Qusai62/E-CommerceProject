using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.SelectSavedAddress
{
    public class SelectCheckoutAddressService
    {
        private readonly ICheckoutAddressReadRepository _repository;

        public SelectCheckoutAddressService(
            ICheckoutAddressReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<SelectCheckoutAddressResponse>> ExecuteAsync(
            Guid applicationUserId,
            long savedAddressId,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate input identifiers.
            if (applicationUserId == Guid.Empty ||
                savedAddressId <= 0)
            {
                return Result<SelectCheckoutAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.InvalidAddressSelection",
                        "A valid customer and address are required.")
                    });
            }

            // 2. Retrieve the customer's selected address.
            var address = await _repository.GetAsync(
                applicationUserId,
                savedAddressId,
                cancellationToken);

            // 3. Reject missing or non-owned addresses.
            if (address is null)
            {
                return Result<SelectCheckoutAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.AddressNotFound",
                        "The selected address was not found.")
                    });
            }

            // 4. Validate required delivery information.
            if (string.IsNullOrWhiteSpace(address.RecipientName) ||
                string.IsNullOrWhiteSpace(address.ContactNumber) ||
                string.IsNullOrWhiteSpace(address.City) ||
                string.IsNullOrWhiteSpace(address.Neighborhood) ||
                string.IsNullOrWhiteSpace(address.Street) ||
                string.IsNullOrWhiteSpace(address.BuildingInfo))
            {
                return Result<SelectCheckoutAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.InvalidAddress",
                        "The selected address has missing required information.")
                    });
            }

            // 5. Return the validated address.
            return Result<SelectCheckoutAddressResponse>.Success(
                new SelectCheckoutAddressResponse(
                    address.SavedAddressId,
                    address.RecipientName,
                    address.ContactNumber,
                    address.City,
                    address.Neighborhood,
                    address.Street,
                    address.BuildingInfo,
                    address.AdditionalAddressInfo
                ));
        }
    }
}
