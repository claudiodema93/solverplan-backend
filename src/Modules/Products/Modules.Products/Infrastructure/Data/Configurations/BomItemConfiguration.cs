using FSH.Modules.Products.Domain;
using FSH.Modules.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Products.Infrastructure.Data.Configurations;

public class BomItemConfiguration : IEntityTypeConfiguration<BomItem>
{
    public void Configure(EntityTypeBuilder<BomItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("BomItems", ProductsModuleConstants.SchemaName);

        builder.HasKey(b => b.Id);

        // Performance indexes for frequently filtered columns
        builder.HasIndex(b => b.TenantId);
        builder.HasIndex(b => b.ProductId);
        builder.HasIndex(b => b.ChildProductId);
        builder.HasIndex(b => new { b.TenantId, b.ProductId });
        builder.HasIndex(b => new { b.TenantId, b.ChildProductId });

        builder.Property(b => b.TenantId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(b => b.Quantity)
            .IsRequired();

        builder.Property(b => b.Unit)
            .HasConversion<int>();

        builder.Property(b => b.Notes)
            .HasMaxLength(2000);

        // Audit
        builder.Property(b => b.CreatedOnUtc).IsRequired();
        builder.Property(b => b.CreatedBy).HasMaxLength(256);
        builder.Property(b => b.LastModifiedBy).HasMaxLength(256);

        // Relationships
        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.ChildProduct)
            .WithMany()
            .HasForeignKey(b => b.ChildProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
