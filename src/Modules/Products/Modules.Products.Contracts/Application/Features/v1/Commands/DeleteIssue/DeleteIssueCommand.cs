using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteIssue;

/// <summary>
/// Command to delete an issue.
/// </summary>
public sealed record DeleteIssueCommand(int Id) : ICommand;
