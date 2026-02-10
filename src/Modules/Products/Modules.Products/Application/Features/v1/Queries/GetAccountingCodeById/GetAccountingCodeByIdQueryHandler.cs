using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetAccountingCodeById;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Queries.GetAccountingCodeById;

/// <summary>
/// Handler for retrieving an accounting code by its unique identifier.
/// </summary>
public sealed class GetAccountingCodeByIdQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<GetAccountingCodeByIdQuery, AccountingCodeDto?>
{
    public async ValueTask<AccountingCodeDto?> Handle(GetAccountingCodeByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to retrieve accounting code: tenant context is required but not available.");

        var accountingCode = await dbContext.AccountingCodes
            .Include(a => a.Product)
            .Where(a => a.Id == query.Id && a.TenantId == tenantId)
            .Select(a => new AccountingCodeDto
            {
                Id = a.Id,
                ProductId = a.ProductId,
                ProductTitle = a.Product!.Title,
                Code = a.Code,
                Description = a.Description,
                TenantId = a.TenantId,
                CreatedOnUtc = a.CreatedOnUtc,
                CreatedBy = a.CreatedBy,
                LastModifiedOnUtc = a.LastModifiedOnUtc,
                LastModifiedBy = a.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (accountingCode == null)
        {
            throw new NotFoundException($"Accounting code with ID {query.Id} not found.");
        }

        return accountingCode;
    }
}
