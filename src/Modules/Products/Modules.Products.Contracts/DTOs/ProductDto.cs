using FSH.Modules.Products.Contracts.Domain.Enums;

namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for Product entity.
/// </summary>
public sealed class ProductDto
{
    /// <summary>
    /// The unique identifier of the product.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The product title.
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// The product revision number.
    /// </summary>
    public int Revision { get; set; }

    /// <summary>
    /// The product status (Draft, Active, Archived, etc.).
    /// </summary>
    public ProductState Status { get; set; }

    /// <summary>
    /// Optional product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the product is private.
    /// </summary>
    public bool Private { get; set; }

    /// <summary>
    /// Optional customer identifier.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Product variant identifier.
    /// </summary>
    public string Variant { get; set; } = default!;

    /// <summary>
    /// Product version number.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Optional category identifier.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// The name of the associated category.
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Optional keywords for search and categorization.
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// Optional language code.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Optional subject or topic.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Optional internal notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Optional SHA-256 hash for verification.
    /// </summary>
    public string? HashSha256 { get; set; }

    /// <summary>
    /// Optional product characteristics.
    /// </summary>
    public ProductCharacteristicsDto? Characteristics { get; set; }

    /// <summary>
    /// Indicates whether this is the latest version.
    /// </summary>
    public bool IsLatest { get; set; }

    /// <summary>
    /// Optional mirroring information.
    /// </summary>
    public MirroringInfoDto? Mirroring { get; set; }

    /// <summary>
    /// The identifier of the tenant that owns this product.
    /// </summary>
    public string TenantId { get; set; } = default!;

    /// <summary>
    /// The UTC date and time when the product was created.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who created the product.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The UTC date and time when the product was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who last modified the product.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}

/// <summary>
/// DTO for product characteristics.
/// </summary>
/// <param name="IsAssembly">Indicates whether the product is an assembly.</param>
/// <param name="IsJobWork">Indicates whether the product is for job work.</param>
/// <param name="IsManufacturable">Indicates whether the product is manufacturable.</param>
/// <param name="IsCommercial">Indicates whether the product is commercial.</param>
/// <param name="IsSellable">Indicates whether the product is sellable.</param>
/// <param name="IsQualityCheckRequired">Indicates whether quality check is required for the product.</param>
public sealed record ProductCharacteristicsDto(
    bool IsAssembly = false,
    bool IsJobWork = false,
    bool IsManufacturable = false,
    bool IsCommercial = false,
    bool IsSellable = false,
    bool IsQualityCheckRequired = false);

/// <summary>
/// DTO for mirroring information.
/// </summary>
/// <param name="IsMirrored">Indicates whether the product is mirrored from another source.</param>
/// <param name="SourceProductTitle">The title of the source product being mirrored.</param>
/// <param name="SourceProductRevision">The revision of the source product being mirrored.</param>
/// <param name="CopyProduct">Indicates whether to copy the product during mirroring.</param>
public sealed record MirroringInfoDto(
    bool IsMirrored = false,
    string? SourceProductTitle = null,
    string? SourceProductRevision = null,
    bool CopyProduct = false);
