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
                .Where(p => p.IsActive && (
        p.BrandId == null ||
        (p.Brand != null && p.Brand.IsActive)
    ));
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search;

                productsQuery = productsQuery.Where(
                    p => p.Name.Contains(search));
            }

            if (query.BrandId.HasValue)
            {
                productsQuery = productsQuery.Where(
                    p => p.BrandId == query.BrandId.Value);
            }

            if (query.CategoryId.HasValue)
            {
                productsQuery = productsQuery.Where(
                    p => p.CategoryId == query.CategoryId.Value);
            }


            if (query.MinPrice.HasValue || query.MaxPrice.HasValue)
            {
                var minPrice = query.MinPrice;
                var maxPrice = query.MaxPrice;

                productsQuery = productsQuery.Where(p =>
                    p.Variants.Any(v =>
                        v.IsActive &&                                       //true || (800 >= minPrice.Value)
                        (!minPrice.HasValue || v.Price >= minPrice.Value) &&//Because the left side is already true,
                                                                            //C# stops evaluating the OR expression. It does not execute the right side.This is called short-circuit evaluation.
                        (!maxPrice.HasValue || v.Price <= maxPrice.Value)
                    ));
            }


            //Count matching products
            var totalCount = await productsQuery
                .CountAsync(cancellationToken);


            var ascending = query.SortDirection == "asc";

            var orderedProducts = query.SortBy switch//Switch Expression:its main purpose is to evaluate an expression and produce a value.
            {
                "name" => ascending //condition ? valueIfTrue : valueIfFalse
                    ? productsQuery
                        .OrderBy(p => p.Name)
                        .ThenBy(p => p.ProductId)
                    : productsQuery
                        .OrderByDescending(p => p.Name)
                        .ThenBy(p => p.ProductId),

                "price" => ascending
                    ? productsQuery
                        .OrderBy(p =>
                            !p.Variants.Any(v => v.IsActive))
                        .ThenBy(p => p.Variants
                            .Where(v => v.IsActive)
                            .Min(v => (decimal?)v.Price))
                        .ThenBy(p => p.ProductId)
                    : productsQuery
                        .OrderBy(p =>
                            !p.Variants.Any(v => v.IsActive))
                        .ThenByDescending(p => p.Variants
                            .Where(v => v.IsActive)
                            .Min(v => (decimal?)v.Price))
                        .ThenBy(p => p.ProductId),

                _ => ascending //default: this is how to define the default in switch expression
                    ? productsQuery
                        .OrderBy(p => p.CreatedAtUtc)
                        .ThenBy(p => p.ProductId)
                    : productsQuery
                        .OrderByDescending(p => p.CreatedAtUtc)
                        .ThenByDescending(p => p.ProductId)
            };


            //(3 - 1) × 10 = 20 this is just an example
            //So SQL Server skips the first 20 active products, using the defined ordering, and retrieves the next 10.
            var items = await orderedProducts
                .Skip((query.PageNumber - 1) * query.PageSize)//why -1 : Because page numbers start from 1, but page 1 should skip zero records.
                .Take(query.PageSize)
                .Select(p => new ProductListItemResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Slug = p.Slug,
                    BrandId = p.BrandId,
                    CategoryId = p.CategoryId,
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
