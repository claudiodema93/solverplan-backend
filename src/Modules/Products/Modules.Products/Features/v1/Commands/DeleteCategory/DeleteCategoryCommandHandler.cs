using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Features.v1.Commands.DeleteCategory;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FSH.Modules.Products.Features.v1.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async ValueTask<Unit> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to delete category: tenant context is required but not available.");

        // Find existing category by Id and tenant
        var category = await dbContext.Categories
            .Where(c => c.Id == command.Id && c.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Category with Id {command.Id} not found.");

        // Check if any products reference this category
        var productCount = await dbContext.Products
            .Where(p => p.CategoryId == command.Id && p.TenantId == tenantId)
            .CountAsync(cancellationToken);

        if (productCount > 0)
        {
            var productWord = productCount == 1 ? "product" : "products";
            throw new CustomException(
                $"Cannot delete category '{category.Name}' because it is currently assigned to {productCount} {productWord}. " +
                $"Please reassign or remove these products before deleting the category.",
                Array.Empty<string>(),
                HttpStatusCode.BadRequest);
        }

        // Remove category
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
