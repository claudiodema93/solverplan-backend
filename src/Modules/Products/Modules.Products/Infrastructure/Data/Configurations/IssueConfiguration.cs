using FSH.Modules.Products.Domain;
using FSH.Modules.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Products.Infrastructure.Data.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Issues", ProductsModuleConstants.SchemaName);

        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.TenantId);

        builder.Property(i => i.TenantId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(i => i.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(i => i.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(i => i.ResolutionNotes)
            .HasMaxLength(4000);

        builder.Property(i => i.Severity)
            .HasConversion<int>();

        builder.Property(i => i.Status)
            .HasConversion<int>();

        // Audit
        builder.Property(i => i.CreatedOnUtc).IsRequired();
        builder.Property(i => i.CreatedBy).HasMaxLength(256);
        builder.Property(i => i.LastModifiedBy).HasMaxLength(256);

        // Relationships
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
