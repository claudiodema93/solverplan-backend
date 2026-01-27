using FSH.Framework.Core.Domain;
using FSH.Modules.Products.Contracts.Domain.Enums;
using System;

namespace FSH.Modules.Products.Domain.Entities;

/// <summary>
/// Represents an issue or problem associated with a product.
/// Issues are tracked with severity levels, status, and can have attachments.
/// </summary>
public class Issue : BaseEntity<int>, IHasTenant, IAuditableEntity
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
    /// The title or summary of the issue.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Detailed description of the issue, including steps to reproduce or impact.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// The severity level of the issue (e.g., Low, Medium, High, Critical).
    /// Defaults to Medium.
    /// </summary>
    public IssueSeverity Severity { get; set; } = IssueSeverity.Medium;

    /// <summary>
    /// The current status of the issue (e.g., Open, In Progress, Resolved, Closed).
    /// Defaults to Open.
    /// </summary>
    public IssueState Status { get; set; } = IssueState.Open;

    /// <summary>
    /// Notes or comments about the resolution of the issue.
    /// Populated when the issue is resolved or closed.
    /// </summary>
    public string? ResolutionNotes { get; set; }

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
