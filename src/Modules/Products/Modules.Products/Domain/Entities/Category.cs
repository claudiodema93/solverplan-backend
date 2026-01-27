using FSH.Framework.Core.Domain;

namespace FSH.Modules.Products.Domain.Entities;

public class Category : BaseEntity<int>, IHasTenant, IAuditableEntity
{
    /// <summary>
    /// Category name (e.g., "Sheet", "Doors")
    /// </summary>
    public required string Name { get; set; }

/// <summary>
    /// Information about the tenant.
    /// </summary>
    public string TenantId { get; set; } = default!;

    /// <summary>
    /// The date and time when the product was created in UTC.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The identifier of the user who created the product.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The date and time when the product was last modified in UTC.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who last modified the product.
    ///  </summary>
    public string? LastModifiedBy { get; set; }
}