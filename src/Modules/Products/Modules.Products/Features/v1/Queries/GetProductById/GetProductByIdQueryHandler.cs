using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetProductById;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Queries.GetProductById;

/// <summary>
/// Handler for retrieving a product by its unique identifier.
/// </summary>
public sealed class GetProductByIdQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    public async ValueTask<ProductDto?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to retrieve product: tenant context is required but not available.");

        var product = await dbContext.Products
            .Include(p => p.Category)
            .Where(p => p.Id == query.Id && p.TenantId == tenantId)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Revision = p.Revision,
                Status = p.Status,
                Description = p.Description,
                Private = p.Private,
                CustomerId = p.CustomerId,
                Variant = p.Variant,
                Version = p.Version,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                Keywords = p.Keywords,
                Language = p.Language,
                Subject = p.Subject,
                Notes = p.Notes,
                HashSha256 = p.HashSha256,
                Characteristics = new ProductCharacteristicsDto(
                    p.Characteristics.IsAssembly,
                    p.Characteristics.IsJobWork,
                    p.Characteristics.IsManufacturable,
                    p.Characteristics.IsCommercial,
                    p.Characteristics.IsSellable,
                    p.Characteristics.IsQualityCheckRequired),
                IsLatest = p.IsLatest,
                Mirroring = new MirroringInfoDto(
                    p.Mirroring.IsMirrored,
                    p.Mirroring.SourceProductTitle,
                    p.Mirroring.SourceProductRevision,
                    p.Mirroring.CopyProduct),
                TenantId = p.TenantId,
                CreatedOnUtc = p.CreatedOnUtc,
                CreatedBy = p.CreatedBy,
                LastModifiedOnUtc = p.LastModifiedOnUtc,
                LastModifiedBy = p.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            throw new NotFoundException($"Product with ID {query.Id} not found.");
        }

        return product;
    }
}
