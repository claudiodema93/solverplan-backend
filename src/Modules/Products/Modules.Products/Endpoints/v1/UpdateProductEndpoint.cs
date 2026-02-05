using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.Commands.UpdateProduct;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

public static class UpdateProductEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateProductRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateProductCommand(
                Id: id,
                Title: request.Title,
                Revision: request.Revision,
                Description: request.Description,
                Status: request.Status,
                Private: request.Private,
                CustomerId: request.CustomerId,
                Variant: request.Variant,
                Version: request.Version,
                CategoryId: request.CategoryId,
                Keywords: request.Keywords,
                Language: request.Language,
                Subject: request.Subject,
                Notes: request.Notes,
                HashSha256: request.HashSha256,
                IsLatest: request.IsLatest,
                Characteristics: request.Characteristics,
                Mirroring: request.Mirroring);

            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("UpdateProduct")
        .WithSummary("Update product")
        .RequirePermission(ProductsPermissions.Update)
        .WithDescription("Update an existing product.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request model for updating a product (excludes Id which comes from route).
/// </summary>
public sealed record UpdateProductRequest(
    string Title,
    int Revision = 0,
    string? Description = null,
    FSH.Modules.Products.Contracts.Domain.Enums.ProductState Status = FSH.Modules.Products.Contracts.Domain.Enums.ProductState.Draft,
    bool Private = false,
    string? CustomerId = null,
    string Variant = "",
    int Version = 0,
    int? CategoryId = null,
    string? Keywords = null,
    string? Language = null,
    string? Subject = null,
    string? Notes = null,
    string? HashSha256 = null,
    bool IsLatest = false,
    ProductCharacteristicsDto? Characteristics = null,
    MirroringInfoDto? Mirroring = null);
