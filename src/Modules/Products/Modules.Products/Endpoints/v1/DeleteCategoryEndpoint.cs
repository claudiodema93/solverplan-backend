using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.Commands.DeleteCategory;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

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
        .WithSummary("Delete category")
        .RequirePermission(ProductsPermissions.Categories.Delete)
        .WithDescription("Delete an existing category. Cannot delete if products reference this category.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
