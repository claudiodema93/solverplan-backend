using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Queries.SearchAccountingCodes;

/// <summary>
/// Query to search and filter accounting codes with pagination and sorting.
/// </summary>
/// <example>
/// Example of searching for accounting codes by product:
/// <code>
/// var query = new SearchAccountingCodesQuery
/// {
///     ProductId = 123,
///     PageNumber = 1,
///     PageSize = 10,
///     Sort = "-CreatedOnUtc"
/// };
/// </code>
/// </example>
public sealed class SearchAccountingCodesQuery : IPagedQuery, IQuery<PagedResponse<AccountingCodeDto>>
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
    /// Multi-column sort expression, for example: "Code,-CreatedOnUtc".
    /// "-" prefix indicates descending order.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Search term to filter accounting codes by Code or Description.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter accounting codes by parent product ID.
    /// </summary>
    public int? ProductId { get; set; }
}
