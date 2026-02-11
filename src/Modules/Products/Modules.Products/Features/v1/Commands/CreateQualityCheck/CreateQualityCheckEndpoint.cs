using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateQualityCheck;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Commands.CreateQualityCheck;

/// <summary>
/// Endpoint for creating a new quality check.
/// </summary>
public static class CreateQualityCheckEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (
            [FromBody] CreateQualityCheckCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return TypedResults.Created($"/api/v1/qualitychecks/{result.Id}", result);
            })
            .WithName("CreateQualityCheck")
            .WithSummary("Create a new quality check")
            .RequirePermission(ProductsPermissions.QualityChecks.CreateQualityCheck)
            .WithDescription("Creates a new quality check for a product within the current tenant.")
            .Produces<CreateQualityCheckResponse>(StatusCodes.Status201Created);
    }
}
