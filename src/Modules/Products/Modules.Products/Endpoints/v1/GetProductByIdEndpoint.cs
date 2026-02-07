using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetProductById;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for retrieving a product by its ID.
/// </summary>
public static class GetProductByIdEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(GetProductByIdQuery))
        .WithSummary("Get product by ID")
        .RequirePermission(ProductsPermissions.View)
        .WithDescription("Retrieve a product by its unique identifier.")
        .Produces<ProductDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
