using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateAccountingCode;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for creating a new accounting code.
/// </summary>
public static class CreateAccountingCodeEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (
            [FromBody] CreateAccountingCodeCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var id = await mediator.Send(command, cancellationToken);
            return TypedResults.Created($"/api/v1/products/accountingcodes/{id}", id);
        })
        .WithName("CreateAccountingCode")
        .WithSummary("Create a new accounting code")
        .RequirePermission(ProductsPermissions.AccountingCodes.CreateAccountingCode)
        .WithDescription("Creates a new accounting code for a product. The code must be unique per product within the tenant. Returns the ID of the newly created accounting code.")
        .Produces<int>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);
    }
}
