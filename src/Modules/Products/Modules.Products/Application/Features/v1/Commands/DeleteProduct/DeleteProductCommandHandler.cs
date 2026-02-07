using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteProduct;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteProductCommand>
{
    public async ValueTask<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        // Find existing product by Id and tenant
        var product = await dbContext.Products
            .Where(p => p.Id == command.Id && p.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Product with Id {command.Id} not found.");

        // Remove product from DbSet
        dbContext.Products.Remove(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
