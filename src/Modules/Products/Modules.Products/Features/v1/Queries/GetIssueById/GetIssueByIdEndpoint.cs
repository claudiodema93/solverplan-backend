using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetIssueById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.GetIssueById;

/// <summary>
/// Endpoint for retrieving an issue by its ID.
/// </summary>
public static class GetIssueByIdEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetIssueByIdQuery(id), cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(GetIssueByIdQuery))
        .WithSummary("Retrieve an issue by ID")
        .RequirePermission(ProductsPermissions.Issues.ViewIssue)
        .WithDescription("Retrieves a single product issue by its unique identifier. Returns comprehensive issue details including title, description, severity level, status, associated product, and all relevant metadata. Returns 404 if the issue is not found or does not belong to the current tenant.")
        .Produces<IssueDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
