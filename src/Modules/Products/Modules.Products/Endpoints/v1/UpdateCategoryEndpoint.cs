using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateCategory;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

public static class UpdateCategoryEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateCategoryRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCategoryCommand(
                Id: id,
                Name: request.Name);

            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("UpdateCategory")
        .WithSummary("Update category")
        .RequirePermission(ProductsPermissions.Categories.UpdateCategory)
        .WithDescription("Update an existing category.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request model for updating a category (excludes Id which comes from route).
/// </summary>
public sealed record UpdateCategoryRequest(string Name);
