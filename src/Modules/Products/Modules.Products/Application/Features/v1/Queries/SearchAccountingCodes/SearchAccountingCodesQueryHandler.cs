using FSH.Framework.Core.Context;
using FSH.Framework.Persistence;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchAccountingCodes;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FSH.Modules.Products.Application.Features.v1.Queries.SearchAccountingCodes;

/// <summary>
/// Handler for searching and filtering accounting codes with pagination and sorting.
/// </summary>
public sealed class SearchAccountingCodesQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<SearchAccountingCodesQuery, PagedResponse<AccountingCodeDto>>
{
    public async ValueTask<PagedResponse<AccountingCodeDto>> Handle(SearchAccountingCodesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to search accounting codes: tenant context is required but not available.");

        IQueryable<AccountingCode> accountingCodes = dbContext.AccountingCodes
            .Include(a => a.Product)
            .Where(a => a.TenantId == tenantId)
            .AsNoTracking();

        // Apply search filter on Code or Description
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = $"%{query.Search}%";
            accountingCodes = accountingCodes.Where(a =>
                EF.Functions.Like(a.Code, term) ||
                (a.Description != null && EF.Functions.Like(a.Description, term)));
        }

        // Filter by parent ProductId
        if (query.ProductId.HasValue)
        {
            accountingCodes = accountingCodes.Where(a => a.ProductId == query.ProductId.Value);
        }

        // Apply sorting
        accountingCodes = ApplySorting(accountingCodes, query.Sort);

        // Project to DTO
        var projected = accountingCodes.Select(a => new AccountingCodeDto
        {
            Id = a.Id,
            ProductId = a.ProductId,
            ProductTitle = a.Product != null ? a.Product.Title : string.Empty,
            Code = a.Code,
            Description = a.Description,
            TenantId = a.TenantId,
            CreatedOnUtc = a.CreatedOnUtc,
            CreatedBy = a.CreatedBy,
            LastModifiedOnUtc = a.LastModifiedOnUtc,
            LastModifiedBy = a.LastModifiedBy
        });

        return await projected.ToPagedResponseAsync(query, cancellationToken).ConfigureAwait(false);
    }

    private static readonly Dictionary<string, Expression<Func<AccountingCode, object?>>> SortableFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["code"] = a => a.Code,
        ["description"] = a => a.Description,
        ["createdonutc"] = a => a.CreatedOnUtc
    };

    private static IQueryable<AccountingCode> ApplySorting(IQueryable<AccountingCode> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderByDescending(a => a.CreatedOnUtc);
        }

        var sortParts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedQueryable<AccountingCode>? orderedQuery = null;

        foreach (var part in sortParts)
        {
            var (field, descending) = ParseSortField(part);

            if (!SortableFields.TryGetValue(field, out var selector))
            {
                selector = a => a.CreatedOnUtc;
            }

            orderedQuery = ApplySortExpression(query, orderedQuery, selector, descending);
        }

        return orderedQuery ?? query.OrderByDescending(a => a.CreatedOnUtc);
    }

    private static (string field, bool descending) ParseSortField(string part)
    {
        var descending = part.StartsWith('-');
        var field = descending ? part[1..] : part;
        return (field, descending);
    }

    private static IOrderedQueryable<AccountingCode> ApplySortExpression(
        IQueryable<AccountingCode> query,
        IOrderedQueryable<AccountingCode>? orderedQuery,
        Expression<Func<AccountingCode, object?>> selector,
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
