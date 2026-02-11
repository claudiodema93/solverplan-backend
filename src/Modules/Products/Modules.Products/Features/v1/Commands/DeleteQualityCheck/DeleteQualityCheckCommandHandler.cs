using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Features.v1.Commands.DeleteQualityCheck;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Commands.DeleteQualityCheck;

/// <summary>
/// Handler for deleting a quality check.
/// </summary>
public sealed class DeleteQualityCheckCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteQualityCheckCommand>
{
    public async ValueTask<Unit> Handle(DeleteQualityCheckCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to delete quality check: tenant context is required but not available.");

        // Find existing quality check by Id and tenant
        var qualityCheck = await dbContext.QualityChecks
            .Where(qc => qc.Id == command.Id && qc.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Quality check with ID {command.Id} not found.");

        // Remove quality check from DbSet
        dbContext.QualityChecks.Remove(qualityCheck);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
