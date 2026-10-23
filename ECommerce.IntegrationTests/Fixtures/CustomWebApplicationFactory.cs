using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace ECommerce.IntegrationTests.Fixtures
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {

        //Why new DataBase bec if we have an delete API we want now to delete from real DataBase
        //Both databases are on your local SQL Server Express instance, but they're logically separate.

        //Why use SQL Server instead of EF Core InMemory?
        //Because our production application uses SQL Server.
        //Your database has foreign keys, unique constraints, check constraints, transactions, and RowVersion.
        //EF Core InMemory doesn't reproduce SQL Server's relational behavior.A query may work in memory but fail when EF Core translates it to SQL
        private const string TestConnectionString =
            @"Server=localhost\SQLEXPRESS;Database=ECommerceIntegrationTestsDb;Trusted_Connection=True;TrustServerCertificate=True;";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) => //Allows the factory to supply testing-specific configuration.
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] =
                            TestConnectionString
                    });
            });
        }

        public async Task InitializeDatabaseAsync()
        {
            // Start the application using the test configuration.
            //Starts the test host and creates an HTTP client
            _ = CreateClient();

            // Create a Dependency Injection scope.
            using var scope = Services.CreateScope();

            // Resolve the ApplicationDbContext.
            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            // Verify the database name before applying migrations.
            var databaseName =
                dbContext.Database.GetDbConnection().Database;

            if (!string.Equals(
                databaseName,
                "ECommerceIntegrationTestsDb",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Unsafe database target: {databaseName}");
            }

            // Apply pending EF Core migrations.
            await dbContext.Database.MigrateAsync();
        }
    }
}