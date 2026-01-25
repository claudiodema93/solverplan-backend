using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.v1.CreateProduct;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.CreateProduct;

public static class CreateProductEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (
            [FromBody] CreateProductCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
            => TypedResults.Ok(await mediator.Send(command, cancellationToken)))
            .WithName("CreateProduct")
            .WithSummary("Create product")
            .RequirePermission(ProductsPermissions.Create)
            .WithDescription("Create a new product.")
            .Produces<CreateProductResponse>(StatusCodes.Status200OK);
    }
}
