using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateAccountingCode;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Commands.UpdateAccountingCode;

/// <summary>
/// Endpoint for updating an existing accounting code.
/// </summary>
public static class UpdateAccountingCodeEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateAccountingCodeRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateAccountingCodeCommand(
                Id: id,
                Code: request.Code,
                Description: request.Description);

            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("UpdateAccountingCode")
        .WithSummary("Update an accounting code")
        .RequirePermission(ProductsPermissions.AccountingCodes.UpdateAccountingCode)
        .WithDescription("Updates an existing accounting code within the current tenant. The new code must be unique per product. Returns 404 if the accounting code is not found.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);
    }
}

/// <summary>
/// Request body for updating an accounting code (excludes the route-bound Id).
/// </summary>
public sealed record UpdateAccountingCodeRequest(
    string Code,
    string? Description);
