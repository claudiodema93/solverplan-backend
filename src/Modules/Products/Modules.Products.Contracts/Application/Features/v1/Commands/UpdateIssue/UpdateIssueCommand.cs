using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateIssue;

/// <summary>
/// Command to update an existing issue.
/// </summary>
public sealed record UpdateIssueCommand(
    int Id,
    string Title,
    string Description,
    IssueSeverity Severity,
    IssueState Status,
    string? ResolutionNotes) : ICommand;
