using System.Net;
using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.Commands.DeleteCategory;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async ValueTask<Unit> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        // Find existing category by Id and tenant
        var category = await dbContext.Categories
            .Where(c => c.Id == command.Id && c.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Category with Id {command.Id} not found.");

        // Check if any products reference this category
        var hasProducts = await dbContext.Products
            .Where(p => p.CategoryId == command.Id && p.TenantId == tenantId)
            .AnyAsync(cancellationToken);

        if (hasProducts)
        {
            throw new CustomException(
                $"Cannot delete category '{category.Name}' because it is referenced by one or more products.",
                Array.Empty<string>(),
                HttpStatusCode.BadRequest);
        }

        // Remove category
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
