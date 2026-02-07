using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Queries.SearchIssues;

/// <summary>
/// Query to search and filter issues with pagination and sorting.
/// </summary>
public sealed class SearchIssuesQuery : IPagedQuery, IQuery<PagedResponse<IssueDto>>
{
    /// <summary>
    /// 1-based page number. Values less than 1 are normalized to 1.
    /// </summary>
    public int? PageNumber { get; set; }

    /// <summary>
    /// Requested page size. Implementations may enforce caps.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Multi-column sort expression, for example: "Title,-CreatedOnUtc".
    /// "-" prefix indicates descending order.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Search term to filter issues by title or description.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter issues by specific product ID.
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// Filter issues by severity level.
    /// </summary>
    public IssueSeverity? Severity { get; set; }

    /// <summary>
    /// Filter issues by status.
    /// </summary>
    public IssueState? Status { get; set; }
}
