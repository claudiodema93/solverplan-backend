using FSH.Framework.Core.Domain;

namespace FSH.Modules.Products.Domain;

public class Product : BaseEntity<Guid>, IHasTenant, IAuditableEntity
{
    public string TenantId { get; private set; } = default!;

    public string Title { get; private set; } = default!;

    public string Revision { get; private set; } = default!;

    // IAuditableEntity
    public DateTimeOffset CreatedOnUtc { get; private set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedOnUtc { get; private set; }
    public string? LastModifiedBy { get; private set; }

    private Product() { } // EF Core

    public static Product Create(string tenantId, string title, string revision, string? createdBy = null)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = title,
            Revision = revision,
            CreatedBy = createdBy,
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
    }

    public void Update(string title, string revision, string? modifiedBy)
    {
        Title = title;
        Revision = revision;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
