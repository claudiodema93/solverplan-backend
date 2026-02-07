using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateIssue;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for updating an existing product issue.
/// </summary>
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
            .WithSummary("Update an existing issue")
            .RequirePermission(ProductsPermissions.Issues.UpdateIssue)
            .WithDescription("Updates an existing product issue with new information. Allows modification of issue details such as title, description, severity, and status. The issue must exist within the current tenant. Returns 400 if the route ID does not match the body ID, or 404 if the issue is not found.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
