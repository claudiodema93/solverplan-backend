using System.Linq.Expressions;
using FSH.Framework.Core.Context;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Application.Features.Queries.SearchCategories;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.Queries.SearchCategories;

/// <summary>
/// Handler for searching and filtering categories with pagination and sorting.
/// </summary>
public sealed class SearchCategoriesQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<SearchCategoriesQuery, PagedResponse<CategoryDto>>
{
    public async ValueTask<PagedResponse<CategoryDto>> Handle(SearchCategoriesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        IQueryable<Category> categories = dbContext.Categories
            .Where(c => c.TenantId == tenantId)
            .AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = $"%{query.Search}%";
            categories = categories.Where(c =>
                c.Name != null && EF.Functions.Like(c.Name, term));
        }

        // Apply sorting
        categories = ApplySorting(categories, query.Sort);

        // Project to DTO
        var projected = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            TenantId = c.TenantId,
            CreatedOnUtc = c.CreatedOnUtc,
            CreatedBy = c.CreatedBy,
            LastModifiedOnUtc = c.LastModifiedOnUtc,
            LastModifiedBy = c.LastModifiedBy
        });

        var pagedResult = await projected.ToPagedResponseAsync(query, cancellationToken).ConfigureAwait(false);

        return pagedResult;
    }

    private static readonly Dictionary<string, Expression<Func<Category, object?>>> SortableFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = c => c.Name,
        ["createdonutc"] = c => c.CreatedOnUtc
    };

    private static IQueryable<Category> ApplySorting(IQueryable<Category> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(c => c.Name);
        }

        var sortParts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedQueryable<Category>? orderedQuery = null;

        foreach (var part in sortParts)
        {
            var (field, descending) = ParseSortField(part);

            if (!SortableFields.TryGetValue(field, out var selector))
            {
                selector = c => c.Name; // Default fallback
            }

            orderedQuery = ApplySortExpression(query, orderedQuery, selector, descending);
        }

        return orderedQuery ?? query.OrderBy(c => c.Name);
    }

    private static (string field, bool descending) ParseSortField(string part)
    {
        var descending = part.StartsWith('-');
        var field = descending ? part[1..] : part;
        return (field, descending);
    }

    private static IOrderedQueryable<Category> ApplySortExpression(
        IQueryable<Category> query,
        IOrderedQueryable<Category>? orderedQuery,
        Expression<Func<Category, object?>> selector,
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
