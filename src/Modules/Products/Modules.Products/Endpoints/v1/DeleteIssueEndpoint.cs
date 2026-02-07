using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.Commands.DeleteIssue;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

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
        .WithSummary("Delete issue")
        .RequirePermission(ProductsPermissions.Issues.Delete)
        .WithDescription("Delete an existing issue.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
