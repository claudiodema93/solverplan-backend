using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Domain.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetQualityCheckById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.GetQualityCheckById;

/// <summary>
/// Endpoint for retrieving a quality check by its ID.
/// </summary>
public static class GetQualityCheckByIdEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetQualityCheckByIdQuery(id), cancellationToken);
                return TypedResults.Ok(result);
            })
            .WithName("GetQualityCheckById")
            .WithSummary("Get quality check by ID")
            .RequirePermission(ProductsPermissions.QualityChecks.ViewQualityCheck)
            .WithDescription("Retrieves a quality check by its unique identifier.")
            .Produces<QualityCheckDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
