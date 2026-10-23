using ECommerce.Infrastructure.Persistence;
using ECommerce.IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Catalog.Product.UpdateProduct;
using ECommerce.Application.Catalog.Product.CreateProduct;
namespace ECommerce.IntegrationTests.Products
{
    public class UpdateProductTests
    {
        [Fact]
        public async Task UpdateProduct_WithValidData_Returns200AndUpdatesDatabase()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var originalName = $"Original Product {Guid.NewGuid():N}";
            var updatedName = $"Updated Product {Guid.NewGuid():N}";

            long? productId = null;

            try
            {
                // Create a real product using the API
                using var createResponse = await client.PostAsJsonAsync(
                    "/api/products",
                    new
                    {
                        Name = originalName,
                        Description = "Original description",
                        BrandId = (long?)null,
                        CategoryId = (long?)null
                    });

                Assert.Equal(
                    HttpStatusCode.Created,
                    createResponse.StatusCode);

                var createdProduct = await createResponse.Content
                    .ReadFromJsonAsync<CreateProductResponse>();

                Assert.NotNull(createdProduct);

                productId = createdProduct.ProductId;

                // Save original database values
                DateTime originalCreatedAt;

                using (var scope = factory.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    var product = await dbContext.Products
                        .AsNoTracking()
                        .SingleAsync(p => p.ProductId == productId.Value);

                    originalCreatedAt = product.CreatedAtUtc;
                }

                // Act: update the existing product
                using var updateResponse = await client.PutAsJsonAsync(
                    $"/api/products/{productId.Value}",
                    new
                    {
                        Name = updatedName,
                        Description = "Updated description",
                        BrandId = (long?)null,
                        CategoryId = (long?)null
                    });

                // Assert HTTP response
                Assert.Equal(
                    HttpStatusCode.OK,
                    updateResponse.StatusCode);

                var updatedProduct = await updateResponse.Content
                    .ReadFromJsonAsync<UpdateProductResponse>();

                Assert.NotNull(updatedProduct);

                Assert.Equal(productId.Value, updatedProduct.ProductId);
                Assert.Equal(updatedName, updatedProduct.Name);

                // Assert database persistence
                using var verificationScope = factory.Services.CreateScope();

                var verificationDb = verificationScope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var savedProduct = await verificationDb.Products
                    .AsNoTracking()
                    .SingleAsync(p => p.ProductId == productId.Value);

                Assert.Equal(updatedName, savedProduct.Name);
                Assert.Equal("Updated description", savedProduct.Description);
                Assert.Equal(updatedProduct.Slug, savedProduct.Slug);
                Assert.Equal(originalCreatedAt, savedProduct.CreatedAtUtc);
                Assert.False(savedProduct.IsActive);
                Assert.True(
     Math.Abs(
         (updatedProduct.UpdatedAtUtc - savedProduct.UpdatedAtUtc)
             .TotalMilliseconds) < 1,
     "UpdatedAtUtc differs by more than 1 millisecond.");
            }
            finally
            {
                // Cleanup even if an assertion fails
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                if (productId.HasValue)
                {
                    await dbContext.Products
                        .Where(p => p.ProductId == productId.Value)
                        .ExecuteDeleteAsync();
                }
                else
                {
                    await dbContext.Products
                        .Where(p => p.Name == originalName ||
                                    p.Name == updatedName)
                        .ExecuteDeleteAsync();
                }
            }
        }

        [Fact]
        public async Task UpdateProduct_WhenProductDoesNotExist_Returns404()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            const long productId = long.MaxValue;

