using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.Queries.SearchProducts;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for searching and filtering products with pagination.
/// </summary>
public static class SearchProductsEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (
            [AsParameters] SearchProductsQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(SearchProductsQuery))
        .WithSummary("Search products with pagination")
        .RequirePermission(ProductsPermissions.View)
        .WithDescription("Search and filter products with server-side pagination, sorting, and filtering by category, status, and latest version.")
        .Produces<PagedResponse<ProductDto>>(StatusCodes.Status200OK);
    }
}
