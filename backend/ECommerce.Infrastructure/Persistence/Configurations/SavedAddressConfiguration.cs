using ECommerce.Domain.CustomerAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class SavedAddressConfiguration : IEntityTypeConfiguration<SavedAddress>
    {
        public void Configure(EntityTypeBuilder<SavedAddress> builder)
        {
            builder.ToTable("SavedAddress");

            // Primary Key
            builder.HasKey(x => x.SavedAddressId)
                .HasName("PK_SavedAddress");

            builder.Property(x => x.SavedAddressId)
                .ValueGeneratedOnAdd();

            // Customer Relationship
            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.SavedAddresses)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SavedAddress_Customer_CustomerId");

            // Address Information
            builder.Property(x => x.RecipientName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ContactNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Neighborhood)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.BuildingInfo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.AdditionalAddressInfo)
                .HasMaxLength(500);

            // Default Address
            builder.Property(x => x.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            // Normal index for retrieving a customer's addresses
            builder.HasIndex(x => x.CustomerId)
                .HasDatabaseName("IX_SavedAddress_CustomerId");

            // Unique filtered index for the default address
            builder.HasIndex(
                    x => x.CustomerId,
                    "UQ_SavedAddress_CustomerId_Default")
                .IsUnique()
                .HasFilter("[IsDefault] = 1");

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
