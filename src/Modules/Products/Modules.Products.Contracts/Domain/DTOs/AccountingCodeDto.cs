namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for AccountingCode entity.
/// </summary>
public sealed class AccountingCodeDto
{
    /// <summary>
    /// The unique identifier of the accounting code.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The identifier of the parent product this accounting code belongs to.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The title of the parent product.
    /// </summary>
    public string ProductTitle { get; set; } = default!;

    /// <summary>
    /// The accounting code string (e.g., "MAT-100", "LABOR-001").
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Optional description of the accounting code purpose.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The identifier of the tenant that owns this accounting code.
    /// </summary>
    public string TenantId { get; set; } = default!;

    /// <summary>
    /// The UTC date and time when the accounting code was created.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who created the accounting code.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The UTC date and time when the accounting code was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who last modified the accounting code.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
