using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetBomItemById;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Queries.GetBomItemById;

/// <summary>
/// Handler for retrieving a BOM item by its unique identifier.
/// </summary>
public sealed class GetBomItemByIdQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<GetBomItemByIdQuery, BomItemDto?>
{
    public async ValueTask<BomItemDto?> Handle(GetBomItemByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to retrieve BOM item: tenant context is required but not available.");

        var bomItem = await dbContext.BomItems
            .Include(b => b.Product)
            .Include(b => b.ChildProduct)
            .Where(b => b.Id == query.Id && b.TenantId == tenantId)
            .Select(b => new BomItemDto
            {
                Id = b.Id,
                ProductId = b.ProductId,
                ProductTitle = b.Product!.Title,
                ChildProductId = b.ChildProductId,
                ChildProductTitle = b.ChildProduct!.Title,
                Quantity = b.Quantity,
                Unit = b.Unit,
                IsManual = b.IsManual,
                Notes = b.Notes,
                TenantId = b.TenantId,
                CreatedOnUtc = b.CreatedOnUtc,
                CreatedBy = b.CreatedBy,
                LastModifiedOnUtc = b.LastModifiedOnUtc,
                LastModifiedBy = b.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (bomItem == null)
        {
            throw new NotFoundException($"BOM item with ID {query.Id} not found.");
        }

        return bomItem;
    }
}
