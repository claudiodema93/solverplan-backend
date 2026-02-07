using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetCategoryById;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Queries.GetCategoryById;

/// <summary>
/// Handler for retrieving a category by its unique identifier.
/// </summary>
public sealed class GetCategoryByIdQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<GetCategoryByIdQuery, CategoryDto?>
{
    public async ValueTask<CategoryDto?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to retrieve category: tenant context is required but not available.");

        var category = await dbContext.Categories
            .Where(c => c.Id == query.Id && c.TenantId == tenantId)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                TenantId = c.TenantId,
                CreatedOnUtc = c.CreatedOnUtc,
                CreatedBy = c.CreatedBy,
                LastModifiedOnUtc = c.LastModifiedOnUtc,
                LastModifiedBy = c.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category == null)
        {
            throw new NotFoundException($"Category with ID {query.Id} not found.");
        }

        return category;
    }
}
