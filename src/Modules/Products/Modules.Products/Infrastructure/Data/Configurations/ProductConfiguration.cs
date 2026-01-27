using FSH.Modules.Products.Domain;
using FSH.Modules.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Products.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Products", ProductsModuleConstants.SchemaName);

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.TenantId);

        builder.Property(p => p.TenantId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(p => p.Revision)
            .HasMaxLength(64)
            .IsRequired();

        // Audit
        builder.Property(p => p.CreatedOnUtc).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(256);
        builder.Property(p => p.LastModifiedBy).HasMaxLength(256);
    }
}
