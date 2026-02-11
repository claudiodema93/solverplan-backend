using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchIssues;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.SearchIssues;

/// <summary>
/// Endpoint for searching and filtering issues with pagination.
/// </summary>
public static class SearchIssuesEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (
            [AsParameters] SearchIssuesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(SearchIssuesQuery))
        .WithSummary("Search and filter issues")
        .RequirePermission(ProductsPermissions.Issues.ViewIssue)
        .WithDescription("Retrieves a paginated list of product issues with optional filtering and sorting capabilities. Supports filtering by associated product, severity level (Low, Medium, High, Critical), and issue status (Open, In Progress, Resolved, Closed). Returns issues scoped to the current tenant with configurable page size and page number.")
        .Produces<PagedResponse<IssueDto>>(StatusCodes.Status200OK);
    }
}
