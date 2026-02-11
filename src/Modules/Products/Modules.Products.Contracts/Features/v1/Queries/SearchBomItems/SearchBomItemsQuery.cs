using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Queries.SearchBomItems;

/// <summary>
/// Query to search and filter BOM items with pagination and sorting.
/// </summary>
/// <example>
/// Example of searching for BOM items by product:
/// <code>
/// var query = new SearchBomItemsQuery
/// {
///     ProductId = 123,
///     PageNumber = 1,
///     PageSize = 10,
///     Sort = "-CreatedOnUtc"
/// };
/// </code>
/// </example>
public sealed class SearchBomItemsQuery : IPagedQuery, IQuery<PagedResponse<BomItemDto>>
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
    /// Multi-column sort expression, for example: "Quantity,-CreatedOnUtc".
    /// "-" prefix indicates descending order.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Search term to filter BOM items by notes.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter BOM items by parent product ID.
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// Filter BOM items by child product ID.
    /// </summary>
    public int? ChildProductId { get; set; }

    /// <summary>
    /// Filter BOM items by unit of measurement.
    /// </summary>
    public UnitOfMeasurement? Unit { get; set; }

    /// <summary>
    /// Filter BOM items by manual entry flag.
    /// </summary>
    public bool? IsManual { get; set; }
}
