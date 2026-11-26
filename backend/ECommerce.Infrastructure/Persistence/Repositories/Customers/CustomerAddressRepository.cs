using ECommerce.Application.Customers.Addresses.Abstractions;
using ECommerce.Application.Customers.Addresses.GetCustomerAddresses;
using ECommerce.Domain.CustomerAccount;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Customers
{
    public class CustomerAddressRepository: ICustomerAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerAddressRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<long?> GetCustomerIdByUserIdAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(customer =>
                    customer.ApplicationUserId == applicationUserId)
                .Select(customer =>
                    (long?)customer.CustomerId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public void Add(SavedAddress address)
        {
            _context.SavedAddresses.Add(address);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CustomerAddressResponse>>
    GetByCustomerIdAsync(
        long customerId,
        CancellationToken cancellationToken = default)
        {
            return await _context.SavedAddresses
                .AsNoTracking()
                .Where(address => address.CustomerId == customerId)
                .OrderByDescending(address => address.IsDefault)
                .ThenByDescending(address => address.CreatedAtUtc)
                .ThenByDescending(address => address.SavedAddressId)
                .Select(address => new CustomerAddressResponse(
                    address.SavedAddressId,
                    address.RecipientName,
                    address.ContactNumber,
                    address.City,
                    address.Neighborhood,
                    address.Street,
                    address.BuildingInfo,
                    address.AdditionalAddressInfo,
                    address.IsDefault
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<SavedAddress?> GetTrackedByIdAsync(
    long addressId,
    long customerId,
    CancellationToken cancellationToken = default)
        {
            return await _context.SavedAddresses
                .AsTracking()
                .FirstOrDefaultAsync(
                    address =>
                        address.SavedAddressId == addressId &&
                        address.CustomerId == customerId,
                    cancellationToken);
        }

        public Task<bool> HasOtherAddressesAsync(
    long customerId,
    long addressId,
    CancellationToken cancellationToken = default)
        {
            return _context.SavedAddresses
                .AsNoTracking()
                .AnyAsync(
                    address =>
                        address.CustomerId == customerId &&
                        address.SavedAddressId != addressId,
                    cancellationToken);
        }

        public void Remove(SavedAddress address)
        {
            _context.SavedAddresses.Remove(address);
        }

        public async Task<bool> SetDefaultAsync(
    long customerId,
    long addressId,
    DateTime updatedAtUtc,
    CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                // Step 1: Begin transaction
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(
                        IsolationLevel.Serializable,
                        cancellationToken);

                // Step 2: Verify ownership and retrieve default status
                bool? isAlreadyDefault =
                    await _context.SavedAddresses
                        .AsNoTracking()
                        .Where(address =>
                            address.SavedAddressId == addressId &&
                            address.CustomerId == customerId)
                        .Select(address => (bool?)address.IsDefault)
                        .SingleOrDefaultAsync(cancellationToken);

                // Step 3: Address not found or not owned
                if (isAlreadyDefault is null)
                {
                    return false;
                }

                // Step 4: Selected address is already default
                if (isAlreadyDefault.Value)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return true;
                }

                // Step 5: Remove the previous default
                await _context.SavedAddresses
                    .Where(address =>
                        address.CustomerId == customerId &&
                        address.IsDefault)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(
                                address => address.IsDefault,
                                false)
                            .SetProperty(
                                address => address.UpdatedAtUtc,
                                updatedAtUtc),
                        cancellationToken);

                // Step 6: Set the selected address as default
                var affectedRows =
                    await _context.SavedAddresses
                        .Where(address =>
                            address.CustomerId == customerId &&
                            address.SavedAddressId == addressId)
                        .ExecuteUpdateAsync(
                            setters => setters
                                .SetProperty(
                                    address => address.IsDefault,
                                    true)
                                .SetProperty(
                                    address => address.UpdatedAtUtc,
                                    updatedAtUtc),
                            cancellationToken);

                // Step 7: Validate the update
                if (affectedRows != 1)
                {
                    // Disposing without commit rolls back the transaction.
                    return false;
                }

                // Step 8: Commit both updates
                await transaction.CommitAsync(cancellationToken);

                return true;
            });
        }
    }
}
