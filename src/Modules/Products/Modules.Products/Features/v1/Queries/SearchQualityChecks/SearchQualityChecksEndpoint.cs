using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Domain.DTOs;
using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchQualityChecks;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Queries.SearchQualityChecks;

/// <summary>
/// Endpoint for searching quality checks with pagination and filtering.
/// </summary>
public static class SearchQualityChecksEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (
            [AsParameters] SearchQualityChecksQuery query,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(query, cancellationToken);
                return TypedResults.Ok(result);
            })
            .WithName("SearchQualityChecks")
            .WithSummary("Search quality checks")
            .RequirePermission(ProductsPermissions.QualityChecks.List)
            .WithDescription("Search and filter quality checks with pagination and sorting.")
            .Produces<PagedResponse<QualityCheckDto>>(StatusCodes.Status200OK);
    }
}
