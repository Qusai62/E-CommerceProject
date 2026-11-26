using ECommerce.Application.Brands.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using BrandEntity = ECommerce.Domain.Catalog.Brand;

namespace ECommerce.Infrastructure.Persistence.Repositories.Brand
{
    public class BrandRepository: IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check whether a brand name already exists
        public async Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return await _context.Brands
                .AnyAsync(
                    brand => brand.Name == name,
                    cancellationToken);
        }

        // Check whether a slug already exists
        public async Task<bool> ExistsBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Brands
                .AnyAsync(
                    brand => brand.Slug == slug,
                    cancellationToken);
        }

        // Add the brand to EF Core's change tracker
        public async Task AddAsync(
            BrandEntity brand,
            CancellationToken cancellationToken = default)
        {
            await _context.Brands.AddAsync(
                brand,
                cancellationToken);
        }

        // Persist pending changes to SQL Server
        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<BrandEntity?> GetByIdAsync(
    long brandId,
    CancellationToken cancellationToken = default)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(
                    brand => brand.BrandId == brandId,
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            long excludeBrandId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Brands
                .AnyAsync(
                    brand =>
                        brand.Name == name &&
                        brand.BrandId != excludeBrandId,
                    cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(
            string slug,
            long excludeBrandId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Brands
                .AnyAsync(
                    brand =>
                        brand.Slug == slug &&
                        brand.BrandId != excludeBrandId,
                    cancellationToken);
        }
        public async Task<bool> TryDeactivateAsync(
            long brandId,
            DateTime updatedAtUtc,
            CancellationToken cancellationToken = default)
        {
            int affectedRows = await _context.Brands
                .Where(brand =>
                    brand.BrandId == brandId &&
                    brand.IsActive)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            brand => brand.IsActive,
                            false)
                        .SetProperty(
                            brand => brand.UpdatedAtUtc,
                            updatedAtUtc),
                    cancellationToken);

            return affectedRows == 1;
        }

    }
}
