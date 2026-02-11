using FSH.Framework.Core.Context;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchBomItems;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FSH.Modules.Products.Features.v1.Queries.SearchBomItems;

/// <summary>
/// Handler for searching and filtering BOM items with pagination and sorting.
/// </summary>
public sealed class SearchBomItemsQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<SearchBomItemsQuery, PagedResponse<BomItemDto>>
{
    public async ValueTask<PagedResponse<BomItemDto>> Handle(SearchBomItemsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to search BOM items: tenant context is required but not available.");

        IQueryable<BomItem> bomItems = dbContext.BomItems
            .Include(b => b.Product)
            .Include(b => b.ChildProduct)
            .Where(b => b.TenantId == tenantId)
            .AsNoTracking();

        // Apply search filter on Notes
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = $"%{query.Search}%";
            bomItems = bomItems.Where(b =>
                b.Notes != null && EF.Functions.Like(b.Notes, term));
        }

        // Filter by parent ProductId
        if (query.ProductId.HasValue)
        {
            bomItems = bomItems.Where(b => b.ProductId == query.ProductId.Value);
        }

        // Filter by ChildProductId
        if (query.ChildProductId.HasValue)
        {
            bomItems = bomItems.Where(b => b.ChildProductId == query.ChildProductId.Value);
        }

        // Filter by Unit
        if (query.Unit.HasValue)
        {
            bomItems = bomItems.Where(b => b.Unit == query.Unit.Value);
        }

        // Filter by IsManual
        if (query.IsManual.HasValue)
        {
            bomItems = bomItems.Where(b => b.IsManual == query.IsManual.Value);
        }

        // Apply sorting
        bomItems = ApplySorting(bomItems, query.Sort);

        // Project to DTO
        var projected = bomItems.Select(b => new BomItemDto
        {
            Id = b.Id,
            ProductId = b.ProductId,
            ProductTitle = b.Product != null ? b.Product.Title : string.Empty,
            ChildProductId = b.ChildProductId,
            ChildProductTitle = b.ChildProduct != null ? b.ChildProduct.Title : string.Empty,
            Quantity = b.Quantity,
            Unit = b.Unit,
            IsManual = b.IsManual,
            Notes = b.Notes,
            TenantId = b.TenantId,
            CreatedOnUtc = b.CreatedOnUtc,
            CreatedBy = b.CreatedBy,
            LastModifiedOnUtc = b.LastModifiedOnUtc,
            LastModifiedBy = b.LastModifiedBy
        });

        return await projected.ToPagedResponseAsync(query, cancellationToken).ConfigureAwait(false);
    }

    private static readonly Dictionary<string, Expression<Func<BomItem, object?>>> SortableFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["quantity"] = b => b.Quantity,
        ["unit"] = b => b.Unit,
        ["ismanual"] = b => b.IsManual,
        ["createdonutc"] = b => b.CreatedOnUtc
    };

    private static IQueryable<BomItem> ApplySorting(IQueryable<BomItem> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderByDescending(b => b.CreatedOnUtc);
        }

        var sortParts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedQueryable<BomItem>? orderedQuery = null;

        foreach (var part in sortParts)
        {
            var (field, descending) = ParseSortField(part);

            if (!SortableFields.TryGetValue(field, out var selector))
            {
                selector = b => b.CreatedOnUtc;
            }

            orderedQuery = ApplySortExpression(query, orderedQuery, selector, descending);
        }

        return orderedQuery ?? query.OrderByDescending(b => b.CreatedOnUtc);
    }

    private static (string field, bool descending) ParseSortField(string part)
    {
        var descending = part.StartsWith('-');
        var field = descending ? part[1..] : part;
        return (field, descending);
    }

    private static IOrderedQueryable<BomItem> ApplySortExpression(
        IQueryable<BomItem> query,
        IOrderedQueryable<BomItem>? orderedQuery,
        Expression<Func<BomItem, object?>> selector,
        bool descending)
    {
        if (orderedQuery is null)
        {
            return descending
                ? query.OrderByDescending(selector)
                : query.OrderBy(selector);
        }

        return descending
            ? orderedQuery.ThenByDescending(selector)
            : orderedQuery.ThenBy(selector);
    }
}
