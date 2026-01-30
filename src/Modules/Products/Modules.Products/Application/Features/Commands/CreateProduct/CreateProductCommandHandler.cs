using FSH.Framework.Core.Context;
using FSH.Modules.Products.Contracts.Application.Features.Commands.CreateProduct;
using FSH.Modules.Products.Contracts.v1.CreateProduct;
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

        // Create product characteristics
        var characteristics = command.Characteristics is not null
            ? Domain.ValueObjects.ProductCharacteristics.Create(
                isAssembly: command.Characteristics.IsAssembly,
                isJobWork: command.Characteristics.IsJobWork,
                isManufacturable: command.Characteristics.IsManufacturable,
                isCommercial: command.Characteristics.IsCommercial,
                isSellable: command.Characteristics.IsSellable,
                isQualityCheckRequired: command.Characteristics.IsQualityCheckRequired)
            : Domain.ValueObjects.ProductCharacteristics.Default();

        // Create mirroring info
        var mirroring = command.Mirroring is not null
            ? Domain.ValueObjects.MirroringInfo.Create(
                isMirrored: command.Mirroring.IsMirrored,
                sourceProductTitle: command.Mirroring.SourceProductTitle,
                sourceProductRevision: command.Mirroring.SourceProductRevision,
                copyProduct: command.Mirroring.CopyProduct)
            : Domain.ValueObjects.MirroringInfo.Default();

        // Create product entity
        var product = new Product
        {
            Title = command.Title,
            Revision = command.Revision,
            Description = command.Description,
            Status = command.Status,
            Private = command.Private,
            CustomerId = command.CustomerId,
            Variant = command.Variant,
            Version = command.Version,
            CategoryId = command.CategoryId,
            Keywords = command.Keywords,
            Language = command.Language,
            Subject = command.Subject,
            Notes = command.Notes,
            HashSha256 = command.HashSha256,
            IsLatest = command.IsLatest,
            Characteristics = characteristics,
            Mirroring = mirroring,
            TenantId = tenantId
        };

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id);
    }
}
