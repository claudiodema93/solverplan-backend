using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.Commands.UpdateIssue;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

public static class UpdateIssueEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id}", async (
            [FromRoute] int id,
            [FromBody] UpdateIssueCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest("Id mismatch between route and body.");
            }

            await mediator.Send(command, cancellationToken);
            return Results.NoContent();
        })
            .WithName("UpdateIssue")
            .WithSummary("Update issue")
            .RequirePermission(ProductsPermissions.Issues.Update)
            .WithDescription("Update an existing issue.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
