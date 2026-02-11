using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Features.v1.Commands.DeleteCategory;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Commands.DeleteCategory;

/// <summary>
/// Endpoint for deleting a product category.
/// </summary>
public static class DeleteCategoryEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:int}", async (
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteCategoryCommand(Id: id);

            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("DeleteCategory")
        .WithSummary("Delete a category")
        .RequirePermission(ProductsPermissions.Categories.DeleteCategory)
        .WithDescription("Deletes an existing product category from the current tenant. The operation will fail if any products are currently assigned to this category. Returns 404 if the category is not found, or 400 if the category cannot be deleted due to existing references.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
