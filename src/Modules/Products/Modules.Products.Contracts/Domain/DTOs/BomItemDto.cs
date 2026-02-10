using FSH.Modules.Products.Contracts.Domain.Enums;

namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for BomItem entity.
/// </summary>
public sealed class BomItemDto
{
    /// <summary>
    /// The unique identifier of the BOM item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The identifier of the parent product this BOM item belongs to.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The title of the parent product.
    /// </summary>
    public string ProductTitle { get; set; } = default!;

    /// <summary>
    /// The identifier of the child product (component).
    /// </summary>
    public int ChildProductId { get; set; }

    /// <summary>
    /// The title of the child product (component).
    /// </summary>
    public string ChildProductTitle { get; set; } = default!;

    /// <summary>
    /// The quantity of the child product required.
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// The unit of measurement for the quantity.
    /// </summary>
    public UnitOfMeasurement Unit { get; set; }

    /// <summary>
    /// Indicates whether this BOM item was entered manually by the user.
    /// </summary>
    public bool IsManual { get; set; }

    /// <summary>
    /// Optional notes about this BOM item.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// The identifier of the tenant that owns this BOM item.
    /// </summary>
    public string TenantId { get; set; } = default!;

    /// <summary>
    /// The UTC date and time when the BOM item was created.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who created the BOM item.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The UTC date and time when the BOM item was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who last modified the BOM item.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
