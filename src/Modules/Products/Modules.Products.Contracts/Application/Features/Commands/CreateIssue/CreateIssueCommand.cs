using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Commands.CreateIssue;

/// <summary>
/// Command to create a new issue.
/// </summary>
public sealed record CreateIssueCommand(
    int ProductId,
    string Title,
    string Description,
    IssueSeverity? Severity,
    IssueState? Status) : ICommand<int>;
