using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateQualityCheck;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for updating an existing quality check.
/// </summary>
public static class UpdateQualityCheckEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id:int}", async (
            int id,
            [FromBody] UpdateQualityCheckCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Route ID does not match command ID.");
                }

                await mediator.Send(command, cancellationToken);
                return TypedResults.NoContent();
            })
            .WithName("UpdateQualityCheck")
            .WithSummary("Update an existing quality check")
            .RequirePermission(ProductsPermissions.QualityChecks.UpdateQualityCheck)
            .WithDescription("Updates an existing quality check with new data.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }
}
