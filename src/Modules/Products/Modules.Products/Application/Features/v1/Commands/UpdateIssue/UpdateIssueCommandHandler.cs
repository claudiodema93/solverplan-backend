using FSH.Framework.Core.Context;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateIssue;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateIssue;

public sealed class UpdateIssueCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateIssueCommand>
{
    public async ValueTask<Unit> Handle(UpdateIssueCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        // Find issue by Id and tenant
        var issue = await dbContext.Issues
            .FirstOrDefaultAsync(i => i.Id == command.Id && i.TenantId == tenantId, cancellationToken)
            ?? throw new InvalidOperationException($"Issue with ID {command.Id} not found or does not belong to the current tenant.");

        // Update properties
        issue.Title = command.Title;
        issue.Description = command.Description;
        issue.Severity = command.Severity;
        issue.Status = command.Status;
        issue.ResolutionNotes = command.ResolutionNotes;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
