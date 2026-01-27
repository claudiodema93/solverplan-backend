using FSH.Framework.Core.Context;
using FSH.Modules.Products.Contracts.Application.Features.Commands.CreateProduct;
using FSH.Modules.Products.Contracts.v1.CreateProduct;
using FSH.Modules.Products.Domain;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;

namespace FSH.Modules.Products.Application.Features.Commands.CreateProduct;

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

        //create product entity

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id);
    }
}
