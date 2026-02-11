using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetBomItemById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.GetBomItemById;

/// <summary>
/// Endpoint for retrieving a BOM item by its unique identifier.
/// </summary>
public static class GetBomItemByIdEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:int}", async (
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetBomItemByIdQuery(id), cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetBomItemById")
        .WithSummary("Get a BOM item by ID")
        .RequirePermission(ProductsPermissions.BomItems.ViewBomItem)
        .WithDescription("Retrieves a single BOM item by its unique identifier, including the parent product and child product details. Returns 404 if the BOM item is not found within the current tenant.")
        .Produces<BomItemDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
