namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateIssue;

/// <summary>
/// Response containing the identifier of the newly created issue.
/// </summary>
/// <param name="Id">The unique identifier of the created issue.</param>
public sealed record CreateIssueResponse(int Id);
