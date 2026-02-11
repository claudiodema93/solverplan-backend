using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetCategoryById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.GetCategoryById;

/// <summary>
/// Endpoint for retrieving a category by its ID.
/// </summary>
public static class GetCategoryByIdEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(GetCategoryByIdQuery))
        .WithSummary("Retrieve a category by ID")
        .RequirePermission(ProductsPermissions.Categories.ViewCategory)
        .WithDescription("Retrieves a single product category by its unique identifier. Returns the category details including name and associated metadata. Returns 404 if the category is not found or does not belong to the current tenant.")
        .Produces<CategoryDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
