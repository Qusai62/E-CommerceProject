using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.Addresses.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.UpdateCustomerAddress
{
    public class UpdateCustomerAddressService
    {
        private readonly ICustomerAddressRepository _repository;
        private readonly TimeProvider _timeProvider;

        public UpdateCustomerAddressService(
            ICustomerAddressRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<UpdateCustomerAddressResponse>> ExecuteAsync(
            Guid applicationUserId,
            long addressId,
            UpdateCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            // Step 1: Validate the address ID
            if (addressId <= 0)
            {
                return Result<UpdateCustomerAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Address.InvalidId",
                        "A valid address ID is required.")
                    });
            }

            // Step 2: Validate all address information
            var errors = new List<Error>();

            ValidateRequiredField(
                command.RecipientName, 200, "RecipientName", errors);

            ValidateRequiredField(
                command.City, 100, "City", errors);

            ValidateRequiredField(
                command.Neighborhood, 150, "Neighborhood", errors);

            ValidateRequiredField(
                command.Street, 200, "Street", errors);

            ValidateRequiredField(
                command.BuildingInfo, 200, "BuildingInfo", errors);

            var contactNumber = command.ContactNumber?.Trim();

            if (string.IsNullOrWhiteSpace(contactNumber) ||
                contactNumber.Length > 30 ||
                !Regex.IsMatch(
                    contactNumber,
                    @"^\+?[0-9]{7,15}$"))
            {
                errors.Add(new Error(
                    "ContactNumber.Invalid",
                    "A valid contact number is required."));
            }

            if (command.AdditionalAddressInfo?.Trim().Length > 500)
            {
                errors.Add(new Error(
                    "AdditionalAddressInfo.TooLong",
                    "Additional address information cannot exceed 500 characters."));
            }

            if (errors.Count > 0)
            {
                return Result<UpdateCustomerAddressResponse>.Failure(errors);
            }

            // Step 3: Find the authenticated customer
            var customerId =
                await _repository.GetCustomerIdByUserIdAsync(
                    applicationUserId,
                    cancellationToken);

            if (customerId is null)
            {
                return Result<UpdateCustomerAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Customer.NotFound",
                        "Customer account was not found.")
                    });
            }

            // Step 4: Find the address AND verify ownership
            var address = await _repository.GetTrackedByIdAsync(
                addressId,
                customerId.Value,
                cancellationToken);

            if (address is null)
            {
                return Result<UpdateCustomerAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Address.NotFound",
                        "Address was not found.")
                    });
            }

            // Step 5: Update the tracked entity
            address.RecipientName = command.RecipientName.Trim();
            address.ContactNumber = contactNumber!;
            address.City = command.City.Trim();
            address.Neighborhood = command.Neighborhood.Trim();
            address.Street = command.Street.Trim();
            address.BuildingInfo = command.BuildingInfo.Trim();

            address.AdditionalAddressInfo =
                string.IsNullOrWhiteSpace(command.AdditionalAddressInfo)
                    ? null
                    : command.AdditionalAddressInfo.Trim();

            address.UpdatedAtUtc =
                _timeProvider.GetUtcNow().UtcDateTime;

            // Step 6: Save changes
            await _repository.SaveChangesAsync(cancellationToken);

            // Step 7: Build the response
            var response = new UpdateCustomerAddressResponse(
                address.SavedAddressId,
                address.RecipientName,
                address.ContactNumber,
                address.City,
                address.Neighborhood,
                address.Street,
                address.BuildingInfo,
                address.AdditionalAddressInfo,
                address.IsDefault,
                address.UpdatedAtUtc);

            return Result<UpdateCustomerAddressResponse>.Success(response);
        }

        private static void ValidateRequiredField(
            string? value,
            int maxLength,
            string fieldName,
            List<Error> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new Error(
                    $"{fieldName}.Required",
                    $"{fieldName} is required."));

                return;
            }

            if (value.Trim().Length > maxLength)
            {
                errors.Add(new Error(
                    $"{fieldName}.TooLong",
                    $"{fieldName} cannot exceed {maxLength} characters."));
            }
        }
    }
}
