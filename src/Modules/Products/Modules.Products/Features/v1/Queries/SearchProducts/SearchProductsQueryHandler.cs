using FSH.Framework.Core.Context;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchProducts;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FSH.Modules.Products.Features.v1.Queries.SearchProducts;

/// <summary>
/// Handler for searching and filtering products with pagination and sorting.
/// </summary>
public sealed class SearchProductsQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<SearchProductsQuery, PagedResponse<ProductDto>>
{
    public async ValueTask<PagedResponse<ProductDto>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to search products: tenant context is required but not available.");

        IQueryable<Product> products = dbContext.Products
            .Include(p => p.Category)
            .Where(p => p.TenantId == tenantId)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = $"%{query.Search}%";
            products = products.Where(p =>
                (p.Title != null && EF.Functions.Like(p.Title, term)) ||
                (p.Description != null && EF.Functions.Like(p.Description, term)) ||
                (p.Keywords != null && EF.Functions.Like(p.Keywords, term)) ||
                (p.Subject != null && EF.Functions.Like(p.Subject, term)));
        }

        if (query.CategoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        if (query.Status.HasValue)
        {
            products = products.Where(p => p.Status == query.Status.Value);
        }

        if (query.IsLatest.HasValue)
        {
            products = products.Where(p => p.IsLatest == query.IsLatest.Value);
        }

        // Apply sorting
        products = ApplySorting(products, query.Sort);

        // Project to DTO
        var projected = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Title = p.Title,
            Revision = p.Revision,
            Status = p.Status,
            Description = p.Description,
            Private = p.Private,
            CustomerId = p.CustomerId,
            Variant = p.Variant,
            Version = p.Version,
            CategoryId = p.CategoryId,
            CategoryName = p.Category != null ? p.Category.Name : null,
            Keywords = p.Keywords,
            Language = p.Language,
            Subject = p.Subject,
            Notes = p.Notes,
            HashSha256 = p.HashSha256,
            Characteristics = new ProductCharacteristicsDto(
                p.Characteristics.IsAssembly,
                p.Characteristics.IsJobWork,
                p.Characteristics.IsManufacturable,
                p.Characteristics.IsCommercial,
                p.Characteristics.IsSellable,
                p.Characteristics.IsQualityCheckRequired),
            IsLatest = p.IsLatest,
            Mirroring = new MirroringInfoDto(
                p.Mirroring.IsMirrored,
                p.Mirroring.SourceProductTitle,
                p.Mirroring.SourceProductRevision,
                p.Mirroring.CopyProduct),
            TenantId = p.TenantId,
            CreatedOnUtc = p.CreatedOnUtc,
            CreatedBy = p.CreatedBy,
            LastModifiedOnUtc = p.LastModifiedOnUtc,
            LastModifiedBy = p.LastModifiedBy
        });

        var pagedResult = await projected.ToPagedResponseAsync(query, cancellationToken).ConfigureAwait(false);

        return pagedResult;
    }

    private static readonly Dictionary<string, Expression<Func<Product, object?>>> SortableFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["title"] = p => p.Title,
        ["status"] = p => p.Status,
        ["createdonutc"] = p => p.CreatedOnUtc,
        ["revision"] = p => p.Revision,
        ["version"] = p => p.Version,
        ["categoryid"] = p => p.CategoryId
    };

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderByDescending(p => p.CreatedOnUtc);
        }

        var sortParts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedQueryable<Product>? orderedQuery = null;

        foreach (var part in sortParts)
        {
            var (field, descending) = ParseSortField(part);

            if (!SortableFields.TryGetValue(field, out var selector))
            {
                selector = p => p.CreatedOnUtc; // Default fallback
            }

            orderedQuery = ApplySortExpression(query, orderedQuery, selector, descending);
        }

        return orderedQuery ?? query.OrderByDescending(p => p.CreatedOnUtc);
    }

    private static (string field, bool descending) ParseSortField(string part)
    {
        var descending = part.StartsWith('-');
        var field = descending ? part[1..] : part;
        return (field, descending);
    }

    private static IOrderedQueryable<Product> ApplySortExpression(
        IQueryable<Product> query,
        IOrderedQueryable<Product>? orderedQuery,
        Expression<Func<Product, object?>> selector,
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
