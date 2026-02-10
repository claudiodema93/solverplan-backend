using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateBomItem;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateBomItem;

public sealed class UpdateBomItemCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateBomItemCommand>
{
    public async ValueTask<Unit> Handle(UpdateBomItemCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to update BOM item: tenant context is required but not available.");

        var bomItem = await dbContext.BomItems
            .FirstOrDefaultAsync(b => b.Id == command.Id && b.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException($"BOM item with ID {command.Id} not found or does not belong to the current tenant.");

        // Verify child product exists if changing it
        if (command.ChildProductId != bomItem.ChildProductId)
        {
            var childProductExists = await dbContext.Products
                .AnyAsync(p => p.Id == command.ChildProductId && p.TenantId == tenantId, cancellationToken);

            if (!childProductExists)
            {
                throw new NotFoundException($"Child product with ID {command.ChildProductId} not found or does not belong to the current tenant.");
            }
        }

        bomItem.ChildProductId = command.ChildProductId;
        bomItem.Quantity = command.Quantity;
        bomItem.Unit = command.Unit;
        bomItem.IsManual = command.IsManual;
        bomItem.Notes = command.Notes;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
