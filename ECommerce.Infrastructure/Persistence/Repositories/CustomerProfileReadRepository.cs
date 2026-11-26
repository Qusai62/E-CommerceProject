using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using ECommerce.Application.Customers.CustomerAccountManag.GetCustomerProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class CustomerProfileReadRepository
           : ICustomerProfileReadRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerProfileReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetCustomerProfileResponse?> GetByApplicationUserIdAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(c => c.ApplicationUserId == applicationUserId)
                .Join(
                //Your Customer entity does not currently have an ApplicationUser navigation property.

                //That is not a problem. We can join these tables using their matching key fields without changing the entity.
                
                    _context.Users.AsNoTracking(),

                    customer => customer.ApplicationUserId,

                    user => user.Id,

                    (customer, user) => new GetCustomerProfileResponse
                    {
                        CustomerId = customer.CustomerId,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = user.PhoneNumber,
                        IsEmailVerified = user.EmailConfirmed,
                        CreatedAtUtc = customer.CreatedAtUtc,
                        UpdatedAtUtc = customer.UpdatedAtUtc
                    })
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
