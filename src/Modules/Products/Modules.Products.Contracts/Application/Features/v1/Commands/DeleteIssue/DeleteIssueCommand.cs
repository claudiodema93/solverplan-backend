using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteIssue;

/// <summary>
/// Command to delete an issue.
/// </summary>
/// <param name="Id">The unique identifier of the issue to delete</param>
public sealed record DeleteIssueCommand(int Id) : ICommand;
