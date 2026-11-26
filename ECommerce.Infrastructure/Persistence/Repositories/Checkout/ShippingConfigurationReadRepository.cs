using ECommerce.Application.Checkout.DetermineShippingCharge;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Checkout
{
    public class ShippingConfigurationReadRepository:IShippingConfigurationReadRepository
    {
        private readonly ApplicationDbContext _context;

        public ShippingConfigurationReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShippingConfigurationSnapshot?> GetAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.ShippingConfigurations
                .AsNoTracking()
                .Where(configuration =>
                    configuration.ShippingConfigurationId == 1)
                .Select(configuration =>
                    new ShippingConfigurationSnapshot(
                        configuration.ShippingMode,
                        configuration.ShippingCharge
                    ))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
