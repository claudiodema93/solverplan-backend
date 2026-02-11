using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchAccountingCodes;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.SearchAccountingCodes;

/// <summary>
/// Endpoint for searching and filtering accounting codes with pagination.
/// </summary>
public static class SearchAccountingCodesEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (
            [AsParameters] SearchAccountingCodesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(SearchAccountingCodesQuery))
        .WithSummary("Search and filter accounting codes")
        .RequirePermission(ProductsPermissions.AccountingCodes.ViewAccountingCode)
        .WithDescription("Retrieves a paginated list of accounting codes with optional filtering and sorting capabilities. Supports filtering by product ID and full-text search on Code and Description. Returns accounting codes scoped to the current tenant.")
        .Produces<PagedResponse<AccountingCodeDto>>(StatusCodes.Status200OK);
    }
}
