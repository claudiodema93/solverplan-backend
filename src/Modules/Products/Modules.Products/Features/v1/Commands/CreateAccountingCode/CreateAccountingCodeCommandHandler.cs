using System.Net;
using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateAccountingCode;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Commands.CreateAccountingCode;

public sealed class CreateAccountingCodeCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateAccountingCodeCommand, int>
{
    public async ValueTask<int> Handle(CreateAccountingCodeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to create accounting code: tenant context is required but not available.");

        // Verify product exists and belongs to current tenant
        var productExists = await dbContext.Products
            .AnyAsync(p => p.Id == command.ProductId && p.TenantId == tenantId, cancellationToken);

        if (!productExists)
        {
            throw new NotFoundException($"Product with ID {command.ProductId} not found or does not belong to the current tenant.");
        }

        // Verify Code is unique for this product within the tenant
        var codeExists = await dbContext.AccountingCodes
            .AnyAsync(a => a.ProductId == command.ProductId && a.TenantId == tenantId && a.Code == command.Code, cancellationToken);

        if (codeExists)
        {
            throw new CustomException($"Accounting code '{command.Code}' already exists for product with ID {command.ProductId}.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
        }

        var accountingCode = new AccountingCode
        {
            ProductId = command.ProductId,
            Code = command.Code,
            Description = command.Description,
            TenantId = tenantId
        };

        await dbContext.AccountingCodes.AddAsync(accountingCode, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return accountingCode.Id;
    }
}
