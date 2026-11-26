using ECommerce.Application.Brands.Abstractions;
using ECommerce.Application.Brands.GetBrands;
using Microsoft.EntityFrameworkCore;
using System;
using BrandEntity = ECommerce.Domain.Catalog.Brand;

namespace ECommerce.Infrastructure.Persistence.Repositories.Brand
{
    public class BrandReadRepository: IBrandReadRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandReadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(
            IReadOnlyList<BrandListItemResponse> Items,
            int TotalCount)> GetPagedAsync(
            GetBrandsQuery request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Start with all brands
            IQueryable<BrandEntity> query = _context.Brands
                .AsNoTracking();

            // Step 2: Search by name
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                string search = request.Search.Trim();

                query = query.Where(brand =>
                    brand.Name.Contains(search));
            }

            // Step 3: Filter by activation status
            if (request.IsActive.HasValue)
            {
                bool isActive = request.IsActive.Value;

                query = query.Where(brand =>
                    brand.IsActive == isActive);
            }

            // Step 4: Count all matching brands
            int totalCount = await query.CountAsync(
                cancellationToken);

            // Step 5: Calculate pagination offset
            int skip = (request.PageNumber - 1)
                * request.PageSize;

            // Step 6: Order, paginate, and project
            List<BrandListItemResponse> items = await query
                .OrderBy(brand => brand.Name)
                .ThenBy(brand => brand.BrandId)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(brand => new BrandListItemResponse(
                    brand.BrandId,
                    brand.Name,
                    brand.Slug,
                    brand.Description,
                    brand.LogoStorageKey,
                    brand.IsActive,
                    brand.CreatedAtUtc
                ))
                .ToListAsync(cancellationToken);

            // Step 7: Return both results
            return (items, totalCount);
        }
    }
}
