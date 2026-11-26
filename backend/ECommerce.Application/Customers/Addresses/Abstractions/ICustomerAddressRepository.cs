using ECommerce.Application.Customers.Addresses.GetCustomerAddresses;
using ECommerce.Domain.CustomerAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Addresses.Abstractions
{
    public interface ICustomerAddressRepository
    {
        Task<long?> GetCustomerIdByUserIdAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default);

        void Add(SavedAddress address);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<CustomerAddressResponse>> GetByCustomerIdAsync(
        long customerId,
        CancellationToken cancellationToken = default);
    

    Task<SavedAddress?> GetTrackedByIdAsync(
    long addressId,
    long customerId,
    CancellationToken cancellationToken = default);

        Task<bool> HasOtherAddressesAsync(
    long customerId,
    long addressId,
    CancellationToken cancellationToken = default);

        void Remove(SavedAddress address);

        Task<bool> SetDefaultAsync(
    long customerId,
    long addressId,
    DateTime updatedAtUtc,
    CancellationToken cancellationToken = default);
    }
}