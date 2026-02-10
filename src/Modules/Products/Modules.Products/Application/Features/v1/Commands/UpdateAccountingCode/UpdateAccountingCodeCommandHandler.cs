using System.Net;
using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateAccountingCode;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateAccountingCode;

public sealed class UpdateAccountingCodeCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateAccountingCodeCommand>
{
    public async ValueTask<Unit> Handle(UpdateAccountingCodeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to update accounting code: tenant context is required but not available.");

        var accountingCode = await dbContext.AccountingCodes
            .FirstOrDefaultAsync(a => a.Id == command.Id && a.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException($"Accounting code with ID {command.Id} not found or does not belong to the current tenant.");

        // Verify Code uniqueness (excluding self)
        if (command.Code != accountingCode.Code)
        {
            var codeExists = await dbContext.AccountingCodes
                .AnyAsync(a => a.ProductId == accountingCode.ProductId && a.TenantId == tenantId && a.Code == command.Code && a.Id != command.Id, cancellationToken);

            if (codeExists)
            {
                throw new CustomException($"Accounting code '{command.Code}' already exists for this product.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
            }
        }

        accountingCode.Code = command.Code;
        accountingCode.Description = command.Description;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
