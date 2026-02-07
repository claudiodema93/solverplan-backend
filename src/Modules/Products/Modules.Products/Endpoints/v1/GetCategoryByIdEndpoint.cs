using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.GetCategoryById;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

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
        .WithSummary("Get category by ID")
        .RequirePermission(ProductsPermissions.Categories.ViewCategory)
        .WithDescription("Retrieve a category by its unique identifier.")
        .Produces<CategoryDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
