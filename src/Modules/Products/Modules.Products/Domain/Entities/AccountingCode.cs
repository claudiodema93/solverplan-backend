using FSH.Framework.Core.Domain;

namespace FSH.Modules.Products.Domain.Entities;

public class AccountingCode : BaseEntity<int>, IHasTenant, IAuditableEntity
{
    /// <summary>
    /// Foreign key to the associated product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Navigation property to the associated product.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Accounting code (e.g., "MAT-100", "LABOR-001")
    /// Must be unique per product
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Optional description of the accounting code purpose
    /// </summary>
    public string? Description { get; set; }

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