            // Verify the chosen ID really does not exist
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                Assert.False(await db.Products
                    .AnyAsync(p => p.ProductId == productId));
            }

            var request = new
            {
                Name = "Updated Product",
                Description = "Testing nonexistent product",
                BrandId = (long?)null,
                CategoryId = (long?)null
            };

            // Act
            using var response = await client.PutAsJsonAsync(
                $"/api/products/{productId}",
                request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Contains("Product.NotFound", responseBody);
        }


        [Theory]
        [InlineData("", null, null, "Product.NameRequired")]
        [InlineData("   ", null, null, "Product.NameRequired")]
        [InlineData("Valid Product", -1L, null, "Brand.InvalidId")]
        [InlineData("Valid Product", null, -5L, "Category.InvalidId")]
        [InlineData("Valid Product", 0L, 0L, "Brand.InvalidId")]
        public async Task UpdateProduct_WithInvalidInput_Returns400(
            string name,
            long? brandId,
            long? categoryId,
            string expectedError)
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var request = new
            {
                Name = name,
                Description = "Testing validation",
                BrandId = brandId,
                CategoryId = categoryId
            };

            // Act
            using var response = await client.PutAsJsonAsync(
                "/api/products/999999",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();

            Assert.Contains(expectedError, body);
        }

        [Fact]
        public async Task UpdateProduct_WhenReferencesDoNotExist_Returns400()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var name = $"Reference Test {Guid.NewGuid():N}";
            var productId = 0L;

            try
            {
                var product = await CreateTestProductAsync(client, name);
                productId = product.ProductId;

                const long missingId = long.MaxValue;

                using (var scope = factory.Services.CreateScope())
                {
                    var db = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    Assert.False(await db.Brands
                        .AnyAsync(b => b.BrandId == missingId));

                    Assert.False(await db.Categories
                        .AnyAsync(c => c.CategoryId == missingId));
                }

                var request = new
                {
                    Name = "Updated Reference Test",
                    Description = "Updated description",
                    BrandId = (long?)missingId,
                    CategoryId = (long?)missingId
                };

                // Act
                using var response = await client.PutAsJsonAsync(
                    $"/api/products/{productId}",
                    request);

                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var body = await response.Content.ReadAsStringAsync();

                Assert.Contains("Brand.NotFound", body);
                Assert.Contains("Category.NotFound", body);

                // Verify original product remains unchanged
                using var scope2 = factory.Services.CreateScope();

                var verifyDb = scope2.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var saved = await verifyDb.Products
                    .AsNoTracking()
                    .SingleAsync(p => p.ProductId == productId);

                Assert.Equal(name, saved.Name);
                Assert.Null(saved.BrandId);
                Assert.Null(saved.CategoryId);
            }
            finally
            {
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                if (productId > 0)
                {
                    await db.Products
                        .Where(p => p.ProductId == productId)
                        .ExecuteDeleteAsync();
                }
                else
                {
                    await db.Products
                        .Where(p => p.Name == name)
                        .ExecuteDeleteAsync();
                }
            }
        }

        [Fact]
        public async Task UpdateProduct_WhenNameUnchanged_PreservesSlug()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var name = $"Slug Test {Guid.NewGuid():N}";
            var productId = 0L;

            try
            {
                var created = await CreateTestProductAsync(client, name);
                productId = created.ProductId;

                var originalSlug = created.Slug;

                // Act: same name, different description
                using var response = await client.PutAsJsonAsync(
                    $"/api/products/{productId}",
                    new
                    {
                        Name = name,
                        Description = "New description only",
                        BrandId = (long?)null,
                        CategoryId = (long?)null
                    });

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var updated = await response.Content
                    .ReadFromJsonAsync<UpdateProductResponse>();

                Assert.NotNull(updated);
                Assert.Equal(originalSlug, updated.Slug);

                // Verify SQL Server
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var saved = await db.Products
                    .AsNoTracking()
                    .SingleAsync(p => p.ProductId == productId);

                Assert.Equal(originalSlug, saved.Slug);
                Assert.Equal("New description only", saved.Description);
            }
            finally
            {
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                if (productId > 0)
                {
                    await db.Products
                        .Where(p => p.ProductId == productId)
                        .ExecuteDeleteAsync();
                }
                else
                {
                    await db.Products
                        .Where(p => p.Name == name)
                        .ExecuteDeleteAsync();
                }
            }
        }


        [Fact]
        public async Task UpdateProduct_WhenSlugExists_GeneratesUniqueSlug()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var suffix = Guid.NewGuid().ToString("N");
            var firstName = $"Existing Product {suffix}";
            var secondName = $"Other Product {suffix}";

            var firstId = 0L;
            var secondId = 0L;

            try
            {
                var first = await CreateTestProductAsync(
                    client, firstName);

                firstId = first.ProductId;

                var second = await CreateTestProductAsync(
                    client, secondName);

                secondId = second.ProductId;

                // Act: rename second product to first product's name
                using var response = await client.PutAsJsonAsync(
                    $"/api/products/{secondId}",
                    new
                    {
                        Name = firstName,
                        Description = "Renamed product",
                        BrandId = (long?)null,
                        CategoryId = (long?)null
                    });

                // Assert HTTP response
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var updated = await response.Content
                    .ReadFromJsonAsync<UpdateProductResponse>();

                Assert.NotNull(updated);

                Assert.Equal(first.Slug + "-2", updated.Slug);
                Assert.Equal(secondId, updated.ProductId);

                // Verify both products remain in SQL Server
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var products = await db.Products
                    .AsNoTracking()
                    .Where(p => p.ProductId == firstId ||
                                p.ProductId == secondId)
                    .ToListAsync();

                Assert.Equal(2, products.Count);
                Assert.Equal(2, products.Select(p => p.Slug).Distinct().Count());
            }
            finally
            {
                using var scope = factory.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                await db.Products
                    .Where(p =>
                        (firstId > 0 && p.ProductId == firstId) ||
                        (secondId > 0 && p.ProductId == secondId) ||
                        p.Name == firstName ||
                        p.Name == secondName)
                    .ExecuteDeleteAsync();
            }
        }

        private static async Task<CreateProductResponse> CreateTestProductAsync(
    HttpClient client,
    string name)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/products",
                new
                {
                    Name = name,
                    Description = "Original description",
                    BrandId = (long?)null,
                    CategoryId = (long?)null
                });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var product = await response.Content
                .ReadFromJsonAsync<CreateProductResponse>();

            Assert.NotNull(product);

            return product;
        }
    }
}
