using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchBomItems;
using FSH.Modules.Products.Contracts.DTOs;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for searching and filtering BOM items with pagination.
/// </summary>
public static class SearchBomItemsEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (
            [AsParameters] SearchBomItemsQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName(nameof(SearchBomItemsQuery))
        .WithSummary("Search and filter BOM items")
        .RequirePermission(ProductsPermissions.BomItems.ViewBomItem)
        .WithDescription("Retrieves a paginated list of BOM items with optional filtering and sorting capabilities. Supports filtering by parent product ID, child product ID, unit of measurement, and manual entry flag. Returns BOM items scoped to the current tenant with configurable page size and page number.")
        .Produces<PagedResponse<BomItemDto>>(StatusCodes.Status200OK);
    }
}
