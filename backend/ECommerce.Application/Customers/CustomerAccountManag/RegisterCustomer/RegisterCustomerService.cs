using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.RegisterCustomer
{
    public class RegisterCustomerService
    {
        private readonly ICustomerRegistrationGateway _gateway;

        public RegisterCustomerService(
            ICustomerRegistrationGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<Result<RegisterCustomerResponse>> ExecuteAsync(
            RegisterCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            // Normalize customer information
            string firstName = command.FirstName?.Trim() ?? string.Empty;
            string lastName = command.LastName?.Trim() ?? string.Empty;
            string email = command.Email?.Trim() ?? string.Empty;
            string phoneNumber = command.PhoneNumber?.Trim() ?? string.Empty;

            // BR-004: First name and last name are required
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

            // BR-002, BR-003: Email validation
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(new Error(
                    "Customer.EmailRequired",
                    "Email address is required.",
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

            // BR-005: Phone number validation
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add(new Error(
                    "Customer.PhoneRequired",
                    "Phone number is required.",
                    nameof(command.PhoneNumber)));
            }
            else if (!Regex.IsMatch(
                phoneNumber, @"^\+[1-9]\d{7,14}$"))
            {
                errors.Add(new Error(
                    "Customer.InvalidPhone",
                    "Phone number must use international format.",
                    nameof(command.PhoneNumber)));
            }

            // BR-006: Password is required
            if (string.IsNullOrWhiteSpace(command.Password))
            {
                errors.Add(new Error(
                    "Customer.PasswordRequired",
                    "Password is required.",
                    nameof(command.Password)));
            }

            // Stop registration if validation fails
            if (errors.Count > 0)
            {
                return Result<RegisterCustomerResponse>.Failure(errors);
            }

            // Create the normalized command
            var normalizedCommand = new RegisterCustomerCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Password = command.Password
            };

            // Delegate account creation to Infrastructure
            return await _gateway.RegisterAsync(
                normalizedCommand,
                cancellationToken);
        }
    }
}
