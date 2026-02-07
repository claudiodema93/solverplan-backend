using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteProduct;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

public static class DeleteProductEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:int}", async (
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteProductCommand(id);
            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("DeleteProduct")
        .WithSummary("Delete product")
        .RequirePermission(ProductsPermissions.Delete)
        .WithDescription("Delete an existing product.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
