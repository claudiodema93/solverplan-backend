using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateQualityCheck;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Commands.CreateQualityCheck;

/// <summary>
/// Handler for creating a new quality check.
/// </summary>
public sealed class CreateQualityCheckCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateQualityCheckCommand, CreateQualityCheckResponse>
{
    public async ValueTask<CreateQualityCheckResponse> Handle(
        CreateQualityCheckCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to create quality check: tenant context is required but not available.");

        // Validate that the product exists and belongs to the current tenant
        var productExists = await dbContext.Products
            .AnyAsync(p => p.Id == command.ProductId && p.TenantId == tenantId, cancellationToken);

        if (!productExists)
        {
            throw new NotFoundException($"Product with ID {command.ProductId} not found.");
        }

        var qualityCheck = new QualityCheck
        {
            ProductId = command.ProductId,
            Title = command.Title,
            Description = command.Description,
            Department = command.Department,
            IsRequired = command.IsRequired,
            DisplayOrder = command.DisplayOrder,
            TenantId = tenantId
        };

        dbContext.QualityChecks.Add(qualityCheck);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateQualityCheckResponse(qualityCheck.Id);
    }
}
