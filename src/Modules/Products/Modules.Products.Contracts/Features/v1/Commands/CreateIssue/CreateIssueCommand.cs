using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Features.v1.Commands.CreateIssue;

/// <summary>
/// Command to create a new issue.
/// </summary>
/// <param name="ProductId">The identifier of the product this issue belongs to.</param>
/// <param name="Title">The issue title.</param>
/// <param name="Description">Detailed description of the issue.</param>
/// <param name="Severity">Optional severity level of the issue.</param>
/// <param name="Status">Optional current status of the issue.</param>
public sealed record CreateIssueCommand(
    int ProductId,
    string Title,
    string Description,
    IssueSeverity? Severity,
    IssueState? Status) : ICommand<int>;
