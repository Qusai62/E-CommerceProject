using ECommerce.Application.Catalog.Product.GetProductById;
using ECommerce.Domain.Catalog;
using ECommerce.Infrastructure.Persistence;
using ECommerce.IntegrationTests.Fixtures;
using ECommerce.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
namespace ECommerce.IntegrationTests.Products
{
    public class GetProductByIdTests
    {
        [Fact]
        public async Task GetById_WhenProductExists_Returns200OK()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var scope = factory.Services.CreateScope();//

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var databaseName = dbContext.Database
                .GetDbConnection()
                .Database;

            Assert.Equal("ECommerceIntegrationTestsDb", databaseName);

            var productId = await ProductTestDataSeeder
                .SeedProductAsync(dbContext);

            try
            {
                ///This is where the actual API test happens.Sending the HTTP request
                using var client = factory.CreateClient();

                // Act
                var response = await client.GetAsync(
                    $"/api/products/{productId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var product = await response.Content
                    .ReadFromJsonAsync<ProductResponse>();

                Assert.NotNull(product);
                Assert.Equal(productId, product.ProductId);
                Assert.Equal("Integration Test Product", product.ProductName);
                Assert.Empty(product.Images);
                Assert.Empty(product.Variants);
            }
            finally
            {
                // Delete only the product created by this test.
                await dbContext.Products
                    .Where(p => p.ProductId == productId)
                    .ExecuteDeleteAsync();
            }
        }

        [Fact]
        public async Task GetById_WhenProductDoesNotExist_Returns404NotFound()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(
                "/api/products/999999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById_WhenProductIsInactive_Returns404NotFound()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var scope = factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            Assert.Equal(
                "ECommerceIntegrationTestsDb",
                dbContext.Database.GetDbConnection().Database);

            var productId = await ProductTestDataSeeder.SeedProductAsync(
                dbContext,
                isActive: false);

            try
            {
                using var client = factory.CreateClient();

                // Act
                var response = await client.GetAsync(
                    $"/api/products/{productId}");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
            finally
            {
                // Cleanup
                await dbContext.Products
                    .Where(p => p.ProductId == productId)
                    .ExecuteDeleteAsync();
            }
        }

        [Fact]
        public async Task GetById_WhenProductHasImages_ReturnsImagesInCorrectOrder()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var scope = factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            Assert.Equal(
                "ECommerceIntegrationTestsDb",
                dbContext.Database.GetDbConnection().Database);

            // Create the product
            var productId = await ProductTestDataSeeder
                .SeedProductAsync(dbContext);

            try
            {
                // Create the three images
                await ProductTestDataSeeder.SeedProductImagesAsync(
                    dbContext,
                    productId);

                using var client = factory.CreateClient();

                // Act
                var response = await client.GetAsync(
                    $"/api/products/{productId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var product = await response.Content
                    .ReadFromJsonAsync<ProductResponse>();

                Assert.NotNull(product);
                Assert.Equal(3, product.Images.Count);

                var images = product.Images.ToList();

                Assert.Equal("front.jpg", images[0].ImageUrl);
                Assert.Equal("side.jpg", images[1].ImageUrl);
                Assert.Equal("back.jpg", images[2].ImageUrl);

                Assert.True(images[0].IsPrimary);
                Assert.False(images[1].IsPrimary);
                Assert.False(images[2].IsPrimary);

                Assert.Equal("Front view", images[0].AltText);
            }
            finally
            {
                // Cleanup test product and its images
                await dbContext.Products
                    .Where(p => p.ProductId == productId)
                    .ExecuteDeleteAsync();
            }
        }

        [Fact]
        public async Task GetById_WhenProductHasVariants_ReturnsOnlyActiveVariantsWithSortedAttributes()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();

            using var scope = factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            // Safety: verify we are using the test database
            Assert.Equal(
                "ECommerceIntegrationTestsDb",
                dbContext.Database.GetDbConnection().Database);

            // Create the product
            var productId = await ProductTestDataSeeder
                .SeedProductAsync(dbContext);

            try
            {
                // Create 3 variants with their attributes
                await ProductTestDataSeeder.SeedProductVariantsAsync(
                    dbContext,
                    productId);

                using var client = factory.CreateClient();

                // Act
                var response = await client.GetAsync(
                    $"/api/products/{productId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var product = await response.Content
                    .ReadFromJsonAsync<ProductResponse>();

                Assert.NotNull(product);

                // Only 2 active variants should be returned
                Assert.Equal(2, product.Variants.Count);

                var variants = product.Variants.ToList();

                // Verify active variant prices
                Assert.Equal(650.000m, variants[0].Price);
                Assert.Equal(750.000m, variants[1].Price);

                // Verify the inactive variant is excluded
                Assert.DoesNotContain(
                    variants,
                    v => v.Price == 850.000m);

                // Verify the first variant has 2 attributes
                var attributes = variants[0]
                    .VariantAttributeResponses
                    .ToList();

                Assert.Equal(2, attributes.Count);

                // Verify attributes are correctly sorted
                Assert.Equal("Color", attributes[0].Name);
                Assert.Equal("Black", attributes[0].Value);
                Assert.Equal(1, attributes[0].SortOrder);

                Assert.Equal("Storage", attributes[1].Name);
                Assert.Equal("128GB", attributes[1].Value);
                Assert.Equal(2, attributes[1].SortOrder);

                // Verify the second variant's attributes
                var secondAttributes = variants[1]
                    .VariantAttributeResponses
                    .ToList();

                Assert.Equal(2, secondAttributes.Count);

                Assert.Equal("Color", secondAttributes[0].Name);
                Assert.Equal("Blue", secondAttributes[0].Value);

                Assert.Equal("Storage", secondAttributes[1].Name);
                Assert.Equal("256GB", secondAttributes[1].Value);
            }
            finally
            {
                // Delete attributes first (via cascade from variants)
                await dbContext.Set<ProductVariant>()
                    .Where(v => v.ProductId == productId)
                    .ExecuteDeleteAsync();

                // Delete the product
                await dbContext.Products
                    .Where(p => p.ProductId == productId)
                    .ExecuteDeleteAsync();
            }
        }
    }
}
