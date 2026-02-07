using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Commands.DeleteIssue;

/// <summary>
/// Command to delete an issue.
/// </summary>
public sealed record DeleteIssueCommand(int Id) : ICommand;
