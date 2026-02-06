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

        builder.Property(p => p.Description)
            .HasMaxLength(4000);

        builder.Property(p => p.CustomerId)
            .HasMaxLength(128);

        builder.Property(p => p.Variant)
            .HasMaxLength(128);

        builder.Property(p => p.Keywords)
            .HasMaxLength(512);

        builder.Property(p => p.Language)
            .HasMaxLength(16);

        builder.Property(p => p.Subject)
            .HasMaxLength(256);

        builder.Property(p => p.Notes)
            .HasMaxLength(4000);

        builder.Property(p => p.HashSha256)
            .HasMaxLength(64);

        builder.Property(p => p.Status)
            .HasConversion<int>();

        // Audit
        builder.Property(p => p.CreatedOnUtc).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(256);
        builder.Property(p => p.LastModifiedBy).HasMaxLength(256);

        // Value Objects
        builder.OwnsOne(p => p.Characteristics, chars =>
        {
            chars.Property(c => c.IsAssembly).HasColumnName("IsAssembly");
            chars.Property(c => c.IsJobWork).HasColumnName("IsJobWork");
            chars.Property(c => c.IsManufacturable).HasColumnName("IsManufacturable");
            chars.Property(c => c.IsCommercial).HasColumnName("IsCommercial");
            chars.Property(c => c.IsSellable).HasColumnName("IsSellable");
            chars.Property(c => c.IsQualityCheckRequired).HasColumnName("IsQualityCheckRequired");
        });

        builder.OwnsOne(p => p.Mirroring, mirror =>
        {
            mirror.Property(m => m.IsMirrored).HasColumnName("IsMirrored");
            mirror.Property(m => m.SourceProductTitle).HasColumnName("MirrorSourceTitle").HasMaxLength(256);
            mirror.Property(m => m.SourceProductRevision).HasColumnName("MirrorSourceRevision").HasMaxLength(64);
            mirror.Property(m => m.CopyProduct).HasColumnName("MirrorCopyProduct");
        });

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
