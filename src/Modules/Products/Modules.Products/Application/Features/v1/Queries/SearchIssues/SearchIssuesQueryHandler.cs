using FSH.Framework.Core.Context;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchIssues;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FSH.Modules.Products.Application.Features.v1.Queries.SearchIssues;

/// <summary>
/// Handler for searching and filtering issues with pagination and sorting.
/// </summary>
public sealed class SearchIssuesQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<SearchIssuesQuery, PagedResponse<IssueDto>>
{
    public async ValueTask<PagedResponse<IssueDto>> Handle(SearchIssuesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        IQueryable<Issue> issues = dbContext.Issues
            .Include(i => i.Product)
            .Where(i => i.TenantId == tenantId)
            .AsNoTracking();

        // Apply search filter on Title and Description
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = $"%{query.Search}%";
            issues = issues.Where(i =>
                (i.Title != null && EF.Functions.Like(i.Title, term)) ||
                (i.Description != null && EF.Functions.Like(i.Description, term)));
        }

        // Filter by ProductId
        if (query.ProductId.HasValue)
        {
            issues = issues.Where(i => i.ProductId == query.ProductId.Value);
        }

        // Filter by Severity
        if (query.Severity.HasValue)
        {
            issues = issues.Where(i => i.Severity == query.Severity.Value);
        }

        // Filter by Status
        if (query.Status.HasValue)
        {
            issues = issues.Where(i => i.Status == query.Status.Value);
        }

        // Apply sorting
        issues = ApplySorting(issues, query.Sort);

        // Project to DTO
        var projected = issues.Select(i => new IssueDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductTitle = i.Product != null ? i.Product.Title : string.Empty,
            Title = i.Title,
            Description = i.Description,
            Severity = i.Severity,
            Status = i.Status,
            ResolutionNotes = i.ResolutionNotes,
            TenantId = i.TenantId,
            CreatedOnUtc = i.CreatedOnUtc,
            CreatedBy = i.CreatedBy,
            LastModifiedOnUtc = i.LastModifiedOnUtc,
            LastModifiedBy = i.LastModifiedBy
        });

        var pagedResult = await projected.ToPagedResponseAsync(query, cancellationToken).ConfigureAwait(false);

        return pagedResult;
    }

    private static readonly Dictionary<string, Expression<Func<Issue, object?>>> SortableFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["title"] = i => i.Title,
        ["severity"] = i => i.Severity,
        ["status"] = i => i.Status,
        ["createdonutc"] = i => i.CreatedOnUtc
    };

    private static IQueryable<Issue> ApplySorting(IQueryable<Issue> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderByDescending(i => i.CreatedOnUtc);
        }

        var sortParts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedQueryable<Issue>? orderedQuery = null;

        foreach (var part in sortParts)
        {
            var (field, descending) = ParseSortField(part);

            if (!SortableFields.TryGetValue(field, out var selector))
            {
                selector = i => i.CreatedOnUtc; // Default fallback
            }

            orderedQuery = ApplySortExpression(query, orderedQuery, selector, descending);
        }

        return orderedQuery ?? query.OrderByDescending(i => i.CreatedOnUtc);
    }

    private static (string field, bool descending) ParseSortField(string part)
    {
        var descending = part.StartsWith('-');
        var field = descending ? part[1..] : part;
        return (field, descending);
    }

    private static IOrderedQueryable<Issue> ApplySortExpression(
        IQueryable<Issue> query,
        IOrderedQueryable<Issue>? orderedQuery,
        Expression<Func<Issue, object?>> selector,
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
