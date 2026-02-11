using FSH.Framework.Core.Context;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Domain.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchQualityChecks;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FSH.Modules.Products.Features.v1.Queries.SearchQualityChecks;

/// <summary>
/// Handler for searching and filtering quality checks with pagination and sorting.
/// </summary>
public sealed class SearchQualityChecksQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<SearchQualityChecksQuery, PagedResponse<QualityCheckDto>>
{
    public async ValueTask<PagedResponse<QualityCheckDto>> Handle(
        SearchQualityChecksQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to search quality checks: tenant context is required but not available.");

        IQueryable<QualityCheck> qualityChecks = dbContext.QualityChecks
            .Where(qc => qc.TenantId == tenantId)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = $"%{query.Search}%";
            qualityChecks = qualityChecks.Where(qc =>
                (qc.Title != null && EF.Functions.Like(qc.Title, term)) ||
                (qc.Description != null && EF.Functions.Like(qc.Description, term)) ||
                (qc.Department != null && EF.Functions.Like(qc.Department, term)));
        }

        if (query.ProductId.HasValue)
        {
            qualityChecks = qualityChecks.Where(qc => qc.ProductId == query.ProductId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Department))
        {
            qualityChecks = qualityChecks.Where(qc => qc.Department == query.Department);
        }

        if (query.IsRequired.HasValue)
        {
            qualityChecks = qualityChecks.Where(qc => qc.IsRequired == query.IsRequired.Value);
        }

        // Apply sorting
        qualityChecks = ApplySorting(qualityChecks, query.Sort);

        // Project to DTO
        var projected = qualityChecks.Select(qc => new QualityCheckDto
        {
            Id = qc.Id,
            ProductId = qc.ProductId,
            Title = qc.Title,
            Description = qc.Description,
            Department = qc.Department,
            IsRequired = qc.IsRequired,
            DisplayOrder = qc.DisplayOrder,
            TenantId = qc.TenantId,
            CreatedOnUtc = qc.CreatedOnUtc,
            CreatedBy = qc.CreatedBy,
            LastModifiedOnUtc = qc.LastModifiedOnUtc,
            LastModifiedBy = qc.LastModifiedBy
        });

        var pagedResult = await projected.ToPagedResponseAsync(query, cancellationToken).ConfigureAwait(false);

        return pagedResult;
    }

    private static readonly Dictionary<string, Expression<Func<QualityCheck, object?>>> SortableFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["title"] = qc => qc.Title,
        ["department"] = qc => qc.Department,
        ["displayorder"] = qc => qc.DisplayOrder,
        ["createdonutc"] = qc => qc.CreatedOnUtc,
        ["isrequired"] = qc => qc.IsRequired
    };

    private static IQueryable<QualityCheck> ApplySorting(IQueryable<QualityCheck> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(qc => qc.DisplayOrder).ThenByDescending(qc => qc.CreatedOnUtc);
        }

        var sortParts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedQueryable<QualityCheck>? orderedQuery = null;

        foreach (var part in sortParts)
        {
            var (field, descending) = ParseSortField(part);

            if (!SortableFields.TryGetValue(field, out var selector))
            {
                selector = qc => qc.DisplayOrder;
            }

            orderedQuery = ApplySortExpression(query, orderedQuery, selector, descending);
        }

        return orderedQuery ?? query.OrderBy(qc => qc.DisplayOrder).ThenByDescending(qc => qc.CreatedOnUtc);
    }

    private static (string field, bool descending) ParseSortField(string part)
    {
        var descending = part.StartsWith('-');
        var field = descending ? part[1..] : part;
        return (field, descending);
    }

    private static IOrderedQueryable<QualityCheck> ApplySortExpression(
        IQueryable<QualityCheck> query,
        IOrderedQueryable<QualityCheck>? orderedQuery,
        Expression<Func<QualityCheck, object?>> selector,
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
