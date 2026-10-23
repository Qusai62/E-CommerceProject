using ECommerce.Domain.Catalog;
using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace ECommerce.IntegrationTests.TestData
{
    // Imagine you're testing a product endpoint, but your test database contains no products.
    //How can you verify that an existing product returns 200 OK?
    //You must first insert a product.
    //That's the job of our seeder.
    public class ProductTestDataSeeder
    {
        public static async Task<long> SeedProductAsync(
      ApplicationDbContext context,
      bool isActive = true,
      CancellationToken cancellationToken = default)
        {
            var nowUtc = DateTime.UtcNow;

            var product = new Product
            {
                Name = "Integration Test Product",
                Slug = $"integration-test-product-{Guid.NewGuid():N}",
                Description = "Product created for integration testing",
                IsActive = isActive,
                BrandId = null,
                CategoryId = null,
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc
            };

            context.Products.Add(product);

            await context.SaveChangesAsync(cancellationToken);

            return product.ProductId;
        }


        public static async Task SeedProductImagesAsync(
    ApplicationDbContext context,
    long productId,
    CancellationToken cancellationToken = default)
        {
            var nowUtc = DateTime.UtcNow;

            var images = new List<ProductImage>
    {
        new ProductImage
        {
            ProductId = productId,
            StorageKey = "back.jpg",
            AltText = "Back view",
            SortOrder = 3,
            IsPrimary = false,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        },
        new ProductImage
        {
            ProductId = productId,
            StorageKey = "front.jpg",
            AltText = "Front view",
            SortOrder = 1,
            IsPrimary = true,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        },
        new ProductImage
        {
            ProductId = productId,
            StorageKey = "side.jpg",
            AltText = "Side view",
            SortOrder = 2,
            IsPrimary = false,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        }
    };

            context.Set<ProductImage>().AddRange(images);//gives us access to the ProductImage entity set through EF Core.

            await context.SaveChangesAsync(cancellationToken);
        }


        public static async Task SeedProductVariantsAsync(
    ApplicationDbContext context,
    long productId,
    CancellationToken cancellationToken = default)
        {
            var nowUtc = DateTime.UtcNow;

            var uniqueSuffix = Guid.NewGuid().ToString("N");

            var variants = new List<ProductVariant>
    {
        new ProductVariant
        {
            ProductId = productId,
            Sku = $"TEST-BLK-128-{uniqueSuffix}",
            Price = 650.000m,
            IsActive = true,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc,

            Attributes = new List<VariantAttribute>
            {
                new VariantAttribute
                {
                    Name = "Storage",
                    Value = "128GB",
                    SortOrder = 2,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc
                },
                new VariantAttribute
                {
                    Name = "Color",
                    Value = "Black",
                    SortOrder = 1,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc
                }
            }
        },

        new ProductVariant
        {
            ProductId = productId,
            Sku = $"TEST-BLU-256-{uniqueSuffix}",
            Price = 750.000m,
            IsActive = true,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc,

            Attributes = new List<VariantAttribute>
            {
                new VariantAttribute
                {
                    Name = "Color",
                    Value = "Blue",
                    SortOrder = 1,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc
                },
                new VariantAttribute
                {
                    Name = "Storage",
                    Value = "256GB",
                    SortOrder = 2,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc
                }
            }
        },

        new ProductVariant
        {
            ProductId = productId,
            Sku = $"TEST-RED-512-{uniqueSuffix}",
            Price = 850.000m,
            IsActive = false,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc,

            Attributes = new List<VariantAttribute>
            {
                new VariantAttribute
                {
                    Name = "Color",
                    Value = "Red",
                    SortOrder = 1,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc
                },
                new VariantAttribute
                {
                    Name = "Storage",
                    Value = "512GB",
                    SortOrder = 2,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc
                }
            }
        }
    };

            context.Set<ProductVariant>().AddRange(variants);

            await context.SaveChangesAsync(cancellationToken);
        }
    }


}
