using FSH.Modules.Products.Contracts.Domain.Enums;

namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for Issue entity.
/// </summary>
public sealed class IssueDto
{
    /// <summary>
    /// The unique identifier of the issue.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The identifier of the product this issue belongs to.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The title of the associated product.
    /// </summary>
    public string ProductTitle { get; set; } = default!;

    /// <summary>
    /// The issue title.
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// Detailed description of the issue.
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// The severity level of the issue.
    /// </summary>
    public IssueSeverity Severity { get; set; }

    /// <summary>
    /// The current status of the issue.
    /// </summary>
    public IssueState Status { get; set; }

    /// <summary>
    /// Optional notes about the issue resolution.
    /// </summary>
    public string? ResolutionNotes { get; set; }

    /// <summary>
    /// The identifier of the tenant that owns this issue.
    /// </summary>
    public string TenantId { get; set; } = default!;

    /// <summary>
    /// The UTC date and time when the issue was created.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who created the issue.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The UTC date and time when the issue was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who last modified the issue.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
