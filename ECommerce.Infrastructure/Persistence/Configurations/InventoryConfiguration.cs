using ECommerce.Domain.InventoryManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable(
                "Inventory",
                table =>

                {
                    table.HasCheckConstraint(
                        "CK_Inventory_OnHandQuantity_NonNegative",
                        "[OnHandQuantity] >= 0");
                });

            // Primary Key
            builder.HasKey(x => x.InventoryId)
                .HasName("PK_Inventory");

            builder.Property(x => x.InventoryId)
                .ValueGeneratedOnAdd();

            // ProductVariant Relationship
            builder.Property(x => x.ProductVariantId)
                .IsRequired();

            builder.HasOne(x => x.ProductVariant)
                .WithOne(x => x.Inventory)
                .HasForeignKey<Inventory>(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_Inventory_ProductVariant_ProductVariantId");

            // One Inventory record per ProductVariant
            builder.HasIndex(x => x.ProductVariantId)
                .IsUnique()
                .HasDatabaseName(
                    "UQ_Inventory_ProductVariantId");

            // Stock
            builder.Property(x => x.OnHandQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            // Optimistic Concurrency
            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsConcurrencyToken();

            // Audit Fields
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");
        }
    }
}
