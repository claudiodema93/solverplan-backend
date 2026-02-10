using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteBomItem;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.DeleteBomItem;

public sealed class DeleteBomItemCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteBomItemCommand>
{
    public async ValueTask<Unit> Handle(DeleteBomItemCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to delete BOM item: tenant context is required but not available.");

        var bomItem = await dbContext.BomItems
            .FirstOrDefaultAsync(b => b.Id == command.Id && b.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException($"BOM item with ID {command.Id} not found or does not belong to the current tenant.");

        dbContext.BomItems.Remove(bomItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
