using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Domain.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Queries.SearchQualityChecks;

/// <summary>
/// Query to search and filter quality checks with pagination and sorting.
/// </summary>
public sealed class SearchQualityChecksQuery : IPagedQuery, IQuery<PagedResponse<QualityCheckDto>>
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
    /// Search term to filter quality checks by title, description, or department.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by product ID.
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// Filter by department.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Filter by whether the quality check is required.
    /// </summary>
    public bool? IsRequired { get; set; }
}
