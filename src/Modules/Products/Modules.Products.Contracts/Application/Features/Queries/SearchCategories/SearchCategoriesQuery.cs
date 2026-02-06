using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Queries.SearchCategories;

/// <summary>
/// Query to search and filter categories with pagination and sorting.
/// </summary>
public sealed class SearchCategoriesQuery : IPagedQuery, IQuery<PagedResponse<CategoryDto>>
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
    /// Multi-column sort expression, for example: "Name,-CreatedOnUtc".
    /// "-" prefix indicates descending order.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Search term to filter categories by name.
    /// </summary>
    public string? Search { get; set; }
}
