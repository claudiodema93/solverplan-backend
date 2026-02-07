using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateCategory;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for creating a new product category.
/// </summary>
public static class CreateCategoryEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (
            [FromBody] CreateCategoryCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var categoryId = await mediator.Send(command, cancellationToken);
            return TypedResults.Created($"/categories/{categoryId}", new CreateCategoryResponse(categoryId));
        })
            .WithName("CreateCategory")
            .WithSummary("Create a new category")
            .RequirePermission(ProductsPermissions.Categories.CreateCategory)
            .WithDescription("Creates a new product category within the current tenant. The category name must be unique within the tenant. Returns the newly created category ID in the response.")
            .Produces<CreateCategoryResponse>(StatusCodes.Status201Created);
    }
}
