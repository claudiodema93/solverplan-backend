using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.Queries.GetIssueById;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

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
        .WithSummary("Get issue by ID")
        .RequirePermission(ProductsPermissions.Issues.View)
        .WithDescription("Retrieve an issue by its unique identifier.")
        .Produces<IssueDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
