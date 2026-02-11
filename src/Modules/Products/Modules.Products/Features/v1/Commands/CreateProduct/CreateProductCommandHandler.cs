using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateProduct;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async ValueTask<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to create product: tenant context is required but not available.");

        // Validate CategoryId exists if provided
        if (command.CategoryId.HasValue)
        {
            var categoryExists = await dbContext.Categories
                .AnyAsync(c => c.Id == command.CategoryId.Value && c.TenantId == tenantId, cancellationToken);

            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {command.CategoryId.Value} not found. Please create the category first or provide a valid category ID.");
            }
        }

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
