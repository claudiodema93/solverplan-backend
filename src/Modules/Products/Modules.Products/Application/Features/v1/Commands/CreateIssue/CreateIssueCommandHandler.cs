using FSH.Framework.Core.Context;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateIssue;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Domain.Entities;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Application.Features.v1.Commands.CreateIssue;

public sealed class CreateIssueCommandHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateIssueCommand, int>
{
    public async ValueTask<int> Handle(CreateIssueCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Tenant not found.");

        // Verify ProductId exists and belongs to current tenant
        var productExists = await dbContext.Products
            .AnyAsync(p => p.Id == command.ProductId && p.TenantId == tenantId, cancellationToken);

        if (!productExists)
        {
            throw new InvalidOperationException($"Product with ID {command.ProductId} not found or does not belong to the current tenant.");
        }

        // Create issue entity with default values
        var issue = new Issue
        {
            ProductId = command.ProductId,
            Title = command.Title,
            Description = command.Description,
            Severity = command.Severity ?? IssueSeverity.Medium,
            Status = command.Status ?? IssueState.Open,
            TenantId = tenantId
        };

        await dbContext.Issues.AddAsync(issue, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return issue.Id;
    }
}
