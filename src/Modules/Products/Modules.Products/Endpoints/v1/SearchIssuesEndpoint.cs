using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchIssues;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

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
        .WithSummary("Search issues with pagination")
        .RequirePermission(ProductsPermissions.Issues.ViewIssue)
        .WithDescription("Search and filter issues with server-side pagination and sorting. Supports filtering by product, severity, and status.")
        .Produces<PagedResponse<IssueDto>>(StatusCodes.Status200OK);
    }
}
