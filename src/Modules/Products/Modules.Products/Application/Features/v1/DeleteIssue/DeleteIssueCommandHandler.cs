using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Application.Features.Commands.DeleteIssue;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.Commands.DeleteIssue;

public sealed class DeleteIssueCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteIssueCommand>
{
    public async ValueTask<Unit> Handle(DeleteIssueCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        // Find existing issue by Id and tenant
        var issue = await dbContext.Issues
            .Where(i => i.Id == command.Id && i.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Issue with Id {command.Id} not found.");

        // Remove issue
        dbContext.Issues.Remove(issue);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
