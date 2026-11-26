using ECommerce.Domain.CustomerAccount;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customer");

            // Primary Key
            builder.HasKey(x => x.CustomerId)
                .HasName("PK_Customer");

            builder.Property(x => x.CustomerId)
                .ValueGeneratedOnAdd();

            // Application User
            builder.Property(x => x.ApplicationUserId)
                .IsRequired();

            builder.HasIndex(x => x.ApplicationUserId)
                .IsUnique()
                .HasDatabaseName("UQ_Customer_ApplicationUserId");

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Customer>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_Customer_ApplicationUser_ApplicationUserId");

            // Customer Information
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

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
