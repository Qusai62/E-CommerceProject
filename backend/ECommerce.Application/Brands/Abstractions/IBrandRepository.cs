using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.Abstractions
{
    public interface IBrandRepository
    {
        Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Brand brand,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);

        Task<Brand?> GetByIdAsync(
    long brandId,
    CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            long excludeBrandId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsBySlugAsync(
            string slug,
            long excludeBrandId,
            CancellationToken cancellationToken = default);

        Task<bool> TryDeactivateAsync(
    long brandId,
    DateTime updatedAtUtc,
    CancellationToken cancellationToken = default);
    }
}
