using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.GetAccountingCodeById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.GetAccountingCodeById;

/// <summary>
/// Endpoint for retrieving an accounting code by its unique identifier.
/// </summary>
public static class GetAccountingCodeByIdEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:int}", async (
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetAccountingCodeByIdQuery(id), cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetAccountingCodeById")
        .WithSummary("Get an accounting code by ID")
        .RequirePermission(ProductsPermissions.AccountingCodes.ViewAccountingCode)
        .WithDescription("Retrieves a single accounting code by its unique identifier, including the associated product title. Returns 404 if the accounting code is not found within the current tenant.")
        .Produces<AccountingCodeDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
