using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Products.Contracts;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateIssue;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Products.Endpoints.v1;

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
            .WithSummary("Create issue")
            .RequirePermission(ProductsPermissions.Issues.Create)
            .WithDescription("Create a new issue for a product.")
            .Produces<CreateIssueResponse>(StatusCodes.Status201Created);
    }
}
