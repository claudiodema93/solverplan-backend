using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateIssue;
using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

/// <summary>
/// Endpoint for updating an existing product issue.
/// </summary>
public static class UpdateIssueEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateIssueRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateIssueCommand(
                Id: id,
                Title: request.Title,
                Description: request.Description,
                Severity: request.Severity,
                Status: request.Status,
                ResolutionNotes: request.ResolutionNotes);

            await mediator.Send(command, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("UpdateIssue")
        .WithSummary("Update an existing issue")
        .RequirePermission(ProductsPermissions.Issues.UpdateIssue)
        .WithDescription("Updates an existing product issue with new information. Allows modification of issue details such as title, description, severity, and status. The issue must exist within the current tenant. Returns 404 if the issue is not found.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request model for updating an issue (excludes Id which comes from route).
/// </summary>
public sealed record UpdateIssueRequest(
    string Title,
    string Description,
    IssueSeverity Severity,
    IssueState Status,
    string? ResolutionNotes);
