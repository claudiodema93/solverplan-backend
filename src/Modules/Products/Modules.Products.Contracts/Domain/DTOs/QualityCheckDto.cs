namespace FSH.Modules.Products.Contracts.Domain.DTOs;

/// <summary>
/// Data transfer object representing a quality check.
/// </summary>
public sealed record QualityCheckDto
{
    /// <summary>
    /// The unique identifier of the quality check.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Foreign key to the associated product.
    /// </summary>
    public int ProductId { get; init; }

    /// <summary>
    /// The title or summary of the quality check.
    /// </summary>
    public string Title { get; init; } = default!;

    /// <summary>
    /// Detailed description of the quality check.
    /// </summary>
    public string Description { get; init; } = default!;

    /// <summary>
    /// The department responsible for the quality check.
    /// </summary>
    public string Department { get; init; } = default!;

    /// <summary>
    /// Indicates whether this quality check is mandatory.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// The display order for presenting quality checks.
    /// </summary>
    public int DisplayOrder { get; init; }

    /// <summary>
    /// The tenant identifier.
    /// </summary>
    public string TenantId { get; init; } = default!;

    /// <summary>
    /// The date and time when the quality check was created in UTC.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; init; }

    /// <summary>
    /// The identifier of the user who created the quality check.
    /// </summary>
    public string? CreatedBy { get; init; }

    /// <summary>
    /// The date and time when the quality check was last modified in UTC.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; init; }

    /// <summary>
    /// The identifier of the user who last modified the quality check.
    /// </summary>
    public string? LastModifiedBy { get; init; }
}
