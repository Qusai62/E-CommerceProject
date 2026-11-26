using ECommerce.Domain.Supplier_Invoice___Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public sealed class SupplierConfiguration
    : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.SupplierId);

            builder.Property(s => s.SupplierId)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(30);

            builder.Property(s => s.Email)
                .HasMaxLength(254);

            builder.Property(s => s.CreatedAtUtc)
                .IsRequired();

            builder.Property(s => s.UpdatedAtUtc)
                .IsRequired();

            builder.HasIndex(s => s.Name);
        }
    }
}
