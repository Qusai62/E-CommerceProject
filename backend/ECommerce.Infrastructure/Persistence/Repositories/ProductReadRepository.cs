using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Domain.InventoryManagement;
using Microsoft.EntityFrameworkCore;
using System;

using ECommerce.Domain.Catalog;
using ECommerce.Application.Catalog.Product.GetProductById;
namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductReadRepository : IProductReadRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductReadRepository(ApplicationDbContext context)
        {
            _context= context;
        }
        public Task<ProductResponse?> GetByIdAsync(long productId, CancellationToken cancellationToken = default)
        {
            return _context.Products.AsNoTracking().Where(p=>productId == p.ProductId && p.IsActive && (p.BrandId == null || (p.Brand != null && p.Brand.IsActive))).Select(p=> new ProductResponse{
                ProductId=p.ProductId,
                ProductName=p.Name,
                Description=p.Description,
                BrandName = p.Brand != null ? p.Brand.Name : "",

                  Images = p.Images
                .OrderBy(i => i.SortOrder)
                .ThenBy(i => i.ProductImageId)
                .Select(i => new ProductImageResponse
                {
                    ProductImageId = i.ProductImageId,
                    ImageUrl = i.StorageKey, // Temporary mapping
                    AltText = i.AltText,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary
                })
                .ToList(),

                Variants = p.Variants
                .Where(v => v.IsActive)
                .OrderBy(v => v.ProductVariantId)
                .Select(v => new ProductVariantResponse
                {
                    ProductVariantId = v.ProductVariantId,
                    Price = v.Price,
                    VariantAttributeResponses = v.Attributes
                        .OrderBy(a => a.SortOrder)
                        .ThenBy(a => a.VariantAttributeId)
                        .Select(a => new VariantAttributeResponse
                        {
                            Name = a.Name,
                            Value = a.Value,
                            SortOrder = a.SortOrder
                        })
                        .ToList()
                })
                .ToList(),
                 // NEW: Technical Specifications
            TechnicalSpecifications = p.TechnicalSpecifications
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.TechnicalSpecificationId)
                .Select(s => new TechnicalSpecificationResponse
                {
                    TechnicalSpecificationId =
                        s.TechnicalSpecificationId,
                    Name = s.Name,
                    Value = s.Value,
                    SortOrder = s.SortOrder
                })
                .ToList()

            }).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
