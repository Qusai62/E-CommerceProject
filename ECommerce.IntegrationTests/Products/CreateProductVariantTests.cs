using ECommerce.Infrastructure.Persistence;
using ECommerce.IntegrationTests.Fixtures;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Application.Catalog.Product.CreateProduct;
using ECommerce.Application.Catalog.ProductVariant.CreateProductVariant;

namespace ECommerce.IntegrationTests.Products
{
    public class CreateProductVariantTests
    {
        [Fact]
        public async Task CreateVariant_WithValidData_CreatesVariantAttributesAndInventory()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var uniqueId = Guid.NewGuid().ToString("N");
            var productName = $"Test Product {uniqueId}";
            var sku = $"TEST-BLK-{uniqueId}".ToUpperInvariant();

            long? productId = null;

            try
            {
                // 1. Create the parent Product
                using var createProductResponse =
      await client.PostAsJsonAsync(
          "/api/products",
          new
          {
              Name = productName,
              Description = "Test description",
              BrandId = (long?)null,
              CategoryId = (long?)null
          });

                var responseBody = await createProductResponse.Content
     .ReadAsStringAsync();

                Assert.True(
                    createProductResponse.StatusCode == HttpStatusCode.Created,
                    $"Expected 201 Created, but received " +
                    $"{(int)createProductResponse.StatusCode}. " +
                    $"Response: {responseBody}");
                var createdProduct = await createProductResponse.Content
                    .ReadFromJsonAsync<CreateProductResponse>();

                Assert.NotNull(createdProduct);

                productId = createdProduct.ProductId;

                // 2. Prepare the variant request
                var variantRequest = new
                {
                    Sku = sku,
                    Price = 899.990m,
                    Attributes = new[]
                    {
                    new
                    {
                        Name = "Color",
                        Value = "Black",
                        SortOrder = 1
                    },
                    new
                    {
                        Name = "Storage",
                        Value = "256GB",
                        SortOrder = 2
                    }
                }
                };

                // Act
                using var response = await client.PostAsJsonAsync(
                    $"/api/products/{productId.Value}/variants",
                    variantRequest);

                // Assert: HTTP response
                Assert.Equal(
                    HttpStatusCode.Created,
                    response.StatusCode);

                var result = await response.Content
                    .ReadFromJsonAsync<CreateProductVariantResponse>();

                Assert.NotNull(result);

                Assert.True(result.ProductVariantId > 0);
                Assert.Equal(productId.Value, result.ProductId);
                Assert.Equal(sku, result.Sku);
                Assert.Equal(899.990m, result.Price);
                Assert.False(result.IsActive);
                Assert.Equal(0, result.OnHandQuantity);

                // Assert: actual database state
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var savedVariant = await db.ProductVariants
                    .AsNoTracking()
                    .SingleAsync(v =>
                        v.ProductVariantId == result.ProductVariantId);

                Assert.Equal(productId.Value, savedVariant.ProductId);
                Assert.Equal(sku, savedVariant.Sku);
                Assert.False(savedVariant.IsActive);

                // Verify VariantAttribute records
                var savedAttributes = await db.VariantAttributes
                    .AsNoTracking()
                    .Where(a =>
                        a.ProductVariantId == result.ProductVariantId)
                    .OrderBy(a => a.SortOrder)
                    .ToListAsync();

                Assert.Equal(2, savedAttributes.Count);

                Assert.Equal("Color", savedAttributes[0].Name);
                Assert.Equal("Black", savedAttributes[0].Value);
                Assert.Equal(1, savedAttributes[0].SortOrder);

                Assert.Equal("Storage", savedAttributes[1].Name);
                Assert.Equal("256GB", savedAttributes[1].Value);
                Assert.Equal(2, savedAttributes[1].SortOrder);

                // Verify Inventory record
                var inventory = await db.Inventories
                    .AsNoTracking()
                    .SingleAsync(i =>
                        i.ProductVariantId == result.ProductVariantId);

                Assert.Equal(0, inventory.OnHandQuantity);
                Assert.True(inventory.InventoryId > 0);
            }
            finally
            {
                // Cleanup in FK dependency order
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var variantIds = await db.ProductVariants
                    .Where(v => v.Sku == sku)
                    .Select(v => v.ProductVariantId)
                    .ToListAsync();

                if (variantIds.Count > 0)
                {
                    await db.VariantAttributes
                        .Where(a => variantIds.Contains(a.ProductVariantId))
                        .ExecuteDeleteAsync();

                    await db.Inventories
                        .Where(i => variantIds.Contains(i.ProductVariantId))
                        .ExecuteDeleteAsync();

                    await db.ProductVariants
                        .Where(v => variantIds.Contains(v.ProductVariantId))
                        .ExecuteDeleteAsync();
                }

                if (productId.HasValue)
                {
                    await db.Products
                        .Where(p => p.ProductId == productId.Value)
                        .ExecuteDeleteAsync();
                }
                else
                {
                    await db.Products
                        .Where(p => p.Name == productName)
                        .ExecuteDeleteAsync();
                }
            }
        }

