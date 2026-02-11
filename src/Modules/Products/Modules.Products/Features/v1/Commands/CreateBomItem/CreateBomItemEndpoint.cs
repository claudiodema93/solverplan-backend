using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateBomItem;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Commands.CreateBomItem;

/// <summary>
/// Endpoint for creating a new BOM item.
/// </summary>
public static class CreateBomItemEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (
            [FromBody] CreateBomItemCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(command, cancellationToken);
            return TypedResults.Created($"/api/v1/bomitems/{id}", id);
        })
        .WithName("CreateBomItem")
        .WithSummary("Create a new BOM item")
        .RequirePermission(ProductsPermissions.BomItems.CreateBomItem)
        .WithDescription("Creates a new Bill of Materials item linking a parent product to a child component product. Validates that both the parent and child products exist within the current tenant. Returns the ID of the newly created BOM item.")
        .Produces<int>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound);
    }
}
