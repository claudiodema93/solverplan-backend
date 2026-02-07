using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchProducts;
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
        .WithSummary("Search and filter products")
        .RequirePermission(ProductsPermissions.View)
        .WithDescription("Retrieves a paginated list of products with optional filtering and sorting capabilities. Supports filtering by category, product status (Draft, Published, Archived), and latest version flag. Allows sorting by various product fields. Returns products scoped to the current tenant with configurable page size and page number.")
        .Produces<PagedResponse<ProductDto>>(StatusCodes.Status200OK);
    }
}
