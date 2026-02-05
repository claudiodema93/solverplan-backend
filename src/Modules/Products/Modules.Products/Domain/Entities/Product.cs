using FSH.Framework.Core.Domain;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Domain.ValueObjects;

namespace FSH.Modules.Products.Domain.Entities;

/// <summary>
/// Represents a product in the system with versioning, categorization, and tracking capabilities.
/// Supports multiple product types including assemblies, job work, and commercial products.
/// </summary>
public class Product : BaseEntity<int>, IHasTenant, IAuditableEntity
{
    /// <summary>
    /// The title or name of the product.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The revision number of the product. Increments with each revision.
    /// </summary>
    public int Revision { get; set; }

    /// <summary>
    /// The current state of the product (e.g., Draft, Active, Archived).
    /// </summary>
    public ProductState Status { get; set; } = ProductState.Draft;

    /// <summary>
    /// Detailed description of the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the product is private or publicly accessible.
    /// </summary>
    public bool Private { get; set; }

    /// <summary>
    /// Foreign key to the associated customer. Nullable if product is not customer-specific.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// The variant identifier for the product (e.g., different configurations or models).
    /// </summary>
    public string Variant { get; set; } = string.Empty;

    /// <summary>
    /// The version number of the product. Used for tracking product evolution.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Foreign key to the category. Nullable if product is uncategorized.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Navigation property to the category.
    /// </summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Keywords for search and categorization purposes.
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// The language of the product documentation or description.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// The subject or main topic of the product.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Additional notes or comments about the product.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// SHA-256 hash for integrity verification and change detection.
    /// </summary>
    public string? HashSha256 { get; set; }

    /// <summary>
    /// Product characteristics including assembly, job work, and quality flags.
    /// </summary>
    public ProductCharacteristics Characteristics { get; set; } = ProductCharacteristics.Default();

    /// <summary>
    /// Indicates whether the product has new version and is no longer active.
    /// </summary>
    public bool IsLatest { get; set; }

    /// <summary>
    /// Information about product mirroring from another product.
    /// </summary>
    public MirroringInfo Mirroring { get; set; } = MirroringInfo.Default();

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