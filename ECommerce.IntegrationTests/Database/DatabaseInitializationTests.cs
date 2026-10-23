using ECommerce.IntegrationTests.Fixtures;

namespace ECommerce.IntegrationTests.Database
{
    public class DatabaseInitializationTests
    {
        [Fact]
        public async Task InitializeDatabase_ShouldApplyMigrations()
        {
            //It checks that the application can initialize its test database without throwing an exception.
            using var factory = new CustomWebApplicationFactory();

            await factory.InitializeDatabaseAsync();
        }
    }
}
