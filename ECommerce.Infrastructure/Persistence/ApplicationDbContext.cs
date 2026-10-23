using ECommerce.Domain.Cart;
using ECommerce.Domain.Catalog;
using ECommerce.Domain.CustomerAccount;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using ECommerce.Domain.Shipping;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Infrastructure.Persistence
{
    public class ApplicationDbContext
     : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<SavedAddress> SavedAddresses { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<TechnicalSpecification> TechnicalSpecifications { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<VariantAttribute> VariantAttributes { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderFulfillmentStatusHistory> OrderFulfillmentStatusHistories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentAttempt> PaymentAttempts { get; set; }
        public DbSet<StockReservation> StockReservations { get; set; }
        public DbSet<CODStockAllocation> CODStockAllocations { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShippingConfiguration> ShippingConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}
