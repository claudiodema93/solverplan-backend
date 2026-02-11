using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Features.v1.Commands.DeleteIssue;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Commands.DeleteIssue;

/// <summary>
/// Endpoint for deleting a product issue.
/// </summary>
public static class DeleteIssueEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:int}", async (
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteIssueCommand(Id: id);

            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("DeleteIssue")
        .WithSummary("Delete an issue")
        .RequirePermission(ProductsPermissions.Issues.DeleteIssue)
        .WithDescription("Deletes an existing product issue from the current tenant. This operation permanently removes the issue and all its associated data. Returns 404 if the issue is not found.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
