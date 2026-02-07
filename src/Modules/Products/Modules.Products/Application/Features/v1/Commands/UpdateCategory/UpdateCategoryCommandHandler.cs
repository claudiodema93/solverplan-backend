using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateCategory;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async ValueTask<Unit> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        // Find existing category by Id and tenant
        var category = await dbContext.Categories
            .Where(c => c.Id == command.Id && c.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Category with Id {command.Id} not found.");

        // Update category name
        category.Name = command.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
