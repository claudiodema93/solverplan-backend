using FSH.Framework.Core.Domain;

namespace FSH.Modules.Products.Domain.Entities;

/// <summary>
/// Represents an quality check required for a product. Quality checks are used to ensure that products meet certain standards before they can be released or sold.
/// </summary>
public class QualityCheck : BaseEntity<int>, IHasTenant, IAuditableEntity
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
    /// The title or summary of the quality check.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Detailed description of the quality check, including steps to reproduce or impact.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Indicates whether this quality check is mandatory.
    /// Required checks must be completed before the product can proceed.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// The display order for presenting quality checks in a consistent sequence.
    /// Lower values appear first. Defaults to 0.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

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
