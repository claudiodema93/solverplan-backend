using FSH.Modules.Products.Contracts.Domain.Enums;

namespace FSH.Modules.Products.Contracts.DTOs;

/// <summary>
/// Data transfer object for Issue entity.
/// </summary>
public sealed class IssueDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public IssueSeverity Severity { get; set; }
    public IssueState Status { get; set; }
    public string? ResolutionNotes { get; set; }
    public string TenantId { get; set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
