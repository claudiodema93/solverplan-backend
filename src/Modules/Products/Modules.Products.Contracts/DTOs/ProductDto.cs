using FSH.Modules.Products.Contracts.Domain.Enums;

namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for Product entity.
/// </summary>
public sealed class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public int Revision { get; set; }
    public ProductState Status { get; set; }
    public string? Description { get; set; }
    public bool Private { get; set; }
    public string? CustomerId { get; set; }
    public string Variant { get; set; } = default!;
    public int Version { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Keywords { get; set; }
    public string? Language { get; set; }
    public string? Subject { get; set; }
    public string? Notes { get; set; }
    public string? HashSha256 { get; set; }
    public ProductCharacteristicsDto? Characteristics { get; set; }
    public bool IsLatest { get; set; }
    public MirroringInfoDto? Mirroring { get; set; }
    public string TenantId { get; set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}

/// <summary>
/// DTO for product characteristics.
/// </summary>
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
public sealed record MirroringInfoDto(
    bool IsMirrored = false,
    string? SourceProductTitle = null,
    string? SourceProductRevision = null,
    bool CopyProduct = false);
