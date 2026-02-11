using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateBomItem;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Commands.UpdateBomItem;

/// <summary>
/// Endpoint for updating an existing BOM item.
/// </summary>
public static class UpdateBomItemEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateBomItemRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateBomItemCommand(
                Id: id,
                ChildProductId: request.ChildProductId,
                Quantity: request.Quantity,
                Unit: request.Unit,
                IsManual: request.IsManual,
                Notes: request.Notes);

            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("UpdateBomItem")
        .WithSummary("Update a BOM item")
        .RequirePermission(ProductsPermissions.BomItems.UpdateBomItem)
        .WithDescription("Updates an existing BOM item within the current tenant. Allows changing the child product, quantity, unit of measurement, manual flag, and notes. The parent product (ProductId) cannot be changed. Returns 404 if the BOM item or child product is not found.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request body for updating a BOM item (excludes the route-bound Id).
/// </summary>
public sealed record UpdateBomItemRequest(
    int ChildProductId,
    double Quantity,
    Contracts.Domain.Enums.UnitOfMeasurement Unit,
    bool IsManual,
    string? Notes);
