using FSH.Modules.Products.Domain;
using FSH.Modules.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Products.Infrastructure.Data.Configurations;

public class AccountingCodeConfiguration : IEntityTypeConfiguration<AccountingCode>
{
    public void Configure(EntityTypeBuilder<AccountingCode> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("AccountingCodes", ProductsModuleConstants.SchemaName);

        builder.HasKey(a => a.Id);

        // Performance indexes for frequently filtered columns
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.ProductId);
        builder.HasIndex(a => new { a.TenantId, a.ProductId });

        // Unique constraint: Code must be unique per product per tenant
        builder.HasIndex(a => new { a.TenantId, a.ProductId, a.Code }).IsUnique();

        builder.Property(a => a.TenantId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(a => a.Code)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(512);

        // Audit
        builder.Property(a => a.CreatedOnUtc).IsRequired();
        builder.Property(a => a.CreatedBy).HasMaxLength(256);
        builder.Property(a => a.LastModifiedBy).HasMaxLength(256);

        // Relationships
        builder.HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
