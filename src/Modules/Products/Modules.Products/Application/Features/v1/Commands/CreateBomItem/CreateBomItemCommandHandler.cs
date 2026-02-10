using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateBomItem;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.CreateBomItem;

public sealed class CreateBomItemCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateBomItemCommand, int>
{
    public async ValueTask<int> Handle(CreateBomItemCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to create BOM item: tenant context is required but not available.");

        // Verify parent product exists and belongs to current tenant
        var productExists = await dbContext.Products
            .AnyAsync(p => p.Id == command.ProductId && p.TenantId == tenantId, cancellationToken);

        if (!productExists)
        {
            throw new NotFoundException($"Product with ID {command.ProductId} not found or does not belong to the current tenant.");
        }

        // Verify child product exists and belongs to current tenant
        var childProductExists = await dbContext.Products
            .AnyAsync(p => p.Id == command.ChildProductId && p.TenantId == tenantId, cancellationToken);

        if (!childProductExists)
        {
            throw new NotFoundException($"Child product with ID {command.ChildProductId} not found or does not belong to the current tenant.");
        }

        var bomItem = new BomItem
        {
            ProductId = command.ProductId,
            ChildProductId = command.ChildProductId,
            Quantity = command.Quantity,
            Unit = command.Unit,
            IsManual = command.IsManual,
            Notes = command.Notes,
            TenantId = tenantId
        };

        await dbContext.BomItems.AddAsync(bomItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return bomItem.Id;
    }
}
