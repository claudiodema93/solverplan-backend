using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.Features.v1.Commands.DeleteIssue;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Commands.DeleteIssue;

public sealed class DeleteIssueCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteIssueCommand>
{
    public async ValueTask<Unit> Handle(DeleteIssueCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to delete issue: tenant context is required but not available.");

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
