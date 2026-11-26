using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.Addresses.Abstractions;
using ECommerce.Domain.CustomerAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.AddCustomerAddress
{
    public sealed class AddCustomerAddressService
    {
        private readonly ICustomerAddressRepository _repository;
        private readonly TimeProvider _timeProvider;

        public AddCustomerAddressService(
            ICustomerAddressRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<AddCustomerAddressResponse>> ExecuteAsync(
            Guid applicationUserId,
            AddCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the command
            ArgumentNullException.ThrowIfNull(command);

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

            // Step 2: Validate contact number
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

            // Step 3: Validate optional information
            if (command.AdditionalAddressInfo?.Trim().Length > 500)
            {
                errors.Add(new Error(
                    "AdditionalAddressInfo.TooLong",
                    "Additional address information cannot exceed 500 characters."));
            }

            // Step 4: Return validation errors
            if (errors.Count > 0)
            {
                return Result<AddCustomerAddressResponse>.Failure(errors);
            }

            // Step 5: Find the authenticated customer
            var customerId =
                await _repository.GetCustomerIdByUserIdAsync(
                    applicationUserId,
                    cancellationToken);

            if (customerId is null)
            {
                return Result<AddCustomerAddressResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Customer.NotFound",
                        "Customer account was not found.")
                    });
            }

            // Step 6: Create the entity
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var address = new SavedAddress
            {
                CustomerId = customerId.Value,
                RecipientName = command.RecipientName.Trim(),
                ContactNumber = contactNumber!,
                City = command.City.Trim(),
                Neighborhood = command.Neighborhood.Trim(),
                Street = command.Street.Trim(),
                BuildingInfo = command.BuildingInfo.Trim(),
                AdditionalAddressInfo =
                    string.IsNullOrWhiteSpace(command.AdditionalAddressInfo)
                        ? null
                        : command.AdditionalAddressInfo.Trim(),
                IsDefault = false,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            // Step 7: Track and save the new address
            _repository.Add(address);

            await _repository.SaveChangesAsync(cancellationToken);

            // Step 8: Create the response
            var response = new AddCustomerAddressResponse(
                address.SavedAddressId,
                address.RecipientName,
                address.ContactNumber,
                address.City,
                address.Neighborhood,
                address.Street,
                address.BuildingInfo,
                address.AdditionalAddressInfo,
                address.IsDefault
            );

            // Step 9: Return success
            return Result<AddCustomerAddressResponse>.Success(response);
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
