using ECommerce.Domain.Shipping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class ShippingConfigurationConfiguration : IEntityTypeConfiguration<ShippingConfiguration>
    {
        public void Configure(EntityTypeBuilder<ShippingConfiguration> builder)
        {
            builder.ToTable(
                "ShippingConfiguration",
                table =>
                {
                    // V1: This table contains exactly one configuration row.
                    table.HasCheckConstraint(
                        "CK_ShippingConfiguration_Singleton",
                        "[ShippingConfigurationId] = 1");

                    table.HasCheckConstraint(
                        "CK_ShippingConfiguration_ShippingMode_Valid",
                        "[ShippingMode] IN (1, 2)");

                    table.HasCheckConstraint(
                        "CK_ShippingConfiguration_ShippingCharge_NonNegative",
                        "[ShippingCharge] >= 0");
                });

            // Primary Key
            builder.HasKey(x => x.ShippingConfigurationId)
                .HasName("PK_ShippingConfiguration");

            // We explicitly use ID = 1 for the singleton row.
            builder.Property(x => x.ShippingConfigurationId)
                .ValueGeneratedNever();

            // Shipping Mode
            builder.Property(x => x.ShippingMode)
                .IsRequired()
                .HasConversion<int>();

            // Shipping Charge
            builder.Property(x => x.ShippingCharge)
                .IsRequired()
                .HasPrecision(18, 3);

            // Audit timestamps
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // Optimistic concurrency
            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsConcurrencyToken();
        }
    }
}
