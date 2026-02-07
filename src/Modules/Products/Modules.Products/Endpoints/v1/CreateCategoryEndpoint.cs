using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateCategory;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

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
            .WithSummary("Create category")
            .RequirePermission(ProductsPermissions.Categories.CreateCategory)
            .WithDescription("Create a new category.")
            .Produces<CreateCategoryResponse>(StatusCodes.Status201Created);
    }
}
