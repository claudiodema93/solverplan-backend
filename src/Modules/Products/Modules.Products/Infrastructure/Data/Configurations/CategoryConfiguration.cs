using FSH.Modules.Products.Domain;
using FSH.Modules.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Products.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Categories", ProductsModuleConstants.SchemaName);

        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.TenantId);

        builder.Property(c => c.TenantId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(128)
            .IsRequired();

        // Audit
        builder.Property(c => c.CreatedOnUtc).IsRequired();
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.LastModifiedBy).HasMaxLength(256);
    }
}