        [Fact]
        public async Task CreateVariant_WhenProductDoesNotExist_Returns404()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            const long productId = long.MaxValue;

            var request = new
            {
                Sku = $"TEST-{Guid.NewGuid():N}",
                Price = 100.000m,
                Attributes = Array.Empty<object>()
            };

            // Act
            using var response = await client.PostAsJsonAsync(
                $"/api/products/{productId}/variants",
                request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();

            Assert.Contains("Product.NotFound", body);
        }

        [Fact]
        public async Task CreateVariant_WhenSkuAlreadyExists_Returns409()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var productName = $"Duplicate SKU Test {Guid.NewGuid():N}";
            var sku = $"DUP-{Guid.NewGuid():N}".ToUpperInvariant();

            long? productId = null;

            try
            {
                productId = await CreateParentProductAsync(
                    client, productName);

                var request = new
                {
                    Sku = sku,
                    Price = 250.000m,
                    Attributes = Array.Empty<object>()
                };

                // Create first variant successfully
                using var firstResponse = await client.PostAsJsonAsync(
                    $"/api/products/{productId.Value}/variants",
                    request);

                Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

                // Act: submit the same SKU again
                using var secondResponse = await client.PostAsJsonAsync(
                    $"/api/products/{productId.Value}/variants",
                    request);

                // Assert
                Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

                var body = await secondResponse.Content.ReadAsStringAsync();

                Assert.Contains("Variant.DuplicateSku", body);

                // Verify only one variant exists with this SKU
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var count = await db.ProductVariants
                    .CountAsync(v => v.Sku == sku);

                Assert.Equal(1, count);
            }
            finally
            {
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                // Remove dependent Inventory before ProductVariant
                var variantIds = await db.ProductVariants
                    .Where(v => v.Sku == sku)
                    .Select(v => v.ProductVariantId)
                    .ToListAsync();

                await db.VariantAttributes
                    .Where(a => variantIds.Contains(a.ProductVariantId))
                    .ExecuteDeleteAsync();

                await db.Inventories
                    .Where(i => variantIds.Contains(i.ProductVariantId))
                    .ExecuteDeleteAsync();

                await db.ProductVariants
                    .Where(v => variantIds.Contains(v.ProductVariantId))
                    .ExecuteDeleteAsync();

                if (productId.HasValue)
                {
                    await db.Products
                        .Where(p => p.ProductId == productId.Value)
                        .ExecuteDeleteAsync();
                }
                else
                {
                    await db.Products
                        .Where(p => p.Name == productName)
                        .ExecuteDeleteAsync();
                }
            }
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-50")]
        [InlineData("10.1234")]
        [InlineData("1000000000000000")]
        public async Task CreateVariant_WithInvalidPrice_Returns400(
            string priceText)
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var sku = $"PRICE-{Guid.NewGuid():N}".ToUpperInvariant();
            var price = decimal.Parse(
                priceText,
                System.Globalization.CultureInfo.InvariantCulture);

            var request = new
            {
                Sku = sku,
                Price = price,
                Attributes = Array.Empty<object>()
            };

            // Act
            using var response = await client.PostAsJsonAsync(
                "/api/products/999999/variants",
                request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();

            var expectedCode = price <= 0
                ? "Variant.InvalidPrice"
                : "Variant.InvalidPricePrecision";

            Assert.Contains(expectedCode, body);
        }
        [Fact]
        public async Task CreateVariant_WithDuplicateAttributeNames_Returns400()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var sku = $"ATTR-{Guid.NewGuid():N}".ToUpperInvariant();

            var request = new
            {
                Sku = sku,
                Price = 199.000m,
                Attributes = new[]
                {
            new
            {
                Name = "Color",
                Value = "Black",
                SortOrder = 1
            },
            new
            {
                Name = "color",
                Value = "Silver",
                SortOrder = 2
            }
        }
            };

            // Act
            using var response = await client.PostAsJsonAsync(
                "/api/products/999999/variants",
                request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();

            Assert.Contains("Variant.DuplicateAttributeName", body);
        }
        private static async Task<long> CreateParentProductAsync(
    HttpClient client,
    string productName)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/products",
                new
                {
                    Name = productName,
                    Description = "Integration test product",
                    BrandId = (long?)null,
                    CategoryId = (long?)null
                });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var product = await response.Content
                .ReadFromJsonAsync<CreateProductResponse>();

            Assert.NotNull(product);

            return product.ProductId;
        }
    }
}
