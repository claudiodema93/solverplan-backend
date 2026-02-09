using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateQualityCheck;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateQualityCheck;

/// <summary>
/// Handler for updating an existing quality check.
/// </summary>
public sealed class UpdateQualityCheckCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateQualityCheckCommand>
{
    public async ValueTask<Unit> Handle(UpdateQualityCheckCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to update quality check: tenant context is required but not available.");

        // Find existing quality check by Id and tenant
        var qualityCheck = await dbContext.QualityChecks
            .Where(qc => qc.Id == command.Id && qc.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Quality check with ID {command.Id} not found.");

        // Validate that the product exists and belongs to the current tenant if changed
        if (command.ProductId != qualityCheck.ProductId)
        {
            var productExists = await dbContext.Products
                .AnyAsync(p => p.Id == command.ProductId && p.TenantId == tenantId, cancellationToken);

            if (!productExists)
            {
                throw new NotFoundException($"Product with ID {command.ProductId} not found.");
            }
        }

        // Update quality check properties
        qualityCheck.ProductId = command.ProductId;
        qualityCheck.Title = command.Title;
        qualityCheck.Description = command.Description;
        qualityCheck.Department = command.Department;
        qualityCheck.IsRequired = command.IsRequired;
        qualityCheck.DisplayOrder = command.DisplayOrder;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
