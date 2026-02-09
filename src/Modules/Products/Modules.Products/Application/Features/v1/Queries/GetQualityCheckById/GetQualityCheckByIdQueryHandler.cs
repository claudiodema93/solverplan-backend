using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetQualityCheckById;
using FSH.Modules.Products.Contracts.Domain.DTOs;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Queries.GetQualityCheckById;

/// <summary>
/// Handler for retrieving a quality check by its unique identifier.
/// </summary>
public sealed class GetQualityCheckByIdQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<GetQualityCheckByIdQuery, QualityCheckDto?>
{
    public async ValueTask<QualityCheckDto?> Handle(
        GetQualityCheckByIdQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to retrieve quality check: tenant context is required but not available.");

        var qualityCheck = await dbContext.QualityChecks
            .Where(qc => qc.Id == query.Id && qc.TenantId == tenantId)
            .Select(qc => new QualityCheckDto
            {
                Id = qc.Id,
                ProductId = qc.ProductId,
                Title = qc.Title,
                Description = qc.Description,
                Department = qc.Department,
                IsRequired = qc.IsRequired,
                DisplayOrder = qc.DisplayOrder,
                TenantId = qc.TenantId,
                CreatedOnUtc = qc.CreatedOnUtc,
                CreatedBy = qc.CreatedBy,
                LastModifiedOnUtc = qc.LastModifiedOnUtc,
                LastModifiedBy = qc.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (qualityCheck == null)
        {
            throw new NotFoundException($"Quality check with ID {query.Id} not found.");
        }

        return qualityCheck;
    }
}
