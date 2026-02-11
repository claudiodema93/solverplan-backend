using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Queries.SearchProducts;

/// <summary>
/// Query to search and filter products with pagination and sorting.
/// </summary>
/// <example>
/// Example of searching for active products in a category:
/// <code>
/// var query = new SearchProductsQuery
/// {
///     Search = "widget",
///     CategoryId = 5,
///     Status = ProductState.Active,
///     IsLatest = true,
///     PageNumber = 1,
///     PageSize = 20,
///     Sort = "Title,-CreatedOnUtc"
/// };
/// </code>
/// </example>
public sealed class SearchProductsQuery : IPagedQuery, IQuery<PagedResponse<ProductDto>>
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
    /// Search term to filter products by title, description, keywords, or subject.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by category ID.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filter by product status (Draft, Active, Archived, etc.).
    /// </summary>
    public ProductState? Status { get; set; }

    /// <summary>
    /// Filter by whether the product is the latest version.
    /// </summary>
    public bool? IsLatest { get; set; }
}
