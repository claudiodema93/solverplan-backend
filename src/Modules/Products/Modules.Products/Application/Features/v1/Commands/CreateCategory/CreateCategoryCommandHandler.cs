using FSH.Framework.Core.Context;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateCategory;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;

namespace FSH.Modules.Products.Application.Features.v1.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateCategoryCommand, int>
{
    public async ValueTask<int> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to create category: tenant context is required but not available.");

        // Create category entity
        var category = new Category
        {
            Name = command.Name,
            TenantId = tenantId
        };

        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
