using ECommerce.Application.Checkout.SelectSavedAddress;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Checkout
{
    public class CheckoutAddressReadRepository: ICheckoutAddressReadRepository
    {
        private readonly ApplicationDbContext _context;

        public CheckoutAddressReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CheckoutAddressSnapshot?> GetAsync(
            Guid applicationUserId,
            long savedAddressId,
            CancellationToken cancellationToken = default)
        {
            return await _context.SavedAddresses
                .AsNoTracking()
                .Where(address =>
                    address.SavedAddressId == savedAddressId &&
                    address.Customer.ApplicationUserId == applicationUserId)
                .Select(address => new CheckoutAddressSnapshot(
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
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
