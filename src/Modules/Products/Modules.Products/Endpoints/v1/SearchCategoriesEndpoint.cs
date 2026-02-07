using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchCategories;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for searching and filtering categories with pagination.
/// </summary>
public static class SearchCategoriesEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (
            [AsParameters] SearchCategoriesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(SearchCategoriesQuery))
        .WithSummary("Search and filter categories")
        .RequirePermission(ProductsPermissions.Categories.ViewCategory)
        .WithDescription("Retrieves a paginated list of product categories with optional filtering and sorting. Supports sorting by name or creation date. Returns categories scoped to the current tenant with configurable page size and page number.")
        .Produces<PagedResponse<CategoryDto>>(StatusCodes.Status200OK);
    }
}
