using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Features.v1.Commands.DeleteAccountingCode;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Commands.DeleteAccountingCode;

public sealed class DeleteAccountingCodeCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteAccountingCodeCommand>
{
    public async ValueTask<Unit> Handle(DeleteAccountingCodeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to delete accounting code: tenant context is required but not available.");

        var accountingCode = await dbContext.AccountingCodes
            .FirstOrDefaultAsync(a => a.Id == command.Id && a.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException($"Accounting code with ID {command.Id} not found or does not belong to the current tenant.");

        dbContext.AccountingCodes.Remove(accountingCode);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
