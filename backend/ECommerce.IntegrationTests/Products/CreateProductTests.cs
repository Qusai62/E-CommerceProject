using ECommerce.Domain.Catalog;
using ECommerce.Infrastructure.Persistence;
using ECommerce.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Catalog.Product.CreateProduct;

namespace ECommerce.IntegrationTests.Products
{
    public class CreateProductTests
    {
        [Fact]
        public async Task CreateProduct_WithValidData_Returns201Created()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var productName = $"Integration Product {Guid.NewGuid():N}";
            long? createdProductId = null;

            try
            {
                var request = new
                {
                    Name = productName,
                    Description = "Integration test product",
                    BrandId = (long?)null,
                    CategoryId = (long?)null
                };

                using var response = await client.PostAsJsonAsync(
                    "/api/products",
                    request);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var createdProduct = await response.Content
                    .ReadFromJsonAsync<CreateProductResponse>();

                Assert.NotNull(createdProduct);

                createdProductId = createdProduct.ProductId;

                Assert.True(createdProductId > 0);

                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var savedProduct = await dbContext.Products
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p =>
                        p.ProductId == createdProductId);

                Assert.NotNull(savedProduct);
                Assert.Equal(productName, savedProduct.Name);
                Assert.False(savedProduct.IsActive);
            }

