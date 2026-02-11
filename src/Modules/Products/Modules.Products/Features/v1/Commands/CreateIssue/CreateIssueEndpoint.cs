using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateIssue;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Features.v1.Commands.CreateIssue;

/// <summary>
/// Endpoint for creating a new product issue.
/// </summary>
public static class CreateIssueEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (
            [FromBody] CreateIssueCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var issueId = await mediator.Send(command, cancellationToken);
            return TypedResults.Created($"/issues/{issueId}", new CreateIssueResponse(issueId));
        })
            .WithName("CreateIssue")
            .WithSummary("Create a new issue")
            .RequirePermission(ProductsPermissions.Issues.CreateIssue)
            .WithDescription("Creates a new issue associated with a product within the current tenant. Issues track problems, bugs, or concerns related to products. The issue must be linked to an existing product. Returns the newly created issue ID in the response.")
            .Produces<CreateIssueResponse>(StatusCodes.Status201Created);
    }
}
