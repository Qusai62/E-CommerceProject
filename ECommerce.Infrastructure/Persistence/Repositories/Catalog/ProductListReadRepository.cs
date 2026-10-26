using ECommerce.Application.Catalog.GetProducts;
using ECommerce.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Catalog
{
    public class ProductListReadRepository:IProductListReadRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductListReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductListItemResponse>>
            GetProductsAsync(
                GetProductsQuery query,
                CancellationToken cancellationToken = default)
        {
            //We tell EF Core to select active products without change tracking.
            var productsQuery = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive);
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search;

                productsQuery = productsQuery.Where(
                    p => p.Name.Contains(search));
            }

            //Count matching products
            var totalCount = await productsQuery
                .CountAsync(cancellationToken);

            //(3 - 1) × 10 = 20 this is just an example
            //So SQL Server skips the first 20 active products, using the defined ordering, and retrieves the next 10.
            var items = await productsQuery
                .OrderBy(p => p.ProductId)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductListItemResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Slug = p.Slug,
                    PriceFrom = p.Variants
            .Where(v => v.IsActive)
            .Select(v => (decimal?)v.Price)
            .Min()
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductListItemResponse>(
                items,
                query.PageNumber,
                query.PageSize,
                totalCount);
        }
    }
}