            //The finally block executes when the try block completes, even if an assertion fails.
            //It also executes during ordinary exception propagation.
            finally
            {
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                if (createdProductId.HasValue)
                {
                    await dbContext.Products
                        .Where(p => p.ProductId == createdProductId.Value)
                        .ExecuteDeleteAsync();
                }
                else
                {
                    await dbContext.Products
                        .Where(p => p.Name == productName)
                        .ExecuteDeleteAsync();
                }
            }

        }

        [Fact]
        public async Task CreateProduct_WithInvalidIds_Returns400BadRequest()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var request = new
            {
                Name = "Invalid Product",
                Description = "Testing validation",
                BrandId = -1L,
                CategoryId = -5L
            };

            // Act
            using var response = await client.PostAsJsonAsync(
                "/api/products",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var responseBody = await response.Content
                .ReadAsStringAsync();

            Assert.Contains("Brand.InvalidId", responseBody);
            Assert.Contains("Category.InvalidId", responseBody);
        }

        [Fact]
        public async Task CreateProduct_WhenBrandAndCategoryDoNotExist_Returns400BadRequest()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var productName = $"Missing References {Guid.NewGuid():N}";

            var request = new
            {
                Name = productName,
                Description = "Testing nonexistent references",
                BrandId = 999999L,
                CategoryId = 999999L
            };

            // Act
            using var response = await client.PostAsJsonAsync(
                "/api/products",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var responseBody = await response.Content
                .ReadAsStringAsync();

            Assert.Contains("Brand.NotFound", responseBody);
            Assert.Contains("Category.NotFound", responseBody);

            // Verify the rejected product was not persisted.
            using var scope = factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var productExists = await dbContext.Products
                .AnyAsync(p => p.Name == productName);

            Assert.False(productExists);
        }

        [Fact]
        public async Task CreateProduct_WithDuplicateSlug_GeneratesUniqueSlug()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var uniqueName = $"Duplicate Test {Guid.NewGuid():N}";

            var request = new
            {
                Name = uniqueName,
                Description = "Testing duplicate slug generation",
                BrandId = (long?)null,
                CategoryId = (long?)null
            };

            var createdIds = new List<long>();

            try
            {
                // Act: create the first product
                using var firstResponse = await client.PostAsJsonAsync(
                    "/api/products",
                    request);

                Assert.Equal(
                    HttpStatusCode.Created,
                    firstResponse.StatusCode);

                var firstProduct = await firstResponse.Content
                    .ReadFromJsonAsync<CreateProductResponse>();

                Assert.NotNull(firstProduct);
                createdIds.Add(firstProduct.ProductId);

                // Act: create another product with the same name
                using var secondResponse = await client.PostAsJsonAsync(
                    "/api/products",
                    request);

                Assert.Equal(
                    HttpStatusCode.Created,
                    secondResponse.StatusCode);

                var secondProduct = await secondResponse.Content
                    .ReadFromJsonAsync<CreateProductResponse>();

                Assert.NotNull(secondProduct);
                createdIds.Add(secondProduct.ProductId);

                // Assert: both products have different IDs and slugs
                Assert.NotEqual(
                    firstProduct.ProductId,
                    secondProduct.ProductId);

                Assert.Equal(
                    firstProduct.Slug + "-2",
                    secondProduct.Slug);

                // Assert: both products exist in SQL Server
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var savedProducts = await dbContext.Products
                    .AsNoTracking()
                    .Where(p => createdIds.Contains(p.ProductId))
                    .ToListAsync();

                Assert.Equal(2, savedProducts.Count);
                Assert.Equal(
                    2,
                    savedProducts.Select(p => p.Slug).Distinct().Count());
            }
            finally
            {
                // Cleanup
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                await dbContext.Products
                    .Where(p => p.Name == uniqueName)
                    .ExecuteDeleteAsync();
            }
        }

        [Fact]
        public async Task CreateProduct_WhenNameExceeds200Characters_Returns400BadRequest()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var longName = new string('A', 201);

            var request = new
            {
                Name = longName,
                Description = "Testing product name length",
                BrandId = (long?)null,
                CategoryId = (long?)null
            };

            // Act
            using var response = await client.PostAsJsonAsync(
                "/api/products",
                request);

            // Assert HTTP response
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var responseBody = await response.Content
                .ReadAsStringAsync();

            Assert.Contains("Product.NameTooLong", responseBody);

            // Assert database state
            using var scope = factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var productExists = await dbContext.Products
                .AsNoTracking()
                .AnyAsync(p => p.Name == longName);

            Assert.False(productExists);
        }
        [Fact]
        public async Task CreateProduct_WhenDescriptionExceeds2000Characters_Returns400BadRequest()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var productName = $"Description Test {Guid.NewGuid():N}";
            var longDescription = new string('A', 2001);

            var request = new
            {
                Name = productName,
                Description = longDescription,
                BrandId = (long?)null,
                CategoryId = (long?)null
            };

            // Act
            using var response = await client.PostAsJsonAsync(
                "/api/products",
                request);

            // Assert: HTTP response
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var responseBody = await response.Content
                .ReadAsStringAsync();

            Assert.Contains(
                "Product.DescriptionTooLong",
                responseBody);

            // Assert: product was not saved
            using var scope = factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var productExists = await dbContext.Products
                .AsNoTracking()
                .AnyAsync(p => p.Name == productName);

            Assert.False(productExists);
        }

        [Fact]
        public async Task CreateProduct_WhenNameHasExactly200Characters_Returns201Created()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var productName = new string('A', 168)
                + Guid.NewGuid().ToString("N");

            Assert.Equal(200, productName.Length);

            var request = new
            {
                Name = productName,
                Description = "Testing maximum allowed name length",
                BrandId = (long?)null,
                CategoryId = (long?)null
            };

            long? productId = null;

            try
            {
                // Act
                using var response = await client.PostAsJsonAsync(
                    "/api/products",
                    request);

                // Assert HTTP response
                Assert.Equal(
                    HttpStatusCode.Created,
                    response.StatusCode);

                var createdProduct = await response.Content
                    .ReadFromJsonAsync<CreateProductResponse>();

                Assert.NotNull(createdProduct);

                productId = createdProduct.ProductId;

                Assert.Equal(productName, createdProduct.Name);
                Assert.True(productId > 0);

                // Assert database state
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var savedProduct = await dbContext.Products
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        p => p.ProductId == productId);

                Assert.NotNull(savedProduct);
                Assert.Equal(200, savedProduct.Name.Length);
                Assert.Equal(productName, savedProduct.Name);
                Assert.False(savedProduct.IsActive);
            }
            finally
            {
                // Cleanup
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                await dbContext.Products
                    .Where(p => p.Name == productName)
                    .ExecuteDeleteAsync();
            }
        }

        [Fact]
        public async Task CreateProduct_WhenRequestsAreConcurrent_DoesNotCreateDuplicateSlugs()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            var productName = $"Concurrent Test {Guid.NewGuid():N}";

            var request = new
            {
                Name = productName,
                Description = "Concurrent creation test",
                BrandId = (long?)null,
                CategoryId = (long?)null
            };

            try
            {
                // Act: start two requests concurrently
                var task1 = client.PostAsJsonAsync(
                    "/api/products",
                    request);

                var task2 = client.PostAsJsonAsync(
                    "/api/products",
                    request);

                using var response1 = await task1;
                using var response2 = await task2;

                // Assert: inspect both HTTP responses
                var statuses = new[]
                {
            response1.StatusCode,
            response2.StatusCode
        };

                // At least one request should succeed
                Assert.Contains(HttpStatusCode.Created, statuses);

                // Assert: check persisted product slugs
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var products = await dbContext.Products
                    .AsNoTracking()
                    .Where(p => p.Name == productName)
                    .ToListAsync();

                Assert.NotEmpty(products);

                var distinctSlugCount = products
                    .Select(p => p.Slug)
                    .Distinct()
                    .Count();

                Assert.Equal(products.Count, distinctSlugCount);
            }
            finally
            {
                // Cleanup
                using var scope = factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                await dbContext.Products
                    .Where(p => p.Name == productName)
                    .ExecuteDeleteAsync();
            }
        }
    }
}
