using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetProductById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.GetProductById;

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
        .WithSummary("Retrieve a product by ID")
        .RequirePermission(ProductsPermissions.View)
        .WithDescription("Retrieves a single product by its unique identifier. Returns comprehensive product details including title, description, version information, category, status, characteristics, and all associated metadata. Returns 404 if the product is not found or does not belong to the current tenant.")
        .Produces<ProductDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
