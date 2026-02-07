using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateProduct;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateProductCommand>
{
    public async ValueTask<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to update product: tenant context is required but not available.");

        // Find existing product by Id and tenant
        var product = await dbContext.Products
            .Where(p => p.Id == command.Id && p.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Product with Id {command.Id} not found.");

        // Validate CategoryId exists if provided and different from current
        if (command.CategoryId.HasValue && command.CategoryId.Value != product.CategoryId)
        {
            var categoryExists = await dbContext.Categories
                .AnyAsync(c => c.Id == command.CategoryId.Value && c.TenantId == tenantId, cancellationToken);

            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {command.CategoryId.Value} not found. Please provide a valid category ID.");
            }
        }

        // Update product properties
        product.Title = command.Title;
        product.Revision = command.Revision;
        product.Description = command.Description;
        product.Status = command.Status;
        product.Private = command.Private;
        product.CustomerId = command.CustomerId;
        product.Variant = command.Variant;
        product.Version = command.Version;
        product.CategoryId = command.CategoryId;
        product.Keywords = command.Keywords;
        product.Language = command.Language;
        product.Subject = command.Subject;
        product.Notes = command.Notes;
        product.HashSha256 = command.HashSha256;
        product.IsLatest = command.IsLatest;

        // Update product characteristics
        if (command.Characteristics is not null)
        {
            product.Characteristics = Domain.ValueObjects.ProductCharacteristics.Create(
                isAssembly: command.Characteristics.IsAssembly,
                isJobWork: command.Characteristics.IsJobWork,
                isManufacturable: command.Characteristics.IsManufacturable,
                isCommercial: command.Characteristics.IsCommercial,
                isSellable: command.Characteristics.IsSellable,
                isQualityCheckRequired: command.Characteristics.IsQualityCheckRequired);
        }

        // Update mirroring info
        if (command.Mirroring is not null)
        {
            product.Mirroring = Domain.ValueObjects.MirroringInfo.Create(
                isMirrored: command.Mirroring.IsMirrored,
                sourceProductTitle: command.Mirroring.SourceProductTitle,
                sourceProductRevision: command.Mirroring.SourceProductRevision,
                copyProduct: command.Mirroring.CopyProduct);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
