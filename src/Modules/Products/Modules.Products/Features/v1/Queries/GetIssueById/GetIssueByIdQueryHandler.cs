using FSH.Framework.Core.Context;
using FSH.Framework.Core.Exceptions;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetIssueById;
using FSH.Modules.Products.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Products.Features.v1.Queries.GetIssueById;

/// <summary>
/// Handler for retrieving an issue by its unique identifier.
/// </summary>
public sealed class GetIssueByIdQueryHandler(
    ProductsDbContext dbContext,
    ICurrentUser currentUser)
    : IQueryHandler<GetIssueByIdQuery, IssueDto?>
{
    public async ValueTask<IssueDto?> Handle(GetIssueByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tenantId = currentUser.GetTenant()
            ?? throw new InvalidOperationException("Unable to retrieve issue: tenant context is required but not available.");

        var issue = await dbContext.Issues
            .Include(i => i.Product)
            .Where(i => i.Id == query.Id && i.TenantId == tenantId)
            .Select(i => new IssueDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductTitle = i.Product!.Title,
                Title = i.Title,
                Description = i.Description,
                Severity = i.Severity,
                Status = i.Status,
                ResolutionNotes = i.ResolutionNotes,
                TenantId = i.TenantId,
                CreatedOnUtc = i.CreatedOnUtc,
                CreatedBy = i.CreatedBy,
                LastModifiedOnUtc = i.LastModifiedOnUtc,
                LastModifiedBy = i.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (issue == null)
        {
            throw new NotFoundException($"Issue with ID {query.Id} not found.");
        }

        return issue;
    }
}
