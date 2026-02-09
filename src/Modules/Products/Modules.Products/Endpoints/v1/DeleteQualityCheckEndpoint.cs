using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteQualityCheck;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for deleting a quality check.
/// </summary>
public static class DeleteQualityCheckEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                await mediator.Send(new DeleteQualityCheckCommand(id), cancellationToken);
                return TypedResults.NoContent();
            })
            .WithName("DeleteQualityCheck")
            .WithSummary("Delete a quality check")
            .RequirePermission(ProductsPermissions.QualityChecks.DeleteQualityCheck)
            .WithDescription("Deletes a quality check by its unique identifier.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
