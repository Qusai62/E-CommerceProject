using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.UpdateCustomerProfile
{
    public sealed class UpdateCustomerProfileService
    {
        private readonly ICustomerProfileUpdateGateway _gateway;

        public UpdateCustomerProfileService(
            ICustomerProfileUpdateGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<Result<UpdateCustomerProfileResponse>> ExecuteAsync(
            Guid applicationUserId,
            UpdateCustomerProfileCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            // Normalize incoming profile information
            string firstName = command.FirstName?.Trim() ?? string.Empty;
            string lastName = command.LastName?.Trim() ?? string.Empty;
            string email = command.Email?.Trim() ?? string.Empty;
            string phone = command.PhoneNumber?.Trim() ?? string.Empty;

            // BR-014: Validate names
            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(new Error(
                    "Customer.FirstNameRequired",
                    "First name is required.",
                    nameof(command.FirstName)));
            }
            else if (firstName.Length > 100)
            {
                errors.Add(new Error(
                    "Customer.FirstNameTooLong",
                    "First name cannot exceed 100 characters.",
                    nameof(command.FirstName)));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(new Error(
                    "Customer.LastNameRequired",
                    "Last name is required.",
                    nameof(command.LastName)));
            }
            else if (lastName.Length > 100)
            {
                errors.Add(new Error(
                    "Customer.LastNameTooLong",
                    "Last name cannot exceed 100 characters.",
                    nameof(command.LastName)));
            }

            // BR-015: Validate email
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(new Error(
                    "Customer.EmailRequired",
                    "Email is required.",
                    nameof(command.Email)));
            }
            else if (
                email.Length > 256 ||
                !new EmailAddressAttribute().IsValid(email))
            {
                errors.Add(new Error(
                    "Customer.InvalidEmail",
                    "Enter a valid email address.",
                    nameof(command.Email)));
            }

            // BR-018: Validate phone number
            if (string.IsNullOrWhiteSpace(phone))
            {
                errors.Add(new Error(
                    "Customer.PhoneRequired",
                    "Phone number is required.",
                    nameof(command.PhoneNumber)));
            }
            else if (!Regex.IsMatch(
                phone, @"^\+[1-9]\d{7,14}$"))
            {
                errors.Add(new Error(
                    "Customer.InvalidPhone",
                    "Phone number must use international format.",
                    nameof(command.PhoneNumber)));
            }

            if (errors.Count > 0)
            {
                return Result<UpdateCustomerProfileResponse>
                    .Failure(errors);
            }

            var normalizedCommand = new UpdateCustomerProfileCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone,
                CurrentPassword = command.CurrentPassword
            };

            return await _gateway.UpdateAsync(
                applicationUserId,
                normalizedCommand,
                cancellationToken);
        }
    }
}
