using FSH.Framework.Core.Context;
using FSH.Modules.Products.Contracts.v1.CreateProduct;
using FSH.Modules.Products.Data;
using FSH.Modules.Products.Domain;
using Mediator;

namespace FSH.Modules.Products.Features.v1.CreateProduct;

public sealed class CreateProductCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async ValueTask<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        var product = Product.Create(
            tenantId,
            command.Title,
            command.Revision,
            currentUser.Name);

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id);
    }
}
